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
    /// Interaction logic for ManualPositionCrossXY.xaml
    /// </summary>
    public partial class ManualPositionXY : UserControl
    {
        public ManualPositionXY()
        {
            InitializeComponent();


            setEvents(btn_yp100, round100);
            setEvents(btn_xp100, round100);
            setEvents(btn_yn100, round100);
            setEvents(btn_xn100, round100);

            setEvents(btn_yp10, round10);
            setEvents(btn_xp10, round10);
            setEvents(btn_yn10, round10);
            setEvents(btn_xn10, round10);

            setEvents(btn_yp1, round1);
            setEvents(btn_xp1, round1);
            setEvents(btn_yn1, round1);
            setEvents(btn_xn1, round1);

            setEvents(btn_yp01, round01);
            setEvents(btn_xp01, round01);
            setEvents(btn_yn01, round01);
            setEvents(btn_xn01, round01);

            setClickEvent(btn_hA);
            setClickEvent(btn_hX);
            setClickEvent(btn_hY);
            setClickEvent(btn_hZ);

        }

        private void setClickEvent(PathButton btn)
        {
            btn.Click += delegate(object sender, RoutedEventArgs e)
            {
                RoutedEventArgs newEventArgs = new RoutedEventArgs(ManualPositionXY.ClickEvent, sender);
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


        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ManualPositionXY));

        // Provide CLR accessors for the event 
        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

    }
}
