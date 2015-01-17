using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeUtils
{
    public class LineParser
    {

        private string line;
        private int pos = 1;
        private int startPos = 1;

        public string Data { get; private set; }
        public char Command { get; private set; }

        public LineParser(string line)
        {
            this.line = line;
        }

        private int findLetter(string text, int offset)
        {
            int i = 0;
            foreach (char ch in text)
            {
                if (i >= offset)
                {
                    if (!((ch >= '0' && ch <= '9') || ch == '.' || ch == '-'))
                    {
                        return i;
                    }
                }
                i++;
            }

            return i;
        }

        public bool next()
        {

            pos = findLetter(line, startPos);

            if ((pos - startPos) <= 0)
            {
                return false;
            }

            Data = line.Substring(startPos, pos - startPos);

            Command = line[startPos - 1];

            startPos = pos + 1;
           
            return true;
        }


    }
}
