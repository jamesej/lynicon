using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Security;

namespace Lynicon.Membership
{
    /// <summary>
    /// Identity for Lynicon's custom ASP.Net membership provider
    /// </summary>
    [Serializable]
    public class LightweightIdentity : IIdentity, ISerializable
    {
        FormsAuthenticationTicket ticket;

        public LightweightIdentity(FormsAuthenticationTicket ticket)
        {
            this.ticket = ticket;
        }

        public FormsAuthenticationTicket Ticket { get { return ticket; } }

        #region IIdentity Members

        public string AuthenticationType
        {
            get { return "Custom"; }
        }

        public bool IsAuthenticated
        {
            get { return true; }
        }

        public string Name
        {
            get { return ticket.Name; }
        }

        #endregion

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (context.State == StreamingContextStates.CrossAppDomain)
            {
                GenericIdentity gIdent = new GenericIdentity(this.Name, this.AuthenticationType);
                info.SetType(gIdent.GetType());

                System.Reflection.MemberInfo[] serializableMembers;
                object[] serializableValues;

                serializableMembers = FormatterServices.GetSerializableMembers(gIdent.GetType());
                serializableValues = FormatterServices.GetObjectData(gIdent, serializableMembers);

                for (int i = 0; i < serializableMembers.Length; i++)
                {
                    info.AddValue(serializableMembers[i].Name, serializableValues[i]);
                }
            }
            else
            {
                throw new InvalidOperationException("Serialization not supported");
            }
        }

        #endregion
    }
}
