using CodeUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;

namespace CncDriller
{
    class EditorCursorItem : BaseEditorItem
    {
        private EllipseGeometry ellipse;

        private GVector position;
        public GVector Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                Left = position.X;
                Top = position.Y;
            }
        }

        public EditorCursorItem()
        {
            PropertyChanged += propChanged;

            ellipse = new EllipseGeometry();
            ellipse.RadiusX = 4;
            ellipse.RadiusY = 4;
            this.Stroke = Brushes.Black;
            this.StrokeThickness = 1;
            Fill = Brushes.Black;
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
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

        }
    }
}
