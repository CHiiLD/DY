using System;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// call by value 를 위해 구조체로 작성
    /// </summary>
    public struct ACKDataFormat
    {
        public ushort SizeOfType; //2byte
        public object Data;       //?byte

        public ACKDataFormat(ushort sizeOfType, object data)
        {
            if (!NumericTypeExtension.IsNumeric(data))
                throw new ArgumentException("data is not numeric type.");
            SizeOfType = sizeOfType;
            Data = data;
        }
    }
}