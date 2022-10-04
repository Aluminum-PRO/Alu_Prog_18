using Admin_App.Classes;
using Admin_App.MySql_Services;
using Admin_App.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;

namespace Admin_App
{
    /// <summary>
    /// Логика взаимодействия для Loaded_Data_Window.xaml
    /// </summary>
    public partial class Loaded_Data_Window : Window
    {
        private MySql_Handler My_Hand;
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
                        MessageBox.Show("Hi, Admin!", "Al-Store", MessageBoxButton.YesNoCancel, MessageBoxImage.Information);
                        Environment.Exit(0);
                    }
                    if (element == "/AutoRun_Update")
                    {
                        //AutoRun_Update = true;
                    }
                }
            }

            if (Copy_Check)
            {
                Process[] processList = Process.GetProcessesByName("Surveillance Admin");
                if (processList.Length > 1)
                { Environment.Exit(0); }
            }
            Load();
        }

        private void Load()
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
            StaticVars._mainPath = _sourceOut;
            StaticVars._pathApp = $"{_sourceOut}\\Surveillance Admin";
            StaticVars._pathSettings = Environment.ExpandEnvironmentVariables(StaticVars._pathApp + "\\lib\\Settings");
            StaticVars._pathShortcut = Environment.ExpandEnvironmentVariables(StaticVars._pathApp + "\\lib\\Иконки ярлыков");
            StaticVars._userIdentyty = Environment.UserName;

            
            handler = new Handler();
            handler.GetApplicationVersion();

            My_Hand = new MySql_Handler();
            //My_Hand.Getting_Data();

            StaticVars._loadingData = false;
            Check_Loaded();
        }

        private void CheckCreateDirectory(string _source)
        {
            if (!Directory.Exists(_source))
            {
                Directory.CreateDirectory(_source);
            }
        }

        private void CheckShortcutCreate()
        {
            if (!File.Exists("C:\\Users\\" + StaticVars._userIdentyty + "\\Desktop\\Surveillance Admin.lnk") && StaticVars._startCreatingShortcut == true)
            {
                Activate();
                MessageBoxResult result = MessageBox.Show(" На вашем рабочем столе нет ярлыка 'Surveillance Admin'.\n\n   Создать?", "Surveillance Admin", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    handler.CreateShortcut("Surveillance Admin", "Surveillance Admin - Просмотр всех запущенных процессов у пользователей", StaticVars._currentVersionApp, "Ctrl+Shift+S");
                }
                else if (result == MessageBoxResult.No)
                {
                    MessageBoxResult _result = MessageBox.Show(" Не предлагать больше этот вопрос?", "Surveillance Admin", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (_result == MessageBoxResult.Yes)
                    { 
                        StaticVars._startCreatingShortcut = false;
                        My_Hand.Set_Properties("StartCreatingShortcut", false, out bool Result);
                        if (Result == true)
                        { }
                        else if (Result == false)
                        {
                            StaticVars._startCreatingShortcut = true;
                            
                            MessageBox.Show(" Не удалось обновить данные. Проверьте подключение к интернету или обратитесь к разработчику за помощью. \n Aluminum.Company163@yandex.ru или https://vk.com/aluminum343", "Surveillance Admin", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            else if (File.Exists("C:\\Users\\" + StaticVars._userIdentyty + "\\Desktop\\Al-Store.lnk") && Properties.Settings.Default.First_Started == true)
            {
                Properties.Settings.Default.First_Started = false;
                Properties.Settings.Default.Save();

                handler.CreateShortcut("Surveillance Admin", "Surveillance Admin - Просмотр всех запущенных процессов у пользователей", StaticVars._currentVersionApp, "Ctrl+Shift+S");
            }
        }
        private void Check_Loaded()
        {
            while (true)
            {
                if (!StaticVars._logoAnimation)
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
