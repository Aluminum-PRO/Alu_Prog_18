using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users_App.Classes;

namespace Users_App.Sevice
{
    internal class ErrorsSaves
    {
        private readonly string path = StaticVars._pathErrorsLog + @"\Errors Log.txt";

        public void Recording_Errors(Exception ex)
        {
            string Msg = $"{DateTime.Now}\nHResult: {ex.HResult}\nErr: {ex.Message}\nMethod: {ex.TargetSite}\n\n";
            using (StreamWriter stream = new StreamWriter(path, true))
                stream.WriteLine(Msg);
            //SendErrors
        }
    }
}
