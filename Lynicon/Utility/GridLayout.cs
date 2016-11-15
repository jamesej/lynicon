using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Lynicon.Utility
{
    public static class GridLayout
    {
        public static MvcHtmlString DistributeRows(int colsStart, int steps, int idx)
        {
            StringBuilder sb = new StringBuilder();
            for (int step = 0; step < steps; step++)
            {
                if (step > 0)
                    sb.Append(";");
                sb.Append((idx / (colsStart + step)).ToString() + "," + (idx % (colsStart + step)).ToString());
            }
            return new MvcHtmlString(sb.ToString());
        }
    }
}
