using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.Mitsubishi.MELSEC
{
    public class MC3EProtocol : IProtocol, IMCQHeader, IMCQnARequestDataSection, IMCCommandSection
    {
        /// <summary>
        /// 서브헤더
        /// </summary>
        public MC3EHeader SubHeader { get; set; }

        /// <summary>
        /// MELSECNET/H, MELSECNET/10 네트워크 시스템의 네트워크 번호
        /// </summary>
        public byte NetworkNumber { get; set; }

        /// <summary>
        /// MELSECNET/H, MELSECNET/10 네트워크 시스템의 국번호
        /// </summary>
        public byte PLCNumber { get; set; }

        /// <summary>
        /// 요구상대 모듈I/O번호
        /// </summary>
        public ushort ModuleIONumber { get; set; }

        /// <summary>
        /// 요구상대모듈 국번호
        /// </summary>
        public byte ModuleLocalNumber { get; set; }

        /// <summary>
        /// 요구데이터길이
        /// </summary>
        public ushort DataLength { get; set; }

        /// <summary>
        /// 시피유 감시 타이머
        /// </summary>
        public ushort CPUMonitorTimer { get; set; }

        /// <summary>
        /// 커맨드
        /// </summary>
        public MCQnACommand Command { get; set; }
        public MCQnADeviceExtension DeviceMemoryExtension { get; set; }
        public MCQnASpecialFunction SpecialFunction { get; set; }
        public MCQnADataType? DataType { get; set; }

        /// <summary>
        /// 종료코드 또는 에러코드
        /// </summary>
        public MCEFrameError Error { get; set; }
        public byte ErrorNetworkNumber { get; set; }
        public byte ErrorPLCNumber { get; set; }
        public ushort ErrorModuleIONumber { get; set; }
        public byte ErrorModuleLocalNumber { get; set; }
        public MCQnACommand ErrorCommand { get; set; }
        public MCQnADeviceExtension ErrorDeviceMemoryExtension { get; set; }
        public MCQnASpecialFunction ErrorSpecialFunction { get; set; }
        public MCQnADataType? ErrorDataType { get; set; }

        //DATA INFORMATION
        public IList<IProtocolData> Data { get; set; }
        public Type Type { get; set; }

        public MC3EProtocol()
        {
            Initialize();
        }

        public MC3EProtocol(MCQnADataType type, MCQnACommand command)
            : this()
        {
            DataType = type;
            Type = type == MCQnADataType.BIT ? typeof(bool) : typeof(ushort);
            Command = command;
        }

        public MC3EProtocol SetModuleAccessLevel(MCModuleAccessLevel lv, byte networkNumber = 0x00, byte plcNumber = 0xFF, byte moduleLocalNumber = 0x00, ushort moduleIONumber = 0x03FF)
        {
            if (lv != MCModuleAccessLevel.LEVEL_1)
                throw new NotSupportedException();
            switch (lv)
            {
                case MCModuleAccessLevel.LEVEL_1:
                    NetworkNumber = 0x00;
                    PLCNumber = 0xFF;
                    ModuleIONumber = 0x03FF;
                    ModuleLocalNumber = 0x00;
                    CPUMonitorTimer = 0x00;
                    break;
                case MCModuleAccessLevel.LEVEL_2:
                    ModuleIONumber = 0x03E0;
                    NetworkNumber = networkNumber;
                    PLCNumber = plcNumber;
                    ModuleLocalNumber = moduleLocalNumber;
                    break;
                case MCModuleAccessLevel.LEVEL_3:
                    ModuleIONumber = 0x03E1;
                    NetworkNumber = networkNumber;
                    PLCNumber = plcNumber;
                    ModuleLocalNumber = moduleLocalNumber;
                    break;
                case MCModuleAccessLevel.LEVEL_4:
                    ModuleIONumber = 0x03E2;
                    NetworkNumber = networkNumber;
                    PLCNumber = plcNumber;
                    ModuleLocalNumber = moduleLocalNumber;
                    break;
                case MCModuleAccessLevel.LEVEL_5:
                    ModuleIONumber = 0x03E3;
                    NetworkNumber = networkNumber;
                    PLCNumber = plcNumber;
                    ModuleLocalNumber = moduleLocalNumber;
                    break;
                case MCModuleAccessLevel.LEVEL_6:
                    ModuleIONumber = moduleIONumber;
                    NetworkNumber = networkNumber;
                    PLCNumber = plcNumber;
                    ModuleLocalNumber = moduleLocalNumber;
                    break;
            }
            return this;
        }

        public void Initialize()
        {
            SubHeader = MC3EHeader.NONE;
            NetworkNumber = byte.MaxValue;
            PLCNumber = byte.MaxValue;
            ModuleIONumber = ushort.MaxValue;
            ModuleLocalNumber = byte.MaxValue;
            DataLength = ushort.MaxValue;
            CPUMonitorTimer = 0;
            Command = MCQnACommand.NONE;
            DeviceMemoryExtension = MCQnADeviceExtension.OFF;
            SpecialFunction = MCQnASpecialFunction.OFF;
            DataType = null;

            Error = MCEFrameError.OK;
            ErrorNetworkNumber = 0;
            ErrorPLCNumber = 0;
            ErrorModuleIONumber = 0;
            ErrorModuleLocalNumber = 0;

            ErrorDeviceMemoryExtension = MCQnADeviceExtension.OFF;
            ErrorSpecialFunction = MCQnASpecialFunction.OFF;
            ErrorDataType = null;

            Data = null;
            Type = null;
        }

        public int GetErrorCode()
        {
            return (int)Error;
        }
    }
}