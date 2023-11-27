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
using System.Windows.Shapes;

namespace kursova
{
    public partial class AverageOutput : Window
    {
        public AverageOutput()
        {
            InitializeComponent();
        }
        public AverageOutput(string parameterValue)
        {
            InitializeComponent();

            ErrorTxt.Text = parameterValue;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
