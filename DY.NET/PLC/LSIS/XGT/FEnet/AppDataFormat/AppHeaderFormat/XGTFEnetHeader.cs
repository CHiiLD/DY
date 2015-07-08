using System;
using System.Linq;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// LSIS 이더넷 모듈 애플리케이션 프레임 구조의 헤더 포맷 클래스
    /// 해당 바이트 데이터를 분석하여 필요한 정보를 쿼리
    /// </summary>
    public class XGTFEnetHeader
    {
        //ERROR CONST STRING
        public const string ERROR_APP_DATA_FMT_HEADER_SIZE = "Data's length must over 20byte.";

        //CONST 맴버 변수
        public const int APPLICATION_HEARDER_FORMAT_SIZE = 20;  //헤더 포맷의 사이즈 (20byte)
        public const int HEADER_FORMAT_COMPANY_ID_SIZE = 8;
        public const int HEADER_FORMAT_RESERVED_SIZE = 2; 
        public const int HEADER_FORMAT_PLC_INFO_SIZE = 2; 
        public const int HEADER_FORMAT_CPU_INFO_SIZE = 1; 
        public const int HEADER_FORMAT_SOURCE_OF_FRAME_SIZE = 1;
        public const int HEADER_FORMAT_INVOKE_ID_SIZE = 2;
        public const int HEADER_FORMAT_INSTRUCTION_LENGTH_SIZE = 2;
        public const int HEADER_FORMAT_FENET_POSITION_SIZE = 1;
        public const int HEADER_FORMAT_BCC_SIZE = 1;

        //HEADER INFOS
        public XGTFEnetCompanyID CompanyID { private set; get; }           //PLC 제품
        public XGTFEnetPLCInfo PLCInfo { private set; get; }               //PLC 정보
        public XGTFEnetSourceOfFrame SourceOfFrame { private set; get; }    //클라 -> 서버 or 서버 -> 클라
        public XGTFEnetCpuInfo CpuInfo { private set; get; }
        public ushort InvokeID { private set; get; }                        //프레임 간의  순서를 구별하기 위한 ID (응답 프레임에 이 번호를 붙여 보내줌)
        public ushort AppInstructionDataLength { private set; get; }        //실질 데이터의 바이트 길이
        public byte SlotPosition { private set; get; }                      // FEnet 모듈의 슬롯 넘버
        public byte BasePosition { private set; get; }                      // FEnet 모듈의 베이스 넘버

        private byte[] _HeaderData;                                         //헤더 바이트 데이터 (20byte)

        /// <summary>
        /// 생성자 방지
        /// </summary>
        private XGTFEnetHeader()
        {
        }

        /// <summary>
        /// 복사생성자
        /// </summary>
        /// <param name="that">복사할 XGTFEnetHeader 객체</param>
        public XGTFEnetHeader(XGTFEnetHeader that)
        {
            this.CompanyID = that.CompanyID;
            this.PLCInfo = new XGTFEnetPLCInfo(that.PLCInfo);
            this.CpuInfo = that.CpuInfo;
            this.SourceOfFrame = that.SourceOfFrame;
            this.InvokeID = that.InvokeID;
            this.AppInstructionDataLength = that.AppInstructionDataLength;
            this.SlotPosition = that.SlotPosition;
            this.BasePosition = that.BasePosition;
            if (that._HeaderData != null)
                this._HeaderData = (byte[])that._HeaderData.Clone();
        }

        /// <summary>
        /// XGTFEnetHeader 팩토리 생성 메서드
        /// </summary>
        /// <param name="headerData">PLC로부터 받은 이더넷 프레임 데이터의 헤더 정보 반드시 데이터는 20byte여야 합니다.</param>
        /// <returns>XGTFEnetHeader 객체</returns>
        /// <exception cref="System.ArgumentException">해더 데이터의 길이가 20byte가 아닐 경우 예외가 발생합니다.</exception>
        /// <exception cref="System.ArgumentNullException">파라미터가 null인 경우 예외가 발생합니다.</exception>
        public static XGTFEnetHeader CreateXGTFEnetHeader(byte[] headerData)
        {
            if (headerData == null)
                throw new ArgumentNullException();
            if (headerData.Length <= APPLICATION_HEARDER_FORMAT_SIZE)
                throw new ArgumentException(ERROR_APP_DATA_FMT_HEADER_SIZE);
            XGTFEnetHeader obj = new XGTFEnetHeader();
            obj._HeaderData = new byte[APPLICATION_HEARDER_FORMAT_SIZE];
            Buffer.BlockCopy(headerData, 0, obj._HeaderData, 0, APPLICATION_HEARDER_FORMAT_SIZE);
            obj.ParseCompanyID();
            obj.PLCInfo = new XGTFEnetPLCInfo(new byte[] { headerData[10], headerData[11] });
            obj.CpuInfo = (XGTFEnetCpuInfo)headerData[12];
            obj.SourceOfFrame = (XGTFEnetSourceOfFrame)headerData[13];
            obj.InvokeID = CV2BR.ToValue(new byte[] { headerData[14], headerData[15] });
            obj.AppInstructionDataLength = CV2BR.ToValue(new byte[] { headerData[16], headerData[17] });
            obj.ParseDevicePosition();
            return obj;
        }

        /// <summary>
        /// XGTFEnetHeader 객체 정적 생성 팩토리 메서드
        /// </summary>
        /// <param name="invokeID">유저 태그 값</param>
        /// <returns>XGTFEnetHeader 객체</returns>
        public static XGTFEnetHeader CreateXGTFEnetHeader(ushort invokeID)
        {
            XGTFEnetHeader instance = new XGTFEnetHeader();
            instance._HeaderData = new byte[APPLICATION_HEARDER_FORMAT_SIZE];
            Buffer.BlockCopy(XGTFEnetCompanyID.LSIS_XGT.ToBytes(), 0, instance._HeaderData, 0, HEADER_FORMAT_COMPANY_ID_SIZE); //회사 인증
            Buffer.BlockCopy(new byte[] {0,0}, 0, instance._HeaderData, 8, HEADER_FORMAT_RESERVED_SIZE); //예약
            Buffer.SetByte(instance._HeaderData, 13, XGTFEnetSourceOfFrame.PC2PLC.ToByte()); //CPU INFO
            Buffer.BlockCopy(CV2BR.ToBytes(invokeID), 0, instance._HeaderData, 14, HEADER_FORMAT_INVOKE_ID_SIZE); //Invoke ID
            return instance;
        }

        /// <summary>
        /// 설정된 맴버변수와 해더 뒤 구조화된 데이터의 크기로 바이트 배열 정보를 얻는다.
        /// </summary>
        /// <param name="instruction_byte_size">구조화된 데이터의 바이트 크기</param>
        /// <returns>헤더 바이트 배열</returns>
        internal byte[] GetBytes(int instruction_byte_size)
        {
            Buffer.BlockCopy(CV2BR.ToBytes((ushort)instruction_byte_size), 0, _HeaderData, 16, HEADER_FORMAT_INSTRUCTION_LENGTH_SIZE); // Instruction byte 크기 <<
            byte sum = 0;
            for (int i = 0; i < APPLICATION_HEARDER_FORMAT_SIZE - 1; i++)
                sum += _HeaderData[i];
            Buffer.SetByte(_HeaderData, 19, sum); //BCC
            return (byte[])_HeaderData.Clone();
        }

        /// <summary>
        /// CompanyID 분석
        /// </summary>
        private void ParseCompanyID()
        {
            byte[] XGT = new byte[8] { 
                (byte)'L', (byte)'S', (byte)'I', (byte)'S', 
                (byte)'-', 
                (byte)'X', (byte)'G', (byte)'T' };

            byte[] target = new byte[8];
            Buffer.BlockCopy(_HeaderData, 0, target, 0, target.Length);

            if (target.SequenceEqual(XGT))
                CompanyID = XGTFEnetCompanyID.LSIS_XGT;
#if DEBUG
            else
                CompanyID = XGTFEnetCompanyID.NONE;
#endif
        }

        /// <summary>
        /// 장착된 FEnet카드의 슬롯과 베이스 위치 분석
        /// </summary>
        private void ParseDevicePosition()
        {
            byte target = _HeaderData[18];
            BasePosition = (byte)(target >> 4);
            SlotPosition = (byte)((byte)0x0F & target);
        }
    }
}