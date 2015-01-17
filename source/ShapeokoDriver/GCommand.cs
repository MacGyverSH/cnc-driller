using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShapeokoDriver
{
    public class GCommand
    {

        protected string parseCommand(string line)
        {
            int commendStart = line.IndexOf('(');
            if (commendStart == -1)
            {
                return line.Trim();
            }

            return line.Substring(0, commendStart - 1).Trim();
        }

        public GCommand(string command)
        {
            Name = parseCommand(command);
            Comment = GComment.parse(command);
        }

        public virtual string Name { get; protected set; }
        public GComment Comment { get; protected set; }

        public override string ToString()
        {
            if (Comment != null)
            {
                return Name + " " + Comment.ToString();
            }
            return Name;
        }

    }
}
