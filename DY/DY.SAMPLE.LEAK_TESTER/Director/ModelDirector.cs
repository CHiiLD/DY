using System.Collections.Generic;
using Newtonsoft.Json;

namespace DY.SAMPLE.LEAK_TESTER
{
    public class ModelDirector
    {
        private static ModelDirector _Model;
        private const string MODEL_FILE = "./MODEL INFO.json";

        public List<Model> Item { get; private set; }

        private ModelDirector()
        {
            Item = new List<Model>();
            if (System.IO.File.Exists(MODEL_FILE))
            {
                IList<Model> list;
                using (var jsonFile = System.IO.File.OpenText(MODEL_FILE))
                using (JsonTextReader jsonTextReader = new JsonTextReader(jsonFile))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    list = serializer.Deserialize<IList<Model>>(jsonTextReader);
                }
                Item.AddRange(list);
            }
            else
            {
                Item.Add(new Model());
                Item.Add(new Model());
                Item.Add(new Model());
            }
        }

        public static ModelDirector GetInstance()
        {
            if(_Model == null)
                _Model = new ModelDirector();
            return _Model;
        }

        public void SaveToFile()
        {
            string json = JsonConvert.SerializeObject(Item, Formatting.Indented);
            System.IO.File.WriteAllText(MODEL_FILE, json, System.Text.Encoding.UTF8);
        }
    }
}
