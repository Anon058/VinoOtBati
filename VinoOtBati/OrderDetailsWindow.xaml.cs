using System.Linq;
using System.Windows;

namespace VinoOtBati
{
    public partial class OrderDetailsWindow : Window
    {
        private DBEntities db = new DBEntities();

        public OrderDetailsWindow(int orderId)
        {
            InitializeComponent();

            var order = db.Orders
                .Include("OrderDetails")
                .Include("OrderDetails.Products")
                .Include("Users")
                .FirstOrDefault(o => o.OrderID == orderId);

            DataContext = order;
        }
    }
}