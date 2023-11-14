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

                    if (rowIndex % itemsPerPage == 0) // First row on the page
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
                    }
                }
            };
        }

        private int TotalPages()
        {
            return (int)Math.Ceiling((double)allTransports.Count / itemsPerPage);
        }

        private void ShowPage(int page)
        {
            currentPage = page;
            int startIndex = currentPage * itemsPerPage;
            int endIndex = Math.Min(startIndex + itemsPerPage, allTransports.Count);
            currentPageTransports = new ObservableCollection<Transport>(allTransports.Skip(startIndex).Take(itemsPerPage));

            table.ItemsSource = currentPageTransports;
            UpdatePageButtons();
        }


        private void UpdatePageButtons()
        {
            int totalPageCount = TotalPages();

            if (totalPageCount == 0)
            {
                firstBut.Opacity = 0;
                firstBut.IsHitTestVisible = false;

                secBut.Opacity = 0;
                secBut.IsHitTestVisible = false;

                dotBut.Opacity = 0;
                dotBut.IsHitTestVisible = false;

                lastBut.Opacity = 0;
                lastBut.IsHitTestVisible = false;

                nextBut.Margin = new Thickness(22, -20, 0, 0);
            }
            else if (totalPageCount == 1)
            {
                secBut.Opacity = 0;
                secBut.IsHitTestVisible = false;

                dotBut.Opacity = 0;
                dotBut.IsHitTestVisible = false;

                lastBut.Opacity = 0;
                lastBut.IsHitTestVisible = false;

                nextBut.Margin = new Thickness(44, -20, 0, 0);

            }
            else if (totalPageCount == 2)
            {

                dotBut.Opacity = 0;
                dotBut.IsHitTestVisible = false;

                lastBut.Opacity = 0;
                lastBut.IsHitTestVisible = false;

                nextBut.Margin = new Thickness(65, -20, 0, 0);
            }
            else if (totalPageCount == 3)
            {
                dotButTxt.Text = "3";

                lastBut.Opacity = 0;
                lastBut.IsHitTestVisible = false;

                nextBut.Margin = new Thickness(88, -20, 0, 0);
            }
            else if (totalPageCount == 4)
            {
                dotButTxt.Text = "3";

                lastButTxt.Text = "4";

                nextBut.Margin = new Thickness(110, -20, 0, 0);
            }
            else
            {
                if (currentPage == totalPageCount - 4)
                {
                    firstButTxt.Text = (currentPage + 1).ToString();
                    firstBut.Style = FindResource("UnActivePageButton") as Style;
                    firstBut.Style = FindResource("ActivePageButton") as Style;
                    secButTxt.Text = (currentPage + 2).ToString();
                    dotButTxt.Text = (totalPageCount - 1).ToString();
                    lastButTxt.Text = totalPageCount.ToString();
                }
                else if (currentPage >= totalPageCount - 4)
                {

                }
                else
                {
                    firstButTxt.Text = (currentPage + 1).ToString();
                    firstBut.Style = FindResource("UnActivePageButton") as Style;
                    firstBut.Style = FindResource("ActivePageButton") as Style;
                    secButTxt.Text = (currentPage + 2).ToString();
                    secBut.Style = FindResource("UnActivePageButton") as Style;
                    secBut.Style = FindResource("ActivePageButton") as Style;
                    dotButTxt.Text = ".....";
                    lastButTxt.Text = totalPageCount.ToString();

                }
            }
            UpdateButtonStyles();
        }



        private void UpdateButtonStyles()
        {
            int totalPageCount = TotalPages();

            for (int i = 1; i < 5; i++)
            {
                var button = (Button)PaginationStackPanel.Children[i];
                var textBlock = (TextBlock)button.Content;
                if (textBlock.Text != ".....")
                {
                    int buttonPage = int.Parse(textBlock.Text);
                    if (buttonPage >= 1 && buttonPage <= totalPageCount)
                    {


                        button.Style = (buttonPage == currentPage + 1) ? FindResource("ActivePageButton") as Style : FindResource("UnActivePageButton") as Style;

                    }
                }
            }
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
                DataGridRow row = table.ItemContainerGenerator.ContainerFromItem(itemToDelete) as DataGridRow;

                if (row != null)
                {
                    var storyboard = new Storyboard();

                    var opacityAnimation = new DoubleAnimation
                    {
                        To = 0,
                        Duration = TimeSpan.FromSeconds(0.3),
                    };

                    Storyboard.SetTarget(opacityAnimation, row);
                    Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(UIElement.OpacityProperty));
                    storyboard.Children.Add(opacityAnimation);

                    storyboard.Begin();

                    await Task.Delay(300);

                    allTransports.Remove(itemToDelete);

                    for (int i = 0; i < allTransports.Count; i++)
                    {
                        allTransports[i].Number = (i + 1).ToString();
                    }

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
