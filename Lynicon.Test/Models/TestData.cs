using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Lynicon.Attributes;

namespace Lynicon.Test.Models
{
    [Table("TestData"), Serializable]
    public class TestData
    {
        [Key]
        public int Id { get; set; }

        [AddressComponent(UsePath=true)]
        public string Path { get; set; }

        public string Value1 { get; set; }
    }
}