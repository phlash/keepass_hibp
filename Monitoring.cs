using System;
using System.IO;

namespace Monitoring {
    public class InterceptionStream : Stream
    {
        public Stream InnerStream { get; private set; }
        public TextWriter CopyStream { get; private set; }
    
        public InterceptionStream(Stream innerStream, TextWriter copyStream)
        {
            if (innerStream == null) throw new ArgumentNullException("innerStream");
            if (copyStream == null) throw new ArgumentNullException("copyStream");
    
            InnerStream = innerStream;
            CopyStream = copyStream;
        }
    
        public override void Flush()
        {
            InnerStream.Flush();
        }
    
        public override long Seek(long offset, SeekOrigin origin)
        {
            return InnerStream.Seek(offset, origin);
        }
    
        public override void SetLength(long value)
        {
            InnerStream.SetLength(value);
        }
    
        public override int Read(byte[] buffer, int offset, int count)
        {
            var bytesRead = InnerStream.Read(buffer, offset, count);
    
            if (bytesRead != 0)
            {
                CopyStream.WriteLine("Read: " + bytesRead);
            }
            return bytesRead;
        }
    
        public override void Write(byte[] buffer, int offset, int count)
        {
            InnerStream.Write(buffer, offset, count);
            CopyStream.Write("Write: " + count);
        }
    
        public override bool CanRead
        {
            get { return InnerStream.CanRead; }
        }
    
        public override bool CanSeek
        {
            get { return InnerStream.CanSeek; }
        }
    
        public override bool CanWrite
        {
            get { return InnerStream.CanWrite; }
        }
    
        public override long Length
        {
            get { return InnerStream.Length; }
        }
    
        public override long Position
        {
            get { return InnerStream.Position; }
            set { InnerStream.Position = value; }
        }
    
        protected override void Dispose(bool disposing)
        {
            CopyStream.WriteLine("Dispose");
            InnerStream.Dispose();
        }
    }
}
