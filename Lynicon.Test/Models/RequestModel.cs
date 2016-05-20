using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lynicon.Test.Models
{
    public class Request
    {
        public int ID { get; set; }

        [Required]
        [UIHint("OEM")]
        public string OEM { get; set; }

        [Required]
        [DisplayName("City")]
        public string LocationCity { get; set; }

        public Request()
        {

        }
    }

    public class RequestCreateViewModel
    {
        public int ID { get; set; }
        public TesterLocationCreateViewModel testerLocation { get; set; }

        public RequestCreateViewModel()
        {

        }
    }

    public class TesterLocationCreateViewModel
    {
        public int ID { get; set; }
        public string OEM { get; set; }
        public string LocationCity { get; set; }

        public TesterLocationCreateViewModel()
        {

        }
    }
}