using ShapeokoDriver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CncDriller
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window, INotifyPropertyChanged
    {
        public Settings()
        {
            InitializeComponent();

            DataContext = this;
        }

        private string portName = "";
        public string PortName
        {
            get
            {
                return portName;
            }
            set
            {
                portName = value;
                OnPropertyChanged("PortName");
            }
        }

        private int baudrate = 9600;
        public int Baudrate
        {
            get
            {
                return baudrate;
            }
            set
            {
                OnPropertyChanged("Baudrate");
                baudrate = value;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void ComboBox_DropDownOpened_1(object sender, EventArgs e)
        {
            ((ComboBox)sender).Items.Clear();
            foreach (string port in GConfig.AvailablePorts())
            {
                ((ComboBox)sender).Items.Add(port);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Window_Closing_1(object sender, CancelEventArgs e)
        {
            
        }
    }
}
