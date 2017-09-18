using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Web;

using System.Configuration;
using System.Web.Configuration;
using System.Configuration.Provider;
using System.Reflection;

using System.Linq.Dynamic;
using System.Security.Principal;
using System.Threading;
using Lynicon.Membership;
using Lynicon.Repositories;
using Lynicon.Utility;
using Lynicon.Extensibility;

namespace Lynicon.Membership
{
    /// <summary>
    /// Lynicon's custom ASP.Net membership provider
    /// </summary>
    public class LightweightMembershipProvider : MembershipProvider
    {
        private string pApplicationName;
        private bool pEnablePasswordReset;
        private bool pEnablePasswordRetrieval;
        private bool pRequiresQuestionAndAnswer;
        private bool pRequiresUniqueEmail;
        private int pMaxInvalidPasswordAttempts;
        private int pPasswordAttemptWindow;
        private MembershipPasswordFormat pPasswordFormat;
        private string pInitPassword;

        //
        // Used when determining encryption key values.
        //

        private MachineKeySection machineKey;
        private int newPasswordLength = 8;

        public Func<IUser, string, List<string>> AlternateEncrypt { get; set; }

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            //
            // Initialize values from web.config.
            //

            if (config == null)
                throw new ArgumentNullException("config");

            if (name == null || name.Length == 0)
                name = "LightweightMembershipProvider";

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Lightweight Linq to Entities membership provider");
            }

            // Initialize the abstract base class.
            base.Initialize(name, config);

            pApplicationName = config["applicationName"] ??
                                            System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath;
            pMaxInvalidPasswordAttempts = Convert.ToInt32(config["maxInvalidPasswordAttempts"] ?? "5");
            pPasswordAttemptWindow = Convert.ToInt32(config["passwordAttemptWindow"] ?? "10");
            pMinRequiredNonAlphanumericCharacters = Convert.ToInt32(config["minRequiredNonAlphanumericCharacters"] ?? "1");
            pMinRequiredPasswordLength = Convert.ToInt32(config["minRequiredPasswordLength"] ?? "7");
            pPasswordStrengthRegularExpression = Convert.ToString(config["passwordStrengthRegularExpression"] ?? "");
            pEnablePasswordReset = Convert.ToBoolean(config["enablePasswordReset"] ?? "true");
            pEnablePasswordRetrieval = Convert.ToBoolean(config["enablePasswordRetrieval"] ?? "true");
            pInitPassword = Convert.ToString(config["initPassword"]);
            pRequiresQuestionAndAnswer = Convert.ToBoolean(config["requiresQuestionAndAnswer"] ?? "false");
            pRequiresUniqueEmail = Convert.ToBoolean(config["requiresUniqueEmail"] ?? "true");
            pWriteExceptionsToEventLog = Convert.ToBoolean(config["writeExceptionsToEventLog"] ?? "true");

            string temp_format = config["passwordFormat"];
            if (temp_format == null)
            {
                temp_format = "Hashed";
            }

            switch (temp_format)
            {
                case "Hashed":
                    pPasswordFormat = MembershipPasswordFormat.Hashed;
                    break;
                case "Encrypted":
                    pPasswordFormat = MembershipPasswordFormat.Encrypted;
                    break;
                case "Clear":
                    pPasswordFormat = MembershipPasswordFormat.Clear;
                    break;
                default:
                    throw new ProviderException("Password format not supported.");
            }

            // Get encryption and decryption key information from the configuration.
            Configuration cfg =
              WebConfigurationManager.OpenWebConfiguration(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            machineKey = (MachineKeySection)cfg.GetSection("system.web/machineKey");

            if (machineKey.ValidationKey.Contains("AutoGenerate"))
                if (PasswordFormat != MembershipPasswordFormat.Clear)
                    throw new ProviderException("Hashed or Encrypted passwords " +
                                                "are not supported with auto-generated keys.");
        }

        private string applicationName = null;
        public override string ApplicationName
        {
            get
            {
                if (applicationName == null && HttpContext.Current != null && HttpContext.Current.Request != null)
                    return HttpContext.Current.Request.ApplicationPath;
                else
                    return applicationName;
            }
            set
            {
                applicationName = value;
            }
        }

        public override bool EnablePasswordReset
        {
            get { return pEnablePasswordReset; }
        }


        public override bool EnablePasswordRetrieval
        {
            get { return pEnablePasswordRetrieval; }
        }


        public override bool RequiresQuestionAndAnswer
        {
            get { return pRequiresQuestionAndAnswer; }
        }


        public override bool RequiresUniqueEmail
        {
            get { return pRequiresUniqueEmail; }
        }


        public override int MaxInvalidPasswordAttempts
        {
            get { return pMaxInvalidPasswordAttempts; }
        }

        public override int PasswordAttemptWindow
        {
            get { return pPasswordAttemptWindow; }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { return pPasswordFormat; }
        }

        public string EncryptPassword(string s)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            byte[] encBytes = this.EncryptPassword(bytes);
            return Convert.ToBase64String(encBytes);
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword))
                throw new ArgumentException("Null string supplied to change password");

            IUserRepository repo = Repository.Instance.Registered(typeof(User)) as IUserRepository;
            IUser user = ValidateUser(username, oldPassword, repo, false);
            if (user == null) return false;
            user.Password = EncryptPassword(newPassword);
            try
            {
                repo.Set(new List<object> { user }, new Dictionary<string, object>());
            }
            catch (Exception ex)
            {
                throw new ProviderException(ex.Message, ex);
            }
            return true;
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                throw new ArgumentException("Null username or password supplied to CreateUser");

            ValidatePasswordEventArgs args = 
                new ValidatePasswordEventArgs(username, password, true);

            OnValidatingPassword(args);

            if (args.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            if (!Validation.IsValidEmail(email))
            {
                status = MembershipCreateStatus.InvalidEmail;
                return null;
            }

            try
            {
                IUserRepository repo = Repository.Instance.Registered(typeof(User)) as IUserRepository;
                IUser existingUser;
                if (RequiresUniqueEmail)
                    existingUser = repo.GetAnyUser(username, email); //ctx.IUserSet.Cast<IUser>().FirstOrDefault(u => u.UserName == username || u.Email == email);
                else
                    existingUser = repo.GetUser(username); //ctx.IUserSet.Cast<IUser>().FirstOrDefault(u => u.UserName == username);

                if (existingUser != null)
                {
                    if (existingUser.UserName == username)
                        status = MembershipCreateStatus.DuplicateUserName;
                    else
                        status = MembershipCreateStatus.DuplicateEmail;
                    return null;
                }
                User user = (User)Activator.CreateInstance(typeof(User));
                user.UserName = username;
                user.Password = EncryptPassword(password);
                user.Email = email;
                user.Created = DateTime.UtcNow;
                user.Modified = DateTime.UtcNow;
                user.Id = Guid.NewGuid();


                repo.Set(new List<object> { user }, new Dictionary<string, object>());
                providerUserKey = user.Id;
                status = MembershipCreateStatus.Success;
                return new LightweightMembershipUser(user);
            }
            catch (Exception ex)
            {
                throw new ProviderException("Failed to create user", ex);
            }
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotSupportedException();
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            IUserRepository repo = Repository.Instance.Registered(typeof(User)) as IUserRepository;
            MembershipUserCollection mUsers = new MembershipUserCollection();
            try
            {
                var users = repo.GetUsers(null, emailToMatch, pageIndex, pageSize, out totalRecords);
                foreach (IUser user in users)
                    mUsers.Add(new LightweightMembershipUser(user));
                return mUsers;
            }
            catch (Exception ex)
            {
                throw new ProviderException("Failed to find users by email", ex);
            }

        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            IUserRepository repo = Repository.Instance.Registered(typeof(User)) as IUserRepository;
            MembershipUserCollection mUsers = new MembershipUserCollection();
            try
            {
                var users = repo.GetUsers(usernameToMatch, null, pageIndex, pageSize, out totalRecords);
                foreach (IUser user in users)
                    mUsers.Add(new LightweightMembershipUser(user));
                return mUsers;
            }
            catch (Exception ex)
            {
                throw new ProviderException("Failed to find users by name", ex);
            }
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            IUserRepository repo = Repository.Instance.Registered(typeof(User)) as IUserRepository;
            MembershipUserCollection mUsers = new MembershipUserCollection();
            try
            {
                var users = repo.GetUsers(null, null, pageIndex, pageSize, out totalRecords);
                foreach (IUser user in users)
                    mUsers.Add(new LightweightMembershipUser(user));
                return mUsers;
            }
            catch (Exception ex)
            {
                throw new ProviderException("Failed to find users", ex);
            }
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new ProviderException("Cannot retrieve Hashed passwords.");
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            try
            {
                IUserRepository repo = Repository.Instance.Registered(typeof(User)) as IUserRepository;
                IUser user = repo.GetUser(username);
                return new LightweightMembershipUser(user);
            }
            catch (Exception ex)
            {
                throw new ProviderException("Failed to get user", ex);
            }
        }
        public virtual IUser GetUser(string username, IUserRepository repo)
        {
            try
            {
                IUser user = repo.GetUser(username);
                return user;
            }
            catch (Exception ex)
            {
                throw new ProviderException("Failed to get user", ex);
            }
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            try
            {
                Guid id = Guid.Empty;
                if (providerUserKey is string)
                    id = new Guid(providerUserKey as string);
                else if (providerUserKey is byte[])
                    id = new Guid(providerUserKey as byte[]);
                else if (providerUserKey is Guid)
                    id = (Guid)providerUserKey;
                IUser user = (Repository.Instance.Registered(typeof(User)) as IUserRepository).GetUser(id);
                return new LightweightMembershipUser(user);
            }
            catch (Exception ex)
            {
                throw new ProviderException("Failed to get user", ex);
            }
        }

        public override string GetUserNameByEmail(string email)
        {
            try
            {
                IUser user = (Repository.Instance.Registered(typeof(User)) as IUserRepository).GetUserByEmail(email);
                return user == null ? null : user.UserName;
            }
            catch (Exception ex)
            {
                throw new ProviderException("Failed to get user", ex);
            }
        }

        private int pMinRequiredNonAlphanumericCharacters;

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return pMinRequiredNonAlphanumericCharacters; }
        }

        private int pMinRequiredPasswordLength;

        public override int MinRequiredPasswordLength
        {
            get { return pMinRequiredPasswordLength; }
        }

        private string pPasswordStrengthRegularExpression;

        public override string PasswordStrengthRegularExpression
        {
            get { return pPasswordStrengthRegularExpression; }
        }

        private bool pWriteExceptionsToEventLog;

        public bool WriteExceptionsToEventLog
        {
            get { return pWriteExceptionsToEventLog; }
            set { pWriteExceptionsToEventLog = value; }
        }

        public override string ResetPassword(string username, string answer)
        {
            string newPassword = System.Web.Security.Membership.GeneratePassword(newPasswordLength, MinRequiredNonAlphanumericCharacters);

            ValidatePasswordEventArgs args =
              new ValidatePasswordEventArgs(username, newPassword, true);

            OnValidatingPassword(args);

            if (args.Cancel)
                if (args.FailureInformation != null)
                    throw args.FailureInformation;
                else
                    throw new MembershipPasswordException("Reset password canceled due to password validation failure.");

            try
            {
                IUserRepository repo = Repository.Instance.Registered(typeof(User)) as IUserRepository;
                IUser user = repo.GetUser(username);
                user.Password = EncryptPassword(newPassword);
                repo.Set(new List<object> { user }, new Dictionary<string, object>());
            }
            catch (Exception ex)
            {
                throw new ProviderException("Failed to reset password", ex);
            }

            return newPassword;

        }

        public override bool UnlockUser(string userName)
        {
            return true;
        }

        public override void UpdateUser(MembershipUser mUser)
        {
            try
            {
                IUser user = (IUser)mUser;
                IUserRepository repo = Repository.Instance.Registered(typeof(User)) as IUserRepository;
                repo.Set(new List<object> { user }, new Dictionary<string, object>());
            }
            catch (Exception ex)
            {
                throw new ProviderException("Failed to update user", ex);
            }
        }

        public override bool ValidateUser(string username, string password)
        {
            return ValidateUser(username, password, null, false) != null;
        }
        protected internal virtual IUser ValidateUser(string username, string password, IUserRepository repo, bool isNewUser)
        {
            if (repo == null) repo = Repository.Instance.Registered(typeof(User)) as IUserRepository;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            IUser user = repo.GetUser(username);
            if (user == null) return null;

            var args = new ValidatePasswordEventArgs(username, password, isNewUser);
            OnValidatingPassword(args);
            if (args.Cancel)
                if (args.FailureInformation != null)
                    throw args.FailureInformation;
                else
                    throw new MembershipPasswordException("Change password canceled due to new password validation failure.");

            bool isInitial = (string.IsNullOrEmpty(user.Password) && password == pInitPassword);
            if (isInitial || EncryptPassword(password) == user.Password)
                return user;
            string pwd = user.Password;
            if (AlternateEncrypt != null && AlternateEncrypt(user, password).Contains(pwd))
            {
                return user;
            }

            return null;
        }

        internal IUser GetUser(IUserRepository repo, string username)
        {
            IUser user = null;
            try
            {
                user = repo.GetUser(username);
            }
            catch (Exception ex)
            {
                throw new ProviderException(ex.Message, ex);
            }
            return user;
        }
        internal IUser GetUser(string username)
        {
            return GetUser(Repository.Instance.Registered(typeof(User)) as IUserRepository, username);
        }
    }
}
