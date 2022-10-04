using Admin_App.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;

namespace Admin_App
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static List<CompletedProcessesClass> _completedProcessesList = null;
        public class CompletedProcessesClass
        {
            public string _processName = "", _processType = "App";
            public double _processTime = 0;
            public bool _processBackground = false;
        }

        bool _isUpdate = false;
        public MainWindow()
        {
            InitializeComponent();
            WindowStyle = WindowStyle.None; /*Main_Border.CornerRadius = new CornerRadius(20);*/ AllowsTransparency = true;
            if (Convert.ToDouble($"{StaticVars._currentVersionApp.Split('.')[0]}.{StaticVars._currentVersionApp.Split('.')[1]}", CultureInfo.InvariantCulture) < Convert.ToDouble($"{StaticVars._newVersionApp.Split('.')[0]}.{StaticVars._newVersionApp.Split('.')[1]}", CultureInfo.InvariantCulture))
            {
                Opacity = 0;
                _isUpdate = true;
                MessageBox.Show($" Доступна новая версия программы!\n\n Текущая версия: {StaticVars._currentVersionApp}\n Доступная версия: {StaticVars._newVersionApp}\n\n Нажмите \"OK\" для начала процесса обоновления...", "Surveillance Admin", MessageBoxButton.OK, MessageBoxImage.Information);
                Update_Window update_Window = new Update_Window();
                update_Window.Show();

            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Activate();
            if (_isUpdate)
                Hide();
            else
                GetData();
        }

        private void GetData()
        {
            //var plt = new ScottPlot.Plot(600, 400);

            string _todaySurveillanceProcessesLogIn = "", _todaySurveillanceProcessesLogOut = "";
            if (File.Exists($"{DateTime.Now.ToString("d")}.txt"))
            {
                _todaySurveillanceProcessesLogIn = File.ReadAllText($"{DateTime.Now.ToString("d")}.txt");
            }
            int _counts = _todaySurveillanceProcessesLogIn.Count(f => f == '\n');
            _completedProcessesList = new List<CompletedProcessesClass>();
            for (int i = 1; i <= _counts; i++)
            {
                _todaySurveillanceProcessesLogOut = _todaySurveillanceProcessesLogIn.Split('\n')[i - 1];
                if (_todaySurveillanceProcessesLogOut.Contains('|') && !Convert.ToBoolean(_todaySurveillanceProcessesLogOut.Split('|')[3]))
                    _completedProcessesList.Add(new CompletedProcessesClass() { _processName = _todaySurveillanceProcessesLogOut.Split('|')[0], _processTime = Convert.ToDouble(_todaySurveillanceProcessesLogOut.Split('|')[1]), _processType = _todaySurveillanceProcessesLogOut.Split('|')[2], _processBackground = Convert.ToBoolean(_todaySurveillanceProcessesLogOut.Split('|')[3]) });
            }

            double[] values = new double[_completedProcessesList.Capacity - 1];
            string[] labels = new string[_completedProcessesList.Capacity - 1];

            int k = 0;
            foreach (CompletedProcessesClass _completedProcessesClass in _completedProcessesList)
            {
                values[k] = _completedProcessesClass._processTime;
                labels[k] = _completedProcessesClass._processName;
                k++;
            }

            var pie = Chart.Plot.AddPie(values);
            pie.SliceLabels = labels;
            pie.ShowPercentages = true;
            pie.ShowValues = true;
            pie.ShowLabels = true;
            Chart.Plot.Legend();

            Chart.Plot.SaveFig("pie_showEverything.png");
            Chart.Refresh();
        }
    }
}
