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
using System.Text.RegularExpressions;
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
                    Brand = "Electron",
                    Engine = "Electric",
                    Power = "854" + i,
                    Axles = "10" + i,
                    Places = "255" + i,
                    Sitting = "56" + i,
                    Doors = "10" + i,
                    Low = "Yes"
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
                firstBut.Opacity = 1;
                firstBut.IsHitTestVisible = true;

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
                firstBut.Opacity = 1;
                firstBut.IsHitTestVisible = true;

                secBut.Opacity = 2;
                secBut.IsHitTestVisible = true;

                dotBut.Opacity = 0;
                dotBut.IsHitTestVisible = false;

                lastBut.Opacity = 0;
                lastBut.IsHitTestVisible = false;

                nextBut.Margin = new Thickness(65, -20, 0, 0);
            }
            else if (totalPageCount == 3)
            {
                firstBut.Opacity = 1;
                firstBut.IsHitTestVisible = true;

                secBut.Opacity = 2;
                secBut.IsHitTestVisible = true;

                dotButTxt.Text = "3";

                dotBut.Opacity = 1;
                dotBut.IsHitTestVisible = true;


                lastBut.Opacity = 0;
                lastBut.IsHitTestVisible = false;

                nextBut.Margin = new Thickness(88, -20, 0, 0);
            }
            else if (totalPageCount == 4)
            {
                firstBut.Opacity = 1;
                firstBut.IsHitTestVisible = true;

                secBut.Opacity = 2;
                secBut.IsHitTestVisible = true;

                dotButTxt.Text = "3";

                dotBut.Opacity = 1;
                dotBut.IsHitTestVisible = true;


                lastBut.Opacity = 1;
                lastBut.IsHitTestVisible = true;

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

        private void AddNewTransportButton_Click(object sender, RoutedEventArgs e)
        {
            addnewtxt.Text = "Add";
            addnewtxt.Margin = new Thickness(46, 0, 0, 0);

            {
                BrandTxt.Foreground = (Brush)new BrushConverter().ConvertFromString("#525252");
                BrandBox.Style = FindResource("BrandBoxStyle") as Style;

                EngineTxt.Foreground = (Brush)new BrushConverter().ConvertFromString("#525252");
                EngineBox.Style = FindResource("EngineBoxStyle") as Style;

                PowerTxt.Foreground = (Brush)new BrushConverter().ConvertFromString("#525252");
                PowerBox.Style = FindResource("PowerBoxStyle") as Style;

                PlacesTxt.Foreground = (Brush)new BrushConverter().ConvertFromString("#525252");
                PlacesBox.Style = FindResource("PlacesBoxStyle") as Style;

                SittingTxt.Foreground = (Brush)new BrushConverter().ConvertFromString("#525252");
                SittingBox.Style = FindResource("SittingBoxStyle") as Style;

                YesCheckBox.Style = FindResource("CheckBoxStyle1") as Style;
                NoCheckBox.Style = FindResource("CheckBoxStyle1") as Style;
            }
            {
                BrandBox.Text = string.Empty;
                EngineBox.Text = string.Empty;
                PowerBox.Text = string.Empty;
                DoorscounttButTxt.Text = "1";
                PlacesBox.Text = string.Empty;  
                AxlescounttButTxt.Text = "1";
                SittingBox.Text = string.Empty;
                YesCheckBox.IsChecked = false;
                NoCheckBox.IsChecked = false;
                
            }
            DoubleAnimation fadeInAnimation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.2));
            addEditTransport.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);

            addEditTransport.IsHitTestVisible = true;
        }

        private void exitAddEditButton_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
            addEditTransport.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

            addEditTransport.IsHitTestVisible = false;
        }
       
        private bool isAnimationInProgress = false;

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            if (!isAnimationInProgress)
            {
                

                ThicknessAnimation moveOut = new ThicknessAnimation
                {
                    To = new Thickness(-220, 37, 0, 0),
                    Duration = TimeSpan.FromSeconds(0.3),
                };
                Button2.BeginAnimation(MarginProperty, moveOut);

                moveOut = new ThicknessAnimation
                {
                    To = new Thickness(-220, 103, 0, 0),
                    Duration = TimeSpan.FromSeconds(0.3),
                    BeginTime = TimeSpan.FromSeconds(0.1)
                };
                Button3.BeginAnimation(MarginProperty, moveOut);

                moveOut = new ThicknessAnimation
                {
                    To = new Thickness(-220, 169, 0, 0),
                    Duration = TimeSpan.FromSeconds(0.3),
                    BeginTime = TimeSpan.FromSeconds(0.2)
                };
                Button4.BeginAnimation(MarginProperty, moveOut);

                moveOut = new ThicknessAnimation
                {
                    To = new Thickness(-220, 235, 0, 0),
                    Duration = TimeSpan.FromSeconds(0.3),
                    BeginTime = TimeSpan.FromSeconds(0.3)
                };
                Button5.BeginAnimation(MarginProperty, moveOut);

                moveOut = new ThicknessAnimation
                {
                    To = new Thickness(-220, 301, 0, 0),
                    Duration = TimeSpan.FromSeconds(0.3),
                    BeginTime = TimeSpan.FromSeconds(0.4)
                };
                Button6.BeginAnimation(MarginProperty, moveOut);

                moveOut = new ThicknessAnimation
                {
                    To = new Thickness(-20, 37, 0, 0),
                    Duration = TimeSpan.FromSeconds(0.3),
                    BeginTime = TimeSpan.FromSeconds(0.1)
                };
                Button2new.BeginAnimation(MarginProperty, moveOut);

                moveOut = new ThicknessAnimation
                {
                    To = new Thickness(-20, 103, 0, 0),
                    Duration = TimeSpan.FromSeconds(0.3),
                    BeginTime = TimeSpan.FromSeconds(0.2)
                };
                Button3new.BeginAnimation(MarginProperty, moveOut);

                moveOut = new ThicknessAnimation
                {
                    To = new Thickness(-20, 169, 0, 0),
                    Duration = TimeSpan.FromSeconds(0.3),
                    BeginTime = TimeSpan.FromSeconds(0.3)
                };
                Button4new.BeginAnimation(MarginProperty, moveOut);

                DoubleAnimation fadeOutAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
                text1.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
                text2.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
                fadeOutAnimation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.2));
                text1new.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
                text2new.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

                isAnimationInProgress = true;
            }
            else
            {
                

                ThicknessAnimation moveOut = new ThicknessAnimation
                {
                    To = new Thickness(-20, 37, 0, 0),
                    Duration = TimeSpan.FromSeconds(0.3),
                };
                Button2.BeginAnimation(MarginProperty, moveOut);

                moveOut = new ThicknessAnimation
                {
                    To = new Thickness(-20, 103, 0, 0),
                    Duration = TimeSpan.FromSeconds(0.3),
                    BeginTime = TimeSpan.FromSeconds(0.1)
                };
                Button3.BeginAnimation(MarginProperty, moveOut);

                moveOut = new ThicknessAnimation
                {
                    To = new Thickness(-20, 169, 0, 0),
                    Duration = TimeSpan.FromSeconds(0.3),
                    BeginTime = TimeSpan.FromSeconds(0.2)
                };
                Button4.BeginAnimation(MarginProperty, moveOut);

                moveOut = new ThicknessAnimation
                {
                    To = new Thickness(-20, 235, 0, 0),
                    Duration = TimeSpan.FromSeconds(0.3),
                    BeginTime = TimeSpan.FromSeconds(0.3)
                };
                Button5.BeginAnimation(MarginProperty, moveOut);

                moveOut = new ThicknessAnimation
                {
                    To = new Thickness(-20, 301, 0, 0),
                    Duration = TimeSpan.FromSeconds(0.3),
                    BeginTime = TimeSpan.FromSeconds(0.4)
                };
                Button6.BeginAnimation(MarginProperty, moveOut);

                moveOut = new ThicknessAnimation
                {
                    To = new Thickness(220, 37, 0, 0),
                    Duration = TimeSpan.FromSeconds(0.3),
                    BeginTime = TimeSpan.FromSeconds(0.1)
                };
                Button2new.BeginAnimation(MarginProperty, moveOut);

                moveOut = new ThicknessAnimation
                {
                    To = new Thickness(220, 103, 0, 0),
                    Duration = TimeSpan.FromSeconds(0.3),
                    BeginTime = TimeSpan.FromSeconds(0.2)
                };
                Button3new.BeginAnimation(MarginProperty, moveOut);

                moveOut = new ThicknessAnimation
                {
                    To = new Thickness(220, 169, 0, 0),
                    Duration = TimeSpan.FromSeconds(0.3),
                    BeginTime = TimeSpan.FromSeconds(0.3)
                };
                Button4new.BeginAnimation(MarginProperty, moveOut);

                DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.2));
                text1.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
                text2.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
                fadeOutAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
                text1new.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
                text2new.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

                isAnimationInProgress = false;
            }
        }

        private void DoorsminusOneBut_Click(object sender, RoutedEventArgs e)
        {
            if(int.Parse(DoorscounttButTxt.Text) > 0)
            {
                DoorscounttButTxt.Text = (int.Parse(DoorscounttButTxt.Text) - 1).ToString();
            }
        }

        private void DoorsplusOneBut_Click(object sender, RoutedEventArgs e)
        {
                DoorscounttButTxt.Text = (int.Parse(DoorscounttButTxt.Text) + 1).ToString();
        }

        private void AxlesminusOneBut_Click(object sender, RoutedEventArgs e)
        {
            if (int.Parse(AxlescounttButTxt.Text) > 1)
            {
                AxlescounttButTxt.Text = (int.Parse(AxlescounttButTxt.Text) - 1).ToString();
            }
        }

        private void AxlesplusOneBut_Click(object sender, RoutedEventArgs e)
        {
            AxlescounttButTxt.Text = (int.Parse(AxlescounttButTxt.Text) + 1).ToString();

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;

            if (checkBox != null)
            {
                if (checkBox == YesCheckBox && NoCheckBox.IsChecked == true)
                {
                    NoCheckBox.IsChecked = false;
                }
                else if (checkBox == NoCheckBox && YesCheckBox.IsChecked == true)
                {
                    YesCheckBox.IsChecked = false;
                }
            }
        }

        private void AddNewButton_Click(object sender, RoutedEventArgs e)
        {
            if(addnewtxt.Text == "Add")
            {
                Transport transport = new Transport();
                bool allFieldsEntered = true;

                if (string.IsNullOrEmpty(BrandBox.Text))
                {
                    BrandBox.Style = FindResource("NotEnteredBrandBoxStyle") as Style;
                    BrandTxt.Foreground = Brushes.Red;

                    allFieldsEntered = false;
                }
                else
                {
                    transport.Brand = BrandBox.Text;
                }
                if (string.IsNullOrEmpty(EngineBox.Text))
                {
                    EngineBox.Style = FindResource("NotEnteredEngineBoxStyle") as Style;
                    EngineTxt.Foreground = Brushes.Red;

                    allFieldsEntered = false;
                }
                else
                {
                    transport.Engine = EngineBox.Text;
                }
                if (string.IsNullOrEmpty(PowerBox.Text))
                {
                    PowerBox.Style = FindResource("NotEnteredPowerBoxStyle") as Style;
                    PowerTxt.Foreground = Brushes.Red;

                    allFieldsEntered = false;
                }
                else
                {
                    transport.Power = PowerBox.Text;
                }
                if (string.IsNullOrEmpty(PlacesBox.Text))
                {
                    PlacesBox.Style = FindResource("NotEnteredPlacesBoxStyle") as Style;
                    PlacesTxt.Foreground = Brushes.Red;

                    allFieldsEntered = false;
                }
                else
                {
                    transport.Places = PlacesBox.Text;
                }
                if (string.IsNullOrEmpty(SittingBox.Text))
                {
                    SittingBox.Style = FindResource("NotEnteredSittingBoxStyle") as Style;
                    SittingTxt.Foreground = Brushes.Red;

                    allFieldsEntered = false;
                }
                else
                {
                    transport.Sitting = SittingBox.Text;
                }

                transport.Doors = DoorscounttButTxt.Text;
                transport.Axles = AxlescounttButTxt.Text;

                if (!(YesCheckBox.IsChecked == true || NoCheckBox.IsChecked == true))
                {
                    YesCheckBox.Style = FindResource("NotEnteredCheckBoxStyle1") as Style;
                    NoCheckBox.Style = FindResource("NotEnteredCheckBoxStyle1") as Style;

                }
                else
                {
                    transport.Low = (YesCheckBox.IsChecked == true) ? "Yes" : "No";

                }

                if (allFieldsEntered)
                {
                    transport.Number = (allTransports.Count + 1).ToString();


                    allTransports.Add(transport);
                    ShowPage(currentPage);

                    DoubleAnimation fadeOutAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
                    addEditTransport.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

                    addEditTransport.IsHitTestVisible = false;
                }

            }
            else if(addnewtxt.Text == "Apply")
            {
                Transport transport = new Transport();
                bool allFieldsEntered = true;


                if (string.IsNullOrEmpty(BrandBox.Text))
                {
                    BrandBox.Style = FindResource("NotEnteredBrandBoxStyle") as Style;
                    BrandTxt.Foreground = Brushes.Red;

                    allFieldsEntered = false;
                }
                else
                {
                    transport.Brand = BrandBox.Text;
                }
                if (string.IsNullOrEmpty(EngineBox.Text))
                {
                    EngineBox.Style = FindResource("NotEnteredEngineBoxStyle") as Style;
                    EngineTxt.Foreground = Brushes.Red;

                    allFieldsEntered = false;
                }
                else
                {
                    transport.Engine = EngineBox.Text;
                }
                if (string.IsNullOrEmpty(PowerBox.Text))
                {
                    PowerBox.Style = FindResource("NotEnteredPowerBoxStyle") as Style;
                    PowerTxt.Foreground = Brushes.Red;

                    allFieldsEntered = false;
                }
                else
                {
                    transport.Power = PowerBox.Text;
                }
                if (string.IsNullOrEmpty(PlacesBox.Text))
                {
                    PlacesBox.Style = FindResource("NotEnteredPlacesBoxStyle") as Style;
                    PlacesTxt.Foreground = Brushes.Red;

                    allFieldsEntered = false;
                }
                else
                {
                    transport.Places = PlacesBox.Text;
                }
                if (string.IsNullOrEmpty(SittingBox.Text))
                {
                    SittingBox.Style = FindResource("NotEnteredSittingBoxStyle") as Style;
                    SittingTxt.Foreground = Brushes.Red;

                    allFieldsEntered = false;
                }
                else
                {
                    transport.Sitting = SittingBox.Text;
                }

                transport.Doors = DoorscounttButTxt.Text;
                transport.Axles = AxlescounttButTxt.Text;

                if (!(YesCheckBox.IsChecked == true || NoCheckBox.IsChecked == true))
                {
                    YesCheckBox.Style = FindResource("NotEnteredCheckBoxStyle1") as Style;
                    NoCheckBox.Style = FindResource("NotEnteredCheckBoxStyle1") as Style;

                }
                else
                {
                    transport.Low = (YesCheckBox.IsChecked == true) ? "Yes" : "No";

                }

                if (allFieldsEntered)
                {
                    transport.Number = (currentEditing).ToString();

                    allTransports[currentEditing - 1] = transport;
                    ShowPage(currentPage);

                    DoubleAnimation fadeOutAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
                    addEditTransport.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

                    addEditTransport.IsHitTestVisible = false;
                }

            }
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!IsNumericInput(e.Text))
            {
                e.Handled = true;
            }
        }
        private bool IsNumericInput(string text)
        {
            Regex regex = new Regex(@"^[+-]?[1-9]\d*$|^0$");

            bool hasLeadingZeros = text.Length > 1 && text[0] == '0';

            return regex.IsMatch(text) && !hasLeadingZeros;
        }

        private void TextBox_PreviewTextInput1(object sender, TextCompositionEventArgs e)
        {
            if (!IsAlphabeticNumericInput(e.Text))
            {
                e.Handled = true;
            }
        }
        private bool IsAlphabeticNumericInput(string text)
        {
            Regex regex = new Regex("^[a-zA-Z-]+$");

            return regex.IsMatch(text);
        }

        int currentEditing = -1;
        private void EditEdit_Click(object sender, RoutedEventArgs e)
        {
            addnewtxt.Text = "Apply";
            addnewtxt.Margin = new Thickness(43, 0, 0, 0);
            {
                BrandTxt.Foreground = (Brush)new BrushConverter().ConvertFromString("#525252");
                BrandBox.Style = FindResource("BrandBoxStyle") as Style;

                EngineTxt.Foreground = (Brush)new BrushConverter().ConvertFromString("#525252");
                EngineBox.Style = FindResource("EngineBoxStyle") as Style;

                PowerTxt.Foreground = (Brush)new BrushConverter().ConvertFromString("#525252");
                PowerBox.Style = FindResource("PowerBoxStyle") as Style;

                PlacesTxt.Foreground = (Brush)new BrushConverter().ConvertFromString("#525252");
                PlacesBox.Style = FindResource("PlacesBoxStyle") as Style;

                SittingTxt.Foreground = (Brush)new BrushConverter().ConvertFromString("#525252");
                SittingBox.Style = FindResource("SittingBoxStyle") as Style;

                YesCheckBox.Style = FindResource("CheckBoxStyle1") as Style;
                NoCheckBox.Style = FindResource("CheckBoxStyle1") as Style;
            }
            
            Transport itemToEdit = (sender as Button).Tag as Transport;

            {
                
                BrandBox.Text = itemToEdit.Brand;
                EngineBox.Text = itemToEdit.Engine;
                PowerBox.Text = itemToEdit.Power;
                DoorscounttButTxt.Text = itemToEdit.Doors;
                PlacesBox.Text = itemToEdit.Places;
                AxlescounttButTxt.Text = itemToEdit.Axles;
                SittingBox.Text = itemToEdit.Sitting;
                if (itemToEdit.Low == "Yes")
                {
                    YesCheckBox.IsChecked = true;
                }
                else
                {
                    NoCheckBox.IsChecked = true;
                }

            }
            currentEditing = int.Parse(itemToEdit.Number);

            DoubleAnimation fadeInAnimation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.2));
            addEditTransport.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);

            addEditTransport.IsHitTestVisible = true;

        }

        private void BackToMainButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPage(0);
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

            return Number == other.Number; 
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
