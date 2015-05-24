using System;

namespace DY.SAMPLE.LEAK_TESTER
{
    public class ModelItem : Model
    {
        public static class RESULT
        {
            public const string OK = "OK";
            public const string NG = "NG";
            public const string PNG = "PNG";
        }

        public static class DIRECTION
        {
            public const string L = "L";
            public const string R = "R";
        }

        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public double Leak { get; set; }
        public string Result { get; set; }
        public string LR { get; set; }

        public int Index { get; set; }
        public string ModelNumber { get; set; }
        public string SerialNumber { get; set; }
        public string QRCode { get; set; }

        public ModelItem(double leak, string result, string direction)
        {
            Leak = leak;
            Result = result;
            LR = direction;

            Date = DateTime.Now;
            Time = Date.TimeOfDay;
        }

        public ModelItem()
        {
        }

        public void PullRestMember(int idx, string modelN, string serialN, string qrcode)
        {
            Index = idx;
            ModelNumber = modelN;
            SerialNumber = serialN;
            QRCode = qrcode;
        }

        public ModelItem(ModelItem item)
            : base(item)
        {
            Index = item.Index;
            ModelNumber = item.ModelNumber;
            Date = item.Date;
            Time = item.Time;
            Leak = item.Leak;
            Result = item.Result;
            SerialNumber = item.SerialNumber;
            QRCode = item.QRCode;
            LR = item.LR;
        }

        public static string AssembleSerialNumber(ModelItem item, SerialNumber serialnumber)
        {
            var nt = item.Time;
            var dbt = SetupDirector.GetInstance().Package.TimeInfo.Day.BeginTime;
            var det = SetupDirector.GetInstance().Package.TimeInfo.Day.EndTime;
            var nbt = SetupDirector.GetInstance().Package.TimeInfo.Night.BeginTime;
            var net = SetupDirector.GetInstance().Package.TimeInfo.Night.EndTime;
            string ABN = "N";
            if (dbt <= nt && nt <= det) //주간 
                ABN = "A";
            else if (nbt <= nt || nt <= net) //야간
                ABN = "B";
            int serialn = item.LR == ModelItem.DIRECTION.L ? serialnumber.SerialNumber_L++ : serialnumber.SerialNumber_R++;
            string serials = serialn > 999 ? string.Format("{0}", serialn) : string.Format("{0:D4}", serialn);
            string serial_number = item.Date.ToString("yyMMdd") + ABN + serials;
            return serial_number;
        }

        public static string AssembleQRCode(ModelItem item, string serial_number)
        {
            return item.ProductInfo + item.PartNumber + serial_number;
        }
    }
}