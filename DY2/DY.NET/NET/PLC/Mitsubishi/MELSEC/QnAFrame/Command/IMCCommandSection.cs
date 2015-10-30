using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.Mitsubishi.MELSEC
{
    public interface IMCCommandSection
    {
        /*command section*/
        MCQnACommand Command { get; set; }

        /*subcommand section*/
        MCQnADeviceExtension DeviceMemoryExtension { get; set; }
        MCQnASpecialFunction SpecialFunction { get; set; }
        MCQnADataType? DataType { get; set; }
    }
}