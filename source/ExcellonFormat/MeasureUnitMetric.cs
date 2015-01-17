using CodeUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExcellonFormat
{
    public class MeasureUnitMetric : MeasureUnit
    {
        public override GVector toMetric(GVector c)
        {
            return c;
        }

        public override GVector toImperial(GVector c)
        {
            return c / 25.4;
        }
    }
}
