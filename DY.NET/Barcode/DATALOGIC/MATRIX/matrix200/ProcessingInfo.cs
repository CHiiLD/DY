using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DY.NET.DATALOGIC.MATRIX
{
    public struct Point
    {
        public int X, Y;
    }

    public struct Bounds
    {
        public Point TL, TR, BL, BR;
    }

    public struct ProcessingInfo
    {
        public string NewCode;
        public string Symbology;
        public int NumberofCharacters;
        public Point CodeCenterPosition;
        public float PixelPerElement;
        public TimeSpan DecodingTime;
        public int CodeOrientation;
        public Bounds CodeBounds;
        public string Data;
        public int ExposureQuality;
        public TimeSpan ProcessingTime;
    }
}