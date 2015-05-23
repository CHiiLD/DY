using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO.Ports;

namespace DY.SAMPLE.LEAK_TESTER
{
    public class SetupDirector
    {
        public enum HOW_TO_CONNECT
        {
            SERIAL,
            ETHERNET
        }

        public class SetupPackage
        {
            public DayNightTimeInfo TimeInfo { get; set; }
            public SerialPortInfo SerialInfo { get; set; }
            public EthernetInfo EthernetInfo { get; set; }

            public SetupPackage(SetupPackage pack)
            {
                TimeInfo = new DayNightTimeInfo(pack.TimeInfo);
                SerialInfo = new SerialPortInfo(pack.SerialInfo);
                EthernetInfo = new EthernetInfo(pack.EthernetInfo);
            }

            public SetupPackage()
            {
                TimeInfo = new DayNightTimeInfo();
                SerialInfo = new SerialPortInfo();
                EthernetInfo = new EthernetInfo();

                TimeInfo.Day.BeginTime = new TimeSpan(08, 00, 00);
                TimeInfo.Day.EndTime = new TimeSpan(19, 59, 59);
                TimeInfo.Night.BeginTime = new TimeSpan(20, 00, 00);
                TimeInfo.Night.EndTime = new TimeSpan(7, 59, 59);
            }
        }
        private const string SETUP_FILE = "./SETUP INFO.JSON";
        private static SetupDirector _Setup;

        public HOW_TO_CONNECT Comm { get; set; }
        public SetupPackage Package { get; set; }

        private SetupDirector()
        {
            Package = new SetupPackage();
            Comm = HOW_TO_CONNECT.ETHERNET;

            if (System.IO.File.Exists(SETUP_FILE))
            {
                using (var jsonFile = System.IO.File.OpenText(SETUP_FILE))
                using (JsonTextReader jsonTextReader = new JsonTextReader(jsonFile))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    Package = (SetupPackage)serializer.Deserialize(jsonTextReader, Package.GetType());
                }
            }
        }

        public static SetupDirector GetInstance()
        {
            if (_Setup == null)
                _Setup = new SetupDirector();
            return _Setup;
        }

        public void SaveToFile()
        {
            string json = JsonConvert.SerializeObject(Package, Formatting.Indented);
            System.IO.File.WriteAllText(SETUP_FILE, json);
        }
    }
}
