using System;
using System.Text;
using System.IO;

using Newtonsoft.Json;

namespace DY.WPF.SYSTEM.JSON
{
    public static class Json<T>
    {
        private static object LOCK = new object();

        public static void Write(string path, T instance)
        {
            if (instance == null)
                throw new ArgumentNullException();
            lock(LOCK)
            {
                string json = JsonConvert.SerializeObject(instance, Formatting.Indented);
                File.WriteAllText(path, json, System.Text.Encoding.UTF8);
            }
        }

        public static T Read(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException();
            if (!File.Exists(path))
                return default(T);
            lock (LOCK)
            {
                string json = File.ReadAllText(path, Encoding.UTF8);
                T obj = JsonConvert.DeserializeObject<T>(json);
                return obj;
            }
        }
    }
}