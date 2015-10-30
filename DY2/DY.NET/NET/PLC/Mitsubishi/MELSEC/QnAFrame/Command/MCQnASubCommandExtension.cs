using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.Mitsubishi.MELSEC
{
    public static class MCQnASubCommandExtension
    {
        public static void AnalysisSubCommand(this IMCCommandSection commandSection, ushort integer)
        {
            commandSection.DeviceMemoryExtension = (MCQnADeviceExtension)(integer & (ushort)MCQnADeviceExtension.ON);
            commandSection.SpecialFunction = (MCQnASpecialFunction)(integer & (ushort)MCQnASpecialFunction.ON);
            commandSection.DataType = (MCQnADataType)(integer & (ushort)MCQnADataType.BIT);
        }

        public static ushort AssembleSubCommand(this IMCCommandSection commandSection)
        {
            int integer = (ushort)commandSection.DeviceMemoryExtension | (ushort)commandSection.SpecialFunction | (ushort)commandSection.DataType;
            return (ushort)integer;
        }
    }
}