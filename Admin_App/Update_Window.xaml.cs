using Admin_App.Classes;
using Admin_App.MySql_Services;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Admin_App
{
    /// <summary>
    /// Логика взаимодействия для Update_Window.xaml
    /// </summary>
    public partial class Update_Window : Window
    {
        bool _isUpdating = false;
        MySql_Handler My_Hand;

        string app_reference;
        string _pathApp = Assembly.GetExecutingAssembly().Location;
        string _nameApp = AppDomain.CurrentDomain.FriendlyName;
        int ProgressBar_Value = 0;
        double _sizeApp;
        public Update_Window()
        {
            InitializeComponent();
            Directory.SetCurrentDirectory(AppContext.BaseDirectory);
            WindowStyle = WindowStyle.None; Main_Border.CornerRadius = new CornerRadius(20); AllowsTransparency = true;

            _ = Start_Process();
        }

        public async Task Start_Process()
        {
            for (int _ProgressBar = 0; _ProgressBar != 1;)
            {
                if (ProgressBar_Value < 100)
                {
                    ProgressBar_Value++;
                }
                else if (ProgressBar_Value == 100)
                {
                    ProgressBar_Value = 0;
                    Status_TextBlock.Text = "Скачивание файлов";
                    Process_TextBlock.Text = "Скачивание";
                    _ProgressBar = 1;
                    StartUpdateProcess();
                }
                ProgressBar.Value = ProgressBar_Value;
                await Task.Delay(20);
            }
        }



        private async void StartUpdateProcess()
        {
            My_Hand = new MySql_Handler();
            //await My_Hand.Getting_Information_of_Update_Al(out double size, out app_reference);

            if (Internet.OK())
            {
                try
                {
                    File.Delete($"{StaticVars._mainPath}\\New.zip");
                }
                catch { }
                WebClient webClient = new WebClient();
                try
                {
                    _isUpdating = true;
                    webClient.DownloadProgressChanged += (s, e_) =>
                    {
                        if (Process_TextBlock.Text == "Скачивание")
                        { Process_TextBlock.Text = "Скачивание."; }
                        else if (Process_TextBlock.Text == "Скачивание.")
                        { Process_TextBlock.Text = "Скачивание.."; }
                        else if (Process_TextBlock.Text == "Скачивание..")
                        { Process_TextBlock.Text = "Скачивание..."; }
                        else if (Process_TextBlock.Text == "Скачивание...")
                        { Process_TextBlock.Text = "Скачивание"; }
                        _sizeApp = 10;
                        ProgressBar.Value = (double)e_.BytesReceived / 1048576 * 100 / _sizeApp;
                    };
                    webClient.DownloadFileAsync(new Uri("https://getfile.dokpub.com/yandex/get/https://disk.yandex.ru/d/qhqpLsYhK9YEiw"), $"{StaticVars._mainPath}\\New.zip");
                    webClient.DownloadFileCompleted += (s, e_) =>
                    {
                        FinalUpdateProcess();
                    };
                }
                catch
                {
                    MessageBox.Show($" Не удалось скачать файлы, обратитесь к системному администратору.\n\n Программа будет закрыта...", "Surveillance Admin", MessageBoxButton.OK, MessageBoxImage.Error);
                    Environment.Exit(0);
                }
            }
            else
            {
                MessageBox.Show($" Нет доступа в сеть, обратитесь к системному администратору.\n\n Программа будет закрыта...", "Surveillance Admin", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }

        }
        public async void FinalUpdateProcess()
        {
            Status_TextBlock.Text = "Обработка файлов";
            Process_TextBlock.Text = "Удаление старого Updater-а";
            string zipPath = $"{StaticVars._mainPath}\\New.zip";
            string extractPath = $"{StaticVars._mainPath}\\";

            try
            { Directory.Delete($"{StaticVars._mainPath}\\New Users Surveillance", true); }
            catch { }
            try
            {
                Process_TextBlock.Text = "Разархивация файлов";
                await Task.Run(() =>
                {
                    ZipFile.ExtractToDirectory(zipPath, extractPath);
                });
                await Task.Delay(50);
                Process_TextBlock.Text = "Завершение обновления";
                await Task.Delay(100);
                Cmd($"taskkill /f /im \"{_nameApp}\" && timeout /t 1 && del \"{StaticVars._mainPath}\\New.zip\" && RD /s/q \"{StaticVars._pathApp}\" && ren \"{StaticVars._mainPath}\\New Surveillance Admin\" \"Surveillance Admin\" && \"{_pathApp}\"");
                _isUpdating = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($" Не удалось распаковать файлы, обратитесь к системному администратору.\n\n Программа будет закрыта...", "Surveillance Admin", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
                //ErrorsSaves errorsSaves = new ErrorsSaves();
                //errorsSaves.Recording_Errors(ex);
            }
        }

        private void Cmd(string _line)
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = "cmd",
                Arguments = $"/c {_line}",
                WindowStyle = ProcessWindowStyle.Hidden,
                WorkingDirectory = @"C:\Windows\System32",
            });
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch { }
        }

        private void ReSize_Min_But_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
            }
            else if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
        }

        private void Close_But_Click(object sender, RoutedEventArgs e)
        {
            if (!_isUpdating)
            {
                Environment.Exit(0);
            }
            else if (_isUpdating)
            {
                MessageBoxResult _result = MessageBox.Show(" Вы точно хотите закрыть программу?\n Если закрыть программу во время обновления, могут возникнуть ошибки при дальнейшей работе.\n Дождитетесь окончания обновления и програма перезапустится.\n\n В случае если программа дала сбой и вам приходится закрывать это окно нажмите \"Да\" или \"Нет\" для отмены.", "Surveillance Admin", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                if (_result == MessageBoxResult.Yes)
                    Environment.Exit(0);
            }
        }
    }
}
