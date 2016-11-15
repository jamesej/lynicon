using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lynicon.Models
{
    /// <summary>
    /// Basic illustration of a Switchable inheritor
    /// </summary>
    [Serializable]
    public class SimpleSwitchable : Switchable
    {
        /// <summary>
        /// The holder for the value when the switchable is an html block
        /// </summary>
        public MinHtml Text { get; set; }
        /// <summary>
        /// The holder for the value when the switchable is an image
        /// </summary>
        public Image Image { get; set; }
        /// <summary>
        /// The holder for the value when the switchable is a heading
        /// </summary>
        public BbText Heading { get; set; }

        public SimpleSwitchable()
        {
            Text = new MinHtml();
            Image = new Image();
            Heading = new BbText();
        }
    }
}
