using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.LSIS.XGT
{
    public partial class XGTFEnetProtocol<T>
    {
        public static List<char> GetPossibleRSBList()
        {
            List<char> list = new List<char>();
            list.Add('P');
            list.Add('N');
            list.Add('L');
            list.Add('K');
            list.Add('T');
            list.Add('C');
            list.Add('D');
            list.Add('F');
            return list;
        }

        public static List<char> GetPossibleWSBList()
        {
            List<char> list = new List<char>();
            list.Add('P');
            list.Add('N');
            list.Add('L');
            list.Add('K');
            list.Add('T');
            list.Add('C');
            list.Add('D');
            return list;
        }

#if false
        public static List<char> GetPossibleDeviceList(XGTFEnetDataType type)
        {

        }
#endif
    }
}