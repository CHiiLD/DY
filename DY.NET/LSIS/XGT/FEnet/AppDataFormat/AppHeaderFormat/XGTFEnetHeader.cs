using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// LSIS 이더넷 모듈 애플리케이션 프레임 구조의 헤더 포맷 클래스
    /// 해당 바이트 데이터를 분석하여 필요한 정보를 얻을 수 있다.
    /// </summary>
    public class XGTFEnetHeader
    {
        //ERROR CONST STRING
        public const string ERROR_APP_DATA_FMT_HEADER_SIZE = "DATA'S LENGTH MUST HAVE 20BYTE.";

        //CONST 맴버 변수
        public const int APPLICATION_HEARDER_FORMAT_SIZE = 20;  //헤더 포맷의 사이즈 (20byte)
        private const byte RESERVED_VALUE = 0x00;             //예약영역 초기값 or Don't care
        private const byte CPUINFO_VALUE = 0xA0;              //RESERVED 영역을 통해 XGK/XGI 시리즈임을 판단

        //HEADER INFOS
        public XGTFEnetCompanyID _CompanyID { private set; get; }           //PLC 제품
        public XGTFEnetPLCInfo _PLCInfo { private set; get; }               //PLC 정보
        public XGTFEnetSourceOfFrame SourceOfFrame { private set; get; }    //클라 -> 서버 or 서버 -> 클라
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
            this._CompanyID = that._CompanyID;
            this._PLCInfo = that._PLCInfo;
            this.SourceOfFrame = that.SourceOfFrame;
            this.InvokeID = that.InvokeID;
            this.AppInstructionDataLength = that.AppInstructionDataLength;
            this.SlotPosition = that.SlotPosition;
            this.BasePosition = that.BasePosition;
        }

        /// <summary>
        /// XGTFEnetHeader 팩토리 생성 메서드
        /// </summary>
        /// <param name="headerData">PLC로부터 받은 이더넷 프레임 데이터의 헤더 정보 반드시 데이터는 20byte여야 합니다.</param>
        /// <returns>XGTFEnetHeader 객체</returns>
        /// <exception cref="System.ArgumentException">해더 데이터의 길이가 20byte가 아닐 경우 예외가 발생합니다.</exception>
        /// <exception cref="System.ArgumentNullException">파라미터가 null인 경우 예외가 발생합니다.</exception>
        public XGTFEnetHeader CreateXGTFEnetHeader(byte[] headerData)
        {
            if (headerData == null)
                throw new ArgumentNullException();
            if (headerData.Length != APPLICATION_HEARDER_FORMAT_SIZE)
                throw new ArgumentException(ERROR_APP_DATA_FMT_HEADER_SIZE);
            XGTFEnetHeader header = new XGTFEnetHeader();
            header._HeaderData = new byte[APPLICATION_HEARDER_FORMAT_SIZE];
            Buffer.BlockCopy(headerData, 0, header._HeaderData, 0, APPLICATION_HEARDER_FORMAT_SIZE);
            header.ParseCompanyID();
            header.ParsePLCInfo();
            header.ParseSourceOfFrame();
            header.ParseInvolkeID();
            header.ParseAppDataLen();
            header.ParseDevicePosition();
            return header;
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
                _CompanyID = XGTFEnetCompanyID.LSIS_XGT;
#if DEBUG
            else
                System.Diagnostics.Debug.Assert(false);
#endif
        }

        /// <summary>
        /// PLC INFO 분석
        /// </summary>
        private void ParsePLCInfo()
        {
            byte[] target = new byte[2];
            Buffer.BlockCopy(_HeaderData, 8, target, 0, target.Length);
            if (target[0] == 0x00 && target[1] == 0x00)
            {
                Buffer.BlockCopy(_HeaderData, 10, target, 0, target.Length);
                _PLCInfo.Init(target);
            }
#if DEBUG
            else
                System.Diagnostics.Debug.Assert(false);
#endif
        }

        /// <summary>
        /// 프레임 서버 측 분석
        /// </summary>
        private void ParseSourceOfFrame()
        {
            byte target = _HeaderData[13];
            SourceOfFrame = (XGTFEnetSourceOfFrame)target;
        }

        /// <summary>
        /// 프레임 순서를 구분하기 위한 ID
        /// </summary>
        private void ParseInvolkeID()
        {
            byte[] target = new byte[2];
            Buffer.BlockCopy(_HeaderData, 14, target, 0, target.Length);
            InvokeID = (ushort)CA2C.ToValue(target, typeof(ushort));
        }

        /// <summary>
        /// 실질 데이터의 바이트 배열 길이
        /// </summary>
        private void ParseAppDataLen()
        {
            byte[] target = new byte[2];
            Buffer.BlockCopy(_HeaderData, 16, target, 0, target.Length);
            AppInstructionDataLength = (ushort)CA2C.ToValue(target, typeof(ushort));
        }

        /// <summary>
        /// 장착된 FEnet카드의 슬롯과 베이스 위치 분석
        /// </summary>
        private void ParseDevicePosition()
        {
            byte target = _HeaderData[18];
            SlotPosition = (byte)(target >> 4);
            BasePosition = (byte)(target << 4);
            BasePosition = (byte)(BasePosition >> 4);
        }
    }
}