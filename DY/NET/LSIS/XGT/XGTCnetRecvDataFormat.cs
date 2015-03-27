/*
 * 작성자: CHILD	
 * 기능: LS산전의 XGT Cnet 전용 프로토콜 RWSS/RWSB 클래스의 구조체 변수 구현
 * 날짜: 2015-03-26
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.LSIS.XGT
{
    public struct XGTCnetRecvDataFormat
    {
        public byte Block_Num;       // R 블록 수 최대 16블록 1 ~ 16
        public byte Data_Size;       // R 데이터 개수의 자료형에 따른 byte의 크기를 나타냄 B(1), Byte(8), Word(16), DWord(32), LWord(64)
        public byte[] Data;          // R 데이터
    }
}
