using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.Mitsubishi.MELSEC
{
    public enum MCCommand : ushort
    {
        NONE = 0x0000,
        JR = 0x4A52, //Bit 일괄 읽기 XYM
        QR = 0x5152, //Word 일괄 읽기 DRTC
        JW = 0x4A57,//Bit 일괄 쓰기 XYM
        QW = 0x5157, //Word 일괄 쓰기 DRTC
    }

    public static class MCCommandExtension
    {
        /// <summary>
        /// XGTFEnetCommand 변수를 byte[]로 변환한다.
        /// </summary>
        public static byte[] ToBytes(this MCCommand cmd)
        {
            byte[] result = new byte[2] { 0x00, 0x00 };
            switch (cmd)
            {
                case MCCommand.JR:
                    result = new byte[2] { 0x4A, 0x52 };
                    break;
                case MCCommand.QR:
                    result = new byte[2] { 0x51, 0x52 };
                    break;
                case MCCommand.JW:
                    result = new byte[2] { 0x4A, 0x57 };
                    break;
                case MCCommand.QW:
                    result = new byte[2] { 0x51, 0x57 };
                    break;
            }
            return result;
        }
    }
}