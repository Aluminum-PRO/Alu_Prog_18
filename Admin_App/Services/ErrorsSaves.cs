using Admin_App.Classes;
using System;
using System.IO;
using static Admin_App.Classes.StaticVars;

namespace Admin_App.Sevice
{
    internal class ErrorsSaves
    {
        private MyMessageBox myMessageBox = new MyMessageBox();

        private readonly string _path =  $"{_pathErrorsLog}\\Errors Log.txt";

        public void Recording_Errors(Exception ex)
        {
            string Msg = $"{DateTime.Now}\nHResult: {ex.HResult}\nErr: {ex.Message}\nMethod: {ex.TargetSite}\n\n";
            using (StreamWriter stream = new StreamWriter(_path, true))
                stream.WriteLine(Msg);
            string _txt = $"{ex.Message}\n{ex.StackTrace}";
            if (ex.InnerException != null) _txt += $"\n{ex.InnerException.Message}";
            myMessageBox.Error(_txt);
        }
    }
}
