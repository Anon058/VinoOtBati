using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// 

    public partial class ProductWindow : Window
    {
        DBEntities db = new DBEntities();


        public ProductWindow(string username)
        {
            InitializeComponent();
            personListView.ItemsSource = db.Products.ToList();

            var products = db.Products.ToList();
            var orderDetails = db.OrderDetails.ToList();
            var orders = db.Orders.ToList();
            decimal partnerTotalSales = orderDetails.Sum(od => od.Products.PricePerUnit);
            decimal maxSalesThreshold = orders.Sum(o => o.TotalCost);

            foreach (var product in products)
            {
                product.Discount = CalculateDiscount(partnerTotalSales, maxSalesThreshold);
                product.DiscountedPrice = product.PricePerUnit * (1 - product.Discount / 100);
            }

            personListView.ItemsSource = products;

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
        private decimal CalculateDiscount(decimal totalSales, decimal maxSales)
        {
            if (totalSales < maxSales / 4) return 0;
            if (totalSales < maxSales / 2) return 5;
            if (totalSales < (3 * maxSales) / 4) return 10;
            return 15;
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                Products products = (Products)e.AddedItems[0];

                string ProductName = products.ProductName;

            }
        }
    }
}