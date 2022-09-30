using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Users_App.Classes;
using Users_App.MySql_Services;

namespace Users_App
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MySql_Handler my_Handler;
        private string Source = "";
        public MainWindow()
        {
            InitializeComponent();
            SetAutorunValue(true);
            StaticVars._mainSource = GetMainDirectiry();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //my_Handler = new MySql_Handler();
            //my_Handler.Getting_Update_Data(Source);

            //try
            //{
            //    Process process = Process.Start(new ProcessStartInfo
            //    {
            //        FileName = Source += "\\Al-Store\\Al-Store.exe",
            //        //Arguments = "/Hi"
            //        Arguments = "/AutoRun_Update"
            //    });
            //}
            //catch (Exception Ex) { MessageBox.Show("    Авто обновление Al-Store не запущено по причине ниже. Если вам мешает окно подтверждения, вы можете отключить функцию автообновления и уведомдения об обновлении в настройках обновления Al-Store.\n\n    Причина: " + Ex.Message, "Auto Update Al-Store", MessageBoxButton.OK, MessageBoxImage.Error); }
            //Close();


        }

        private async void GetProcess()
        {
            while (true)
            {
                StaticVars.processesList.Clear();

                StaticVars.processesList = Process.GetProcesses().ToList<Process>();

                foreach (Process _processList in StaticVars.processesList)
                {
                    bool _checkProcess = false;
                    foreach (List<> _openProcessesClass)
                    {

                    }
                    _processList.ProcessName.ToString();
                }


                await Task.Delay(990);
            }

            

            
        }

        private string GetReplacement(int _day)
        {
            try
            { File.Delete(CheckSurveillanceLogDirectory() + $"{DateTime.Now.AddDays(-1).ToString("d")}.xlsx"); }
            catch
            { }

            
            return "";
        }

        private string CheckSurveillanceLogDirectory()
        {
            string _source = StaticVars._mainSource + "\\lib\\Surveillance Log\\";
            if (!Directory.Exists(_source))
            {
                Directory.CreateDirectory(_source);
            }
            return _source;
        }

        private string GetMainDirectiry()
        {
            string _sourceOut = "", _source = Assembly.GetExecutingAssembly().Location;
            int _counts = _source.Count(f => f == '\\');
            for (int i = 1; i <= _counts; i++)
            {
                if (i != _counts)
                {
                    _sourceOut += _source.Split('\\')[i - 1] + '\\';
                }
                else if (i == _counts)
                {
                    _sourceOut += _source.Split('\\')[i - 1];
                }
            }
            return _sourceOut;
        }

        private bool SetAutorunValue(bool autorun)
        {
            string ExePath = Assembly.GetExecutingAssembly().Location;
            RegistryKey reg;
            reg = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run\\");
            try
            {
                string name = "Users Surveillance";
                if (autorun)
                {
                    reg.SetValue(name, ExePath);

                }
                else
                    reg.DeleteValue(name);

                reg.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
