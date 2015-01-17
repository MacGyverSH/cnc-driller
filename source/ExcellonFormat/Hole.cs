using CodeUtils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ExcellonFormat
{
    public class Hole
    {
    
        public GVector Coords { get; private set; }
        public Tool ActiveTool { get; private set; }
        public ExcellonFile File { get; private set; }

        public Hole(double x, double y, Tool activeTool, ExcellonFile file)
        {
            GVector v = new GVector(x, y);
            if(file != null && file.Units != null){
                this.Coords = file.Units.toMetric(v);
            }else{
                this.Coords = v;
            }
            this.ActiveTool = activeTool;
            this.File = file;

        }

        public Hole(GVector vect, Tool activeTool, ExcellonFile file)
        {
            this.Coords = vect;
            this.ActiveTool = activeTool;
            this.File = file;
        }

        public static Hole createFromData(string line, Tool activeTool, ExcellonFile file)
        {
            double x = double.NaN;
            double y = double.NaN;
            
            LineParser p = new LineParser(line);
            while (p.next())
            {
                switch (p.Command)
                {
                    case 'X':
                        x = Double.Parse(p.Data, CultureInfo.InvariantCulture.NumberFormat) / 10000;
                        break;
                    case 'Y':
                        y = Double.Parse(p.Data, CultureInfo.InvariantCulture.NumberFormat) / 10000;
                        break;
                }
            }

            if (!Double.IsNaN(x) && !Double.IsNaN(y))
            {
                return new Hole(x, y, activeTool, file);
            }

            return null;
        }

        public override string ToString()
        {
            return Coords.ToString();
        }

        public void Translate(GVector translation)
        {
            Coords += translation;
        }

        public void Rotate(double angle)
        {
            GVector vect = new GVector((Coords.X * Math.Cos(angle)) - (Coords.Y * Math.Sin(angle)), (Coords.X * Math.Sin(angle)) + (Coords.Y * Math.Cos(angle)));
            Coords = vect;
        }

    }
}
