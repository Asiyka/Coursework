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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Security.Cryptography.Xml;
using System.Windows.Media.Animation;
using System.Windows;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Security.Policy;


namespace kursova
{
    

    public partial class MainWindow : Window
    {
        private static ObservableCollection<Transport> allTransports;
        private ObservableCollection<Transport> currentPageTransports;
        private int itemsPerPage = 8;
        private int currentPage = 0;
        public MainWindow()
        {
            InitializeComponent();
            allTransports = new ObservableCollection<Transport> { };

            for(int i = 0; i < 12; i++)
            {
                Transport newTransport = new Transport
                {
                    Brand = "Electron" + i,
                    Engine = "Electric" + i,
                    Power = "854" + i,
                    Axles = "10" + i,
                    Places = "255" + i,
                    Sitting = "56" + i,
                    Doors = "10" + i,
                    Low = "Yes" + i
                };

                newTransport.Number = (allTransports.Count + 1).ToString();

                allTransports.Add(newTransport);
            }
            
            ShowPage(0);
            table.LoadingRow += (sender, e) =>
            {
                DataGridRow row = e.Row as DataGridRow;
                if (row != null)
                {
                    int rowIndex = e.Row.GetIndex();
                    int itemsPerPage = 8;
                    int pageIndex = rowIndex / itemsPerPage;

                    /*if (rowIndex % itemsPerPage == 0) // First row on the page
                    {
                        row.Style = FindResource("FirstRowStyle") as Style;
                    }
                    else if (rowIndex % itemsPerPage == itemsPerPage - 1) // Last row on the page
                    {
                        row.Style = FindResource("LastRowStyle") as Style;
                    }
                    else
                    {
                        row.Style = FindResource("DataGridRowStyle1") as Style;
                    }*/
                }
            };
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Clicked!", "Sample App", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowPage(int page)
        {
            currentPage = page;
            int startIndex = currentPage * itemsPerPage;
            int endIndex = Math.Min(startIndex + itemsPerPage, allTransports.Count);
            currentPageTransports = new ObservableCollection<Transport>(allTransports.Skip(startIndex).Take(itemsPerPage));

            table.ItemsSource = currentPageTransports;
        }
        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            int nextPage = currentPage + 1;
            int totalPageCount = (int)Math.Ceiling((double)allTransports.Count / itemsPerPage);

            if (nextPage < totalPageCount)
            {
                ShowPage(nextPage);
            }
        }
        private void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 0)
            {
                ShowPage(currentPage - 1);
            }
        }
        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Transport itemToDelete = (sender as Button).Tag as Transport;

            if (itemToDelete != null)
            {
                // Get the DataGridRow for the item
                DataGridRow row = table.ItemContainerGenerator.ContainerFromItem(itemToDelete) as DataGridRow;

                if (row != null)
                {
                    // Create a storyboard for the animation
                    var storyboard = new Storyboard();

                    // Define the animation for fading out the row
                    var opacityAnimation = new DoubleAnimation
                    {
                        To = 0,
                        Duration = TimeSpan.FromSeconds(0.3),
                    };

                    // Add the opacity animation to the storyboard
                    Storyboard.SetTarget(opacityAnimation, row);
                    Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(UIElement.OpacityProperty));
                    storyboard.Children.Add(opacityAnimation);

                    // Start the storyboard
                    storyboard.Begin();

                    // Wait for the animation to complete before removing the item
                    await Task.Delay(300);

                    // Remove the item from the ObservableCollection
                    allTransports.Remove(itemToDelete);

                    for (int i = 0; i < allTransports.Count; i++)
                    {
                        allTransports[i].Number = (i + 1).ToString();
                    }

                    // Refresh the DataGrid
                    CollectionViewSource.GetDefaultView(table.ItemsSource).Refresh();
                    ShowPage(currentPage);
                }
            }
        }

    }

    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string text = value as string;
            return string.IsNullOrWhiteSpace(text) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class Transport : IEquatable<Transport>
    {
        public string Number { get; set; }
        public string Brand { get; set; }
        public string Engine { get; set; }
        public string Power { get; set; }
        public string Axles { get; set; }
        public string Places { get; set; }
        public string Sitting { get; set; }
        public string Doors { get; set; }
        public string Low { get; set; }

        public bool Equals(Transport other)
        {
            if (other == null)
                return false;

            return Number == other.Number; // Use a unique identifier for comparison
        }

        public override bool Equals(object obj)
        {
            if (obj is Transport other)
                return Equals(other);
            return false;
        }

        public override int GetHashCode()
        {
            return Number.GetHashCode();
        }
    }
}
