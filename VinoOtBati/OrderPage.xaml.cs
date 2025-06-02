using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Packaging;
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
using Microsoft.Win32;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using PdfSharp.UniversalAccessibility.Drawing;

namespace VinoOtBati
{
    /// <summary>
    /// Логика взаимодействия для OrderWindow.xaml
    /// </summary>
    public partial class OrderWindow : Page
    {
        private ObservableCollection<Products> orderItems;
        private int orderNumber;
        private string pickupPoint;
        private string receiptCode;
        DBEntities db = new DBEntities();

        public OrderWindow(ObservableCollection<Products> orderItems)
        {
            InitializeComponent();

            this.orderItems = orderItems;
            OrderListView.ItemsSource = orderItems;
            CalculateTotalCost();

            orderNumber = GetNextOrderNumber();

            receiptCode = new Random().Next(100, 999).ToString();

            pickupPoint = "ул. чЁпо, д. 1";

        }

        private int GetNextOrderNumber()
        {
            if (!db.Orders.Any())
            {
                return 1;
            }

            int maxOrderNumber = db.Orders.Max(o => o.OrderID);
            return maxOrderNumber + 1;
        }

        private void CreateReceiptButton_Click(object sender, RoutedEventArgs e)
        {
            if (orderItems.Count == 0)
            {
                MessageBox.Show("Добавьте товары в заказ");
                return;
            }

            bool allAvailable = orderItems.Count < 3;
            string deliveryTime = allAvailable ? "3 дня" : "6 дней";

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF файл (*.pdf)|*.pdf",
                FileName = $"Заказ_{orderNumber}.pdf"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    PdfDocument document = new PdfDocument();
                    PdfPage page = document.AddPage();
                    XGraphics gfx = XGraphics.FromPdfPage(page);


                    document.Save(saveFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            }
            

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
