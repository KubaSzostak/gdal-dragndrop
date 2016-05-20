using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace System
{


    public class YLineSection : IComparable<YLineSection>
    {
        public double Y { get; private set; }
        public int FirstLineNumber { get; private set; }
        public long FirstLinePosition { get; private set; }
        public int LinesCount { get; set; }

        public YLineSection(double y, int firstLnNo, long firstLnPos)
        {
            this.Y = y;
            this.FirstLineNumber = firstLnNo;
            this.FirstLinePosition = firstLnPos;
            this.LinesCount = 1;
        }

        public int CompareTo(YLineSection other)
        {
            return other.Y.CompareTo(this.Y);
        }
    }

    public class XLine : IComparable<XLine>
    {
        public double X { get; private set; }
        public string Text { get; private set; }
        public int LineNumber { get; private set; }

        public XLine(double x, string text, int lineNo)
        {
            this.X = x;
            this.Text = text;
            this.LineNumber = lineNo;
        }

        public int CompareTo(XLine other)
        {
            return this.X.CompareTo(other.X);
        }
    }

    public class XYZGridLines : XYZLinesReaderBase
    {

        public XYZGridLines(string filePath) : base(filePath)
        {
            WritedLines = 0;
            Duplicates = 0;
        }

        List<YLineSection> _sections = new List<YLineSection>();
        YLineSection _lastLn = null;

        protected override void InitLine(XYZLine ln)
        {
            base.InitLine(ln);

            if ((_lastLn == null) || (_lastLn.Y != ln.Y))
            {
                _lastLn = new YLineSection(ln.Y, ln.Number, ln.Position);
                _sections.Add(_lastLn);
            }
            else
            {
                _lastLn.LinesCount++;
            }            
        }

        public int WritedLines { get; private set; }
        public int Duplicates { get; private set; }

        public void SortAndWriteLines(string dstFilePath)
        {
            WritedLines = 0;
            Duplicates = 0;
            
            using (var dstFile = new StreamWriter(dstFilePath))
            {
                dstFile.AutoFlush = false;
                _sections.Sort();
                var ySectionsLookup = _sections.ToLookup(s => s.Y);

                foreach (var ySections in ySectionsLookup)
                {
                    var xLines = ReadXLines(ySections);
                    xLines.Sort();
                    
                    WriteXLines(dstFile, xLines);
                    dstFile.Flush();
                }

                dstFile.Close();
            }
        }


        private List<XLine> ReadXLines(IGrouping<double, YLineSection> ySections)
        {
            var res = new List<XLine>();

            foreach (var ySec in ySections)
            {
                this.SetPosition(ySec.FirstLinePosition);

                for (int i = 0; i < ySec.LinesCount; i++)
                {
                    if (this.ReadLine())
                    {
                        var lnValues = Utils.Split(this.LineText);
                        var x = Utils.ToDouble(lnValues[0]);
                        var xln = new XLine(x, this.LineText, ySec.FirstLineNumber + i);
                        res.Add(xln);
                    }
                }
            }
            return res;
        }


        private void WriteXLines(StreamWriter dstFile, List<XLine> xLines)
        {
            var xLinesLookup = xLines.ToLookup(ln => ln.X);
            foreach (var xLookup in xLinesLookup)
            {
                var ln = xLookup.First();
                dstFile.WriteLine(ln.Text);
                WritedLines++;

                if (xLookup.Count() != 1)
                {
                    Duplicates++;
                    if (DuplicateFound != null)
                        DuplicateFound(this, new XLineEventArgs(xLookup, ln.Text));
                }
            }
        }

        public event EventHandler<XLineEventArgs> DuplicateFound;
    }

    public class XLineEventArgs : EventArgs
    {
        public IEnumerable<XLine> Lines { get; private set; }
        public string UsedLine { get; private set; }

        public XLineEventArgs(IEnumerable<XLine> lines, string usedLine)
        {
            this.Lines = lines;
            this.UsedLine = usedLine;
        }
    }
}
