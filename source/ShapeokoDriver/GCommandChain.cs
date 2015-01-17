using CodeUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShapeokoDriver
{
    public class GCommandChain : GCommand
    {
        public List<GCommand> Commands { get; private set; }

        public GCommandChain()
            : base("")
        {
            Commands = new List<GCommand>();
        }

        public GCommandChain(string line)
            : base("")
        {
            Commands = new List<GCommand>();

            Comment = GComment.parse(line);

       
            LineParser parser = new LineParser(parseCommand(line).Replace(" ",""));
            while (parser.next())
            {
                Commands.Add(new GCommand(parser.Command + parser.Data));
            }

        }

        public override string Name
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (GCommand d in Commands)
                {
                    sb.AppendFormat("{0} ", d.Name);
                }
                return sb.ToString().Trim();
            }
        }

        public void Add(string cmd)
        {
            Add(new GCommandChain(cmd));
        }

        public void Add(GCommand cmd)
        {
            if (cmd != null)
            {
                Commands.Add(cmd);
            }
        }
    }
}
