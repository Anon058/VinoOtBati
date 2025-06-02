using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace VinoOtBati
{
    public partial class ManagerOrdersWindow : Window
    {
        private DBEntities db = new DBEntities();

        public ManagerOrdersWindow()
        {
            InitializeComponent();
            LoadOrders();
        }

        private void LoadOrders()
        {
            OrdersGrid.ItemsSource = db.Orders.OrderByDescending(o => o.DateOfOrder).ToList();
        }

        private void ViewOrderDetails_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var order = button?.DataContext as Orders;

            if (order != null)
            {
                var detailsWindow = new OrderDetailsWindow(order.OrderID);
                detailsWindow.ShowDialog();
            }
        }
    }
}