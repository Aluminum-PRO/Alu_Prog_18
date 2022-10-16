using System;
using System.IO;
using System.Windows;
using Users_App.Classes;
using Users_App.MySql_Services;

namespace Users_App.Sevice
{
    internal class ErrorsSaves
    {
        private MySql_Handler MyHandler;
        private readonly string path = StaticVars._pathErrorsLog + @"\Errors Log.txt";

        public void Recording_Errors(Exception ex, bool _bdSend = true)
        {
            string Msg = $"{DateTime.Now}\nHResult: {ex.HResult}\nErr: {ex.Message}\nMethod: {ex.TargetSite}\n\n";
            using (StreamWriter stream = new StreamWriter(path, true))
                stream.WriteLine(Msg);
            if (_bdSend)
            { MyHandler = new MySql_Handler(); MyHandler.SendError(Msg); }
            if (StaticVars._isAdminLaunch)
            {
                string _txt = $"{ex.Message}\n{ex.StackTrace}";
                if (ex.InnerException != null) _txt += $"\n{ex.InnerException.Message}";
                MessageBox.Show(_txt, "User Survalence", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
