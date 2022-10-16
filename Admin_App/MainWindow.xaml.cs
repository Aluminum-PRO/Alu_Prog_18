using Admin_App.Pages;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Admin_App
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            WindowStyle = WindowStyle.None; /*Main_Border.CornerRadius = new CornerRadius(20);*/ AllowsTransparency = true;

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Activate();
            Directory.SetCurrentDirectory(AppContext.BaseDirectory);

            WindowStyle = WindowStyle.None; Main_Border.CornerRadius = new CornerRadius(20); AllowsTransparency = true;

            //MainWindow = this;
            //Main_Frame = Main_Frame;

            Main_Frame.Content = new Main_Page();
        }

        private void GetData()
        {
            //var plt = new ScottPlot.Plot(600, 400);

            //string _todaySurveillanceProcessesLogIn = "", _todaySurveillanceProcessesLogOut = "";
            //if (File.Exists($"{DateTime.Now.ToString("d")}.txt"))
            //{
            //    _todaySurveillanceProcessesLogIn = File.ReadAllText($"{DateTime.Now.ToString("d")}.txt");
            //}
            //int _counts = _todaySurveillanceProcessesLogIn.Count(f => f == '\n');
            //_completedProcessesList = new List<CompletedProcessesClass>();
            //for (int i = 1; i <= _counts; i++)
            //{
            //    _todaySurveillanceProcessesLogOut = _todaySurveillanceProcessesLogIn.Split('\n')[i - 1];
            //    if (_todaySurveillanceProcessesLogOut.Contains('|') && !Convert.ToBoolean(_todaySurveillanceProcessesLogOut.Split('|')[3]))
            //        _completedProcessesList.Add(new CompletedProcessesClass() { _processName = _todaySurveillanceProcessesLogOut.Split('|')[0], _processTime = Convert.ToDouble(_todaySurveillanceProcessesLogOut.Split('|')[1]), _processType = _todaySurveillanceProcessesLogOut.Split('|')[2], _processBackground = Convert.ToBoolean(_todaySurveillanceProcessesLogOut.Split('|')[3]) });
            //}

            //double[] values = new double[_completedProcessesList.Capacity - 1];
            //string[] labels = new string[_completedProcessesList.Capacity - 1];

            //int k = 0;
            //foreach (CompletedProcessesClass _completedProcessesClass in _completedProcessesList)
            //{
            //    values[k] = _completedProcessesClass._processTime;
            //    labels[k] = _completedProcessesClass._processName;
            //    k++;
            //}

            //var pie = Chart.Plot.AddPie(values);
            //pie.SliceLabels = labels;
            //pie.ShowPercentages = true;
            //pie.ShowValues = true;
            //pie.ShowLabels = true;
            //Chart.Plot.Legend();

            //Chart.Plot.SaveFig("pie_showEverything.png");
            //Chart.Refresh();
        }

        private void Main_Frame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Forward || e.NavigationMode == NavigationMode.Back)
            {
                e.Cancel = true;
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        { try { DragMove(); } catch { } }

        private void Close_But_Click(object sender, RoutedEventArgs e)
        { Close(); }

        private void ReSize_Max_But_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
        }

        public void ReSize_Min_But_Click(object sender, RoutedEventArgs e)
        { WindowState = WindowState.Minimized; }
    }
}
