using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShapeokoDriver
{
    public class GReply
    {
        public string Text { get; set; }

        public GReply(string text)
        {
            Text = text;
            afterInit();
        }

        protected virtual void afterInit()
        {
        }

        public static GReply parse(string reply)
        {
            if (reply.Trim() == "OK")
            {
                return new GReplyOK();
            }
            else if (reply.Trim().StartsWith("<"))
            {
                return new GReplyStatus(reply);
            }
            return new GReply(reply);
        }

        public override string ToString()
        {
            return "Reply:" + Text;
        } 
    }
}
