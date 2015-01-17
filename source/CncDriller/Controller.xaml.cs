using CodeUtils;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CncDriller
{
    /// <summary>
    /// Interaction logic for Controller.xaml
    /// </summary>
    public partial class Controller : UserControl, INotifyPropertyChanged
    {
        public Controller()
        {
            InitializeComponent();
            DataContext = this;

            gsender = ServiceHolder.Instance.Sender;

            ServiceHolder.Instance.OnReplyRecieved += delegate(GReply reply)
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (reply is GReplyStatus)
                    {
                        GReplyStatus statusReply = (GReplyStatus)reply;

                        MachinePosition = statusReply.MachinePosition;
                        WorldPosition = statusReply.WorldPosition;
                        Status = statusReply.Status;
                    }
                }));
            };

        }

        private GStatus status;
        public GStatus Status
        {
            get
            {
                return status;
            }
            private set{
                status = value;
                OnPropertyChanged("Status");
            }
        }

        public bool IsInIdleMode
        {
            get
            {
                return Status == GStatus.Idle;
            }
        }

        private GSender gsender;
       
        private GVector machinePosition = GVector.empty(3);
        public GVector MachinePosition
        {
            get
            {
                return machinePosition;
            }
            set
            {
                machinePosition = value;
                OnPropertyChanged("MachinePosition");
                OnPropertyChanged("MachineX");
                OnPropertyChanged("MachineY");
                OnPropertyChanged("MachineZ");
            }
        }

        public double MachineX
        {
            get
            {
                return machinePosition.X;
            }
        }

        public double MachineY
        {
            get
            {
                return machinePosition.Y;
            }
        }

        public double MachineZ
        {
            get
            {
                return machinePosition.Z;
            }
        }

        private GVector worldPosition = GVector.empty(3);
        public GVector WorldPosition
        {
            get
            {
                return worldPosition;
            }
            set
            {
                worldPosition = value;
                OnPropertyChanged("WorldPosition");
                OnPropertyChanged("WorldX");
                OnPropertyChanged("WorldY");
                OnPropertyChanged("WorldZ");
            }
        }

        public double WorldX
        {
            get
            {
                return worldPosition.X;
            }
        }

        public double WorldY
        {
            get
            {
                return worldPosition.Y;
            }
        }

        public double WorldZ
        {
            get
            {
                return worldPosition.Z;
            }
        }


        private void ManualPosition_Click_1(object sender, RoutedEventArgs e)
        {

            if (e.OriginalSource is PathButton)
            {
                string name = ((PathButton)e.OriginalSource).Name;

                double val = 0;
                switch (name.Substring(6))
                {
                    case "01":
                        val = 0.1;
                        break;
                    case "1":
                        val = 1;
                        break;
                    case "10":
                        val = 10;
                        break;
                    case "100":
                        val = 100;
                        break;
                }
                GVector vect = null;

                if (name[5] == 'n')
                {
                    val *= -1;
                } 

                switch (name.Substring(4, 1))
                {
                    case "x":
                        vect = new GVector(val, 0, 0);
                        break;
                    case "y":
                        vect = new GVector(0, val, 0);
                        break;
                    case "z":
                        vect = new GVector(0, 0, val);
                        break;
                   
                }
                
                gsender.JoggingTo(vect);


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

        public void MoveTo(GVector HoleA_coords)
        {
            gsender.MoveToMachineCoords(HoleA_coords);
        }
    }
}
