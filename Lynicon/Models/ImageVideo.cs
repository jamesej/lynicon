using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lynicon.Models
{
    /// <summary>
    /// A content type which can either contain an image or a video via the Switchable type
    /// </summary>
    [Serializable]
    public class ImageVideo : Switchable
    {
        public Image Image { get; set; }
        public Video Video { get; set; }

        public ImageVideo()
        {
            BaseContent.InitialiseProperties(this);
        }
    }
}
