using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace DY.NET.TEST
{
    public abstract class AFakeStream : Stream
    {
        protected byte[] Buffer;
        
        public int ReadDalayTime { get; set; }
        public int WriteDalayTime { get; set; }

        public override bool CanRead { get { return false; } }
        public override bool CanSeek { get { return false; } }
        public override bool CanWrite { get { return false; } }
        public override long Length { get { return -1; } }
        public override long Position { get; set; }
        public override void Write(byte[] buffer, int offset, int count) { }
        public override void SetLength(long value) { }
        public override long Seek(long offset, SeekOrigin origin) { return -1; }
        public override void Flush() { }
        public override int Read(byte[] buffer, int offset, int count) { return -1; }

        //public AFakeStream()
        //{
        //    DataType = typeof(ushort);
        //}

        //public AFakeStream(Type type)
        //{
        //    DataType = type;
        //}
    }
}
