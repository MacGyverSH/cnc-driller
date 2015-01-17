using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShapeokoDriver
{
    public class GComment
    {
        public string Text { get; private set; }

        public GComment(string text)
        {
            
            if (text.IndexOfAny(new char[] {'(', ')'}) != -1)
            {
                throw new FormatException();
            }

            Text = text;
        }

        public static GComment parse(string line)
        {
    
            int start = line.IndexOf('(', 0);
            if (start == -1)
            {
                return null;
            }

            int end = -1;
            int tmp = start;
            while (tmp != -1 && tmp < line.Length)
            {
                end = tmp;
                tmp++;
                tmp = line.IndexOf(')', tmp);
            }
            
            if (end == -1)
            {
                throw new FormatException();
            }

            if ((end - start) > 1)
            {
                return new GComment(line.Substring(start + 1, end - start - 1));
            }
            else
            {
                throw new FormatException();
            }
        }

        public override string ToString()
        {
            return "(" + Text + ")";
        }

    }
}
