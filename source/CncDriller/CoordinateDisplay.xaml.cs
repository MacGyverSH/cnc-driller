using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CncDriller
{
    /// <summary>
    /// Interaction logic for CoordinateDisplay.xaml
    /// </summary>
    public partial class CoordinateDisplay : UserControl, INotifyPropertyChanged
    {
        public CoordinateDisplay()
        {
            InitializeComponent();

            DataContext = this;
            Foreground = Brushes.White;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public string FormattedData
        {
            get
            {
                return String.Format(CultureInfo.InvariantCulture.NumberFormat, "{0:0000.000}", Data);
            }
        }

        public double Data
        {
            get { return (double)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); OnPropertyChanged("Data"); OnPropertyChanged("FormattedData"); }
        }

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(double), typeof(CoordinateDisplay), new PropertyMetadata((double)0, delegate(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CoordinateDisplay)d).OnPropertyChanged("Data");
            ((CoordinateDisplay)d).OnPropertyChanged("FormattedData");
        }));

    }
}
