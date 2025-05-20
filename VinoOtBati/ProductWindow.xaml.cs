using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace VinoOtBati
{
    /// <summary>
    /// Логика взаимодействия для ProductWindow.xaml
    /// </summary>
    public partial class ProductWindow : Window
    {
        DBEntities db = new DBEntities();
        public ProductWindow(string username)
        {
            InitializeComponent();
            personListView.ItemsSource = db.Products.ToList();
            if (string.IsNullOrEmpty(username))
            {
                WelcomeText.Text = "Вы вошли как гость";
                Title = "Просмотр товаров (Гость)";
            }
            else
            {
                WelcomeText.Text = $"Добро пожаловать, {username}!";
                Title = $"Просмотр товаров ({username})";
            }
        }
        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count > 0)
            {
                Products products = (Products)e.AddedItems[0];

                string ProductName = products.ProductName;

            }
        }
    }
}
