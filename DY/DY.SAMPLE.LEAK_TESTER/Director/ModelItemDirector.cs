using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

namespace DY.SAMPLE.LEAK_TESTER
{
    public class ModelItemDirector
    {
        private static ModelItemDirector _ModelItemDirector;
        private List<ModelItem> _ModelItemList;
        private const int STORAGE_LIMIT_COUNT = 1024;
        private const string FILE_PREFIX = "DY.SAMPLE.LEAK_TESTER ";
        private const string DIRECTORY_PATH = "./STORAGE";
        private DateTime _CurrentDateTime;

        private ModelItemDirector()
        {
            _ModelItemList = new List<ModelItem>();
            _CurrentDateTime = DateTime.Now;
        }

        public void AddItem(ModelItem model_item)
        {
            if (model_item == null)
                return;
            if (_CurrentDateTime.DayOfYear == model_item.Date.DayOfYear && _CurrentDateTime.Year == model_item.Date.Year)
                if (_CurrentDateTime.DayOfYear == model_item.Date.DayOfYear && _CurrentDateTime.Year == model_item.Date.Year)
                {
                    _ModelItemList.Add(model_item);
                    if (_ModelItemList.Count > STORAGE_LIMIT_COUNT)
                        SaveToFile(model_item.Date);
                }
                else //날짜가 바뀐 경우 
                {
                    SaveToFile(_CurrentDateTime);
                    _ModelItemList.Add(model_item);
                    _CurrentDateTime = model_item.Date;
                }
        }

        public void SaveToFile(DateTime time)
        {
            if (_ModelItemList.Count == 0)
                return;
            string filename = AssembleFileName(time);

            List<ModelItem> temp;
            if (LoadByFile(out temp, time))
                temp.AddRange(_ModelItemList);
            else
                temp = _ModelItemList;

            string json = JsonConvert.SerializeObject(temp, Formatting.Indented);
            System.IO.File.WriteAllText(filename, json, System.Text.Encoding.UTF8);
            _ModelItemList.Clear();
        }

        public void SaveToFile()
        {
            SaveToFile(_CurrentDateTime);
        }

        public static string AssembleFileName(DateTime time)
        {
            if (!System.IO.Directory.Exists(DIRECTORY_PATH))
                System.IO.Directory.CreateDirectory(DIRECTORY_PATH);
            string date = time.ToString("yyyy.MM.dd");
            string filename = DIRECTORY_PATH + "/" + FILE_PREFIX + date + ".json";
            return filename;
        }

        public static bool LoadByFile(out List<ModelItem> list, DateTime time)
        {
            list = null;
            string filename = AssembleFileName(time);
            if (!System.IO.File.Exists(filename))
                return false;

            IList<ModelItem> ilist;
            using (var jsonFile = System.IO.File.OpenText(filename))
            using (JsonTextReader jsonTextReader = new JsonTextReader(jsonFile))
            {
                JsonSerializer serializer = new JsonSerializer();
                ilist = serializer.Deserialize<IList<ModelItem>>(jsonTextReader);
            }
            list = ilist.ToList<ModelItem>();
            return true;
        }

        public static ModelItemDirector GetInstance()
        {
            if (_ModelItemDirector == null)
                _ModelItemDirector = new ModelItemDirector();
            return _ModelItemDirector;
        }
    }
}
