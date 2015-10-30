using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.Mitsubishi.MELSEC
{
    public static class MCDeviceCodeExtension
    {
        public static byte[] GetDeviceASCIICode(this MCDevice device)
        {
            switch (device)
            {
                case MCDevice.SM: return new byte[] { (byte)'S', (byte)'M' };
                case MCDevice.SD: return new byte[] { (byte)'S', (byte)'D' };
                case MCDevice.X: return new byte[] { (byte)'X', (byte)'*' };
                case MCDevice.Y: return new byte[] { (byte)'Y', (byte)'*' };
                case MCDevice.M: return new byte[] { (byte)'M', (byte)'*' };
                case MCDevice.L: return new byte[] { (byte)'L', (byte)'*' };
                case MCDevice.F: return new byte[] { (byte)'F', (byte)'*' };
                case MCDevice.V: return new byte[] { (byte)'V', (byte)'*' };
                case MCDevice.B: return new byte[] { (byte)'B', (byte)'*' };
                case MCDevice.D: return new byte[] { (byte)'D', (byte)'*' };
                case MCDevice.W: return new byte[] { (byte)'W', (byte)'*' };
                case MCDevice.TS: return new byte[] { (byte)'T', (byte)'S' };
                case MCDevice.TC: return new byte[] { (byte)'T', (byte)'C' };
                case MCDevice.TN: return new byte[] { (byte)'T', (byte)'N' };
                case MCDevice.SS: return new byte[] { (byte)'S', (byte)'S' };
                case MCDevice.SC: return new byte[] { (byte)'S', (byte)'C' };
                case MCDevice.SN: return new byte[] { (byte)'S', (byte)'N' };
                case MCDevice.CS: return new byte[] { (byte)'C', (byte)'S' };
                case MCDevice.CC: return new byte[] { (byte)'C', (byte)'C' };
                case MCDevice.CN: return new byte[] { (byte)'C', (byte)'N' };
                case MCDevice.SB: return new byte[] { (byte)'S', (byte)'B' };
                case MCDevice.SW: return new byte[] { (byte)'S', (byte)'W' };
                case MCDevice.S: return new byte[] { (byte)'S', (byte)'*' };
                case MCDevice.DX: return new byte[] { (byte)'D', (byte)'X' };
                case MCDevice.DY: return new byte[] { (byte)'D', (byte)'Y' };
                case MCDevice.Z: return new byte[] { (byte)'Z', (byte)'*' };
                case MCDevice.R: return new byte[] { (byte)'R', (byte)'*' };
                case MCDevice.ZR: return new byte[] { (byte)'Z', (byte)'R' };
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public static byte GetDeviceBinaryCode(this MCDevice device)
        {
            switch (device)
            {
                case MCDevice.SM: return 0x91;
                case MCDevice.SD: return 0xA9;
                case MCDevice.X: return 0x9C;
                case MCDevice.Y: return 0x9D;
                case MCDevice.M: return 0x90;
                case MCDevice.L: return 0x92;
                case MCDevice.F: return 0x93;
                case MCDevice.V: return 0x94;
                case MCDevice.B: return 0xA0;
                case MCDevice.D: return 0xA8;
                case MCDevice.W: return 0xB4;
                case MCDevice.TS: return 0xC1;
                case MCDevice.TC: return 0xC0;
                case MCDevice.TN: return 0xC2;
                case MCDevice.SS: return 0xC7;
                case MCDevice.SC: return 0xC6;
                case MCDevice.SN: return 0xC8;
                case MCDevice.CS: return 0xC4;
                case MCDevice.CC: return 0xC3;
                case MCDevice.CN: return 0xC5;
                case MCDevice.SB: return 0xA1;
                case MCDevice.SW: return 0xB5;
                case MCDevice.S: return 0x98;
                case MCDevice.DX: return 0xA2;
                case MCDevice.DY: return 0xA3;
                case MCDevice.Z: return 0xCC;
                case MCDevice.R: return 0xAF;
                case MCDevice.ZR: return 0xB0;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static MCDevice GetMCDeviceByASCII(byte[] code)
        {
            if (code[0] == 'S' && code[1] == 'M') { return MCDevice.SM; }
            else if (code[0] == 'S' && code[1] == 'D') { return MCDevice.SD; }
            else if (code[0] == 'X' && code[1] == '*') { return MCDevice.X; }
            else if (code[0] == 'Y' && code[1] == '*') { return MCDevice.Y; }
            else if (code[0] == 'M' && code[1] == '*') { return MCDevice.M; }
            else if (code[0] == 'L' && code[1] == '*') { return MCDevice.L; }
            else if (code[0] == 'F' && code[1] == '*') { return MCDevice.F; }
            else if (code[0] == 'V' && code[1] == '*') { return MCDevice.V; }
            else if (code[0] == 'B' && code[1] == '*') { return MCDevice.B; }
            else if (code[0] == 'D' && code[1] == '*') { return MCDevice.D; }
            else if (code[0] == 'W' && code[1] == '*') { return MCDevice.W; }
            else if (code[0] == 'T' && code[1] == 'S') { return MCDevice.TS; }
            else if (code[0] == 'T' && code[1] == 'C') { return MCDevice.TC; }
            else if (code[0] == 'T' && code[1] == 'N') { return MCDevice.TN; }
            else if (code[0] == 'S' && code[1] == 'S') { return MCDevice.SS; }
            else if (code[0] == 'S' && code[1] == 'C') { return MCDevice.SC; }
            else if (code[0] == 'S' && code[1] == 'N') { return MCDevice.SN; }
            else if (code[0] == 'C' && code[1] == 'S') { return MCDevice.CC; }
            else if (code[0] == 'C' && code[1] == 'C') { return MCDevice.CN; }
            else if (code[0] == 'C' && code[1] == 'N') { return MCDevice.SB; }
            else if (code[0] == 'S' && code[1] == 'B') { return MCDevice.SB; }
            else if (code[0] == 'S' && code[1] == 'W') { return MCDevice.SW; }
            else if (code[0] == 'S' && code[1] == '*') { return MCDevice.S; }
            else if (code[0] == 'D' && code[1] == 'X') { return MCDevice.DX; }
            else if (code[0] == 'D' && code[1] == 'Y') { return MCDevice.DY; }
            else if (code[0] == 'Z' && code[1] == '*') { return MCDevice.Z; }
            else if (code[0] == 'R' && code[1] == '*') { return MCDevice.R; }
            else if (code[0] == 'Z' && code[1] == 'R') { return MCDevice.ZR; }
            else throw new ArgumentOutOfRangeException();
        }

        public static MCDevice GetMCDeviceByBinary(byte code)
        {
            if (code == 0x91) { return MCDevice.SM; }
            else if (code == 0xA9) { return MCDevice.SD; }
            else if (code == 0x9C) { return MCDevice.X; }
            else if (code == 0x9D) { return MCDevice.Y; }
            else if (code == 0x90) { return MCDevice.M; }
            else if (code == 0x92) { return MCDevice.L; }
            else if (code == 0x93) { return MCDevice.F; }
            else if (code == 0x94) { return MCDevice.V; }
            else if (code == 0xA0) { return MCDevice.B; }
            else if (code == 0xA8) { return MCDevice.D; }
            else if (code == 0xB4) { return MCDevice.W; }
            else if (code == 0xC1) { return MCDevice.TS; }
            else if (code == 0xC0) { return MCDevice.TC; }
            else if (code == 0xC2) { return MCDevice.TN; }
            else if (code == 0xC7) { return MCDevice.SS; }
            else if (code == 0xC6) { return MCDevice.SC; }
            else if (code == 0xC8) { return MCDevice.SN; }
            else if (code == 0xC4) { return MCDevice.CC; }
            else if (code == 0xC3) { return MCDevice.CN; }
            else if (code == 0xC5) { return MCDevice.SB; }
            else if (code == 0xA1) { return MCDevice.SB; }
            else if (code == 0xB5) { return MCDevice.SW; }
            else if (code == 0x98) { return MCDevice.S; }
            else if (code == 0xA2) { return MCDevice.DX; }
            else if (code == 0xA3) { return MCDevice.DY; }
            else if (code == 0xCC) { return MCDevice.Z; }
            else if (code == 0xAF) { return MCDevice.R; }
            else if (code == 0xB0) { return MCDevice.ZR; }
            else throw new ArgumentOutOfRangeException();
        }
    }
}
