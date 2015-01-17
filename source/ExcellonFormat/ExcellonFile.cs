using CodeUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ExcellonFormat
{
    
    public class ExcellonFile
    {

        public ToolSet Tools { get; private set; }
        public MeasureUnit Units { get; private set; }
        public List<Hole> Holes { get; private set; }

        public ExcellonFile()
        {
            Tools = new ToolSet();
            Holes = new List<Hole>();
        }

        public ExcellonFile(MeasureUnit units)
        {
            Tools = new ToolSet();
            Holes = new List<Hole>();
            Units = units;
        }

        public void Translate(GVector translation)
        {
            foreach (Hole h in Holes)
            {
                h.Translate(translation);
            }
        }

        public void Rotate(double angle)
        {
            foreach (Hole h in Holes)
            {
                h.Rotate(angle);
            }
        }

        public void readFromStream(Stream stream)
        {
            Tool selectedTool = null;
            using(StreamReader sr = new StreamReader(stream)){

                bool inHeader = false;
                string line = "";
                while((line = sr.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (line.Length == 0)
                    {
                        continue;
                    }

                    if (line.StartsWith(";"))
                    {
                        continue;
                    }

                    if (line.StartsWith("M"))
                    {
                        switch (line)
                        {
                            case "M30":
                                return;
                            case "M48":
                                inHeader = true;
                                break;
                            case "M72":
                                Units = new MeasureUnitImperial();
                                break;
                            case "M71":
                                Units = new MeasureUnitMetric();
                                break;
                        }
                    }else if(line.StartsWith("%")){
                        inHeader = false;
                    }
                    else if (line.StartsWith("T"))
                    {
                        if (inHeader)
                        {
                            Tools.createFromCommand(line, this);
                        }
                        else
                        {
                            selectedTool = Tools.createFromCommand(line, this);
                        }
                    }
                    else if (line.StartsWith("X") || line.StartsWith("Y"))
                    {
                        Hole h = Hole.createFromData(line, selectedTool, this);
                        if (h != null)
                        {
                            Holes.Add(h);
                        }
                    }

                }
            }
        }
    }
}
