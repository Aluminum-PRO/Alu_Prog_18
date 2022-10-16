using System.Threading.Tasks;
using System.Windows;
using static Admin_App.Classes.StaticVars;

namespace Admin_App
{
    /// <summary>
    /// Логика взаимодействия для Logo_Window.xaml
    /// </summary>
    public partial class Logo_Window : Window
    {
        public Logo_Window()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Activate();
            Show_Logo();
        }

        private async void Show_Logo()
        {
            for (Opacity = 0; Opacity <= 1;)
            {
                Opacity += 0.05;
                await Task.Delay(30);
            }

            _logoAnimation = false;
            Check_Loaded();
        }

        private async void Check_Loaded()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    if (_isLoadingData)
                    { break; }
                }
            });

            Hide_Logo();
        }

        private async void Hide_Logo()
        {
            for (Opacity = 1; Opacity >= 0;)
            {
                Opacity -= 0.05;
                await Task.Delay(30);
            }
            Close();
        }
    }
}
