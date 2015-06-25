using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// RSS, WSS 프로토콜에 실거나, 받을 데이터 구조체
    /// </summary>
    public struct Mail
    {
        public string Name;
        public Type VType;
        public object Value;

        /// <summary>
        /// RSS 포맷으로 초기화
        /// </summary>
        /// <param name="name">PLC 변수 이름</param>
        /// <param name="type">변수의 타입 </param>
        /// <returns>초기화 성공 결과</returns>
        public bool SetRSSFmt(string name, Type type)
        {
            if (string.IsNullOrEmpty(name) || type == null)
                return false;

            Name = name;
            VType = type;
            return true;
        }

        /// <summary>
        /// WSS 포맷으로 초기화
        /// </summary>
        /// <param name="name">PLC 변수 이름</param>
        /// <param name="type">변수의 타입 </param>
        /// <param name="value">WRITE할 값</param>
        /// <returns>초기화 성공 결과</returns>
        public bool SetWSSFmt(string name, Type type, object value)
        {
            if (string.IsNullOrEmpty(name) || type == null || value.GetType() != type)
                return false;

            Name = name;
            VType = type;
            Value = value;
            return true;
        }
    }
}
