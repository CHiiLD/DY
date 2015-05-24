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

        public enum DIRECTION
        {
            L, R
        }

        public int Index { get; set; }
        public int ItemNumber { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public float Leak { get; set; }
        public RESULT Result { get; set; }
        public string SerialNumber { get; set; }
        public string QRCode { get; set; }
        public DIRECTION LR { get; set; }

        public ModelItem()
        {
        }

        public ModelItem(ModelItem item) : base(item)
        {
            Index = item.Index;
            ItemNumber = item.ItemNumber;
            Date = item.Date;
            Time = item.Time;
            Leak = item.Leak;
            Result = item.Result;
            SerialNumber = item.SerialNumber;
            QRCode = item.QRCode;
            LR = item.LR;
        }
    }
}