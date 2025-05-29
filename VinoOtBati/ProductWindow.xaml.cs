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
        public string username;

        private ObservableCollection<Products> orderItems = new ObservableCollection<Products>();

        public ProductWindow(string username)
        {
            InitializeComponent();
            ProductsListView.ItemsSource = db.Products.ToList();
            this.username = username;
            products = db.Products.ToList();

            var orderDetails = db.OrderDetails.ToList();
            var orders = db.Orders.ToList();
            decimal totalSales = orderDetails.Sum(od => od.Quantity * od.Products.PricePerUnit);
            decimal maxSales = orders.Sum(o => o.TotalCost);

            var user = db.Users.FirstOrDefault(u => u.UserName == username);
            if (user != null && (user.RoleID == 3 || user.RoleID == 2)) 
            {
                ManagerOrdersBtn.Visibility = Visibility.Visible;
            }
            else
            {
                ManagerOrdersBtn.Visibility = Visibility.Collapsed;
            }

                foreach (var product in products)
                {
                    product.Discount = CalculateDiscount(maxSales, totalSales);
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
        private void ManagerOrdersBtn_Click(object sender, RoutedEventArgs e)
        {
            var ordersWindow = new ManagerOrdersWindow();
            ordersWindow.ShowDialog();
            //this.Close();
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
            }
        }
   
        
        private void ViewOrderButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new OrderWindow(orderItems));
        }
        private decimal CalculateDiscount(decimal maxSales, decimal totalSales)
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
            UpdateProducts();
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

        
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateProducts();
        }

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
            UpdateProducts();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new OrderWindow(orderItems));
        }

        private void BrandCombo_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (BrandCombo.SelectedIndex == 0)
                ProductsListView.ItemsSource = products;
            else
                ProductsListView.ItemsSource = products.Where(p => p.Brands.BrandName == BrandCombo.SelectedItem.ToString()).ToList();
            UpdateProducts();
        }
    }
}