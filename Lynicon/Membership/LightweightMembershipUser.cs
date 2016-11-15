using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;

namespace Lynicon.Membership
{
    /// <summary>
    /// Membership user for Lynicon's custom ASP.Net membership provider
    /// </summary>
    public class LightweightMembershipUser : MembershipUser
    {
        public IUser Entity { get; set; }

        public LightweightMembershipUser(IUser user) :
            base("LightweightMembershipProvider",
                  user.UserName,
                  user.IdAsString,
                  user.Email,
                  "", "", true, false,
                  user.Created,
                  user.Modified, user.Modified, user.Modified, DateTime.MinValue)
        {
            this.Entity = user;
        }
    }
}
