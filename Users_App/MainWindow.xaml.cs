using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Users_App.MySql_Services;
using Users_App.Sevice;
using static Users_App.Classes.StaticVars;
using MessageBox = System.Windows.MessageBox;

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

        private MySql_Handler MyHandler;
        private List<int> _openProcessesListId;
        private static Bitmap BM = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

        private int _logSendingCounter = 0, _logSendCounter = 0;
        private bool _isStareted = true/*, _getProcessing = false*/;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            Visibility = Visibility.Hidden;
            Directory.SetCurrentDirectory(AppContext.BaseDirectory);
            AppLoading();
        }

        private void AppLoading()
        {
            SetAutorunValue(true);
            GetMainDirectiry();
            _pathErrorsLog = CheckErrorsLogDirectory();
            MyHandler = new MySql_Handler();
            try
            {
                if (!MyHandler.GetData())
                {
                    if (_isAdminLaunch)
                        System.Windows.MessageBox.Show("Ошибка связанная с БД. Программа будет закрыта...");
                    Environment.Exit(0);
                }
                CurrentVersionTextBlock.Text = $"cur: {_currentVersionApp}";
                NewVersionTextBlock.Text = $"new: {_newVersionApp}";
                if (_isNeedUpdateApp)
                { Update update = new Update(); update.StartUpdate(); }
                else
                { AppStarted(); }
            }
            catch (Exception ex)
            {
                ErrorsSaves errorsSaves = new ErrorsSaves();
                errorsSaves.Recording_Errors(ex);
            }
        }

        private async void AppStarted()
        {
            _processesList = new List<Process>();
            _completedProcessesList = new List<CompletedProcessesClass>();
            _openProcessesList = new List<OpenProcessesClass>();
            _openProcessesListId = new List<int>();

            GetSurveillanceProcessesLog();
            await Task.Run(() =>
            {
                while (true)
                {
                    GetProcess();
                    //while (!_getProcessing)
                    //    _getProcessing = false;
                    Thread.Sleep(4800);
                }
            });


        }

        private void GetProcess()
        {
            if (_processesList != null)
                _processesList.Clear();

            _processesList = Process.GetProcesses().ToList<Process>();
            CheckOpenProcesses();
            CheckCompletedProcesses();
            SaveSurveillanceProcessesLog();
            _logSendingCounter++;
            Action action = () =>
            { CurrentCountTextBlock.Text = $"{_logSendingCounter}"; CurrentSendCountTextBlock.Text = $"{_logSendCounter}"; };
            if (!Dispatcher.CheckAccess())
                Dispatcher.Invoke(action);
            else
                action();
            if (_isStareted || _logSendingCounter >= _updateFrequencySendLogs)
            {
                _logSendingCounter = 0; _isStareted = false; _logSendCounter++;

                SendSurveillanceProcessesLog();
            }
        }

        private void SendSurveillanceProcessesLog()
        {
            byte[] _imgB;

            if (_currentDay != DateTime.Now.Day)
            {
                _surveillanceProcessesLogId = 0;
                _completedProcessesList.Clear();
                _openProcessesList.Clear();
                _openProcessesListId.Clear();
                _currentDay = DateTime.Now.Day;
            }
            try
            {
                Image_Encoder(BitmapToBitmapImage(ScreenshootSave()), out _imgB);
                MyHandler = new MySql_Handler();
                MyHandler.SendSurveillanceProcessesLog(SaveSurveillanceProcessesLogText(), _imgB);
            }
            catch (Exception ex)
            {
                ErrorsSaves errorsSaves = new ErrorsSaves();
                errorsSaves.Recording_Errors(ex);
            }
        }

        private void CheckOpenProcesses()
        {
            int _countsSeparets = 0; string _processWindowName = "";
            foreach (Process _processList in _processesList)
            {
                bool _checkProcessOpen = false;
                if (_openProcessesList != null)
                {

                    foreach (OpenProcessesClass _openProcessesClass in _openProcessesList)
                    {
                        if (_processList.MainWindowTitle.ToString().Contains('|'))
                        {
                            _countsSeparets = _processList.MainWindowTitle.ToString().Count(f => f == '|');
                            _processWindowName = $"{_processList.MainWindowTitle.Split('|')[_countsSeparets]}";
                        }
                        else
                        {
                            _processWindowName = _processList.MainWindowTitle.ToString();
                        }
                        if (_processList.ProcessName.ToString() == _openProcessesClass._processName && _processWindowName == _openProcessesClass._processWindowName)
                        {
                            _checkProcessOpen = true;
                        }
                    }
                }
                if (!_checkProcessOpen)
                {
                    _openProcessesList.Add(new OpenProcessesClass() { _processName = _processList.ProcessName.ToString(), _processWindowName = _processList.MainWindowTitle.ToString(), _processStartedTime = DateTime.Now, _processBackground = Convert.ToBoolean(_processList.MainWindowHandle == IntPtr.Zero ? "true" : "false") });
                }
            }
        }

        private void CheckCompletedProcesses()
        {
            if (_openProcessesList == null)
                return;
            if (_openProcessesListId != null)
                _openProcessesListId.Clear();
            int k = -1;
            foreach (OpenProcessesClass _openProcessesClass in _openProcessesList)
            {
                bool _checkProcessOpenStatus = false;
                k++;

                foreach (Process _processList in _processesList)
                {
                    if (_openProcessesClass._processName == _processList.ProcessName.ToString())
                    {
                        _checkProcessOpenStatus = true;
                    }
                }

                bool _checkProcessCompletedStatus = false;
                double _secondsTime = 0, _millisecondsTime = 0;
                if (_completedProcessesList != null)
                {
                    foreach (CompletedProcessesClass _completedProcessesClass in _completedProcessesList)
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
                    foreach (String _processSystemClass in _processSystemList)
                    {
                        if (_openProcessesClass._processName == _processSystemClass)
                            _checkProcessSystem = true;
                    }
                    if (!_checkProcessSystem)
                    {
                        foreach (String _processSystemVirusClass in _processSystemVirusList)
                        {
                            if (_openProcessesClass._processName == _processSystemVirusClass)
                                _processType = "Virus";
                        }
                        _completedProcessesList.Add(new CompletedProcessesClass() { _processWindowName = _openProcessesClass._processWindowName, _processName = _openProcessesClass._processName, _processTime = _secondsTime + _millisecondsTime / 100, _processType = _processType, _processBackground = _openProcessesClass._processBackground });
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
                    _openProcessesList.RemoveAt(_openProcessesClassId);
                }
                CheckCompletedProcesses();
            }
        }

        private string SaveSurveillanceProcessesLogText()
        {
            string _outCompletedProcesses = "";
            foreach (CompletedProcessesClass _completedProcessesClass in _completedProcessesList)
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

            int _countsProcess = _todaySurveillanceProcessesLogIn.Count(f => f == '\n');
            int _countsSeparets;
            for (int i = 1; i <= _countsProcess; i++)
            {
                _todaySurveillanceProcessesLogOut = _todaySurveillanceProcessesLogIn.Split('\n')[i - 1];
                try
                {
                    _countsSeparets = _todaySurveillanceProcessesLogOut.Count(f => f == '|');
                    if (_todaySurveillanceProcessesLogOut.Contains('|'))
                        if (_countsSeparets == 4)
                            _completedProcessesList.Add(new CompletedProcessesClass()
                            {
                                _processWindowName = _todaySurveillanceProcessesLogOut.Split('|')[0],
                                _processName = _todaySurveillanceProcessesLogOut.Split('|')[1],
                                _processTime = Convert.ToDouble(_todaySurveillanceProcessesLogOut.Split('|')[2]),
                                _processType = _todaySurveillanceProcessesLogOut.Split('|')[3],
                                _processBackground = Convert.ToBoolean(_todaySurveillanceProcessesLogOut.Split('|')[4])
                            });
                        else if (_countsSeparets > 4)
                            _completedProcessesList.Add(new CompletedProcessesClass()
                            {
                                _processWindowName = _todaySurveillanceProcessesLogOut.Split('|')[_countsSeparets - 4],
                                _processName = _todaySurveillanceProcessesLogOut.Split('|')[_countsSeparets - 3],
                                _processTime = Convert.ToDouble(_todaySurveillanceProcessesLogOut.Split('|')[_countsSeparets - 2]),
                                _processType = _todaySurveillanceProcessesLogOut.Split('|')[_countsSeparets - 1],
                                _processBackground = Convert.ToBoolean(_todaySurveillanceProcessesLogOut.Split('|')[_countsSeparets])
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
            try
            { File.Delete(CheckSurveillanceLogDirectory() + $"{DateTime.Now.AddDays(-1).ToString("d")}.jpeg"); }
            catch
            { }
        }

        private string CheckSurveillanceLogDirectory()
        {
            string _source = _mainPath + "\\Logs\\Surveillance Log\\";
            if (!Directory.Exists(_source))
            {
                Directory.CreateDirectory(_source);
            }
            return _source;
        }

        private string CheckErrorsLogDirectory()
        {
            string _source = _mainPath + "\\Logs\\Errors Log\\";
            if (!Directory.Exists(_source))
            {
                Directory.CreateDirectory(_source);
            }
            return _source;
        }

        private void GetMainDirectiry()
        {
            string _sourceOut = "", _source = Assembly.GetExecutingAssembly().Location;
            int _counts = _source.Count(f => f == '\\'); _counts--;
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
            _mainPath = _sourceOut;
            _pathApp = $"{_sourceOut}\\Users Surveillance App";
            GetApplicationVersion();

            _userIdentyty = Environment.UserName;
            _currentDay = DateTime.Now.Day;
            if (Directory.Exists($"{_mainPath}\\AluAdmin"))
            { Visibility = Visibility.Visible; ShowInTaskbar = true; IsEnabled = true; _isAdminLaunch = true; }

            bool isAdmin = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

            if (isAdmin)
            {
                string _hostsTxt, _hostsPath;
                _hostsPath = @"C:\Windows\System32\drivers\etc\hosts";
                _hostsTxt = File.ReadAllText(_hostsPath);
                if (!_hostsTxt.ToLower().Contains("alsurdb"))
                {
                    _hostsTxt += $"\n192.168.1.69\t AlSurDb";
                    using (StreamWriter _stream = new StreamWriter(_hostsPath))
                        _stream.WriteLine(_hostsTxt);
                    MessageBox.Show($" Файл \"{_hostsPath}\" изменён, добавлен домен \"AlSurDb\"\n\n Пожалуйста Дмитрий!", "Users Surveillance", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
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
            try
            { File.Delete(CheckSurveillanceLogDirectory() + $"{DateTime.Now.ToString("d")}.jpeg"); }
            catch
            { }
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

        private void GetApplicationVersion()
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        { SendSurveillanceProcessesLog(); }

        private void Border_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        { try { DragMove(); } catch { } }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        { Update update = new Update(); update.StartUpdate(); }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        { if (File.Exists(_pathErrorsLog + @"\Errors Log.txt")) try { Process.Start(_pathErrorsLog + @"\Errors Log.txt"); } catch (Exception ex) { ErrorsSaves errorsSaves = new ErrorsSaves(); errorsSaves.Recording_Errors(ex); } }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        { Environment.Exit(0); }
    }
}
