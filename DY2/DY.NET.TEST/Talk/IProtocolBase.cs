﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET
{
    public interface IProtocolBase
    {
        ProtocolType ProtocolType { get; set; }
        int GetErrorCode();
    }
}
