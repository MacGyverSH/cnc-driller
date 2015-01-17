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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CncDriller
{
    /// <summary>
    /// Interaction logic for PathButton.xaml
    /// </summary>
    public partial class PathButton : UserControl, INotifyPropertyChanged
    {
        public PathButton()
        {
            InitializeComponent();
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public Geometry Data
        {
            get { return (Geometry)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); OnPropertyChanged("Data"); }
        }

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(Geometry), typeof(PathButton), new PropertyMetadata(null, delegate(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PathButton)d).OnPropertyChanged("Data");
        }));

        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PathButton));

        // Provide CLR accessors for the event 
        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        void btn_click(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(PathButton.ClickEvent);
            RaiseEvent(newEventArgs);
        }
 

    }
}
