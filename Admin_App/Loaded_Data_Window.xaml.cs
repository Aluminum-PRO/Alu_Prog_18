using Admin_App.Classes;
using Admin_App.MySql_Services;
using Admin_App.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using static Admin_App.Classes.StaticVars;

namespace Admin_App
{
    /// <summary>
    /// Логика взаимодействия для Loaded_Data_Window.xaml
    /// </summary>
    public partial class Loaded_Data_Window : Window
    {
        private MySql_Handler My_Hand;
        private MyMessageBox myMessageBox = new MyMessageBox();
        private Handler handler;

        private bool Copy_Check = false;
        public Loaded_Data_Window()
        {
            InitializeComponent();
            Directory.SetCurrentDirectory(AppContext.BaseDirectory);
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length == 1)
            {
                Copy_Check = true;
            }
            else
            {
                foreach (string element in args)
                {
                    if (element == "/Hi")
                    {
                        myMessageBox.Information($"Hi, Admin!{_messageInfoClose}");
                        Environment.Exit(0);
                    }
                    if (element == "/No_Password")
                    {
                        myMessageBox.Information($"Пароль сброшен!"); //TODO: Нужно добавить сброс пароля
                    }
                }
            }

            if (Copy_Check)
            {
                Process[] processList = Process.GetProcessesByName("Surveillance Admin");
                if (processList.Length > 1)
                { Environment.Exit(0); }
            }
            AppLoading();
        }

        private void AppLoading()
        {
            Thread thread = new Thread(() =>
            {
                Logo_Window logo_Window = new Logo_Window();
                logo_Window.Show();

                logo_Window.Closed += (sender2, e2) =>
                    logo_Window.Dispatcher.InvokeShutdown();

                System.Windows.Threading.Dispatcher.Run();
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

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
            _pathApp = $"{_sourceOut}\\Surveillance Admin";
            _pathShortcut = $"{_pathApp}\\lib\\Иконки ярлыков";
            _pathErrorsLog = $"{_sourceOut}\\Logs\\Errors Log"; CheckCreateDirectory(_pathErrorsLog);
            _userIdentyty = Environment.UserName;

            handler = new Handler();
            handler.GetApplicationVersion();

            if (!Internet.OK())
            {
                MessageBoxResult _result = MessageBox.Show(" Нет доступа в сеть, дальнейшая работа \"Surveillance Admin\" не возможна, продолжить загрузку?", "Surveillance Admin", MessageBoxButton.YesNo, MessageBoxImage.Error);
                if (_result == MessageBoxResult.Yes) { DownloadDataProcess(); } else { Environment.Exit(0); }
            }
            else if (Internet.OK())
            {
                DownloadDataProcess();
            }
        }

        private void DownloadDataProcess()
        {
            _isLoadingData = false;
            My_Hand = new MySql_Handler();
            My_Hand.GetData();
            _generalUsersLogList = new List<CurrentUsersClass>();
            _generalUsersList = new List<GeneralUsersClass>();
            My_Hand.GetSurveillanceLogs();

                while (true)
                    if (_isLoadingData)
                        break;

            AppStarted();
        }

        private void CheckCreateDirectory(string _source)
        { if (!Directory.Exists(_source)) Directory.CreateDirectory(_source); }

        private void CheckShortcutCreate()
        {
            if (!File.Exists("C:\\Users\\" + _userIdentyty + "\\Desktop\\Surveillance Admin.lnk") && _startCreatingShortcut == true)
            {
                Activate();
                MessageBoxResult result = MessageBox.Show(" На вашем рабочем столе нет ярлыка 'Surveillance Admin'.\n\n   Создать?", "Surveillance Admin", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    handler.CreateShortcut("Surveillance Admin", "Surveillance Admin - Просмотр всех запущенных процессов у пользователей", _currentVersionApp, "Ctrl+Shift+S");
                }
                else if (result == MessageBoxResult.No)
                {
                    MessageBoxResult _result = MessageBox.Show(" Не предлагать больше этот вопрос?", "Surveillance Admin", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (_result == MessageBoxResult.Yes)
                    {
                        _startCreatingShortcut = false;
                        My_Hand.Set_Properties("StartCreatingShortcut", false, out bool Result);
                        if (!Result) { _startCreatingShortcut = true; myMessageBox.Exclamation($" Не удалось обновить данные{_messageInfoContacts}"); }
                    }
                }
            }
            else if (File.Exists("C:\\Users\\" + _userIdentyty + "\\Desktop\\Al-Store.lnk") && Properties.Settings.Default.First_Started == true)
            {
                Properties.Settings.Default.First_Started = false;
                Properties.Settings.Default.Save();

                handler.CreateShortcut("Surveillance Admin", "Surveillance Admin - Просмотр всех запущенных процессов у пользователей", _currentVersionApp, "Ctrl+Shift+S");
            }
        }
        private void AppStarted()
        {
            while (true)
            {
                if (!_logoAnimation)
                { break; }
            }
            Thread.Sleep(800);
            CheckShortcutCreate();

            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }
    }
}
