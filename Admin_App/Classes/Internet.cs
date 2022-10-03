using System.Net;

namespace Admin_App.Classes
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
