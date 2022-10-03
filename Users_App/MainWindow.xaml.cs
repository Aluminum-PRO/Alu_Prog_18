using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Users_App.Classes;
using Users_App.MySql_Services;
using Users_App.Sevice;

namespace Users_App
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Variables
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        private MySql_Handler my_Handler;
        private List<int> _openProcessesListId;
        private static Bitmap BM = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            SetAutorunValue(true);
            StaticVars._mainSource = GetMainDirectiry();
            StaticVars._pathErrorsLog = CheckErrorsLogDirectory();
            my_Handler = new MySql_Handler();
            GetProcess();
        }

        private async void GetProcess()
        {
            StaticVars._processesList = new List<Process>();
            StaticVars._completedProcessesList = new List<StaticVars.CompletedProcessesClass>();
            StaticVars._openProcessesList = new List<StaticVars.OpenProcessesClass>();
            _openProcessesListId = new List<int>();

            GetSurveillanceProcessesLog();
            int _screenshootTime = 0;
            while (true)
            {
                if (StaticVars._processesList != null)
                    StaticVars._processesList.Clear();

                StaticVars._processesList = Process.GetProcesses().ToList<Process>();
                CheckOpenProcesses();
                CheckCompletedProcesses();
                SaveSurveillanceProcessesLog();
                _screenshootTime++;
                if (_screenshootTime >= 600)
                {
                    _screenshootTime = 0;
                    
                    SendSurveillanceProcessesLog();
                }
                await Task.Delay(445);
            }
        }

        private void SendSurveillanceProcessesLog()
        {
            byte[] _imgB;

            Image_Encoder(BitmapToBitmapImage(ScreenshootSave()), out _imgB);
            my_Handler.SendSurveillanceProcessesLog(SaveSurveillanceProcessesLogText(), _imgB);
            System.Windows.Forms.MessageBox.Show("Test");
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
                        if (_processList.ProcessName.ToString() == _openProcessesClass._processName && _processList.MainWindowTitle.ToString() == _openProcessesClass._processWindowName)
                        {
                            _checkProcessOpen = true;
                        }
                    }
                }
                if (!_checkProcessOpen)
                {
                    StaticVars._openProcessesList.Add(new StaticVars.OpenProcessesClass() { _processName = _processList.ProcessName.ToString(), _processWindowName = _processList.MainWindowTitle.ToString(), _processStartedTime = DateTime.Now, _processBackground = Convert.ToBoolean(_processList.MainWindowHandle == IntPtr.Zero ? "true" : "false") });
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

                bool _checkProcessCompletedStatus = false;
                double  _secondsTime = 0, _millisecondsTime = 0;
                if (StaticVars._completedProcessesList != null)
                {
                    foreach (StaticVars.CompletedProcessesClass _completedProcessesClass in StaticVars._completedProcessesList)
                    {
                        if (_openProcessesClass._processName == _completedProcessesClass._processName && _openProcessesClass._processWindowName == _completedProcessesClass._processWindowName)
                        {
                            _checkProcessCompletedStatus = true;
                            _secondsTime = Convert.ToInt32((DateTime.Now - _openProcessesClass._processStartedTime).TotalSeconds);
                            _millisecondsTime = Convert.ToInt32((DateTime.Now - _openProcessesClass._processStartedTime).ToString("ff"));

                            _completedProcessesClass._processTime += _secondsTime + _millisecondsTime / 100;
                            _completedProcessesClass._processBackground = _openProcessesClass._processBackground;
                        }
                    }
                }

                if (!_checkProcessCompletedStatus)
                {
                    _secondsTime = Convert.ToInt32((DateTime.Now - _openProcessesClass._processStartedTime).TotalSeconds);
                    _millisecondsTime = Convert.ToInt32((DateTime.Now - _openProcessesClass._processStartedTime).ToString("ff"));
                    bool _checkProcessSystem = false;
                    string _processType = "App";
                    foreach (String _processSystemClass in StaticVars._processSystemList)
                    {
                        if (_openProcessesClass._processName == _processSystemClass)
                            _checkProcessSystem = true;
                    }
                    if (!_checkProcessSystem)
                    {
                        foreach (String _processSystemVirusClass in StaticVars._processSystemVirusList)
                        {
                            if (_openProcessesClass._processName == _processSystemVirusClass)
                                _processType = "Virus";
                        }
                        StaticVars._completedProcessesList.Add(new StaticVars.CompletedProcessesClass() { _processWindowName = _openProcessesClass._processWindowName, _processName = _openProcessesClass._processName, _processTime = _secondsTime + _millisecondsTime / 100, _processType = _processType, _processBackground = _openProcessesClass._processBackground });
                    }
                }

                _openProcessesClass._processStartedTime = DateTime.Now;
                if (!_checkProcessOpenStatus)
                {
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

        private string SaveSurveillanceProcessesLogText()
        {
            string _outCompletedProcesses = "";
            foreach (StaticVars.CompletedProcessesClass _completedProcessesClass in StaticVars._completedProcessesList)
            {
                _outCompletedProcesses += $"{_completedProcessesClass._processWindowName}|{_completedProcessesClass._processName}|{_completedProcessesClass._processTime.ToString("0.##")}|{_completedProcessesClass._processType}|{_completedProcessesClass._processBackground}\n";
            }
            return _outCompletedProcesses;
        }

        private void SaveSurveillanceProcessesLog()
        {
            using (StreamWriter _stream = new StreamWriter(CheckSurveillanceLogDirectory() + $"{DateTime.Now.ToString("d")}.txt"))
                _stream.WriteLine(SaveSurveillanceProcessesLogText());
        }

        private void GetSurveillanceProcessesLog()
        {
            string _todaySurveillanceProcessesLogIn = "", _todaySurveillanceProcessesLogOut = "";
            if (File.Exists(CheckSurveillanceLogDirectory() + $"{DateTime.Now.ToString("d")}.txt"))
            {
                _todaySurveillanceProcessesLogIn = File.ReadAllText(CheckSurveillanceLogDirectory() + $"{DateTime.Now.ToString("d")}.txt");
            }

            int _counts = _todaySurveillanceProcessesLogIn.Count(f => f == '\n');
            for (int i = 1; i <= _counts; i++)
            {
                _todaySurveillanceProcessesLogOut = _todaySurveillanceProcessesLogIn.Split('\n')[i - 1];
                try
                {
                    if (_todaySurveillanceProcessesLogOut.Contains('|'))
                        StaticVars._completedProcessesList.Add(new StaticVars.CompletedProcessesClass()
                        {
                            _processWindowName = _todaySurveillanceProcessesLogOut.Split('|')[0],
                            _processName = _todaySurveillanceProcessesLogOut.Split('|')[1],
                            _processTime = Convert.ToDouble(_todaySurveillanceProcessesLogOut.Split('|')[2]),
                            _processType = _todaySurveillanceProcessesLogOut.Split('|')[3],
                            _processBackground = Convert.ToBoolean(_todaySurveillanceProcessesLogOut.Split('|')[4])
                        });
                }
                catch (Exception ex)
                {
                    ErrorsSaves errorsSaves = new ErrorsSaves();
                    errorsSaves.Recording_Errors(ex);
                }
            }

            try
            { File.Delete(CheckSurveillanceLogDirectory() + $"{DateTime.Now.AddDays(-1).ToString("d")}.txt"); }
            catch
            { }
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

        private string CheckErrorsLogDirectory()
        {
            string _source = StaticVars._mainSource + "\\lib\\Errors Log\\";
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

        private static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }

        private Bitmap ScreenshootSave()
        {
            Graphics GH = Graphics.FromImage(BM as Image);
            GH.CopyFromScreen(0, 0, 0, 0, BM.Size);
            BM.Save(CheckSurveillanceLogDirectory() + $"{DateTime.Now.ToString("d")}.jpeg");
            return BM;
        }

        private void Image_Encoder(BitmapImage bi, out byte[] _imgB)
        {
            PngBitmapEncoder pbe = new PngBitmapEncoder();
            MemoryStream ms = new MemoryStream();
            pbe.Frames.Add(BitmapFrame.Create(bi));
            pbe.Save(ms);
            byte[] imgB = ms.ToArray();
            _imgB = imgB;
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
