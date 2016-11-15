using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lynicon.Routing
{
    /// <summary>
    /// Utility routing class which can be used as the value of a default route value in order to
    /// generate a value for it at the time the route is being matched to a request, rather than
    /// the time the route is initially created
    /// </summary>
    public class DynamicRouteValue
    {
        private Func<string> generateValue { get; set; }

        /// <summary>
        /// Create a dynamic route value whose value is the value of the given function which returns
        /// a string
        /// </summary>
        /// <param name="generateValue">Function returning the value of the DynamicRouteValue</param>
        public DynamicRouteValue(Func<string> generateValue)
        {
            this.generateValue = generateValue;
        }

        public override string ToString()
        {
            return generateValue();
        }
    }
}
