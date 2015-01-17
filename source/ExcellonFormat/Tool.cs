using CodeUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExcellonFormat
{
    public class Tool
    {
        public int Id { get; private set; }
        public GVector Diameter { get; private set; }
        public ExcellonFile File { get; private set; }

        public Tool(int id, double diameter, ExcellonFile file)
        {
            this.Id = id;
            this.File = file;

            GVector v = new GVector(diameter);
            if(file.Units != null){
                this.Diameter = file.Units.toMetric(v);
            }else{
                this.Diameter = v;
            }
        }
    }
}
