using System;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// PLC 데이터를 처리하기 위한 데이터 정보 구조체
    /// </summary>
    public struct PValue
    {
        public string Name;
        public Type Type;
        public object Value;

        public void InitForRead(string name, Type type)
        {
            this.Name = name;
            this.Type = type;
        }

        public void InitForWrite(string name, Type type, object value)
        {
            this.Name = name;
            this.Type = type;
            this.Value = value;
        }
    }
}