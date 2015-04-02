﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.LSIS.XGT
{
    public class ACKDataFormat
    {
        public ushort SizeOfType; //2byte
        public object Data;       //?byte

        public ACKDataFormat(ushort sizeOfType, object data)
        {
            SizeOfType = sizeOfType;
            Data = data;
        }
    }
}
