using System;

namespace DY.SAMPLE.LEAK_TESTER
{
    public class ModelItem : Model
    {
        public enum RESULT
        {
            OK,
            NG
        }

        public int ItemNumber { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public float Leak { get; set; }
        public RESULT Result { get; set; }
        public string SerialNumber { get; set; }
        public string QRCode { get; set; }
        public string LR { get; set; }

        public ModelItem()
        {
        }
    }
}