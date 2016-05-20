using System;
using System.Collections.Generic;
using System.Text;

namespace System
{

    
    public class LineException : Exception
    {
        public LineException(string message, Exception inner, int lnNumber, string lnText) : base(message, inner)
        {
            this.LineNumber = lnNumber;
            this.LineText = lnText;
        }

        public int LineNumber { get; private set; }
        public string LineText { get; private set; }
    }

    public class XYZLine
    {
        public string Text { get; private set; }
        public int Number { get; private set; }
        public long Position { get; private set; }

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public XYZLine(string text, int number, long pos)
        {
            this.Text = text;
            this.Number = number;
            this.Position = pos;

            var lnValues = Utils.Split(text);
            this.X = Utils.ToDouble(lnValues[0]);
            this.Y = Utils.ToDouble(lnValues[1]);
            this.Z = Utils.ToDouble(lnValues[2]);
        }
    }

    public class XYZLinesReaderBase : LinesReader
    {

        public double XMin { get; private set; }
        public double XMax { get; private set; }

        public double YMin { get; private set; }
        public double YMax { get; private set; }

        public double ZMin { get; private set; }
        public double ZMax { get; private set; }

        private int maxLineLength = 1024;

        public XYZLinesReaderBase(string filePath) : base(filePath)
        {
            XMin = double.MaxValue;
            XMax = double.MinValue;

            YMin = double.MaxValue;
            YMax = double.MinValue;

            ZMin = double.MaxValue;
            ZMax = double.MinValue;
        }

        private void UpdateMinMax(double x, double y, double z)
        {
            XMin = Math.Min(XMin, x);
            XMax = Math.Max(XMax, x);

            YMin = Math.Min(YMin, y);
            YMax = Math.Max(YMax, y);

            ZMin = Math.Min(ZMin, z);
            ZMax = Math.Max(ZMax, z);
        }


        public int InitReader()
        {
            int lnNo = 0;            
            
            while (this.ReadLine())
            {
                lnNo++;
                try
                {
                    if (string.IsNullOrWhiteSpace(LineText))
                        continue;

                    if (LineText.Length > maxLineLength)
                    {
                        throw new LineException("Line too long.", null, lnNo, this.LineText);
                    }

                    var ln = new XYZLine(LineText, lnNo, this.LinePosition);
                    UpdateMinMax(ln.X, ln.Y, ln.Z);
                    InitLine(ln);

                }
                catch (LineException) { throw; }
                catch (Exception ex)
                {
                    throw new LineException("Invalid file format", ex, lnNo, LineText);
                }
            }
            return lnNo;

        }

        protected virtual void InitLine(XYZLine xyzLn)
        {
        }
    }
}
