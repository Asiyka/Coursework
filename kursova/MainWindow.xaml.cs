using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows.Media.Animation;
using System.IO;
using Microsoft.Win32;
using System.ComponentModel;

namespace kursova
{

    public partial class MainWindow : Window
    {
        //Number of that can be displayed per page
        private int itemsPerPage;
        //Index of current page
        private int currentPage;

        //Collection that conatins original data read from file
        private ObservableCollection<CPublicTransport> AllTransports;

        //Data that currently represented(data from search etc.)
        private ObservableCollection<CPublicTransport> currentData;
        //Copy of data to save original indexation to have ability to modify elements in original data
        ObservableCollection<CPublicTransport> dataCopy;

        //Data currently represented in table
        private ObservableCollection<CPublicTransport> currentPageTransports;
        
        
        public MainWindow()
        {
            InitializeComponent();
            //variables initialization
            itemsPerPage = 8;
            currentPage = 0;
            //Collections initialization
            AllTransports = new ObservableCollection<CPublicTransport>();
            currentData = new ObservableCollection<CPublicTransport>();

            //ComboBox initialization
            foreach (Engines engine in Enum.GetValues(typeof(Engines)))
            {
                EngineBox.Items.Add(GetEnumDescription(engine));
            }

            //Show empty table
            currentData = AllTransports;
            ShowPage(0);

            //Row loader to apply specific style for rows
            table.LoadingRow += (sender, e) =>
            {
                //get row
                DataGridRow row = e.Row as DataGridRow;

                if (row != null)
                {
                    int rowIndex = e.Row.GetIndex();

                    //set row style based on position in table
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

        //Function to get enum description to show name of engine rather than number in enum
        private static string GetEnumDescription(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            return attribute == null ? value.ToString() : attribute.Description;
        }

        //Calculate number of total pages
        private int TotalPages()
        {
            return (int)Math.Ceiling((double)currentData.Count / itemsPerPage);
        }

        //Function to show specific page
        private void ShowPage(int page)
        {
            //change current shown page
            currentPage = page;

            //get new 8 element to show based on choosen page
            int startIndex = currentPage * itemsPerPage;
            int endIndex = Math.Min(startIndex + itemsPerPage, currentData.Count);

            //save this elements
            currentPageTransports = new ObservableCollection<CPublicTransport>(currentData.Skip(startIndex).Take(itemsPerPage));

            //show them
            table.ItemsSource = currentPageTransports;

            //adjust pagination buttons
            UpdatePageButtons();
        }
        
        

        //------Pagination--------------------

        //Function to adjust pagination
        private void UpdatePageButtons()
        {

            int totalPageCount = TotalPages();

            //buttons are showen based on number of all pages
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
                    firstBut.Opacity = 1;
                    firstBut.IsHitTestVisible = true;

                    secBut.Opacity = 2;
                    secBut.IsHitTestVisible = true;

                    dotButTxt.Text = "...";

                    dotBut.Opacity = 1;
                    dotBut.IsHitTestVisible = true;


                    lastBut.Opacity = 1;
                    lastBut.IsHitTestVisible = true;

                    lastButTxt.Text = totalPageCount.ToString();

                    nextBut.Margin = new Thickness(110, -20, 0, 0);
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
                    firstBut.Opacity = 1;
                    firstBut.IsHitTestVisible = true;

                    secBut.Opacity = 2;
                    secBut.IsHitTestVisible = true;

                    dotButTxt.Text = "...";

                    dotBut.Opacity = 1;
                    dotBut.IsHitTestVisible = true;


                    lastBut.Opacity = 1;
                    lastBut.IsHitTestVisible = true;

                    lastButTxt.Text = totalPageCount.ToString();

                    nextBut.Margin = new Thickness(110, -20, 0, 0);
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

            //update styles to highlight current shown page
            UpdateButtonStyles();
        }

        //Function to change style in pagination for active page
        private void UpdateButtonStyles()
        {

            //for 3 buttons in pagination (first and last arrows dont count)
            for (int i = 1; i < 5; i++)
            {
                var button = (Button)PaginationStackPanel.Children[i];

                var textBlock = (TextBlock)button.Content;

                if (textBlock.Text != ".....")
                {
                    int buttonPage = int.Parse(textBlock.Text);

                    if (buttonPage >= 1 && buttonPage <= TotalPages())
                    {
                        //choose style for pagination block whether its number equals current page
                        button.Style = (buttonPage == currentPage + 1) ? FindResource("ActivePageButton") as Style : FindResource("UnActivePageButton") as Style;
                    }
                }
            }
        }

        //Move to next page
        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage + 1 < TotalPages())
            {
                ShowPage(currentPage + 1);
            }
        }
        //Move to prev page
        private void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 0)
            {
                ShowPage(currentPage - 1);
            }
        }



        //Animation that is played when button "Table" is pressed
        
        //check if button pressed second time to play reversed animation
        private bool secondClickAnimation = false;
        private void onTableButton_Click(object sender, RoutedEventArgs e)
        {
            if (!secondClickAnimation)
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

                secondClickAnimation = true;
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

                secondClickAnimation = false;
            }
        }



        //-------Add/Edit element------------------

        int currentEditing = -1;

        //Button to open window to add new element
        private void OpenAddNewTransportButton_Click(object sender, RoutedEventArgs e)
        {
            //set add/edit window into add state
            addnewtxt.Text = "Add";
            addnewtxt.Margin = new Thickness(46, 0, 0, 0);
            
            //set default styles
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
            //make fields empty
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
            //animation to show add/edit window
            DoubleAnimation fadeInAnimation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.2));
            addEditTransport.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);

            addEditTransport.IsHitTestVisible = true;
        }

        //Button to open window to edit selected element
        private void OpenEditTransportButton_Click(object sender, RoutedEventArgs e)
        {
            //set add/edit window into edit state
            addnewtxt.Text = "Apply";
            addnewtxt.Margin = new Thickness(42, 0, 0, 0);
            
            //set base styles
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

            //get item that will be edited
            CPublicTransport itemToEdit = (sender as Button).Tag as CPublicTransport;

            //fill fields with current inforamiton of selected elemenet
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

            //animation to open add/edit window
            DoubleAnimation fadeInAnimation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.2));
            addEditTransport.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);

            addEditTransport.IsHitTestVisible = true;

        }
        //Button to delete selected element
        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            //get elemenet to delete
            CPublicTransport itemToDelete = (sender as Button).Tag as CPublicTransport;

            if (itemToDelete != null)
            {
                //get row to delete
                DataGridRow row = table.ItemContainerGenerator.ContainerFromItem(itemToDelete) as DataGridRow;

                if (row != null)
                {
                    //animation for delete
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

                    //delete elemenet based if current table is main or subtable
                    
                    if(currentData == AllTransports) //main table
                    {
                        AllTransports.RemoveAt((dataCopy[itemToDelete.Number - 1].Number) - 1);
                    }
                    else //subtable
                    {
                        currentData.Remove(itemToDelete);
                        AllTransports.RemoveAt((dataCopy[itemToDelete.Number - 1].Number) - 1);
                    }

                    //renumerate elemenets 
                    for (int i = 0; i < currentData.Count; i++)
                    {
                        currentData[i].Number = (i + 1);
                    }

                    //refresh table
                    CollectionViewSource.GetDefaultView(table.ItemsSource).Refresh();
                    ShowPage(currentPage);
                }
            }
        }

        //Button to exit add/edit window
        private void exitAddEditButton_Click(object sender, RoutedEventArgs e)
        {
            //animation to close add/edit window
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
            addEditTransport.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

            addEditTransport.IsHitTestVisible = false;
        }

        //Button to apply add/edit
        private void AddEditTransportButton_Click(object sender, RoutedEventArgs e)

        {
            //to add new
            if (addnewtxt.Text == "Add")
            {
                CPublicTransport transport = new CPublicTransport(); //new elemenet to add to table
                bool allFieldsEntered = true; // check if all fields are entered

                if (string.IsNullOrEmpty(BrandBox.Text)) //if empty changes style to indicate that filed is empty
                {
                    BrandBox.Style = FindResource("NotEnteredBrandBoxStyle") as Style;
                    BrandTxt.Foreground = Brushes.Red;

                    allFieldsEntered = false;
                }
                else //if not empty adds data to new instance of classs
                {
                    transport.Brand = BrandBox.Text;
                }
                
                //same here
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

                if (string.IsNullOrEmpty(PowerBox.Text) )
                {
                    PowerBox.Style = FindResource("NotEnteredPowerBoxStyle") as Style;
                    PowerTxt.Foreground = Brushes.Red;

                    allFieldsEntered = false;
                }
                else if (PowerBox.Text.StartsWith("0"))
                {
                    PowerBox.Style = FindResource("NotEnteredPowerBoxStyle") as Style;
                    PowerTxt.Foreground = Brushes.Red;

                    allFieldsEntered = false;
                }
                else
                {
                    //check if number is not to big
                    try
                    {
                        transport.EnginePower = int.Parse(PowerBox.Text);
                    }
                    catch (Exception ex)
                    {
                        MessageOutput messageOutput = new MessageOutput($"Error reading engine power.\nValue was either too large or too small.");
                        if (messageOutput.ShowDialog() == true)
                        {
                        }
                        allFieldsEntered = false;

                    }
                }
                
                if (string.IsNullOrEmpty(PlacesBox.Text))
                {
                    PlacesBox.Style = FindResource("NotEnteredPlacesBoxStyle") as Style;
                    PlacesTxt.Foreground = Brushes.Red;

                    allFieldsEntered = false;
                } 
                else if (PlacesBox.Text.StartsWith("0"))
                {
                    PlacesBox.Style = FindResource("NotEnteredPlacesBoxStyle") as Style;
                    PlacesTxt.Foreground = Brushes.Red;

                    allFieldsEntered = false;
                }
                else
                {
                    try
                    {
                        transport.PassengerCapacity = int.Parse(PlacesBox.Text);
                    }
                    catch (Exception ex)
                    {
                        MessageOutput messageOutput = new MessageOutput($"Error reading number of places.\nValue was either too large or too small.");
                        if (messageOutput.ShowDialog() == true)
                        {
                        }
                        allFieldsEntered = false;

                    }
                }
                
                if (string.IsNullOrEmpty(SittingBox.Text))
                {
                    SittingBox.Style = FindResource("NotEnteredSittingBoxStyle") as Style;
                    SittingTxt.Foreground = Brushes.Red;

                    allFieldsEntered = false;
                }
                else if (SittingBox.Text.StartsWith("0"))
                {
                    SittingBox.Style = FindResource("NotEnteredSittingBoxStyle") as Style;
                    SittingTxt.Foreground = Brushes.Red;

                    allFieldsEntered = false;
                }
                else
                {
                    try
                    {
                        transport.SeatingCapacity = int.Parse(SittingBox.Text);
                    }
                    catch (Exception ex)
                    {
                        MessageOutput messageOutput = new MessageOutput($"Error reading seating capacity.\nValue was either too large or too small.");
                        if (messageOutput.ShowDialog() == true)
                        {
                        }
                        allFieldsEntered = false;

                    }
                }

                transport.NumberOfDoors = int.Parse(DoorscounttButTxt.Text);
                transport.NumberOfAxles = int.Parse(AxlescounttButTxt.Text);

                if (!(YesCheckBox.IsChecked == true || NoCheckBox.IsChecked == true))
                {
                    YesCheckBox.Style = FindResource("NotEnteredCheckBoxStyle1") as Style;
                    NoCheckBox.Style = FindResource("NotEnteredCheckBoxStyle1") as Style;
                    allFieldsEntered = false;
                }
                else
                {
                    transport.LowFloor = (YesCheckBox.IsChecked == true) ? "Yes" : "No";

                }

                if (allFieldsEntered)
                {
                    transport.Number = (AllTransports.Count + 1);

                    //add new element to main table
                    AllTransports.Add(transport);

                    //return from subtable
                    currentData = AllTransports;
                    dataCopy = new ObservableCollection<CPublicTransport>(AllTransports);
                    
                    ShowPage(currentPage);

                    //animation to close add/edit window
                    DoubleAnimation fadeOutAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
                    addEditTransport.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

                    addEditTransport.IsHitTestVisible = false;

                    //animation to hide "back to main table" button if elemenet was added from subtable
                    DoubleAnimation fadeInAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
                    backToMain.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);

                    backToMain.IsHitTestVisible = false;
                }

            }
            //to edit current
            else if (addnewtxt.Text == "Apply")
            {
                //same here as in "add"
                CPublicTransport transport = new CPublicTransport();

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
                else if (PowerBox.Text.StartsWith("0"))
                {
                    PowerBox.Style = FindResource("NotEnteredPowerBoxStyle") as Style;
                    PowerTxt.Foreground = Brushes.Red;

                    allFieldsEntered = false;
                }
                else
                {
                    try
                    {
                        transport.EnginePower = int.Parse(PowerBox.Text);
                    }
                    catch (Exception ex)
                    {
                        MessageOutput messageOutput = new MessageOutput($"Error reading engine power.\nValue was either too large or too small.");
                        if (messageOutput.ShowDialog() == true)
                        {
                        }
                        allFieldsEntered = false;

                    }
                }

                if (string.IsNullOrEmpty(PlacesBox.Text))
                {
                    PlacesBox.Style = FindResource("NotEnteredPlacesBoxStyle") as Style;
                    PlacesTxt.Foreground = Brushes.Red;

                    allFieldsEntered = false;
                }
                else if (PlacesBox.Text.StartsWith("0"))
                {
                    PlacesBox.Style = FindResource("NotEnteredPlacesBoxStyle") as Style;
                    PlacesTxt.Foreground = Brushes.Red;

                    allFieldsEntered = false;
                }
                else
                {
                    try
                    {
                        transport.PassengerCapacity = int.Parse(PlacesBox.Text);
                    }
                    catch (Exception ex)
                    {
                        MessageOutput messageOutput = new MessageOutput($"Error reading number of places.\nValue was either too large or too small.");
                        if (messageOutput.ShowDialog() == true)
                        {
                        }
                        allFieldsEntered = false;

                    }
                }

                if (string.IsNullOrEmpty(SittingBox.Text))
                {
                    SittingBox.Style = FindResource("NotEnteredSittingBoxStyle") as Style;
                    SittingTxt.Foreground = Brushes.Red;

                    allFieldsEntered = false;
                }
                else if (SittingBox.Text.StartsWith("0"))
                {
                    SittingBox.Style = FindResource("NotEnteredSittingBoxStyle") as Style;
                    SittingTxt.Foreground = Brushes.Red;

                    allFieldsEntered = false;
                }
                else
                {
                    try
                    {
                        transport.SeatingCapacity = int.Parse(SittingBox.Text);
                    }
                    catch (Exception ex)
                    {
                        MessageOutput messageOutput = new MessageOutput($"Error reading seating capacity.\nValue was either too large or too small.");
                        if (messageOutput.ShowDialog() == true)
                        {
                        }
                        allFieldsEntered = false;

                    }
                }

                transport.NumberOfDoors = int.Parse(DoorscounttButTxt.Text);
                transport.NumberOfAxles = int.Parse(AxlescounttButTxt.Text);

                if (!(YesCheckBox.IsChecked == true || NoCheckBox.IsChecked == true))
                {
                    YesCheckBox.Style = FindResource("NotEnteredCheckBoxStyle1") as Style;
                    NoCheckBox.Style = FindResource("NotEnteredCheckBoxStyle1") as Style;

                    allFieldsEntered = false;
                }
                else
                {
                    transport.LowFloor = (YesCheckBox.IsChecked == true) ? "Yes" : "No";

                }

                if (allFieldsEntered)
                {
                    //if elemenet is edite in subtable also change it in main table
                    if (currentData != AllTransports)
                    {
                        transport.Number = dataCopy[currentEditing - 1].Number - 1;
                        AllTransports[dataCopy[currentEditing - 1].Number - 1] = transport;
                    }

                    transport.Number = (currentEditing);
                    currentData[currentEditing - 1] = transport;

                    ShowPage(currentPage);

                    //animtion to close add/edit window
                    DoubleAnimation fadeOutAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
                    addEditTransport.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

                    addEditTransport.IsHitTestVisible = false;
                }

            }

        }



        //---------Control elemenets in window add/edit

        //Doors count add/substract
        private void DoorsminusOneButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.Parse(DoorscounttButTxt.Text) > 1)
            {
                DoorscounttButTxt.Text = (int.Parse(DoorscounttButTxt.Text) - 1).ToString();
            }
        }

        private void DoorsplusOneButton_Click(object sender, RoutedEventArgs e)
        {
            if(int.Parse(DoorscounttButTxt.Text) < 99)
            {
                DoorscounttButTxt.Text = (int.Parse(DoorscounttButTxt.Text) + 1).ToString();
            }
        }

        //Axels count add/substract
        private void AxlesminusOneButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.Parse(AxlescounttButTxt.Text) > 1)
            {
                AxlescounttButTxt.Text = (int.Parse(AxlescounttButTxt.Text) - 1).ToString();
            }
        }

        private void AxlesplusOneButton_Click(object sender, RoutedEventArgs e)
        {
            if(int.Parse(AxlescounttButTxt.Text) < 99)
            {
                AxlescounttButTxt.Text = (int.Parse(AxlescounttButTxt.Text) + 1).ToString();
            }
        }



        //---------Checkers in window add/edit

        //Only one checkbox can be checked at time
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

        //Check if only positive integer numbers are entered
        private void TextBox_PreviewTextInputNumbers(object sender, TextCompositionEventArgs e)
        {
            if (!IsNumericInput(e.Text))
            {
                e.Handled = true;
            }
        }

        private bool IsNumericInput(string text)
        {
            var regex = new Regex("^[0-9]*$");

            return regex.IsMatch(text);
        }




        //Check if only alphabetic letters are entered
        private void TextBox_PreviewTextInputLetters(object sender, TextCompositionEventArgs e)
        {
            if (!IsAlphabeticInput(e.Text))
            {
                e.Handled = true;
            }
        }
        private bool IsAlphabeticInput(string text)
        {
            Regex regex = new Regex("^[a-zA-Z-]+$");

            return regex.IsMatch(text);
        }



        //Button to search specified element
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchTerm = searchBox.Text.ToLower();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                table.ItemsSource = AllTransports;
            }
            else
            {
                if(currentData.Count > 0)
                {
                    //serach data that equals to searchTerm
                    var formatedData = new ObservableCollection<CPublicTransport>(
                        AllTransports.Where(item =>
                            item.Brand.ToLower().Contains(searchTerm) ||
                            GetEnumDescription(item.EngineType).ToLower().Contains(searchTerm) ||
                            item.EnginePower.ToString().Contains(searchTerm) ||
                            item.NumberOfAxles.ToString().Contains(searchTerm) ||
                            item.NumberOfDoors.ToString().Contains(searchTerm) ||
                            item.SeatingCapacity.ToString().Contains(searchTerm) ||
                            item.PassengerCapacity.ToString().Contains(searchTerm)
                        )
                    );


                    dataCopy = new ObservableCollection<CPublicTransport>(formatedData.Select(original => new CPublicTransport(original)));

                    //numerate filtered data
                    for (int i = 0; i < formatedData.Count; i++)
                    {
                        formatedData[i].Number = (i + 1);
                    }

                    //show filtered data
                    if(formatedData.Count > 0)
                    {
                        currentData = formatedData;
                        ShowPage(0);

                        //show button "back to main menu"
                        DoubleAnimation fadeInAnimation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.2));
                        backToMain.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);

                        backToMain.IsHitTestVisible = true;
                    }
                    else
                    {
                        MessageOutput messageOutput = new MessageOutput("No data was found");
                        //show dialog window and check for any positive result( button "ok" for example)
                        if (messageOutput.ShowDialog() == true)
                        {

                        }
                    }
                    
                }
                else
                {
                    MessageOutput messageOutput = new MessageOutput("No data to procces");
                    //show dialog window and check for any positive result( button "ok" for example)
                    if(messageOutput.ShowDialog() == true)
                    {

                    }
                }            
            }
                
        }

        //Button to return to original table
        private void BackToMainTableButton_Click(object sender, RoutedEventArgs e)
        {
            //change data to original
            currentData = AllTransports;

            //renumerate to avoide problems
            for (int i = 0; i < AllTransports.Count; i++)
            {
                AllTransports[i].Number = (i + 1);
            }

            ShowPage(0);

            //hide button "back to main menu"
            DoubleAnimation fadeInAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
            backToMain.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);

            backToMain.IsHitTestVisible = false;

            //empty search field
            searchBox.Text = null;
            
        }


        //------Task Functions------------------------

        //Function to read data from file
        private void OpenFromFile_Click(object sender, RoutedEventArgs e)
        {
            currentData.Clear();
            AllTransports.Clear();

            //choose file
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
                        //check if empty
                        if (reader.Peek() == -1)
                        {
                            throw new Exception("The file is empty.");
                        }

                        //check two first lines if they are header and empty
                        string line = reader.ReadLine();

                        if (!(line != null && System.Text.RegularExpressions.Regex.IsMatch(line, @"^\s*Brand\s+Engine\s+Power\s+Axles\s+Places\s+Seating\s+Doors\s+Low\s*$")))
                        {
                            throw new Exception("First line must be header");

                        }

                        line = reader.ReadLine();
                        if (line != null && !string.IsNullOrWhiteSpace(line))
                        {
                            throw new Exception("Second line must be empty");
                        }

                        //read all data from file
                        while (!reader.EndOfStream)
                        {
                            CPublicTransport transport = new CPublicTransport();
                            transport.ReadFromFile(reader);

                            //add to data
                            transport.Number = (AllTransports.Count + 1);
                            AllTransports.Add(transport);
                        }
                    }
                    //show read data
                    dataCopy = new ObservableCollection<CPublicTransport>(AllTransports);
                    currentData = AllTransports;
                    ShowPage(0);
                }
                catch (Exception ex)
                {
                    AllTransports.Clear();

                    MessageOutput messageOutput = new MessageOutput($"Error reading file.\n{ex.Message}");
                    //show dialog window and check for any positive result( button "ok" for example)
                    if (messageOutput.ShowDialog() == true)
                    {
                    }
                }
            }
        }
        
        //Function to save to file
        private void SaveToFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (currentData.Count > 0)
                {
                    //get file name where data will be stored
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
                                    //width between columns
                                    const int columnWidth = 16;

                                    //write header
                                    writer.WriteLine(string.Format("{0,-" + columnWidth + "} {1,-" + columnWidth + "} {2,-" + columnWidth + "} {3,-" + columnWidth + "} {4,-" + columnWidth + "} {5,-" + columnWidth + "} {6,-" + columnWidth + "} {7,-" + columnWidth + "}",
                                        "Brand", "Engine", "Power", "Axles", "Places", "Seating", "Doors", "Low"));
                                    writer.WriteLine(string.Format(""));

                                    //write all data
                                    foreach (var transport in currentData)
                                    {
                                        transport.WriteToFile(writer);
                                    }
                                }
                    
                   
                        }
                        catch (Exception ex)
                        {
                            MessageOutput messageOutput = new MessageOutput($"{ex.Message}");
                            //show dialog window and check for any positive result( button "ok" for example)
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
        
        //Function to print data
        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(currentData.Count > 0)
                {
                    //create document to print
                    FlowDocument flowDocument = CreateFlowDocument();


                    if (flowDocument != null)
                    {
                        PrintDialog printDialog = new PrintDialog();

                        if (printDialog.ShowDialog() == true)
                        {
                            //documnet parametrs
                            flowDocument.PageHeight = printDialog.PrintableAreaHeight;
                            flowDocument.PageWidth = printDialog.PrintableAreaWidth;
                            flowDocument.PagePadding = new Thickness(30, 30, 30, 20);

                            flowDocument.ColumnGap = 0;

                            flowDocument.ColumnWidth = (flowDocument.PageWidth -
                                                   flowDocument.ColumnGap -
                                                   flowDocument.PagePadding.Left -
                                                   flowDocument.PagePadding.Right);

                            //print document
                            var paginator = ((IDocumentPaginatorSource)flowDocument).DocumentPaginator;
                            printDialog.PrintDocument(paginator, "Public Transport Table");
                        }
                    }
                    else
                    {
                        throw new Exception("Error creating flow document");
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
                //show dialog window and check for any positive result( button "ok" for example)
                if (messageOutput.ShowDialog() == true)
                {

                }
            }
        }

        //Function to create printable document
        private FlowDocument CreateFlowDocument()
        {
            FlowDocument flowDocument = new FlowDocument();
            flowDocument.PageWidth = 793.76;
            flowDocument.PageHeight = 1122.52;

            int maxRowsPerPage = 24;
            try
            {
                Table tableToPrint = new Table();

                //add columns
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

                //add header
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

                //add empty row
                TableRow emptyRow = new TableRow();
                emptyRow.Cells.Add(new TableCell(new Paragraph(new Run(""))));
                tableToPrint.RowGroups[0].Rows.Add(emptyRow);

                int currentRowCount = 0;

                foreach (var data in currentData)
                {
                    //check if new page
                    if (currentRowCount >= maxRowsPerPage)
                    {
                        flowDocument.Blocks.Add(new Section());
                        flowDocument.Blocks.Add(tableToPrint);

                        //add new header
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

                        //add space after header
                        TableRow newEmptyRow = new TableRow();
                        newEmptyRow.Cells.Add(new TableCell(new Paragraph(new Run(""))));
                        tableToPrint.RowGroups[0].Rows.Add(newEmptyRow);

                        currentRowCount = 0;
                    }

                    //add new rows with element data
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

                    //add space row between data
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
                //show dialog window and check for any positive result( button "ok" for example)
                if (messageOutput.ShowDialog() == true)
                {

                }
                return null;
            }

            return flowDocument;
        }

        //Button event to call sort function
        private void Sort_Click(object sender, RoutedEventArgs e)
        {
            //set default styles
            {
                increasingCheckBox.Style = FindResource("CheckBoxStyle1") as Style;
                decreasingCheckBox.Style = FindResource("CheckBoxStyle1") as Style;
            }
            //make fields empty
            {
                increasingCheckBox.IsChecked = false;
                decreasingCheckBox.IsChecked = false;
            }

            //animation to show add/edit window
            DoubleAnimation fadeInAnimation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.2));
            Order.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);

            Order.IsHitTestVisible = true;            
        }
        //only one order checkbox can be checked at time
        private void orderCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;

            if (checkBox != null)
            {
                if (checkBox == increasingCheckBox && decreasingCheckBox.IsChecked == true)
                {
                    decreasingCheckBox.IsChecked = false;
                }
                else if (checkBox == decreasingCheckBox && increasingCheckBox.IsChecked == true)
                {
                    increasingCheckBox.IsChecked = false;
                }
            }
        }

        //exit from order window
        private void ExitOrderButton_Click(object sender, RoutedEventArgs e)
        {
            //animation to close order window
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
            Order.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

            Order.IsHitTestVisible = false;
        }

        //sort based on choosen order
        private void PerformSortButton_Click(object sender, RoutedEventArgs e)
        {
            //check if order is selected
            if (!(increasingCheckBox.IsChecked == true || decreasingCheckBox.IsChecked == true))
            {
                increasingCheckBox.Style = FindResource("NotEnteredCheckBoxStyle1") as Style;
                decreasingCheckBox.Style = FindResource("NotEnteredCheckBoxStyle1") as Style;
            }
            else
            {
                //set order based on checkbox
                int order = -1;

                if (increasingCheckBox.IsChecked == true)
                {
                    order = 0;
                }
                else if (decreasingCheckBox.IsChecked == true)
                {
                    order = 1;
                }

                //check if not empty
                if (currentData.Count > 0)
                {
                    currentData = ShellSort(currentData, order);

                    //renumerate
                    for (int i = 0; i < currentData.Count; i++)
                    {
                        currentData[i].Number = (i + 1);
                    }

                    //animation to close order window
                    DoubleAnimation fadeOutAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
                    Order.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

                    Order.IsHitTestVisible = false;

                    ShowPage(0);
                }
                else
                {
                    //close order window
                    DoubleAnimation fadeOutAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
                    Order.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

                    Order.IsHitTestVisible = false;

                    MessageOutput messageOutput = new MessageOutput("No data to sort");
                    //show dialog window and check for any positive result( button "ok" for example)
                    if (messageOutput.ShowDialog() == true)
                    {

                    }
                }
            }
        }

        //Button event to call calculate average function
        private void EngineAverage_Click(object sender, RoutedEventArgs e)
        {
            if(currentData.Count > 0)
            {
                double averageEnginePower = CalculateEnginePowerAverage(currentData);

                //show dialof window with engine average
                AverageOutput averageoOutput = new AverageOutput($"Average engine power is: {averageEnginePower}");
                if(averageoOutput.ShowDialog() == true)
                {

                }
            }
            else
            {
                MessageOutput messageOutput = new MessageOutput("No data to calculate average");
                //show dialog window and check for any positive result( button "ok" for example)
                if (messageOutput.ShowDialog() == true)
                {

                }
            }
        }
        
        //Button event to call function to search: most powerful electric engine with 20-26 seating places
        private void PowEngineSeating_Click(object sender, RoutedEventArgs e)
        {
            if(currentData.Count > 0)
            {
                currentData = MostPowerfulEngineAndSpecificSeating(currentData);

                //renumerate
                for (int i = 0; i < currentData.Count; i++)
                {
                    currentData[i].Number = i + 1;
                }

                ShowPage(0);

                //show "back to main table"
                DoubleAnimation fadeInAnimation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.2));
                backToMain.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);

                backToMain.IsHitTestVisible = true;
            }
            else
            {
                MessageOutput messageOutput = new MessageOutput("No data to process");
                //show dialog window and check for any positive result( button "ok" for example)
                if (messageOutput.ShowDialog() == true)
                {

                }
            }
            
        }

        //Button event to call function to search: engine power more than average with more than 4 axels

        private void PowEngineAxels_Click(object sender, RoutedEventArgs e)
        {
            if(currentData.Count > 0)
            {
                currentData = PowerfulAverageAndSpecificAxels(currentData);

                //renumerate
                for (int i = 0; i < currentData.Count; i++)
                {
                    currentData[i].Number = (i + 1);
                }

                ShowPage(0);

                //show button "back to main table"
                DoubleAnimation fadeInAnimation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.2));
                backToMain.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);

                backToMain.IsHitTestVisible = true;
            }
            else
            {
                MessageOutput messageOutput = new MessageOutput("No data to process");
                //show dialog window and check for any positive result( button "ok" for example)
                if (messageOutput.ShowDialog() == true)
                {

                }
            }
        }

        //Button event to call function to search: more than 4 doors, more than 40 pasanger capacity with low floor

        private void DoorFloorPasangers_Click(object sender, RoutedEventArgs e)
        {
            if(currentData.Count > 0)
            {
                currentData = DoorsFloorPlaces(currentData);

                //renumerate
                for (int i = 0; i < currentData.Count; i++)
                {
                    currentData[i].Number = (i + 1);
                }

                ShowPage(0);

                //show button "back to main table"
                DoubleAnimation fadeInAnimation = new DoubleAnimation(1, TimeSpan.FromSeconds(0.2));
                backToMain.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);

                backToMain.IsHitTestVisible = true;
            }
            else
            {
                MessageOutput messageOutput = new MessageOutput("No data to process");
                //show dialog window and check for any positive result( button "ok" for example)
                if (messageOutput.ShowDialog() == true)
                {

                }
            }
            
        }




        //Shell Sort
        private ObservableCollection<CPublicTransport> ShellSort(ObservableCollection<CPublicTransport> toSort, int order)
        {
           if(order == 0)
            {
                int n = toSort.Count;

                for (int gap = n / 2; gap > 0; gap /= 2)
                {
                    for (int i = gap; i < n; i++)
                    {
                        CPublicTransport temp = toSort[i];

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
            else
            {
                int n = toSort.Count;

                for (int gap = n / 2; gap > 0; gap /= 2)
                {
                    for (int i = gap; i < n; i++)
                    {
                        CPublicTransport temp = toSort[i];

                        int j;

                        for (j = i; j >= gap && Compare(toSort[j - gap], temp) < 0; j -= gap)
                        {
                            toSort[j] = toSort[j - gap];
                        }

                        toSort[j] = temp;
                    }
                }

                return toSort;
            }
        }

        private int Compare(CPublicTransport a, CPublicTransport b)
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

        //Engine Average
        private double CalculateEnginePowerAverage(ObservableCollection<CPublicTransport> toCalculate)
        {
            double sum = 0;

            foreach (var item in toCalculate)
            {
                sum += item.EnginePower;
            }

            double average = sum / toCalculate.Count;

            return Math.Round(average, 6);
        }


        //Search: most powerful electric engine with 20-26 seating places
        private ObservableCollection<CPublicTransport> MostPowerfulEngineAndSpecificSeating(ObservableCollection<CPublicTransport> toSearch)
        {
            try
            {
                //collection is created to catch all most powerful if there is two or more with same charachteristics
                ObservableCollection<CPublicTransport> mostPowerfulElectricCollection = new ObservableCollection<CPublicTransport>();
                
                //power cannot be negative
                double maxEnginePower = -1;

                for (int i = 0; i < toSearch.Count; i++)
                {
                    CPublicTransport transport = toSearch[i];

                    //check condition 20-26 seating and eletric engine
                    if (transport.SeatingCapacity >= 20 && transport.SeatingCapacity <= 26 && transport.EngineType == Engines.Electric)
                    {
                        //check if its power is bigger than in currently stored
                        if (transport.EnginePower >= maxEnginePower)
                        {
                            if(transport.EnginePower != maxEnginePower)
                            {
                                mostPowerfulElectricCollection.Clear();
                            }
                            maxEnginePower = transport.EnginePower;
                            mostPowerfulElectricCollection.Add(transport);
                        }

                    }
                }

                dataCopy = new ObservableCollection<CPublicTransport>(mostPowerfulElectricCollection.Select(transport => new CPublicTransport(transport)));

                return mostPowerfulElectricCollection;
            }
            catch (Exception ex)
            {
                MessageOutput messageOutput = new MessageOutput($"An error occurred: {ex.Message}");
                //show dialog window and check for any positive result( button "ok" for example)
                if (messageOutput.ShowDialog() == true)
                {

                }
                return null;
            }
        }

        //Search: engine power more than average with more than 4 axels
        private ObservableCollection<CPublicTransport> PowerfulAverageAndSpecificAxels(ObservableCollection<CPublicTransport> toSearch)
        {
            try
            {
                ObservableCollection<CPublicTransport> filteredData = new ObservableCollection<CPublicTransport>();

                for (int i = 0; i < toSearch.Count; i++)
                {
                    CPublicTransport transport = toSearch[i];

                    //check if condition is met: engine power more than average with more than 4 axels
                    if (transport.NumberOfAxles > 4 && transport.EnginePower > CalculateEnginePowerAverage(toSearch))
                    {
                        filteredData.Add(transport);
                    }
                }

                dataCopy = new ObservableCollection<CPublicTransport>(filteredData.Select(original => new CPublicTransport(original)));

                if (filteredData != null)
                {
                    return filteredData;
                }
                else
                {
                    return new ObservableCollection<CPublicTransport>();
                }
            }
            catch (Exception ex)
            {
                MessageOutput messageOutput = new MessageOutput($"An error occurred: {ex.Message}");
                //show dialog window and check for any positive result( button "ok" for example)
                if (messageOutput.ShowDialog() == true)
                {

                }
                return null;
            }
        }

        //Search: more than 4 doors, more than 40 pasanger capacity with low floor
        private ObservableCollection<CPublicTransport> DoorsFloorPlaces(ObservableCollection<CPublicTransport> toSearch)
        {
            try
            {
                ObservableCollection<CPublicTransport> filteredData = new ObservableCollection<CPublicTransport>();

                for (int i = 0; i < toSearch.Count; i++)
                {
                    CPublicTransport transport = toSearch[i];

                    //check if conditions is met: more than 4 doors, more than 40 pasanger capacity with low floor
                    if (transport.NumberOfDoors > 4 && transport.PassengerCapacity > 40 && transport.LowFloor == "Yes")
                    {
                        filteredData.Add(transport);
                    }
                }

                dataCopy = new ObservableCollection<CPublicTransport>(filteredData.Select(original => new CPublicTransport(original)));

                if (filteredData != null)
                {
                    return filteredData;
                }
                else
                {
                    return new ObservableCollection<CPublicTransport>();
                }
            }
            catch (Exception ex)
            {
                MessageOutput messageOutput = new MessageOutput($"An error occurred: {ex.Message}");
                //show dialog window and check for any positive result( button "ok" for example)
                if (messageOutput.ShowDialog() == true)
                {

                }
                return null;
            }
        }

    }
        
}

