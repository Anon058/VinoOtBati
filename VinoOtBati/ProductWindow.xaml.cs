using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Contexts;
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
    /// Логика взаимодействия для ProductWindow.xaml
    /// </summary>
    /// 

    public partial class ProductWindow : Window
    {
        DBEntities db = new DBEntities();
        private List<Products> products;
        private string currentSort = "";

        private ObservableCollection<Products> orderItems = new ObservableCollection<Products>();
        private Button viewOrderButton;

        public ProductWindow(string username)
        {
            InitializeComponent();
            ProductsListView.ItemsSource = db.Products.ToList();

            products = db.Products.ToList();

            viewOrderButton = new Button { Content = "Просмотр заказа", Visibility = Visibility.Hidden };
            viewOrderButton.Click += ViewOrderButton_Click;

            var orderDetails = db.OrderDetails.ToList();
            var orders = db.Orders.ToList();
            decimal partnerTotalSales = orderDetails.Sum(od => od.Products.PricePerUnit);
            decimal maxSalesThreshold = orders.Sum(o => o.TotalCost);

            foreach (var product in products)
            {
                product.Discount = CalculateDiscount(partnerTotalSales, maxSalesThreshold);
                product.DiscountedPrice = product.PricePerUnit * (1 - product.Discount / 100);
            }

            ProductsListView.ItemsSource = products;

            BrandCombo.Items.Add("Все бренды");
            foreach (var brand in db.Brands.ToList())
                BrandCombo.Items.Add(brand.BrandName);
            BrandCombo.SelectedIndex = 0;

            CategoryCombo.Items.Add("Все категории");
            foreach (var category in db.Categories.ToList())
                CategoryCombo.Items.Add(category.CategoryName);
            CategoryCombo.SelectedIndex = 0;


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
        private void personListView_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ProductsListView.SelectedItem != null)
            {
                ProductContextMenu.IsOpen = true;
            }
        }
        private void AddToOrder_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsListView.SelectedItem is Products selectedProduct)
            {
                orderItems.Add(selectedProduct);
                UpdateOrderButtonVisibility();
            }
        }
        private void UpdateOrderButtonVisibility()
        {
            viewOrderButton.Visibility = orderItems.Count > 0 ? Visibility.Visible : Visibility.Hidden;
        }
        private void ViewOrderButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new OrderWindow(orderItems));
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
        private void BrandCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BrandCombo.SelectedIndex == 0)
                ProductsListView.ItemsSource = products;
            else
                ProductsListView.ItemsSource = products.Where(p => p.Brands.BrandName == BrandCombo.SelectedItem.ToString()).ToList();

        }

        private void UpdateProducts()
        {
            if (products == null) return;

            var filtered = products.AsQueryable();

            if (BrandCombo.SelectedIndex > 0)
                filtered = filtered.Where(p => p.Brands.BrandName == BrandCombo.SelectedItem.ToString());

            if (CategoryCombo.SelectedIndex > 0)
                filtered = filtered.Where(p => p.Categories.CategoryName == CategoryCombo.SelectedItem.ToString());

            if (!string.IsNullOrEmpty(SearchBox.Text))
                filtered = filtered.Where(p => p.ProductName.Contains(SearchBox.Text));

            if (currentSort == "asc")
                filtered = filtered.OrderBy(p => p.PricePerUnit);
            else if (currentSort == "desc")
                filtered = filtered.OrderByDescending(p => p.PricePerUnit);

            ProductsListView.ItemsSource = filtered.ToList();
            CountText.Text = $"{filtered.Count()} из {products.Count}";
        }

        private void CategoryCombo_SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateProducts();
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) => UpdateProducts();

        private void SortAsc_Click(object sender, RoutedEventArgs e)
        {
            currentSort = "asc";
            UpdateProducts();
        }

        private void SortDesc_Click(object sender, RoutedEventArgs e)
        {
            currentSort = "desc";
            UpdateProducts();
        }

        private void CategoryCombo_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new OrderWindow(orderItems));
        }
    }
}