using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace System
{


    public class LinesReader : IDisposable
    {
        FileStream fileStream;
        BufferedStream buffStream;
        bool windowsLineEnding = false;

        public LinesReader(string filePath)
        {
            fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            buffStream = new BufferedStream(fileStream);
            LineText = null;
            LinePosition = -1;
            DetectLineEnding();
        }

        public void Dispose()
        {
            buffStream.Dispose();
            fileStream.Dispose();
        }

        public string LineText { get; private set; }
        public long LinePosition { get; private set; }

        private void DetectLineEnding()
        {
            int chByte;
            while ((chByte = buffStream.ReadByte()) >= 0)
            {
                char ch = (char)chByte;
                if (chByte == '\r' || ch == '\n')
                {
                    //Windows:      '\r\n'
                    //Mac(OS 9 -):  '\r'
                    //Mac(OS 10 +): '\n'
                    //Unix / Linux: '\n'

                    chByte = buffStream.ReadByte();// Check if this is windows line ending
                    if (chByte >= 0)
                    {
                        char nextCh = (char)chByte;
                        this.windowsLineEnding = (ch == '\r') && (nextCh == '\n');
                    }

                    buffStream.Position = 0;
                    return;
                }
            }
        }

        public void SetPosition(long position)
        {
            this.buffStream.Position = position;
        }

        public bool ReadLine()
        {
            this.LinePosition = buffStream.Position;
            this.LineText = null;

            int chByte;
            char ch;

            chByte = buffStream.ReadByte();
            if (chByte < 0)
                return false;
            ch = (char)chByte;

            if ((this.windowsLineEnding) && (ch == '\n'))
            {
                this.LinePosition = buffStream.Position;
                chByte = buffStream.ReadByte();
                if (chByte < 0)
                    return false;
                ch = (char)chByte;
            }

            var ln = new StringBuilder();

            do
            {
                ch = (char)chByte;
                if (ch == '\r' || ch == '\n')
                {
                    break;
                }
                ln.Append(ch);

                chByte = buffStream.ReadByte();
            }
            while (chByte >= 0);

            this.LineText = ln.ToString();
            return true;
        }

    }
}
