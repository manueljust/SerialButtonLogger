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

namespace SerialButtonLogger
{
    /// <summary>
    /// Interaction logic for AddTimeStampWindow.xaml
    /// </summary>
    public partial class AddTimeStampWindow : Window
    {
        public DateTime Date { get; set; } = DateTime.Now;
        public string Time { get; set; } = string.Format("{0:D2}:{1:D2}:{2:D2}", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

        public bool Add { get; private set; } = false;

        public AddTimeStampWindow()
        {
            InitializeComponent();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            int hours, minutes, seconds;

            bool ok = true;
            ok &= (8 == Time.Length);
            ok &= int.TryParse(Time.Substring(0, 2), out hours);
            ok &= int.TryParse(Time.Substring(3, 2), out minutes);
            ok &= int.TryParse(Time.Substring(6, 2), out seconds);

            if(true == ok)
            {
                Date = new DateTime(Date.Year, Date.Month, Date.Day, hours, minutes, seconds);
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Expected time in the format hh.mm.ss or hh:mm:ss", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
    }
}
