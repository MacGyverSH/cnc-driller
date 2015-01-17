using ExcellonFormat;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CncDriller
{
    class EditorHoleItem : BaseEditorItem
    {
        private EllipseGeometry ellipse;
        public Hole Hole { get; private set; }

        private Brush brushBase = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF99FF33"));
        private Brush brushSelected = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF9966CC"));
        private Brush brushHover = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFCCFF00"));
        private Brush brushHoverSelected = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC193CC"));


        private double diameter = 0;
        public double Diameter
        {
            get
            {
                return diameter * Scale;
            }
            private set
            {
                diameter = value;
                OnPropertyChanged("Diameter");
            }
        }

        private bool selected = false;
        public bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                selected = value;
                this.Fill = getFill();
            }
        }

        private bool mouseOver = false;
        public bool MouseOver
        {
            get
            {
                return mouseOver;
            }
            set
            {
                mouseOver = value;
                this.Fill = getFill();
            }

        }

        private Brush getFill()
        {
            if (Selected && MouseOver)
            {
                return brushHoverSelected;
            }
            else if (MouseOver)
            {
                return brushHover;
            }
            else if (Selected)
            {
                return brushSelected;
            }
            

            return brushBase;
        }

        public EditorHoleItem(Hole hole)
        {
            Hole = hole;

            ellipse = new EllipseGeometry();

            this.Fill = getFill();

            PropertyChanged += propChanged;

            this.Stroke = Brushes.Black;
            this.StrokeThickness = 1;

            Diameter = Hole.ActiveTool.Diameter.X;
            Left = hole.Coords.X;
            Top = hole.Coords.Y;

            this.MouseEnter += delegate(object sender, MouseEventArgs e)
            {
                MouseOver = true;
            };

            this.MouseLeave += delegate(object sender, MouseEventArgs e)
            {
                MouseOver = false;
            };

        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                ellipse.RadiusX = this.Width / 2;
                ellipse.RadiusY = this.Width / 2;
                
                return ellipse; 
            }
        }

        private void propChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Top" || e.PropertyName == "Scale" || e.PropertyName == "Offset")
            {
                Canvas.SetTop(this, Top);
            }

            if (e.PropertyName == "Left" || e.PropertyName == "Scale" || e.PropertyName == "Offset")
            {
                Canvas.SetLeft(this, Left);
            }

            if (e.PropertyName == "Scale" || e.PropertyName == "Diameter")
            {
                Width = Diameter;
                Height = Diameter;
            }
            
        }

    }
}
