using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Shapes;

namespace CncDriller
{
    abstract class BaseEditorItem : Shape, INotifyPropertyChanged
    {


        private double scale = 1;
        public double Scale
        {
            get
            {
                return scale;
            }
            set
            {
                scale = value;
                OnPropertyChanged("Scale");
            }
        }

        private Point offset = new Point(0, 0);
        public Point Offset
        {
            get
            {
                return offset;
            }
            set
            {
                offset = value;
                OnPropertyChanged("Offset");
            }
        }

        private double top = 0;
        public double Top
        {
            get
            {
                return (top * Scale) + Offset.Y;
            }
            protected set
            {
                top = value;
                OnPropertyChanged("Top");
            }
        }

        private double left = 0;
        public double Left
        {
            get
            {
                return (left * Scale) + Offset.X;
            }
            protected set
            {
                left = value;
                OnPropertyChanged("Left");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
