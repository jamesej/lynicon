using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lynicon.Models;

namespace Lynicon.Base.Models
{
    public class ListingSummary : Lynicon.Models.Summary
    {
        public string Title { get; set; }
        public Image Image { get; set; }

        public ListingSummary()
        {
            Image = new Image();
        }
    }
}
