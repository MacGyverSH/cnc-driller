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
    class EditorLineItem : BaseEditorItem
    {
        private LineGeometry line;

        private Point pointA;
        protected Point PointA
        {
            get
            {
                Point p = new Point(pointA.X * Scale, pointA.Y * Scale);
                return p;
            }
            set
            {
                pointA = value;
                OnPropertyChanged("PointA");
            }
        }

        private Point pointB;
        protected Point PointB
        {
            get
            {
                Point p = new Point(pointB.X * Scale, pointB.Y * Scale);
                return p;
            }
            set
            {
                pointB = value;
                OnPropertyChanged("PointB");
            }
        }

        public EditorLineItem()
        {
            line = new LineGeometry();
            PropertyChanged += propChanged;
        }

        public EditorLineItem(Point pointA, Point pointB)
        {
            line = new LineGeometry();
            PropertyChanged += propChanged;

            this.Stroke = Brushes.Gray;
            this.StrokeThickness = 1;

            PointA = pointA;
            PointB = pointB;

        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                return line; 
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

            if (e.PropertyName == "PointA" || e.PropertyName == "Scale")
            {
                line.StartPoint = PointA;
            }

            if (e.PropertyName == "PointB" || e.PropertyName == "Scale")
            {
                line.EndPoint = PointB;
            }

        }

    }
}
