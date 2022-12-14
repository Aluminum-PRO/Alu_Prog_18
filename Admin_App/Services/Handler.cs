using Admin_App.Classes;
using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using static Admin_App.Classes.StaticVars;

namespace Admin_App.Services
{
    internal class Handler
    {
        [System.Runtime.InteropServices.DllImport("Shell32.dll")]
        private static extern int SHChangeNotify(int eventId, int flags, IntPtr item1, IntPtr item2);

        public void CreateShortcut(string app_name, string short_description, string version, string hot_key)
        {
            if (System.IO.File.Exists("C:\\Users\\" + StaticVars._userIdentyty + "\\Desktop\\" + app_name + ".lnk"))
            {
                System.IO.File.Delete("C:\\Users\\" + StaticVars._userIdentyty + "\\Desktop\\" + app_name + ".lnk");
            }

            object shDesktop = "Desktop";
            WshShell shell = new WshShell();
            string shortcutAddress = (string)shell.SpecialFolders.Item(ref shDesktop) + @"\" + app_name + ".lnk";
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutAddress);
            double size = Counter(StaticVars._mainPath);
            size /= 1048576;
            shortcut.Description = "Описание файла: " + short_description +
                "\nОрганизация: Aluminum-Company" +
                "\nВерсия файла: " + version +
                "\nДата создания: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm") +
                "\nРазмер: " + size.ToString("0.00") + " МБ";
            shortcut.Hotkey = hot_key;
            shortcut.TargetPath = StaticVars._mainPath + "\\" + app_name + ".exe";
            //shortcut.Arguments = "\"C:\\Program Files (x86)\\My Program\\Prog.accdr\"  /runtime";

            if (System.IO.File.Exists(StaticVars._mainPath + "\\" + app_name + ".exe"))
            {
                if (System.IO.File.Exists(StaticVars._mainPath + "\\lib\\Иконки ярлыков\\" + app_name + ".ico"))
                {
                    shortcut.IconLocation = StaticVars._mainPath + "\\lib\\Иконки ярлыков\\" + app_name + ".ico";
                }
                else
                {
                    MessageBox.Show("Ошибка");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Ошибка");
                return;
            }
            SHChangeNotify(0x8000000, 0x2000, IntPtr.Zero, IntPtr.Zero);

            shortcut.Save();
        }

        public void GetApplicationVersion()
        {
            string _stage = "", _version = Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
            if (Convert.ToInt32(_version.Split('.')[0]) != 0)
            { _stage = "Release"; }
            else if (Convert.ToInt32(_version.Split('.')[1]) != 0)
            { _stage = "Beta"; }
            _version = Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
            if (Convert.ToInt32(_version.Split('.')[2]) != 0)
                _currentVersionApp = $"{_version}.{_stage}";
            else
                _currentVersionApp = $"{Assembly.GetExecutingAssembly().GetName().Version.ToString(2)}.{_stage}";
        }

        public void GettingInformationAboutNeedUpdate()
        {
            if (Convert.ToDouble($"{_currentVersionApp.Split('.')[0]}.{_currentVersionApp.Split('.')[1]}", CultureInfo.InvariantCulture) < Convert.ToDouble($"{_newVersionApp.Split('.')[0]}.{_newVersionApp.Split('.')[1]}", CultureInfo.InvariantCulture))
            {
                _isNeedUpdateApp = true;
            }
            else if (Convert.ToDouble($"{_currentVersionApp.Split('.')[0]}.{_currentVersionApp.Split('.')[1]}", CultureInfo.InvariantCulture) > Convert.ToDouble($"{_newVersionApp.Split('.')[0]}.{_newVersionApp.Split('.')[1]}", CultureInfo.InvariantCulture))
            {
                _isCurrentVersionAppNewer = true;
            }
            else if (Convert.ToDouble($"{_currentVersionApp.Split('.')[0]}.{_currentVersionApp.Split('.')[1]}", CultureInfo.InvariantCulture) == Convert.ToDouble($"{_newVersionApp.Split('.')[0]}.{_newVersionApp.Split('.')[1]}", CultureInfo.InvariantCulture))
            {
                if (_currentVersionApp.Count(f => f == '.') == 2 && _newVersionApp.Count(f => f == '.') == 3)
                {
                    _isNeedUpdateApp = true;
                }
                else if (_currentVersionApp.Count(f => f == '.') == 3 && _newVersionApp.Count(f => f == '.') == 2)
                {
                    _isCurrentVersionAppNewer = true;
                }
                else if (_currentVersionApp.Count(f => f == '.') == 3 && _newVersionApp.Count(f => f == '.') == 3)
                {
                    if (Convert.ToInt32(_currentVersionApp.Split('.')[2], CultureInfo.InvariantCulture) < Convert.ToInt32(_newVersionApp.Split('.')[2], CultureInfo.InvariantCulture))
                        _isNeedUpdateApp = true;
                    if (Convert.ToInt32(_currentVersionApp.Split('.')[2], CultureInfo.InvariantCulture) > Convert.ToInt32(_newVersionApp.Split('.')[2], CultureInfo.InvariantCulture))
                        _isCurrentVersionAppNewer = true;
                }
            }
        }

        //public void SaveSettings()
        //{
        //    using (StreamWriter _stream = new StreamWriter($"{StaticVars._patchSettings}\\Settings.txt"))
        //        _stream.WriteLine($"{StaticVars.Start_Creating_Shortcut}\n");
        //}

        //public void GetSettings()
        //{
        //    string _settingsTxt;
        //    if (File.Exists($"{StaticVars._patchSettings}\\Settings.txt"))
        //    {
        //        _settingsTxt = File.ReadAllText($"{StaticVars._patchSettings}\\Settings.txt");
        //        StaticVars.StartCreatingShortcut = Convert.ToBoolean(_settingsTxt.Split('\n')[0]);
        //    }
        //}

        private double Counter(string path)
        {
            double Size = 0;
            IEnumerable<FileInfo> fileInfos = new DirectoryInfo(path)
                .EnumerateFiles("*.*", SearchOption.AllDirectories);

            foreach (FileInfo fi in fileInfos)
            { Size += fi.Length; }

            return Size;
        }
    }
}
