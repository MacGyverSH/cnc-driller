using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CodeUtils
{
    public class GVector
    {

        public double[] Values { get; private set; }

        public double X
        {
            get
            {
                return getCoord(0);
            }
        }

        public double Y
        {
            get
            {
                return getCoord(1);
            }
        }

        public double Z
        {
            get
            {
                return getCoord(2);
            }
        }

        public GVector XY
        {
            get
            {
                GVector v = new GVector(X, Y);
                return v;
            }
        }

        public double getCoord(int i)
        {
            if (Values != null && Values.Length > i)
            {
                return Values[i];                
            }

            return 0;
        }

        public int Size
        {
            get
            {
                if (Values == null)
                {
                    return 0;
                }
                return Values.Length;
            }
        }

        public double Length
        {
            get
            {
                double l = 0;

                foreach (double v in Values)
                {
                    l += Math.Pow(v, 2);
                }
                return Math.Sqrt(l);
            }
        }

        public static GVector empty(int len)
        {
            GVector g = new GVector();
            g.Values = new double[len];

            for (int i = 0; i < len; i++)
            {
                g.Values[i] = 0;
            }

            return g;
        }

        public GVector(int len)
        {
            throw new NotImplementedException();
        }

        public GVector(params double[] values)
        {
            this.Values = values;
        }

        public GVector(GVector vector, params double[] values)
        {
            this.Values = new double[vector.Size + values.Length];
            int i = 0;
            foreach (double d in vector.Values)
            {
                this.Values[i++] = d;
            }

            foreach (double d in values)
            {
                this.Values[i++] = d;
            }

        }

        public GVector(string text)
        {
            string[] axis = text.Split(',');
            this.Values = new double[axis.Length];

            for(int i = 0; i < axis.Length; i++){
                this.Values[i] = Double.Parse(axis[i], CultureInfo.InvariantCulture.NumberFormat);
            }
        }

        public override bool Equals(object obj)
        {
            if (Size != ((GVector)obj).Size)
            {
                return false;
            }

            if (obj is GVector)
            {
                for (int i = 0; i < Size; i++)
                {
                    if (Values[i] != ((GVector)obj).Values[i])
                    {
                        return false;
                    }
                }
                return true;
            }

            return false;
        }

        public static GVector operator +(GVector v1, double d)
        {
            if (v1 == null)
            {
                return null;
            }

            GVector o = GVector.empty(v1.Size);

            for (int i = 0; i < v1.Size; i++)
            {
                o.Values[i] = v1.Values[i] + d;
            }

            return o;
        }

        public static GVector operator +(GVector v1, GVector v2)
        {
            if (v1 == null)
            {
                return null;
            }

            if (v2 == null)
            {
                return null;
            }

            GVector o = GVector.empty(Math.Max(v1.Size, v2.Size));

            for (int i = 0; i < o.Size; i++)
            {
                if (v1.Size > i && v2.Size > i)
                {
                    o.Values[i] = v1.Values[i] + v2.Values[i];
                }
                else if (v2.Size > i)
                {
                    o.Values[i] = v2.Values[i];
                }
            }

            return o;
        }

        public static GVector operator -(GVector v1, double d)
        {
            if (v1 == null)
            {
                return null;
            }

            GVector o = GVector.empty(v1.Size);

            for (int i = 0; i < v1.Size; i++)
            {
                o.Values[i] = v1.Values[i] - d;
            }

            return o;
        }

        public static GVector operator -(GVector v1, GVector v2)
        {
            if (v1 == null)
            {
                return null;
            }

            if (v2 == null)
            {
                return null;
            }

            GVector o = GVector.empty(Math.Max(v1.Size, v2.Size));

            for (int i = 0; i < o.Size; i++)
            {
                if (v1.Size > i && v2.Size > i)
                {
                    o.Values[i] = v1.Values[i] - v2.Values[i];
                }
                else if (v2.Size > i)
                {
                    o.Values[i] = -v2.Values[i];
                }
            }

            return o;
        }

        public static GVector operator *(GVector v1, double d)
        {
            if (v1 == null)
            {
                return null;
            }

            GVector o = GVector.empty(v1.Size);

            for (int i = 0; i < v1.Size; i++)
            {
                o.Values[i] = v1.Values[i] * d;
            }

            return o;
        }

        public static GVector operator /(GVector v1, double d)
        {
            if (v1 == null)
            {
                return null;
            }

            GVector o = GVector.empty(v1.Size);

            for (int i = 0; i < v1.Size; i++)
            {
                o.Values[i] = v1.Values[i] / d;
            }

            return o;
        }

        public static double Dot(GVector v1, GVector v2)
        {
            if (v1 == null)
            {
                return 0;
            }

            if (v2 == null)
            {
                return 0;
            }

            double dot = 0;

            for (int i = 0; i < Math.Max(v1.Size, v2.Size); i++)
            {
                if (v1.Size > i && v2.Size > i)
                {
                    dot += v1.Values[i] * v2.Values[i];
                }               
            }

            return dot;
        }

        public double Dot(GVector v)
        {
            return Dot(this, v);
        }

        public static double Angle(GVector v1, GVector v2)
        {
            
            double length = v2.Length * v1.Length;

            if (length == 0)
            {
                return 0;
            }

            double dot = Dot(v1, v2);

            return Math.Acos(dot/ length);
        }

        public double Angle(GVector v)
        {
            return Angle(this, v);
        }

        public override string ToString()
        {
            
            if(Values == null)
            {
                return "Vector(null)";
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("Vector{0}(", Size);

            int i = 0;
            foreach(double d in Values)
            {
                if (i != 0)
                {
                    sb.Append(", ");
                }
                sb.AppendFormat(CultureInfo.InvariantCulture.NumberFormat, "{0}", d);
                i++;
            }
            sb.Append(')');

            return sb.ToString();
        }

        public string Text
        {
            get
            {
                if (Values == null)
                {
                    return "(null)";
                }

                StringBuilder sb = new StringBuilder();

                int i = 0;
                foreach (double d in Values)
                {
                    if (i != 0)
                    {
                        sb.Append(" x ");
                    }
                    sb.AppendFormat(CultureInfo.InvariantCulture.NumberFormat, "{0:0.###}", d);
                    i++;
                }

                return sb.ToString();
            }
        }


        public double DistanceTo(GVector vect)
        {
            return (vect - this).Length;
        }

        public GVector Normalize
        {
            get
            {
                return this / Length;
            }
        }
    }
}
