using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Users_App.Classes
{
    class Internet
    {
        public static bool OK()
        {
            try
            {
                Dns.GetHostEntry("dotnet.beget.tech");
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
