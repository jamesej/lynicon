using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Lynicon.AutoTests
{
    [SetUpFixture]
    public class Setup
    {
        [OneTimeSetUp]
        public static void GlobalInit()
        {
            //var db = new PreloadDb();
            //db.Database.ExecuteSqlCommand("DELETE FROM TestData");
            //db.Database.ExecuteSqlCommand("DELETE FROM ContentItems WHERE DataType IN ('Lynicon.Test.Models.UrlRedirectContent','Lynicon.Test.Models.SearchContent', 'Lynicon.Test.Models.HeaderContent', 'Lynicon.Test.Models.Sub1TContent', 'Lynicon.Test.Models.Sub2TContent', 'Lynicon.Test.Models.PropertyRedirectContent', 'Lynicon.Test.Models.RefTargetContent', 'Lynicon.Test.Models.RefContent')");

            LyniconConfig.Run();
        }
    }
}
