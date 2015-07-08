using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// RSB, WSB 프로토콜에서 사용 가능한 디바이스 목록 관리
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class XGTFEnetProtocol<T>
    {
        public static List<char> GetPossibleRSBList()
        {
            List<char> list = new List<char>();
            list.Add('P');
            list.Add('M');
            list.Add('L');
            list.Add('F');
            list.Add('C');
            list.Add('D');
            list.Add('T');
            list.Add('N');
            list.Add('R');
            return list;
        }

        public static List<char> GetPossibleWSBList()
        {
            List<char> list = new List<char>();
            list.Add('P');
            list.Add('M');
            list.Add('K');
            list.Add('C');
            list.Add('D');
            list.Add('T');
            list.Add('N');
            list.Add('R');
            return list;
        }
    }
}