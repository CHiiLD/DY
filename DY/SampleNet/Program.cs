using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Config;
using log4net;
using DY.NET.LSIS.XGT;
using DY.NET;

namespace SampleNet
{
    class Program
    {
        protected static ILog Logger;

        static void Main(string[] args)
        {
            log4net.Config.BasicConfigurator.Configure();
            Logger =
                 LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            //RSS reqt
            Logger.Debug("RSS request");
            var protocol = XGTCnetExclusiveProtocol.CreateENQProtocol(32, XGTCnetCommand.r, XGTCnetCommandType.SS);
            protocol.ENQDatas.Add(new ENQDataFormat());
            protocol.BlockCnt = 1;
            protocol.ENQDatas[0].Var_Name = "%MW100";

            protocol.AssembleProtocol();
            Logger.Debug(Bytes2HexString.Change(protocol.ASCData));

            //RSS recv
            byte[] data = { 0x05, 0x30, 0x31, 0x52, 0x53, 0x53, 0x30, 0x32, 0x30, 0x32, 0x31, 0x32, 0x33, 0x33, 0x30, 0x32, 0x31, 0x32, 0x33, 0x33, 0x03 };
            protocol = XGTCnetExclusiveProtocol.CreateACKProtocol(data);

            Logger.Debug("--------------------------------------------");
        }
    }
}
