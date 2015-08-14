using System;

namespace DY.NET.LSIS.XGT
{
    /// <summary>
    /// XGT Cnet - 프로토콜 명령어 타입 프레임
    /// 사용설명서_XGT_Cnet_국문_V2.8(7.2.2 명령어 일람)
    /// </summary>
    [Flags]
    public enum XGTCentActionMode
    {
        /// <summary>
        /// 개별읽기/연속읽기
        /// </summary>
        READ = 0x1,
        /// <summary>
        /// 개별쓰기/연속쓰기
        /// </summary>
        WRITE = 0x2,
        /// <summary>
        /// 모니터변수등록/모니터실행
        /// </summary>
        MONITER = 0x4,
    }
}