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
        List<int> _openProcessesListId;

        public MainWindow()
        {
            InitializeComponent();
            SetAutorunValue(true);
            StaticVars._mainSource = GetMainDirectiry();

            GetProcess();
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
            StaticVars._processesList = new List<Process>();
            StaticVars._completedProcessesList = new List<StaticVars.CompletedProcessesClass>();
            StaticVars._openProcessesList = new List<StaticVars.OpenProcessesClass>();
            _openProcessesListId = new List<int>();

            while (true)
            {
                if (StaticVars._processesList != null)
                    StaticVars._processesList.Clear();

                StaticVars._processesList = Process.GetProcesses().ToList<Process>();
                CheckOpenProcesses();
                CheckCompletedProcesses();
                SaveSurveillanceProcessesLog();

                await Task.Delay(445);
            }
        }

        private void CheckOpenProcesses()
        {

            foreach (Process _processList in StaticVars._processesList)
            {
                bool _checkProcessOpen = false;
                if (StaticVars._openProcessesList != null)
                {
                    foreach (StaticVars.OpenProcessesClass _openProcessesClass in StaticVars._openProcessesList)
                    {
                        if (_processList.ProcessName.ToString() == _openProcessesClass._processName)
                        {
                            _checkProcessOpen = true;
                        }
                    }
                }
                    
                if (!_checkProcessOpen)
                {
                    StaticVars._openProcessesList.Add(new StaticVars.OpenProcessesClass() { _processName = _processList.ProcessName.ToString(), _processStartedTime = DateTime.Now });
                }
            }
        }

        private void CheckCompletedProcesses()
        {
            if (StaticVars._openProcessesList == null)
                return;
            if (_openProcessesListId != null)
                _openProcessesListId.Clear();
            int k = -1;
            foreach (StaticVars.OpenProcessesClass _openProcessesClass in StaticVars._openProcessesList)
            {
                bool _checkProcessOpenStatus = false;
                k++;

                foreach (Process _processList in StaticVars._processesList)
                {
                    if (_openProcessesClass._processName == _processList.ProcessName.ToString())
                    {
                        _checkProcessOpenStatus = true;
                    }
                }
                if (!_checkProcessOpenStatus)
                {
                    bool _checkProcessCompletedStatus = false;
                    int _minetsTime = 0, _secondsTime = 0;
                    if (StaticVars._completedProcessesList != null)
                    {
                        foreach (StaticVars.CompletedProcessesClass _completedProcessesClass in StaticVars._completedProcessesList)
                        {
                            if (_openProcessesClass._processName == _completedProcessesClass._processName)
                            {
                                _checkProcessCompletedStatus = true;
                                _minetsTime = Convert.ToInt32((DateTime.Now - _openProcessesClass._processStartedTime).ToString("mm"));
                                _secondsTime = Convert.ToInt32((DateTime.Now - _openProcessesClass._processStartedTime).ToString("ss"));
                                
                                _completedProcessesClass._processTime += _minetsTime*60 + _secondsTime;
                            }
                        }
                    }
                    
                    if (!_checkProcessCompletedStatus)
                    {
                        _minetsTime = Convert.ToInt32((DateTime.Now - _openProcessesClass._processStartedTime).ToString("mm"));
                        _secondsTime = Convert.ToInt32((DateTime.Now - _openProcessesClass._processStartedTime).ToString("ss"));

                        StaticVars._completedProcessesList.Add(new StaticVars.CompletedProcessesClass() { _processName = _openProcessesClass._processName, _processTime = _minetsTime * 60 + _secondsTime });
                    }
                    _openProcessesListId.Add(k);
                    break;
                }
            }
            if (_openProcessesListId.Count > 0)
            {
                foreach (int _openProcessesClassId in _openProcessesListId)
                {
                    StaticVars._openProcessesList.RemoveAt(_openProcessesClassId);
                }
                CheckCompletedProcesses();
            }
        }

        private void SaveSurveillanceProcessesLog()
        {
            string _outCompletedProcesses = "";
            foreach (StaticVars.CompletedProcessesClass _completedProcessesClass in StaticVars._completedProcessesList)
            {
                _outCompletedProcesses += $"{_completedProcessesClass._processName}|{_completedProcessesClass._processTime}\n";
            }
            using (StreamWriter _stream = new StreamWriter(CheckSurveillanceLogDirectory() + $"{DateTime.Now.ToString("d")}.txt"))
                _stream.WriteLine(_outCompletedProcesses);
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
