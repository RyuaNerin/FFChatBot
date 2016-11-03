using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFChatBot
{
    internal class LogWriter : TextWriter
    {
        private readonly StreamWriter m_writer;

        public LogWriter(Stream stream)
        {
            this.m_writer = new StreamWriter(stream);
        }
        public LogWriter(string path)
        {
            this.m_writer = new StreamWriter(path);
        }
        public LogWriter(Stream stream, Encoding encoding)
        {
            this.m_writer = new StreamWriter(stream, encoding);
        }
        public LogWriter(string path, bool append)
        {
            this.m_writer = new StreamWriter(path, append);
        }
        public LogWriter(Stream stream, Encoding encoding, int bufferSize)
        {
            this.m_writer = new StreamWriter(stream, encoding, bufferSize);
        }
        public LogWriter(string path, bool append, Encoding encoding)
        {
            this.m_writer = new StreamWriter(path, append, encoding);
        }
        public LogWriter(Stream stream, Encoding encoding, int bufferSize, bool leaveOpen)
        {
            this.m_writer = new StreamWriter(stream, encoding, bufferSize, leaveOpen);
        }
        public LogWriter(string path, bool append, Encoding encoding, int bufferSize)
        {
            this.m_writer = new StreamWriter(path, append, encoding, bufferSize);
        }

        public override Encoding Encoding
        {
            get { return this.m_writer.Encoding; }
        }
        public override IFormatProvider FormatProvider
        {
            get { return this.m_writer.FormatProvider; }
        }
        public override string NewLine
        {
            get { return this.m_writer.NewLine; }
            set { this.m_writer.NewLine = value; }
        }

        public virtual bool AutoFlush
        {
            get { return this.m_writer.AutoFlush; }
            set { this.m_writer.AutoFlush = value; }
        }
        public virtual Stream BaseStream
        {
            get { return this.m_writer.BaseStream; }
        }

        public override void Close()
        {
            this.m_writer.Close();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                this.m_writer.Dispose();
        }
        public override void Flush()
        {
            this.m_writer.Flush();
        }
        public override Task FlushAsync()
        {
            return this.m_writer.FlushAsync();
        }

        public override void Write(bool value)
        {
            this.m_writer.Write(DateTime.Now.ToString("[yyyy-MM-dd hh:mm:ss] "));
            this.m_writer.WriteLine(value);
        }
        public override void Write(char value)
        {
            this.m_writer.Write(DateTime.Now.ToString("[yyyy-MM-dd hh:mm:ss] "));
            this.m_writer.WriteLine(value);
        }
        public override void Write(char[] buffer)
        {
            this.m_writer.Write(DateTime.Now.ToString("[yyyy-MM-dd hh:mm:ss] "));
            this.m_writer.WriteLine(buffer);
        }
        public override void Write(decimal value)
        {
            this.m_writer.Write(DateTime.Now.ToString("[yyyy-MM-dd hh:mm:ss] "));
            this.m_writer.WriteLine(value);
        }
        public override void Write(double value)
        {
            this.m_writer.Write(DateTime.Now.ToString("[yyyy-MM-dd hh:mm:ss] "));
            this.m_writer.WriteLine(value);
        }
        public override void Write(float value)
        {
            this.m_writer.Write(DateTime.Now.ToString("[yyyy-MM-dd hh:mm:ss] "));
            this.m_writer.WriteLine(value);
        }
        public override void Write(int value)
        {
            this.m_writer.Write(DateTime.Now.ToString("[yyyy-MM-dd hh:mm:ss] "));
            this.m_writer.WriteLine(value);
        }
        public override void Write(long value)
        {
            this.m_writer.Write(DateTime.Now.ToString("[yyyy-MM-dd hh:mm:ss] "));
            this.m_writer.WriteLine(value);
        }
        public override void Write(object value)
        {
            this.m_writer.Write(DateTime.Now.ToString("[yyyy-MM-dd hh:mm:ss] "));
            this.m_writer.WriteLine(value);
        }
        public override void Write(string value)
        {
            this.m_writer.Write(DateTime.Now.ToString("[yyyy-MM-dd hh:mm:ss] "));
            this.m_writer.WriteLine(value);
        }
        public override void Write(uint value)
        {
            this.m_writer.Write(DateTime.Now.ToString("[yyyy-MM-dd hh:mm:ss] "));
            this.m_writer.WriteLine(value);
        }
        public override void Write(ulong value)
        {
            this.m_writer.Write(DateTime.Now.ToString("[yyyy-MM-dd hh:mm:ss] "));
            this.m_writer.WriteLine(value);
        }
        public override void Write(string format, object arg0)
        {
            this.m_writer.Write(DateTime.Now.ToString("[yyyy-MM-dd hh:mm:ss] "));
            this.m_writer.WriteLine(format, arg0);
        }
        public override void Write(string format, params object[] arg)
        {
            this.m_writer.Write(DateTime.Now.ToString("[yyyy-MM-dd hh:mm:ss] "));
            this.m_writer.WriteLine(format, arg);
        }
        public override void Write(char[] buffer, int index, int count)
        {
            this.m_writer.Write(DateTime.Now.ToString("[yyyy-MM-dd hh:mm:ss] "));
            this.m_writer.WriteLine(buffer, index, count);
        }
        public override void Write(string format, object arg0, object arg1)
        {
            this.m_writer.Write(DateTime.Now.ToString("[yyyy-MM-dd hh:mm:ss] "));
            this.m_writer.WriteLine(format, arg0, arg1);
        }
        public override void Write(string format, object arg0, object arg1, object arg2)
        {
            this.m_writer.Write(DateTime.Now.ToString("[yyyy-MM-dd hh:mm:ss] "));
            this.m_writer.WriteLine(format, arg0, arg1, arg2);
        }
        public override Task WriteAsync(char value)
        {
            return Task.Factory.StartNew(() =>
            {
                this.m_writer.Write(DateTime.Now.ToString("[yyyy-MM-dd hh:mm:ss] "));
                this.m_writer.WriteLine(value);
            });
        }
        public override Task WriteAsync(string value)
        {
            return Task.Factory.StartNew(() =>
            {
                this.m_writer.Write(DateTime.Now.ToString("[yyyy-MM-dd hh:mm:ss] "));
                this.m_writer.WriteLine(value);
            });
        }
        public override Task WriteAsync(char[] buffer, int index, int count)
        {
            return Task.Factory.StartNew(() =>
            {
                this.m_writer.Write(DateTime.Now.ToString("[yyyy-MM-dd hh:mm:ss] "));
                this.m_writer.WriteLine(buffer, index, count);
            });
        }
        public override void WriteLine()
        {
            this.m_writer.WriteLine();
        }
        public override void WriteLine(bool value)
        {
            this.m_writer.Write(DateTime.Now.ToString("[yyyy-MM-dd hh:mm:ss] "));
            this.m_writer.WriteLine(value);
        }
        public override void WriteLine(char value)
        {
            this.m_writer.Write(DateTime.Now.ToString("[yyyy-MM-dd hh:mm:ss] "));
            this.m_writer.WriteLine(value);
        }
        public override void WriteLine(char[] buffer)
        {
            this.m_writer.Write(DateTime.Now.ToString("[yyyy-MM-dd hh:mm:ss] "));
            this.m_writer.WriteLine(buffer);
        }
        public override void WriteLine(decimal value)
        {
            this.m_writer.Write(DateTime.Now.ToString("[yyyy-MM-dd hh:mm:ss] "));
            this.m_writer.WriteLine(value);
        }
        public override void WriteLine(double value)
        {
            this.m_writer.Write(DateTime.Now.ToString("[yyyy-MM-dd hh:mm:ss] "));
            this.m_writer.WriteLine(value);
        }
        public override void WriteLine(float value)
        {
            this.m_writer.Write(DateTime.Now.ToString("[yyyy-MM-dd hh:mm:ss] "));
            this.m_writer.WriteLine(value);
        }
        public override void WriteLine(int value)
        {
            this.m_writer.Write(DateTime.Now.ToString("[yyyy-MM-dd hh:mm:ss] "));
            this.m_writer.WriteLine(value);
        }
        public override void WriteLine(long value)
        {
            this.m_writer.Write(DateTime.Now.ToString("[yyyy-MM-dd hh:mm:ss] "));
            this.m_writer.WriteLine(value);
        }
        public override void WriteLine(object value)
        {
            this.m_writer.Write(DateTime.Now.ToString("[yyyy-MM-dd hh:mm:ss] "));
            this.m_writer.WriteLine(value);
        }
        public override void WriteLine(string value)
        {
            this.m_writer.Write(DateTime.Now.ToString("[yyyy-MM-dd hh:mm:ss] "));
            this.m_writer.WriteLine(value);
        }
        public override void WriteLine(uint value)
        {
            this.m_writer.Write(DateTime.Now.ToString("[yyyy-MM-dd hh:mm:ss] "));
            this.m_writer.WriteLine(value);
        }
        public override void WriteLine(ulong value)
        {
            this.m_writer.Write(DateTime.Now.ToString("[yyyy-MM-dd hh:mm:ss] "));
            this.m_writer.WriteLine(value);
        }
        public override void WriteLine(string format, object arg0)
        {
            this.m_writer.Write(DateTime.Now.ToString("[yyyy-MM-dd hh:mm:ss] "));
            this.m_writer.WriteLine(format, arg0);
        }
        public override void WriteLine(string format, params object[] arg)
        {
            this.m_writer.Write(DateTime.Now.ToString("[yyyy-MM-dd hh:mm:ss] "));
            this.m_writer.WriteLine(format, arg);
        }
        public override void WriteLine(char[] buffer, int index, int count)
        {
            this.m_writer.Write(DateTime.Now.ToString("[yyyy-MM-dd hh:mm:ss] "));
            this.m_writer.WriteLine(buffer, index, count);
        }
        public override void WriteLine(string format, object arg0, object arg1)
        {
            this.m_writer.Write(DateTime.Now.ToString("[yyyy-MM-dd hh:mm:ss] "));
            this.m_writer.WriteLine(format, arg0, arg1);
        }
        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            this.m_writer.Write(DateTime.Now.ToString("[yyyy-MM-dd hh:mm:ss] "));
            this.m_writer.WriteLine(format, arg0, arg1, arg2);
        }
        public override Task WriteLineAsync()
        {
            return this.m_writer.WriteLineAsync();
        }
        public override Task WriteLineAsync(char value)
        {
            return Task.Factory.StartNew(() =>
            {
                this.m_writer.Write(DateTime.Now.ToString("[yyyy-MM-dd hh:mm:ss] "));
                this.m_writer.WriteLine(value);
            });
        }
        public override Task WriteLineAsync(string value)
        {
            return Task.Factory.StartNew(() =>
            {
                this.m_writer.Write(DateTime.Now.ToString("[yyyy-MM-dd hh:mm:ss] "));
                this.m_writer.WriteLine(value);
            });
        }
        public override Task WriteLineAsync(char[] buffer, int index, int count)
        {
            return Task.Factory.StartNew(() =>
            {
                this.m_writer.Write(DateTime.Now.ToString("[yyyy-MM-dd hh:mm:ss] "));
                this.m_writer.WriteLine(buffer, index, count);
            });
        }
    }
}
