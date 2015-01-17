using ShapeokoDriver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CncDriller
{
    
    public delegate void ReplyReceivedEvent(GReply reply);

    class ServiceHolder
    {
        private FileInfo confFile;
        
        private ServiceHolder()
        {
            confFile = new FileInfo("config.xml");
            if (confFile.Exists)
            {
                SenderConfig = GConfig.Load(confFile);
            }
            else
            {
                SenderConfig = new GConfig();
            }

            Sender = new GSender(SenderConfig);

            Sender.OnRecieved += delegate(GSender s)
            {
                GReply reply = ServiceHolder.Instance.Sender.ReadData();

                if (OnReplyRecieved != null)
                {
                    OnReplyRecieved(reply);
                }
            };

        }

        public event ReplyReceivedEvent OnReplyRecieved;

        public GConfig SenderConfig { get; private set; }
        public GSender Sender { get; private set; }


        private static ServiceHolder instance;
        public static ServiceHolder Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ServiceHolder();
                }
                return instance;
            }
        }


        public void SaveConfig()
        {
            SenderConfig.Save(confFile);
        }
    }
}
