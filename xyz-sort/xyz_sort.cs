using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;

namespace xyz_sort
{
    class Program
    {
        static void PrintUsage()
        {
            Console.WriteLine(
               "This utility sorts text file in order to fulfill GDAL ASCII Gridded XYZ specification. \n" +
               "It places cells with same Y coordinates on consecutive lines. \n" +
               "For a same Y coordinate value it organizes the lines in the dataset by increasing X values. \n" +
               "The supported column separators are: space, comma, semicolon and tabulations.\n" +
               "\n" +
               "Usage: \n" +
               "   xyz-sort <source.xyz> [sorted.xyz]\n");
        }

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                PrintUsage();
                Console.ReadKey();
                return;
            }

            var srcFilePath = args[0];
            if (!File.Exists(srcFilePath))
            {
                Console.WriteLine("File does not exists: " + srcFilePath);
                return;
            }

            var dstFilePath = Path.ChangeExtension(srcFilePath, "sorted" + Path.GetExtension(srcFilePath));
            if (args.Length > 1)
                dstFilePath = args[1];
            


            var yLineSections = ReadYLineSections(srcFilePath);
            if (yLineSections == null)
                return;

            SortAndWriteLines(yLineSections, srcFilePath, dstFilePath);
        }

        static List<YLineSection> ReadYLineSections(string filePath)
        {
            var res = new List<YLineSection>();
            int lnNo = 0;
            string lnText = null;
            YLineSection lnSection = null;


            Console.Write("Loading source file...   ");
            using (var srcFile = new LinesReader(filePath))
            {
                while (srcFile.ReadLine())
                {
                    lnNo++;
                    lnText = srcFile.LineText;
                    try
                    {
                        if (string.IsNullOrWhiteSpace(lnText))
                            continue;

                        if (lnText.Length > 999)
                        {
                            lnText = lnText.Substring(0, 100) + "...";
                            throw new Exception("Line too long.");
                        }

                        var lnValues = Utils.Split(lnText);
                        Utils.ToDouble(lnValues[0]); // save resources, but check if X coordinate is valid
                        var y = Utils.ToDouble(lnValues[1]);
                        Utils.ToDouble(lnValues[2]); // save resources, but check if Z coordinate is valid

                        if (lnSection == null)
                        {
                            lnSection = new YLineSection(y, lnNo, srcFile.LinePosition);
                        }

                        if (lnSection.Y != y)
                        {
                            res.Add(lnSection);
                            lnSection = new YLineSection(y, lnNo, srcFile.LinePosition);
                        }

                        lnSection.LinesCount++;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Invalid file format, line number " + lnNo.ToString() + ": ");
                        Console.WriteLine(lnText);
                        Console.WriteLine(ex.Message);
                        Console.ReadKey();
                        return null;
                    }
                }
            }

            Console.WriteLine(lnNo.ToString() + " lines loaded. ");
            res.Add(lnSection); // add last section
            return res;
        }

        static List<XLine> ReadXLines(LinesReader srcFile, IGrouping<double, YLineSection> ySections)
        {
            var res = new List<XLine>();

            foreach (var ySec in ySections)
            {
                srcFile.SetPosition(ySec.FirstLinePosition);

                for (int i = 0; i < ySec.LinesCount; i++)
                {
                    if (srcFile.ReadLine())
                    { 
                        var lnValues = Utils.Split(srcFile.LineText); 
                        var x = Utils.ToDouble(lnValues[0]);
                        var xln = new XLine(x, srcFile.LineText, ySec.FirstLineNumber + i);
                        res.Add(xln);
                    }
                }
            }
            return res;
        }
        

        static void SortAndWriteLines(List<YLineSection> sections, string srcFilePath, string dstFilePath)
        {
            long count = 0;
            long duplicates = 0;

            using (var srcFile = new LinesReader(srcFilePath))
            using (var dstFile = new StreamWriter(dstFilePath))
            {
                dstFile.AutoFlush = false;
                Console.WriteLine("Sorting by Y coordinates...");
                sections.Sort();
                var ySectionsLookup = sections.ToLookup(s => s.Y);

                Console.WriteLine("Writing sorted file...");
                foreach (var ySections in ySectionsLookup)
                {
                    var xLines = ReadXLines(srcFile, ySections);
                    xLines.Sort();

                    int wCount;
                    int wDupl;
                    WriteXLines(dstFile, xLines, out wCount, out wDupl);

                    count += wCount;
                    duplicates += wDupl;
                    dstFile.Flush();
                }

                dstFile.Close();
            }

            var summaryText = count.ToString() + " lines saved. ";
            if (duplicates > 0)
                summaryText += "(" + duplicates.ToString() + " duplicates filtered out)";
            Console.WriteLine(summaryText);
        }


        static void WriteXLines(StreamWriter dstFile, List<XLine> xLines, out int count, out int duplicates)
        {
            count = 0;
            duplicates = 0;

            var xLinesLookup = xLines.ToLookup(ln => ln.X);
            foreach (var xLookup in xLinesLookup)
            {
                var ln = xLookup.First();
                dstFile.WriteLine(ln.Text);
                count++;

                if (xLookup.Count() != 1)
                {
                    duplicates++;
                    var duplicatesAreTheSame = true;
                    foreach (var lnItem in xLookup)
                    {
                        duplicatesAreTheSame = duplicatesAreTheSame && (ln.Text == lnItem.Text);
                    }
                    if (!duplicatesAreTheSame)
                    {
                        Console.WriteLine("Multiple lines with the same XY coordinates found:");
                        foreach (var lnItem in xLookup)
                        {
                            Console.WriteLine("#{0, -10}: {1}", lnItem.LineNumber, lnItem.Text);
                        }
                        Console.WriteLine("Saved line : " + ln.Text);
                        Console.WriteLine();
                    }

                }
            }
        }
    }

    public class Utils
    {
        public static char[] SplitChars = " ,;\t".ToCharArray();

        public static string[] Split(string s)
        {
            return s.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries);
        }
        
        public static double ToDouble(string s)
        {
            return double.Parse(s, CultureInfo.InvariantCulture);
        }
    }

    public class YLineSection : IComparable<YLineSection>
    {
        public double Y { get; private set; }
        public int FirstLineNumber { get; private set; }
        public long FirstLinePosition { get; private set; }
        public int LinesCount { get;  set; }

        public YLineSection(double y, int firstLnNo, long firstLnPos)
        {
            this.Y = y;
            this.FirstLineNumber = firstLnNo;
            this.FirstLinePosition = firstLnPos;
            this.LinesCount = 0;
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

    class DescendingComparer<T> : IComparer<T> where T : IComparable<T>
    {
        public int Compare(T x, T y)
        {
            return -y.CompareTo(x);
        }

    }

    public class LinesReader : IDisposable
    {        
        FileStream fileStream;
        BufferedStream buffStream;
        bool windowsLineEnding = false;
        int rByte = (int)'\n';
        int nByte = (int)'\r';

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
            while ((chByte = buffStream.ReadByte()) >=0)
            {
                char ch = (char)chByte;
                if (chByte == '\r' || ch == '\n')
                {
                    //Windows:      '\r\n'
                    //Mac(OS 9 -):  '\r'
                    //Mac(OS 10 +): '\n'
                    //Unix / Linux: '\n'

                    chByte = buffStream.ReadByte();// Check if this is windows line ending
                    if (chByte >=0)
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
                chByte = buffStream.ReadByte();
                if (chByte < 0)
                    return false;
                ch = (char)chByte;
            }

            var ln = new StringBuilder();

            do {
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
