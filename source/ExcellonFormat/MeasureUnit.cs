using CodeUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExcellonFormat
{
    public abstract class MeasureUnit
    {

        public abstract GVector toMetric(GVector c);
        public abstract GVector toImperial(GVector c);

    }
}
