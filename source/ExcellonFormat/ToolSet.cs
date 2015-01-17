using CodeUtils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ExcellonFormat
{
    public class ToolSet
    {
        private Dictionary<int, Tool> tools = new Dictionary<int, Tool>();

        public List<Tool> List
        {
            get
            {
                return tools.Values.ToList();
            }
        }

        public Tool createFromCommand(string cmd, ExcellonFile file)
        {
            if (!cmd.StartsWith("T"))
            {
                return null;
            }

            int id = -1;
            double diameter = -1;

            LineParser p = new LineParser(cmd);
            while (p.next())
            {
                switch (p.Command)
                {
                    case 'T':
                        id = Int32.Parse(p.Data);
                        break;
                    case 'C':
                        diameter = Double.Parse(p.Data, CultureInfo.InvariantCulture.NumberFormat);
                        break;
                }
            }

            if (id != -1)
            {
                return createTool(id, diameter, file);
            }

            return null;
        }

        public Tool createTool(int id, double diameter, ExcellonFile file)
        {
            if (tools.ContainsKey(id))
            {
                return tools[id];
            }

            Tool t = new Tool(id, diameter, file);

            tools.Add(id, t);

            return t;
        }
    }
}
