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

namespace VinoOtBati
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DBEntities db = new DBEntities();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {

            var enter = db.Users.FirstOrDefault(x => x.UserName == UsernameTextBox.Text && x.Password == PasswordBox.Password);
            
            if(enter == null)
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
