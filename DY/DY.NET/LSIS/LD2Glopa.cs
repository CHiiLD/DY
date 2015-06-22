
namespace DY.NET.LSIS
{
    /// <summary>
    /// 래더변수포맷를 글로파변수포맷으로 변경
    /// </summary>
    public class LD2Glopa
    {
        /// <summary>
        /// 래더변수포맷를 글로파변수포맷으로 변경
        /// </summary>
        /// <param name="ld">래더변수문자열</param>
        /// <param name="type">변수타입</param>
        /// <returns>글로파변수문자열</returns>
        public static string VarConvert(string ld, PLCVarType type)
        {
            string ret = ld;
            string type_s = null;
            switch (type)
            {
                case PLCVarType.BIT:
                    type_s = "X";
                    break;
                case PLCVarType.BYTE:
                    type_s = "B";
                    break;
                case PLCVarType.WORD:
                    type_s = "W";
                    break;
                case PLCVarType.DWORD:
                    type_s = "D";
                    break;
                case PLCVarType.LWORD:
                    type_s = "L";
                    break;
            }
            ret = ret.Insert(0, "%");
            ret = ret.Insert(2, type_s);
            return ret;
        }
    }
}
