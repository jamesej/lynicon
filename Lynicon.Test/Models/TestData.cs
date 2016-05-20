using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Lynicon.Attributes;
using Lynicon.Models;

namespace Lynicon.Test.Models
{
    [Serializable]
    public class TestDataSummary : Summary
    {
        public string Value1 { get; set; }
    }

    [Table("TestData"), Serializable, SummaryType(typeof(TestDataSummary))]
    public class TestData
    {
        [Key]
        public int Id { get; set; }

        [AddressComponent(UsePath=true)]
        public string Path { get; set; }

        [Summary]
        public string Title { get; set; }

        public int ValueInt { get; set; }

        [Summary]
        public string Value1 { get; set; }
    }
}