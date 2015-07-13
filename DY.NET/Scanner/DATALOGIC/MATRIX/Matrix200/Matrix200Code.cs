using System;
using System.Collections.Generic;

namespace DY.NET.DATALOGIC.MATRIX
{
    /// <summary>
    /// 좌표
    /// </summary>
    public struct Matrix200Point
    {
        public int X, Y;
    }

    /// <summary>
    /// 코드 영역 좌표
    /// </summary>
    public struct Matrix200Bounds
    {
        public Matrix200Point TL, TR, BL, BR;
    }

    /// <summary>
    /// 바코드 정보 클래스
    /// </summary>
    public class Matrix200Code
    {
        public string NewCode;
        public string Symbology;
        public int NumberofCharacters;
        public Matrix200Point CodeCenterPosition;
        public float PixelPerElement;
        public TimeSpan DecodingTime;
        public int CodeOrientation;
        public Matrix200Bounds CodeBounds;
        public byte[] Code;
        public int ExposureQuality;
        public TimeSpan ProcessingTime;
        public Dictionary<string, string> RawData;

        public Matrix200Code()
        {
            RawData = new Dictionary<string, string>();
        }
    }
}