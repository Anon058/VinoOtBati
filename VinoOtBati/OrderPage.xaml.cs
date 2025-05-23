using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для OrderWindow.xaml
    /// </summary>
    public partial class OrderWindow : Page
    {
        private ObservableCollection<Products> orderItems;

        public OrderWindow(ObservableCollection<Products> orderItems)
        {
            InitializeComponent();

            this.orderItems = orderItems;
            OrderListView.ItemsSource = orderItems;
            CalculateTotalCost();
                
        }
        private void CalculateTotalCost()
        {
            decimal totalCost = 0;
            decimal totalDiscount = 0;
            foreach (var item in orderItems)
            {
                totalCost += item.DiscountedPrice; 
                totalDiscount += item.PricePerUnit - item.DiscountedPrice; 
            }
            TotalCostText.Text = $"Общая сумма: {totalCost} ₽\nСумма скидки: {totalDiscount} ₽";
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }
        
        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            Button removeButton = sender as Button;
            if (removeButton != null)
            {
                Products productToRemove = removeButton.DataContext as Products;
                if (productToRemove != null)
                {
                    orderItems.Remove(productToRemove);
                    CalculateTotalCost();
                }
            }
        }

    }
}
