using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lynicon.Models;

namespace Lynicon.Models
{
    /// <summary>
    /// A summary with an image to assist in displaying lists
    /// </summary>
    [Serializable]
    public class ListingSummary : Lynicon.Models.Summary
    {
        /// <summary>
        /// Image of the item suitable for listing
        /// </summary>
        public Image Image { get; set; }

        public ListingSummary()
        {
            Image = new Image();
        }
    }
}
