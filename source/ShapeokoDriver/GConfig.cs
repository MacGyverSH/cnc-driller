using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ShapeokoDriver
{
    public class GConfig
    {
        public GConfig()
        {
            setDefaults();
        }

        public GConfig(string comPortName)
        {
            setDefaults();
            this.ComPortName = comPortName;
        }

        private void setDefaults()
        {
            ReadSimpleTimeout = 1000;
            Baudrate = 9600;
            ResetGRBLOnConnect = false;
            ComPortName = "";
        }

        public string ComPortName { get; set; }
        public int Baudrate { get; set; }
        public bool ResetGRBLOnConnect { get; set; }

        public int ReadSimpleTimeout { get; set; }

        public string InitCommand { get; set; }


        public static string[] AvailablePorts()
        {
            return SerialPort.GetPortNames();
        }

        public void Save(FileInfo fi)
        {
            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(GConfig));
            System.IO.StreamWriter file = new System.IO.StreamWriter(fi.FullName);
            writer.Serialize(file, this);
            file.Close();
        }

        public static GConfig Load(FileInfo fi)
        {
            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(GConfig));
            
            using (System.IO.StreamReader file = new System.IO.StreamReader(fi.FullName))
            {
                GConfig conf = (GConfig)reader.Deserialize(file);
                file.Close();

                return conf;
            }
            
        }
        
    }
}
