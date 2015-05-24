using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace DY.SAMPLE.LEAK_TESTER
{
    public class SerialNumberDirector
    {
        private static SerialNumberDirector _SerialDirector;
        private const string SERIAL_FILE = "./SERIAL INFO.json";
        public List<SerialNumber> Item { get; private set; }

        public static SerialNumberDirector GetInstance()
        {
            if (_SerialDirector == null)
                _SerialDirector = new SerialNumberDirector();
            return _SerialDirector;
        }

        private SerialNumberDirector()
        {
            Item = new List<SerialNumber>();
            if (System.IO.File.Exists(SERIAL_FILE))
            {
                IList<SerialNumber> list;
                using (var jsonFile = System.IO.File.OpenText(SERIAL_FILE))
                using (JsonTextReader jsonTextReader = new JsonTextReader(jsonFile))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    list = serializer.Deserialize<IList<SerialNumber>>(jsonTextReader);
                }
                Item.AddRange(list);
            }
            else
            {
                Item.Add(new SerialNumber());
                Item.Add(new SerialNumber());
                Item.Add(new SerialNumber());
            }
        }

        public void SaveToFile()
        {
            string json = JsonConvert.SerializeObject(Item, Formatting.Indented);
            System.IO.File.WriteAllText(SERIAL_FILE, json, System.Text.Encoding.UTF8);
        }

        public void SerialNumbersInit()
        {
            foreach (var i in Item)
                i.SerialNumberInit();
        }
    }
}
