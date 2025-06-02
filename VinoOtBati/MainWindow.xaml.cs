using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace VinoOtBati
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _secondsRemaining = 10;
        private DispatcherTimer _blockTimer;
        DBEntities db = new DBEntities();
        private string currentCaptcha;
        private Random random = new Random();
        int count = 0;
        public MainWindow()
        {
            InitializeComponent();
            GenerateNewCaptcha();
        }
        private void GenerateNewCaptcha()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            char[] captcha = new char[4];

            for (int i = 0; i < captcha.Length; i++)
            {
                captcha[i] = chars[random.Next(chars.Length)];
            }

            currentCaptcha = new string(captcha);
            CaptchaText.Text = currentCaptcha;
            CaptchaInput.Text = ""; 
        }

        private void RefreshCaptcha_Click(object sender, RoutedEventArgs e)
        {
            GenerateNewCaptcha();
        }
        private void BlockSystem()
        {
            btnLogin.IsEnabled = false;
            _secondsRemaining = 10;

            _blockTimer = new DispatcherTimer();
            _blockTimer.Interval = TimeSpan.FromSeconds(1);

            _blockTimer.Tick += (s, e) =>
            {
                _secondsRemaining--;

                if (_secondsRemaining <= 0)
                {
                    _blockTimer.Stop();
                    btnLogin.IsEnabled = true;
                    count = 0;
                }
            };
            _blockTimer.Start();
        }
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string captchaInput = CaptchaInput.Text;

            if(count == 2)
            {
                    BlockSystem();
                    MessageBox.Show("Слишком много неудачных попыток. Система заблокирована на 10 секунд.",
                                  "Блокировка",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Warning);
                
            }
            var enter = db.Users.FirstOrDefault(x => x.UserName == UsernameTextBox.Text && x.Password == PasswordBox.Password);

            if (captchaInput != currentCaptcha)
            {
                count++; 
                if(count <= 2)
                {

                MessageBox.Show("Неверная капча! Попробуйте снова.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                GenerateNewCaptcha();
                return;
                }
            }

            else if (enter == null)
            {
                MessageBox.Show("Такого пользователя нет!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
            MessageBox.Show($"Добро пожаловать, {UsernameTextBox.Text}!", "Успешный вход", MessageBoxButton.OK, MessageBoxImage.Information);
            var productsWindow = new ProductWindow(UsernameTextBox.Text);
            productsWindow.Show();
            this.Close();
            }

        }

        private void GuestLink_Click(object sender, RoutedEventArgs e)
        {
            var productsWindow = new ProductWindow(null);
            productsWindow.Show();
            this.Close();
        }
    }
}
