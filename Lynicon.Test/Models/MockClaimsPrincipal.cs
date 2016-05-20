using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Security;
using Lynicon.Membership;

namespace Lynicon.Test.Models
{
    public class MockClaimsPrincipal : ClaimsPrincipal
    {
        public string Roles { get; set; }

        public MockClaimsPrincipal(string roles) : base()
        {
            Roles = roles;
        }

        public override bool IsInRole(string role)
        {
            return Roles.Contains(role);
        }
    }
}