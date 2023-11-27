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
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using System.IO;
using System.Windows.Controls.Ribbon;
using System.Runtime.CompilerServices;
using Microsoft.Win32;
using Microsoft.VisualBasic;
using System.ComponentModel;
using System.Windows.Controls.Primitives;

namespace kursova
{

    public partial class MainWindow : Window
    {
        private ObservableCollection<PublicTransport> AllTransports = new ObservableCollection<PublicTransport>();
        private ObservableCollection<PublicTransport> currentPageTransports;
        private ObservableCollection<PublicTransport> currentData = new ObservableCollection<PublicTransport>();
        private int itemsPerPage = 8;
        private int currentPage = 0;
        private List<PublicTransport> formatedData = new List<PublicTransport>();
        ObservableCollection<PublicTransport> dataCopy;

        static string GetEnumDescription(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            return attribute == null ? value.ToString() : attribute.Description;
        }
        public MainWindow()
        {
            InitializeComponent();

            foreach (Engines engine in Enum.GetValues(typeof(Engines)))
            {
                EngineBox.Items.Add(GetEnumDescription(engine));
            }

            //for (int i = 0; i < 12; i++)
            //{
            //    PublicTransport newTransport = new PublicTransport
            //    {
            //        Brand = "Electron",
            //        EngineType = 0,
            //        EnginePower = 854,
            //        NumberOfAxles = 10,
            //        PassengerCapacity = 255,
            //        SeatingCapacity = 56,
            //        NumberOfDoors = 10,
            //        LowFloor = "Yes"
            //    };

            //    newTransport.Number = (AllTransports.Count + 1);

            //    AllTransports.Add(newTransport);
            //}

            currentData = AllTransports;
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
            return (int)Math.Ceiling((double)currentData.Count / itemsPerPage);
        }

        private void ShowPage(int page)
        {

            currentPage = page;
            int startIndex = currentPage * itemsPerPage;
            int endIndex = Math.Min(startIndex + itemsPerPage, currentData.Count);
            currentPageTransports = new ObservableCollection<PublicTransport>(currentData.Skip(startIndex).Take(itemsPerPage));

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
            int totalPageCount = (int)Math.Ceiling((double)AllTransports.Count / itemsPerPage);

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
            PublicTransport itemToDelete = (sender as Button).Tag as PublicTransport;

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

                    currentData.Remove(itemToDelete);

                    AllTransports.RemoveAt(dataCopy[itemToDelete.Number-1].Number-1);

                    for (int i = 0; i < AllTransports.Count; i++)
                    {
                        AllTransports[i].Number = (i + 1);
                    }
                    for (int i = 0; i < currentData.Count; i++)
                    {
                        currentData[i].Number = (i + 1);
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
                EngineBox.Style = FindResource("ComboBoxStyle") as Style;

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
            if (int.Parse(DoorscounttButTxt.Text) > 0)
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
            if (addnewtxt.Text == "Add")
            {
                PublicTransport transport = new PublicTransport();
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
                if (EngineBox.SelectedItem == null || string.IsNullOrEmpty(EngineBox.SelectedItem.ToString()))
                {

                    EngineTxt.Foreground = Brushes.Red;

                    allFieldsEntered = false;
                }
                else
                {
                    if (EngineBox.SelectedItem != null && Enum.TryParse(EngineBox.SelectedItem.ToString(), out Engines selectedEngine))
                    {
                        transport.EngineType = selectedEngine;
                    }
                }
                if (string.IsNullOrEmpty(PowerBox.Text))
                {
                    PowerBox.Style = FindResource("NotEnteredPowerBoxStyle") as Style;
                    PowerTxt.Foreground = Brushes.Red;

                    allFieldsEntered = false;
                }
                else
                {
                    transport.EnginePower = int.Parse(PowerBox.Text);
                }
                if (string.IsNullOrEmpty(PlacesBox.Text))
                {
                    PlacesBox.Style = FindResource("NotEnteredPlacesBoxStyle") as Style;
                    PlacesTxt.Foreground = Brushes.Red;

                    allFieldsEntered = false;
                }
                else
                {
                    transport.PassengerCapacity = int.Parse(PlacesBox.Text);
                }
                if (string.IsNullOrEmpty(SittingBox.Text))
                {
                    SittingBox.Style = FindResource("NotEnteredSittingBoxStyle") as Style;
                    SittingTxt.Foreground = Brushes.Red;

                    allFieldsEntered = false;
                }
                else
                {
                    transport.SeatingCapacity = int.Parse(SittingBox.Text);
                }

                transport.NumberOfDoors = int.Parse(DoorscounttButTxt.Text);
                transport.NumberOfAxles = int.Parse(AxlescounttButTxt.Text);

                if (!(YesCheckBox.IsChecked == true || NoCheckBox.IsChecked == true))
                {
                    YesCheckBox.Style = FindResource("NotEnteredCheckBoxStyle1") as Style;
                    NoCheckBox.Style = FindResource("NotEnteredCheckBoxStyle1") as Style;

                }
                else
                {
                    transport.LowFloor = (YesCheckBox.IsChecked == true) ? "Yes" : "No";

                }

                if (allFieldsEntered)
                {
                    transport.Number = (AllTransports.Count + 1);


                    AllTransports.Add(transport);
                    currentData = AllTransports;
                    ShowPage(currentPage);

                    DoubleAnimation fadeOutAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
                    addEditTransport.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

                    addEditTransport.IsHitTestVisible = false;

                    DoubleAnimation fadeInAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
                    backToMain.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);

                    backToMain.IsHitTestVisible = false;
                }

            }
            else if (addnewtxt.Text == "Apply")
            {
                PublicTransport transport = new PublicTransport();

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
                if (EngineBox.SelectedItem == null || string.IsNullOrEmpty(EngineBox.SelectedItem.ToString()))
                {

                    EngineTxt.Foreground = Brushes.Red;

                    allFieldsEntered = false;
                }
                else
                {
                    if (EngineBox.SelectedItem != null && Enum.TryParse(EngineBox.SelectedItem.ToString(), out Engines selectedEngine))
                    {
                        transport.EngineType = selectedEngine;
                    }
                }
                if (string.IsNullOrEmpty(PowerBox.Text))
                {
                    PowerBox.Style = FindResource("NotEnteredPowerBoxStyle") as Style;
                    PowerTxt.Foreground = Brushes.Red;

                    allFieldsEntered = false;
                }
                else
                {
                    transport.EnginePower = int.Parse(PowerBox.Text);
                }
                if (string.IsNullOrEmpty(PlacesBox.Text))
                {
                    PlacesBox.Style = FindResource("NotEnteredPlacesBoxStyle") as Style;
                    PlacesTxt.Foreground = Brushes.Red;

                    allFieldsEntered = false;
                }
                else
                {
                    transport.PassengerCapacity = int.Parse(PlacesBox.Text);
                }
                if (string.IsNullOrEmpty(SittingBox.Text))
                {
                    SittingBox.Style = FindResource("NotEnteredSittingBoxStyle") as Style;
                    SittingTxt.Foreground = Brushes.Red;

                    allFieldsEntered = false;
                }
                else
                {
                    transport.SeatingCapacity = int.Parse(SittingBox.Text);
                }

                transport.NumberOfDoors = int.Parse(DoorscounttButTxt.Text);
                transport.NumberOfAxles = int.Parse(AxlescounttButTxt.Text);

                if (!(YesCheckBox.IsChecked == true || NoCheckBox.IsChecked == true))
                {
                    YesCheckBox.Style = FindResource("NotEnteredCheckBoxStyle1") as Style;
                    NoCheckBox.Style = FindResource("NotEnteredCheckBoxStyle1") as Style;

                }
                else
                {
                    transport.LowFloor = (YesCheckBox.IsChecked == true) ? "Yes" : "No";

                }

                if (allFieldsEntered)
                {
                    if(currentData != AllTransports)
                    {
                        transport.Number = dataCopy[currentEditing - 1].Number - 1;
                        AllTransports[dataCopy[currentEditing - 1].Number - 1] = transport;
                    }

                    transport.Number = (currentEditing);
                    
                    currentData[currentEditing - 1] = transport;

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
            addnewtxt.Margin = new Thickness(42, 0, 0, 0);
            {
                BrandTxt.Foreground = (Brush)new BrushConverter().ConvertFromString("#525252");
                BrandBox.Style = FindResource("BrandBoxStyle") as Style;

                EngineTxt.Foreground = (Brush)new BrushConverter().ConvertFromString("#525252");
                EngineBox.Style = FindResource("ComboBoxStyle") as Style;

                PowerTxt.Foreground = (Brush)new BrushConverter().ConvertFromString("#525252");
                PowerBox.Style = FindResource("PowerBoxStyle") as Style;

                PlacesTxt.Foreground = (Brush)new BrushConverter().ConvertFromString("#525252");
                PlacesBox.Style = FindResource("PlacesBoxStyle") as Style;

                SittingTxt.Foreground = (Brush)new BrushConverter().ConvertFromString("#525252");
                SittingBox.Style = FindResource("SittingBoxStyle") as Style;

                YesCheckBox.Style = FindResource("CheckBoxStyle1") as Style;
                NoCheckBox.Style = FindResource("CheckBoxStyle1") as Style;
            }

            PublicTransport itemToEdit = (sender as Button).Tag as PublicTransport;

            {

                BrandBox.Text = itemToEdit.Brand;
                EngineBox.Text = GetEnumDescription(itemToEdit.EngineType);
                PowerBox.Text = itemToEdit.EnginePower.ToString();
                DoorscounttButTxt.Text = itemToEdit.NumberOfDoors.ToString();
                PlacesBox.Text = itemToEdit.PassengerCapacity.ToString();
                AxlescounttButTxt.Text = itemToEdit.NumberOfAxles.ToString();
                SittingBox.Text = itemToEdit.SeatingCapacity.ToString();
                if (itemToEdit.LowFloor == "Yes")
                {
                    YesCheckBox.IsChecked = true;
                }
                else
                {
                    NoCheckBox.IsChecked = true;
                }

            }
            currentEditing = itemToEdit.Number;
            DoubleAnimation fadeInAnimation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.2));
            addEditTransport.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);

            addEditTransport.IsHitTestVisible = true;

        }

        private void BackToMainButton_Click(object sender, RoutedEventArgs e)
        {
            currentData = AllTransports;
            for (int i = 0; i < AllTransports.Count; i++)
            {
                AllTransports[i].Number = (i + 1);
            }

            ShowPage(0);
            DoubleAnimation fadeInAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
            backToMain.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);

            searchBox.Text = null;
            backToMain.IsHitTestVisible = false;
            if(formatedData.Count != 0)
                formatedData.Clear();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchTerm = searchBox.Text.ToLower();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                table.ItemsSource = AllTransports;
            }
            else
            {
                formatedData = AllTransports.Where(item =>
                item.Brand.ToLower().Contains(searchTerm) ||
                GetEnumDescription(item.EngineType).ToLower().Contains(searchTerm) ||
                item.Number.ToString().Contains(searchTerm) ||
                item.EnginePower.ToString().Contains(searchTerm) ||
                item.NumberOfAxles.ToString().Contains(searchTerm) ||
                item.NumberOfDoors.ToString().Contains(searchTerm) ||
                item.SeatingCapacity.ToString().Contains(searchTerm) ||
                item.PassengerCapacity.ToString().Contains(searchTerm)
            ).ToList();
                table.ItemsSource = formatedData;
                ObservableCollection<PublicTransport> data = new ObservableCollection<PublicTransport>(formatedData);
                dataCopy = new ObservableCollection<PublicTransport>(data.Select(original => new PublicTransport(original)));
                for (int i = 0; i < data.Count; i++)
                {
                    data[i].Number = (i + 1);
                }
                currentData = data;
                ShowPage(0);
            }
            DoubleAnimation fadeInAnimation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.2));
            backToMain.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);

            backToMain.IsHitTestVisible = true;
        }

        private void OpenFromFile_Click(object sender, RoutedEventArgs e)
        {
            currentData.Clear();
            AllTransports.Clear();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                try
                {
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        reader.ReadLine();
                        reader.ReadLine();

                        while (!reader.EndOfStream)
                        {
                            PublicTransport transport = new PublicTransport();
                            transport.ReadFromFile(reader);
                            transport.Number = (AllTransports.Count + 1);
                            AllTransports.Add(transport);
                        }
                    }
                    dataCopy = new ObservableCollection<PublicTransport>(AllTransports);
                    currentData = AllTransports;
                    ShowPage(0);
                }
                catch (Exception ex)
                {
                    AllTransports.Clear();
                    MessageOutput messageOutput = new MessageOutput($"Error reading file.\n{ex.Message}");
                    if (messageOutput.ShowDialog() == true)
                    {

                    }
                }
            }

        }
        private void SaveToFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (currentData.Count > 0)
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();

                    saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        try
                        {
                    
                                string filePath = saveFileDialog.FileName;

                                using (StreamWriter writer = new StreamWriter(filePath))
                                {
                                    const int columnWidth = 16;

                                    writer.WriteLine(string.Format("{0,-" + columnWidth + "} {1,-" + columnWidth + "} {2,-" + columnWidth + "} {3,-" + columnWidth + "} {4,-" + columnWidth + "} {5,-" + columnWidth + "} {6,-" + columnWidth + "} {7,-" + columnWidth + "}",
                                        "Brand", "Engine", "Power", "Axles", "Places", "Seating", "Doors", "Low"));
                                    writer.WriteLine(string.Format(""));

                                    foreach (var transport in currentData)
                                    {
                                        transport.WriteToFile(writer);
                                    }
                                }
                    
                   
                        }
                        catch (Exception ex)
                        {
                            MessageOutput messageOutput = new MessageOutput($"{ex.Message}");
                            if (messageOutput.ShowDialog() == true)
                            {

                            }
                        }
                    }
                }
                else
                {
                    throw new Exception("No data to save");
                }
            }
            catch (Exception ex)
            {
                MessageOutput messageOutput = new MessageOutput($"Error: {ex.Message}");
                if (messageOutput.ShowDialog() == true)
                {

                }
            }
        }
        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(currentData.Count > 0)
                {
                    FlowDocument flowDocument = CreateFlowDocument();


                    if (flowDocument != null)
                    {
                        PrintDialog printDialog = new PrintDialog();

                        if (printDialog.ShowDialog() == true)
                        {
                            flowDocument.PageHeight = printDialog.PrintableAreaHeight;
                            flowDocument.PageWidth = printDialog.PrintableAreaWidth;
                            flowDocument.PagePadding = new Thickness(30, 30, 30, 20);

                            flowDocument.ColumnGap = 0;

                            flowDocument.ColumnWidth = (flowDocument.PageWidth -
                                                   flowDocument.ColumnGap -
                                                   flowDocument.PagePadding.Left -
                                                   flowDocument.PagePadding.Right);

                            var paginator = ((IDocumentPaginatorSource)flowDocument).DocumentPaginator;
                            printDialog.PrintDocument(paginator, "Public Transport Table");
                        }
                    }
                    else
                    {
                        MessageOutput messageOutput = new MessageOutput("Data is null");
                        if (messageOutput.ShowDialog() == true)
                        {

                        }
                    }
                }
                else
                {
                    throw new Exception("No data to print");
                }
            }
            catch (Exception ex)
            {
                
                MessageOutput messageOutput = new MessageOutput($"Error: {ex.Message}");
                if (messageOutput.ShowDialog() == true)
                {

                }
            }
        }

        private FlowDocument CreateFlowDocument()
        {
            FlowDocument flowDocument = new FlowDocument();
            flowDocument.PageWidth = 793.76;
            flowDocument.PageHeight = 1122.52;
            int maxRowsPerPage = 24;
            try
            {
                Table tableToPrint = new Table();

                tableToPrint.Columns.Add(new TableColumn());
                tableToPrint.Columns.Add(new TableColumn());
                tableToPrint.Columns.Add(new TableColumn());
                tableToPrint.Columns.Add(new TableColumn());
                tableToPrint.Columns.Add(new TableColumn());
                tableToPrint.Columns.Add(new TableColumn());
                tableToPrint.Columns.Add(new TableColumn());
                tableToPrint.Columns.Add(new TableColumn());
                tableToPrint.Columns.Add(new TableColumn());

                for (int columnIndex = 0; columnIndex < tableToPrint.Columns.Count; columnIndex++)
                {
                    if (columnIndex == 0)
                    {
                        tableToPrint.Columns[columnIndex].Width = new GridLength(40);
                    }
                    else
                    {
                        tableToPrint.Columns[columnIndex].Width = new GridLength(90);
                    }
                }

                flowDocument.Blocks.Add(tableToPrint);

                TableRow headerRow = new TableRow();
                headerRow.Cells.Add(new TableCell(new Paragraph(new Run("№") { FontSize = 16, FontWeight = FontWeights.Bold, FontFamily = new FontFamily("Arial") })));
                headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Brand") { FontSize = 16, FontWeight = FontWeights.Bold, FontFamily = new FontFamily("Arial") })));
                headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Engine") { FontSize = 16, FontWeight = FontWeights.Bold, FontFamily = new FontFamily("Arial") })));
                headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Power") { FontSize = 16, FontWeight = FontWeights.Bold, FontFamily = new FontFamily("Arial") })));
                headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Axles") { FontSize = 16, FontWeight = FontWeights.Bold, FontFamily = new FontFamily("Arial") })));
                headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Places") { FontSize = 16, FontWeight = FontWeights.Bold, FontFamily = new FontFamily("Arial") })));
                headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Seating") { FontSize = 16, FontWeight = FontWeights.Bold, FontFamily = new FontFamily("Arial") })));
                headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Doors") { FontSize = 16, FontWeight = FontWeights.Bold, FontFamily = new FontFamily("Arial") })));
                headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Low") { FontSize = 16, FontWeight = FontWeights.Bold, FontFamily = new FontFamily("Arial") })));
                tableToPrint.RowGroups.Add(new TableRowGroup());
                tableToPrint.RowGroups[0].Rows.Add(headerRow);

                TableRow emptyRow = new TableRow();
                emptyRow.Cells.Add(new TableCell(new Paragraph(new Run(""))));
                tableToPrint.RowGroups[0].Rows.Add(emptyRow);

                int currentRowCount = 0;

                foreach (var data in currentData)
                {
                    if (currentRowCount >= maxRowsPerPage)
                    {
                        flowDocument.Blocks.Add(new Section());
                        flowDocument.Blocks.Add(tableToPrint);

                        TableRow newHeaderRow = new TableRow();
                        newHeaderRow.Cells.Add(new TableCell(new Paragraph(new Run("№") { FontSize = 16, FontWeight = FontWeights.Bold, FontFamily = new FontFamily("Arial") })));
                        newHeaderRow.Cells.Add(new TableCell(new Paragraph(new Run("Brand") { FontSize = 16, FontWeight = FontWeights.Bold, FontFamily = new FontFamily("Arial") })));
                        newHeaderRow.Cells.Add(new TableCell(new Paragraph(new Run("Engine") { FontSize = 16, FontWeight = FontWeights.Bold, FontFamily = new FontFamily("Arial") })));
                        newHeaderRow.Cells.Add(new TableCell(new Paragraph(new Run("Power") { FontSize = 16, FontWeight = FontWeights.Bold, FontFamily = new FontFamily("Arial") })));
                        newHeaderRow.Cells.Add(new TableCell(new Paragraph(new Run("Axles") { FontSize = 16, FontWeight = FontWeights.Bold, FontFamily = new FontFamily("Arial") })));
                        newHeaderRow.Cells.Add(new TableCell(new Paragraph(new Run("Places") { FontSize = 16, FontWeight = FontWeights.Bold, FontFamily = new FontFamily("Arial") })));
                        newHeaderRow.Cells.Add(new TableCell(new Paragraph(new Run("Seating") { FontSize = 16, FontWeight = FontWeights.Bold, FontFamily = new FontFamily("Arial") })));
                        newHeaderRow.Cells.Add(new TableCell(new Paragraph(new Run("Doors") { FontSize = 16, FontWeight = FontWeights.Bold, FontFamily = new FontFamily("Arial") })));
                        newHeaderRow.Cells.Add(new TableCell(new Paragraph(new Run("Low") { FontSize = 16, FontWeight = FontWeights.Bold, FontFamily = new FontFamily("Arial") })));
                        tableToPrint.RowGroups[0].Rows.Add(newHeaderRow);

                        TableRow newEmptyRow = new TableRow();
                        newEmptyRow.Cells.Add(new TableCell(new Paragraph(new Run(""))));
                        tableToPrint.RowGroups[0].Rows.Add(newEmptyRow);

                        currentRowCount = 0;
                    }

                    TableRow dataRow = new TableRow();
                    dataRow.Cells.Add(new TableCell(new Paragraph(new Run(data.Number.ToString()) { FontSize = 14, FontFamily = new FontFamily("Arial") })));
                    dataRow.Cells.Add(new TableCell(new Paragraph(new Run(data.Brand) { FontSize = 14, FontFamily = new FontFamily("Arial") })));
                    dataRow.Cells.Add(new TableCell(new Paragraph(new Run(GetEnumDescription(data.EngineType)) { FontSize = 14, FontFamily = new FontFamily("Arial") })));
                    dataRow.Cells.Add(new TableCell(new Paragraph(new Run(data.EnginePower.ToString()) { FontSize = 14, FontFamily = new FontFamily("Arial") })));
                    dataRow.Cells.Add(new TableCell(new Paragraph(new Run(data.NumberOfAxles.ToString()) { FontSize = 14, FontFamily = new FontFamily("Arial") })));
                    dataRow.Cells.Add(new TableCell(new Paragraph(new Run(data.PassengerCapacity.ToString()) { FontSize = 14, FontFamily = new FontFamily("Arial") })));
                    dataRow.Cells.Add(new TableCell(new Paragraph(new Run(data.SeatingCapacity.ToString()) { FontSize = 14, FontFamily = new FontFamily("Arial") })));
                    dataRow.Cells.Add(new TableCell(new Paragraph(new Run(data.NumberOfDoors.ToString()) { FontSize = 14, FontFamily = new FontFamily("Arial") })));
                    dataRow.Cells.Add(new TableCell(new Paragraph(new Run(data.LowFloor) { FontSize = 14, FontFamily = new FontFamily("Arial") })));

                    TableRow spaceRow = new TableRow();
                    spaceRow.Cells.Add(new TableCell(new Paragraph(new Run("")) { Margin = new Thickness(0, 3, 0, 2) }));

                    tableToPrint.RowGroups[0].Rows.Add(dataRow);
                    tableToPrint.RowGroups[0].Rows.Add(spaceRow);

                    currentRowCount += 1;
                }

            }
            catch (Exception ex)
            {
                MessageOutput messageOutput = new MessageOutput($"Error creating FlowDocument:\n{ex.Message}");
                if (messageOutput.ShowDialog() == true)
                {

                }
                return null;
            }

            return flowDocument;
        }


        private void Sort_Click(object sender, RoutedEventArgs e)

        {
            if(currentData.Count > 0)
            {
                currentData = ShellSort(currentData);
                for (int i = 0; i < currentData.Count; i++)
                {
                    currentData[i].Number = (i + 1);
                }
                ShowPage(0);
            }
            else
            {
                MessageOutput messageOutput = new MessageOutput("No data to sort");
                if(messageOutput.ShowDialog() == true)
                {

                }
            }
            
        }

        private void EngineAverage_Click(object sender, RoutedEventArgs e)
        {
            if(currentData.Count > 0)
            {
                double averageEnginePower = CalculateEnginePowerAverage(currentData);
                AverageOutput averageoOutput = new AverageOutput($"Average engine power is: {averageEnginePower}");
                if(averageoOutput.ShowDialog() == true)
                {

                }
            }
            else
            {
                MessageOutput messageOutput = new MessageOutput("No data to calculate average");
                if (messageOutput.ShowDialog() == true)
                {

                }
            }
        }
        private void PowEngineSeating_Click(object sender, RoutedEventArgs e)
        {
            if(currentData.Count > 0)
            {
                currentData = MostPowerfulEngineAndSpecificSeating(currentData);
                ShowPage(0);
                DoubleAnimation fadeInAnimation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.2));
                backToMain.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);

                backToMain.IsHitTestVisible = true;
            }
            else
            {
                MessageOutput messageOutput = new MessageOutput("No data to process");
                if (messageOutput.ShowDialog() == true)
                {

                }
            }
            
        }

        private void PowEngineAxels_Click(object sender, RoutedEventArgs e)
        {
            if(currentData.Count > 0)
            {
                currentData = PowerfulAverageAndSpecificAxels(currentData);
                for (int i = 0; i < currentData.Count; i++)
                {
                    currentData[i].Number = (i + 1);
                }
                ShowPage(0);

                DoubleAnimation fadeInAnimation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.2));
                backToMain.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);

                backToMain.IsHitTestVisible = true;
            }
            else
            {
                MessageOutput messageOutput = new MessageOutput("No data to process");
                if (messageOutput.ShowDialog() == true)
                {

                }
            }
        }

        private void DoorFloorPasangers_Click(object sender, RoutedEventArgs e)
        {
            if(currentData.Count > 0)
            {
                currentData = DoorsFloorPlaces(currentData);
                for (int i = 0; i < currentData.Count; i++)
                {
                    currentData[i].Number = (i + 1);
                }
                ShowPage(0);

                DoubleAnimation fadeInAnimation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.2));
                backToMain.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);

                backToMain.IsHitTestVisible = true;
            }
            else
            {
                MessageOutput messageOutput = new MessageOutput("No data to process");
                if (messageOutput.ShowDialog() == true)
                {

                }
            }
            
        }

        //------Task Functions------------------------

        private ObservableCollection<PublicTransport> ShellSort(ObservableCollection<PublicTransport> toSort)
        {
            int n = toSort.Count;

            for (int gap = n / 2; gap > 0; gap /= 2)
            {
                for (int i = gap; i < n; i++)
                {
                    PublicTransport temp = toSort[i];

                    int j;
                    for (j = i; j >= gap && Compare(toSort[j - gap], temp) > 0; j -= gap)
                    {
                        toSort[j] = toSort[j - gap];
                    }

                    toSort[j] = temp;
                }
            }

            return toSort;
        }

        private int Compare(PublicTransport a, PublicTransport b)
        {
            if(a.PassengerCapacity > b.PassengerCapacity)
            {
                return 1;
            }
            else if(a.PassengerCapacity < b.PassengerCapacity)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        private double CalculateEnginePowerAverage(ObservableCollection<PublicTransport> toCalculate)
        {
            int sum = 0;
            foreach(var item in toCalculate){
                sum += item.EnginePower;
            }
            return sum/toCalculate.Count;
        }
        private ObservableCollection<PublicTransport> MostPowerfulEngineAndSpecificSeating(ObservableCollection<PublicTransport> toSearch)
        {
            try
            {
                List<PublicTransport> mostPowerfulElectricList = new List<PublicTransport>();
                double maxEnginePower = double.MinValue;

                for (int i = 0; i < toSearch.Count; i++)
                {
                    PublicTransport transport = toSearch[i];

                    if (transport.SeatingCapacity >= 20 && transport.SeatingCapacity <= 26 && transport.EngineType == Engines.Electric)
                    {
                        if (transport.EnginePower > maxEnginePower)
                        {
                            mostPowerfulElectricList.Clear();
                            maxEnginePower = transport.EnginePower;
                        }

                        mostPowerfulElectricList.Add(transport);
                    }
                }

                dataCopy = new ObservableCollection<PublicTransport>(mostPowerfulElectricList.Select(transport => new PublicTransport(transport)));

                for (int i = 0; i < mostPowerfulElectricList.Count; i++)
                {
                    mostPowerfulElectricList[i].Number = i + 1;
                }

                return new ObservableCollection<PublicTransport>(mostPowerfulElectricList);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return new ObservableCollection<PublicTransport>();
            }
        }

        private ObservableCollection<PublicTransport> PowerfulAverageAndSpecificAxels(ObservableCollection<PublicTransport> toSearch)
        {
            try
            {
                ObservableCollection<PublicTransport> filteredData = new ObservableCollection<PublicTransport>();

                for (int i = 0; i < toSearch.Count; i++)
                {
                    PublicTransport transport = toSearch[i];

                    if (transport.NumberOfAxles > 4 && transport.EnginePower > CalculateEnginePowerAverage(toSearch))
                    {
                        filteredData.Add(transport);
                    }
                }

                dataCopy = new ObservableCollection<PublicTransport>(filteredData.Select(original => new PublicTransport(original)));

                if (filteredData != null)
                {
                    return filteredData;
                }
                else
                {
                    return new ObservableCollection<PublicTransport>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return new ObservableCollection<PublicTransport>();
            }
        }

        private ObservableCollection<PublicTransport> DoorsFloorPlaces(ObservableCollection<PublicTransport> toSearch)
        {
            try
            {
                ObservableCollection<PublicTransport> filteredData = new ObservableCollection<PublicTransport>();

                for (int i = 0; i < toSearch.Count; i++)
                {
                    PublicTransport transport = toSearch[i];

                    if (transport.NumberOfDoors > 4 && transport.PassengerCapacity > 40 && transport.LowFloor == "Yes")
                    {
                        filteredData.Add(transport);
                    }
                }

                dataCopy = new ObservableCollection<PublicTransport>(filteredData.Select(original => new PublicTransport(original)));

                if (filteredData != null)
                {
                    return filteredData;
                }
                else
                {
                    return new ObservableCollection<PublicTransport>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return new ObservableCollection<PublicTransport>();
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

}

