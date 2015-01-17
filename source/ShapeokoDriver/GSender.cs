using CodeUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;

namespace ShapeokoDriver
{
    public class GSender
    {

        private GQueue<GCommand> txQueue;
        private GQueue<GReply> rxQueue;
        private SerialPort sp;
        private GConfig config;
        private Thread t;
        private AutoResetEvent threadRunEvent;
        private AutoResetEvent simpleWriteDoneEvent;

        public delegate void OpennedEvent(GSender sender);
        public delegate void ClosedEvent(GSender sender);
        public delegate void DataRecievedEvent(GSender sender);

        public event OpennedEvent OnOpenned;
        public event ClosedEvent OnClosed;
        public event DataRecievedEvent OnRecieved;

        private bool _stop = false;
        private object _stop_lock = new object();
        private bool stop
        {
            get
            {
                lock (_stop_lock)
                {
                    return _stop;
                }
            }
            set
            {
                lock (_stop_lock)
                {
                    _stop = value;
                }
            }
        }

        private bool _connected = false;
        private object _connected_lock = new object();
        public bool Connected
        {
            get
            {
                lock (_connected_lock)
                {
                    return _connected;
                }
            }
            private set
            {
                lock (_connected_lock)
                {
                    _connected = value;
                }
            }
        }


        public GSender(GConfig config)
        {

            txQueue = new GQueue<GCommand>();
            rxQueue = new GQueue<GReply>();
            sp = new SerialPort();
            this.config = config;
            threadRunEvent = new AutoResetEvent(false);
            simpleWriteDoneEvent = new AutoResetEvent(false);
            t = new Thread(run);
            stop = true;

        }

        ~GSender()
        {

            Close();
        }

        public void Close()
        {
            stop = true;            
        }

        public void OpenPort()
        {
            t = new Thread(run);
            stop = true;

            txQueue.Clear();
            rxQueue.Clear();

            sp.PortName = config.ComPortName;
            sp.BaudRate = config.Baudrate;
            sp.ReadTimeout = 500;
            stop = false;
            t.Start();

        }

        public GReply SendDataBlocking(GCommand command)
        {

            while (txQueue.QueueLength > 0)
            {
                Thread.Sleep(100);
            }

            SendData(command);

            if (simpleWriteDoneEvent.WaitOne(config.ReadSimpleTimeout))
            {
                return rxQueue.Dequeue();
            }

            return null;
        }

        public void SendData(GCommand p)
        {
            if (Connected)
            {
                txQueue.Enqueue(p);
                threadRunEvent.Set();
            }
        }

        public GReply ReadData()
        {
            if (Connected)
            {
                return rxQueue.Dequeue();
            }
            return null;
        }

        private void checkIsAlive()
        {
            sp.ReadTimeout = 2000;
            for (int i = 0; i < 10; i++)
            {
                byte[] initb = Encoding.ASCII.GetBytes("\n");
                sp.Write(initb, 0, initb.Length);
                try
                {
                    if (sp.ReadLine().Trim() == "ok")
                    {
                        return;
                    }
                }
                catch (TimeoutException)
                {
                }
            }

            throw new CncNotConnectedException();
        }

        private void run()
        {
            try
            {
                sp.Open();

            }
            catch (IOException)
            {
                return;
            }

            if (config.ResetGRBLOnConnect)
            {
                sp.DtrEnable = true;
                Thread.Sleep(100);
                sp.DtrEnable = false;
                Thread.Sleep(1000); 
            }

            checkIsAlive();
            
            List<byte> rbuff = new List<byte>();

            DateTime lastInfoCommand = DateTime.Now;

            if (OnOpenned != null)
            {
                OnOpenned(this);
            }

            Connected = true;

            sp.ReadTimeout = 500;
            DateTime lastInfoTime = DateTime.Now;
            while(!stop){
                bool wait = true;
                bool sendStatusRequest = false;

                
                bool stopWhile = false;
                GCommand data = txQueue.Dequeue();
                if (data != null)
                {
                    portWrite(data.Name);
                    wait = false;
                }
                else
                {
                    stopWhile = true;

                    if ((DateTime.Now - lastInfoTime).TotalMilliseconds > 500)
                    {
                        lastInfoTime = DateTime.Now;
                        portWrite("?");
                    }
                }
                    
                do
                {
                    try
                    {
                        string reply = sp.ReadLine();

                        GReply r = GReply.parse(reply);

                        if (r is GReplyStatus)
                        {
                            if (sendStatusRequest)
                            {
                                stopWhile = true;
                            }
                        }
                        else
                        {
                            stopWhile = true;
                            Console.WriteLine("RECV>" + reply);
                        }

                        rxQueue.Enqueue(r);

                        simpleWriteDoneEvent.Set();

                        if (OnRecieved != null)
                        {
                            OnRecieved(this);
                        }

                    }
                    catch (TimeoutException)
                    {
                        if ((DateTime.Now - lastInfoTime).TotalMilliseconds > 500)
                        {
                            lastInfoTime = DateTime.Now;
                            portWrite("?");
                        }
                    }
                } while (!stopWhile);

                

                
                if (wait)
                {
                    threadRunEvent.WaitOne(100);
                }

            }
            Connected = false;
            sp.Close();

            if (OnClosed != null)
            {
                OnClosed(this);
            }

        }

        private void portWrite(string data)
        {
            try
            {
                data = data.Trim();
                if (data == "")
                {
                    return;
                }

                if (!data.StartsWith("?"))
                {
                    Console.WriteLine("SEND>" + data);
                    data = data + "\n";
                }

                byte[] buff = Encoding.ASCII.GetBytes(data);
                sp.Write(buff, 0, buff.Length);
                
            }
            catch (IOException)
            {
                stop = true;
            }
        }

        public void JoggingTo(GVector vect)
        {
            if (!Connected)
            {
                return;
            }

            GCommandChain ch = new GCommandChain("G21 G91 G0"); // run in mm, incremental move, rapid positioning

            appendVectorToChain(ch, vect);

            SendDataBlocking(ch);
        }

        public void MoveToMachineCoords(GVector vect)
        {
            MoveToMachineCoords(vect, -1);
        }

        public void MoveToMachineCoords(GVector vect, double feedRate)
        {
            if (!Connected)
            {
                return;
            }

            GCommandChain ch = new GCommandChain("G21 G53 G0"); // run in mm, machine coordinates, rapid positioning

            appendVectorToChain(ch, vect);

            if (feedRate != -1)
            {
                ch.Add(String.Format(CultureInfo.InvariantCulture.NumberFormat, "F{0:0.##}", feedRate));
            }

            SendDataBlocking(ch);
        }


        public static void appendVectorToChain(GCommandChain ch, GVector vect)
        {
            appendVectorToChain(ch, vect, false);
        }
        public static void appendVectorToChain(GCommandChain ch, GVector vect, bool all)
        {
            if (vect.X != 0 || all)
            {
                ch.Add(String.Format(CultureInfo.InvariantCulture.NumberFormat, "X{0:0.##}", vect.X));
            }

            if (vect.Y != 0 || all)
            {
                ch.Add(String.Format(CultureInfo.InvariantCulture.NumberFormat, "Y{0:0.##}", vect.Y));
            }

            if (vect.Z != 0 || all)
            {
                ch.Add(String.Format(CultureInfo.InvariantCulture.NumberFormat, "Z{0:0.##}", vect.Z));
            }
        }
        
        public void WaitToIdle()
        {
            simpleWriteDoneEvent.WaitOne();            
        }
    }
}
