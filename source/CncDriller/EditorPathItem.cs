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
    class EditorPathItem : EditorLineItem
    {
        
        public Hole HoleA { get; private set; }
        public Hole HoleB { get; private set; }
        
        public EditorPathItem(Hole holeA, Hole holeB) : base()
        {
            HoleA = holeA;
            HoleB = holeB;

            this.Stroke = Brushes.Gray;
            this.StrokeThickness = 1;

            if (HoleA == null)
            {
                PointA = new Point(0, 0);
            }
            else
            {
                PointA = new Point(HoleA.Coords.X, HoleA.Coords.Y);
            }

            PointB = new Point(HoleB.Coords.X, HoleB.Coords.Y);

        }

    }
}
