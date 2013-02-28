using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Lynicon.Membership;

namespace Lynicon.Test.Models
{
    public class ExtendedUser : User
    {
        public string ExtData { get; set; }
    }
}