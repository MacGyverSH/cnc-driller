using CodeUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShapeokoDriver
{
    public enum GStatus
    {
        Idle,   // All systems are go and it's ready for anything.
        Queue,  // Motion(s) are queued in the planner buffer waiting for a cycle start command to be issued. Certain processes like check g-code mode can't run while something is queued. Reset to clear the queue.
        Run,    // Indicates a cycle is running.
        Hold,   // A feed hold is in process of executing, or slowing down to a stop. After the hold is complete, Grbl will enter a Queue state, waiting for a cycle start to resume the program.
        Home,   // In the middle of a homing cycle. NOTE: Positions are not updated live during the homing cycle, but they'll be set to [0,0,0] once done.
        Alarm,  // This indicates something has gone wrong or Grbl doesn't know its position. This state locks out all g-code commands, but allows you to interact with Grbl's settings if you need to. '$X' kill alarm lock releases this state and puts Grbl in the Idle state, which will let you move things again. As said before, be cautious of what you are doing after an alarm.
        Check   // Grbl is in check g-code mode. It will process and respond to all g-code commands, but not motion or turn on anything. Once toggled off with another '$C' command, Grbl will reset itself.
    }

    public class GReplyStatus : GReply
    {

        public GReplyStatus(string text)
            : base(text)
        {
            
        }

        public GStatus Status { get; private set; }
        public GVector MachinePosition { get; private set; }
        public GVector WorldPosition { get; private set; }

        protected override void afterInit()
        {
            // <Idle,MPos:-115.400,0.000,-15.200,WPos:-115.400,0.000,-15.200>

            int start = Text.IndexOf("<");
            if (start == -1)
            {
                throw new FormatException();
            }
            int end = Text.IndexOf(">", start);

            if ((end - start) <= 1)
            {
                throw new FormatException();
            }

            string data = Text.Substring(start + 1, end - start - 1);

            int stateDelimPos = data.IndexOf(",");

            string status = data.Substring(0, stateDelimPos);

            switch (status)
            {
                case "Idle":
                    Status = GStatus.Idle;
                    break;
                case "Queue":
                    Status = GStatus.Queue;
                    break;
                case "Run":
                    Status = GStatus.Run;
                    break;
                case "Hold":
                    Status = GStatus.Hold;
                    break;
                case "Home":
                    Status = GStatus.Home;
                    break;
                case "Alarm":
                    Status = GStatus.Alarm;
                    break;
                case "Check":
                    Status = GStatus.Check;
                    break;
            }

            int wposi = data.IndexOf("WPos:");
            int mposi = stateDelimPos + "MPos:".Length;
            string mpos = data.Substring(mposi + 1, (wposi - mposi) - 2);
            string wpos = data.Substring(wposi + "WPos:".Length);
            
            MachinePosition = new GVector(mpos);
            WorldPosition = new GVector(wpos);

        }

    }
}
