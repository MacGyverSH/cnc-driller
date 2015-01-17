using System;
using System.Collections.Generic;
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
    /// Interaction logic for ManualPositionZ.xaml
    /// </summary>
    public partial class ManualPositionZ : UserControl
    {
        public ManualPositionZ()
        {
            InitializeComponent();

            setClickEvent(btn_zp10);
            setClickEvent(btn_zp1);
            setClickEvent(btn_zp01);

            setEvents(btn_zn10, round10);
            setEvents(btn_zn1, round1);
            setEvents(btn_zn01, round01);

        }

        private void setClickEvent(PathButton btn)
        {
            btn.Click += delegate(object sender, RoutedEventArgs e)
            {
                RoutedEventArgs newEventArgs = new RoutedEventArgs(ManualPositionZ.ClickEvent, sender);
                RaiseEvent(newEventArgs);
            };
        }

        private void setEvents(PathButton btn, FrameworkElement el)
        {
            btn.MouseEnter += delegate(object sender, MouseEventArgs e)
            {
                el.Visibility = System.Windows.Visibility.Visible;
            };
            btn.MouseLeave += delegate(object sender, MouseEventArgs e)
            {
                el.Visibility = System.Windows.Visibility.Hidden;
            };

            setClickEvent(btn);

        }


        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ManualPositionZ));

        // Provide CLR accessors for the event 
        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }
    }
}
