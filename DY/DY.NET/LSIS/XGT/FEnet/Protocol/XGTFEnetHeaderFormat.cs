using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.LSIS.XGT
{
    public class XGTFEnetExclusiveHeaderFormat
    {
        private byte[] _CompanyID;
        private byte[] _Reserved;
        private byte[] _PLCInfo;
        private byte[] _CPUInfo;
        private byte[] _SourceOfFrame;
        private byte[] _InvolkeID;
        private byte[] _Length;
        private byte[] _Position;
        private byte[] _Reserved2;

        protected void Init()
        {
            _CompanyID = new byte[8] { (byte)'L', (byte)'S', (byte)'I', (byte)'S', (byte)'-', (byte)'X', (byte)'G', (byte)'T' };

        }
    }
}
