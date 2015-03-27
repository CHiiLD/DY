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
    public struct XGTCnetReqtDataFormat
    {
        public struct ReqtFormat
        {
            public byte     Var_Len;        // W/R 변수 길이 최대 16byte (예 %MV000 이면 변수 길이는 6byte)
            public string   Var_Name;       // W/R 변수 이름 16byte이내 (대소문자, 숫자, % 이외엔 허용안함)
            //읽을 디바이스의 개수 디바이스 타입이 Word고 데이터 개수가 5개라면, 5개의 Word를 읽으라는 의미임 
            //최대 240byte 단, Word는 최대 60개만 지원(120byte) 
            public byte Data_Count;         // W/R RWSB에서만 쓰입니다. 읽을 디바이스의 개수
            public byte Block_Num;          // W/R RWSS에서만 쓰입니다. 개별적으로 읽을 블록의 숫자 1 ~ 16
            public byte[] Data;             // W 연속으로 쓸 데이터입니다.
        }
    }
}
