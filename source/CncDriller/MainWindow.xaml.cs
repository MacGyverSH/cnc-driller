using ExcellonFormat;
using ShapeokoDriver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace CncDriller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        
        private ExcellonFile efi = null;

        public MainWindow()
        {
         
            InitializeComponent();

            DataContext = this;

            ControllerWindow ctrl = new ControllerWindow();

            ctrl.Closing += delegate(object sender, CancelEventArgs e)
            {
                ctrl.IsEnabled = false;
                if (this.IsEnabled)
                {
                    this.Close();
                }
            };

            this.Closing += delegate(object sender, CancelEventArgs e)
            {
                this.IsEnabled = false;
                if (ctrl.IsEnabled)
                {
                    ctrl.Close();
                }

                ServiceHolder.Instance.Sender.Close();
            };

            ctrl.Show();

            System.Windows.Forms.Timer tm = new System.Windows.Forms.Timer();
            tm.Interval = 500;
            tm.Tick += delegate(object sender, EventArgs e)
            {

                btn_connect.IsEnabled = ServiceHolder.Instance.SenderConfig.ComPortName != "";
                
                updateConnectButton();
            };
            tm.Enabled = true;

            excellonEditor.OnHoleClick += delegate(Hole h)
            {

                StringBuilder sb = new StringBuilder();

                sb.AppendFormat("Position: {0}\n", h.Coords.Text);
                sb.AppendFormat(CultureInfo.InvariantCulture.NumberFormat, "Diameter: {0:0.##}", h.ActiveTool.Diameter.X);

                tbInfo.Text = sb.ToString();

            };

        }

        

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
        


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog fd = new System.Windows.Forms.OpenFileDialog();

            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                
                using (FileStream fs = new FileStream(fd.FileName, FileMode.Open))
                {
                    efi = new ExcellonFile();

                    efi.readFromStream(fs);

                    excellonEditor.ExcelonFile = efi;

                }

            }

        }


        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Settings settings = new Settings();

            settings.PortName = ServiceHolder.Instance.SenderConfig.ComPortName;
            settings.Baudrate = ServiceHolder.Instance.SenderConfig.Baudrate;

            if (settings.ShowDialog() == true)
            {
                ServiceHolder.Instance.SenderConfig.ComPortName = settings.PortName;
                ServiceHolder.Instance.SenderConfig.Baudrate = settings.Baudrate;

                ServiceHolder.Instance.SaveConfig();
            }
        }

        public void updateConnectButton()
        {
            if (!ServiceHolder.Instance.Sender.Connected)
            {
                btn_connect.Content = "Connect";
            }
            else
            {
                btn_connect.Content = "Disconnect";
            }
            
        }

        private void btn_connect_click(object sender, RoutedEventArgs e)
        {
            if (!ServiceHolder.Instance.Sender.Connected)
            {
                ServiceHolder.Instance.Sender.OpenPort();
            }
            else
            {
                ServiceHolder.Instance.Sender.Close();
            }
            updateConnectButton();
        }

        private void btn_collimation_Click(object sender, RoutedEventArgs e)
        {
            if (!ServiceHolder.Instance.Sender.Connected)
            {
                MessageBox.Show("Collimation is available only if CNC is connected!", "Collimation", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return;
            }

            if (efi == null)
            {
                MessageBox.Show("Collimation is available only if openned any Excellon file!", "Collimation", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return;
            }

            Collimation col = new Collimation(efi);
            if (col.ShowDialog() == true)
            {
                excellonEditor.ExcelonFile = efi;
            }
        }

        private void btn_drilling_Click_1(object sender, RoutedEventArgs e)
        {
            if (efi == null)
            {
                MessageBox.Show("Drilling is available only if openned any Excellon file!", "Drilling", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return;
            }

            DrillingWindow window = new DrillingWindow(efi);

            window.Show();
        }

   

    }
}
