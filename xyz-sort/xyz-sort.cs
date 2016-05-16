using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace xyz_sort
{
    class Program
    {
        static void Main(string[] args)
        {
            

            if (args.Length < 1)
            {
                Console.WriteLine("Specify parameters:");
                Console.WriteLine("xyz-sort source.xyz sorted.xyz");
                Console.ReadKey();
                return;
            }

            if (!File.Exists(args[0]))
            {
                Console.WriteLine("File does not exists: " + args[0]);
                return;
            }

            var sortedLines = new SortedLines();
            long lnNo = 0;
            var dstFilePath = Path.ChangeExtension(args[0], "sorted" + Path.GetExtension(args[0]));
            if (args.Length > 1)
                dstFilePath = args[1];

            using (var srcFile = new StreamReader(args[0]))
            {
                Console.WriteLine("Reading file...");
                while (!srcFile.EndOfStream)
                {
                    lnNo++;
                    var ln = srcFile.ReadLine();
                    if (!sortedLines.Add(ln, lnNo))
                    {
                        Console.WriteLine("Invalid file format, line number " + lnNo.ToString() + ": ");
						if (ln.Length > 100)
						  ln = ln.Substring(0, 100);
                        Console.WriteLine(ln);
                        Console.ReadKey();
                        return;
                    }
                }
                srcFile.Close();
            }

            using (var dstFile = new StreamWriter(dstFilePath))
            {
                Console.WriteLine("Writing sorted file...");
                dstFile.AutoFlush = false;
                sortedLines.Write(dstFile);
                dstFile.Close();
                Console.WriteLine(lnNo.ToString() + " lines saved.");
            }    
        }
    }

    class DescendingComparer<T> : IComparer<T> where T : IComparable<T>
    {
        public int Compare(T x, T y)
        {
            return y.CompareTo(x);
        }
    }

    public class SortedLines : SortedDictionary<double, SortedSet<string>>
    {

        public SortedLines()
            :base(new DescendingComparer<double>())
        {

        }

        private char[] splitChars = " ,;\t".ToCharArray();

        public bool Add(string ln, long lnNo)
        {
            if (ln.Length > 999)
                return false;

            var lnValues = ln.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
            if (lnValues.Length < 3)
                return false;

            double yVal = 0.0;
            if (!double.TryParse(lnValues[1], out yVal))
                return false;

            SortedSet<string> lines = null;
            if (!this.TryGetValue(yVal, out lines))
            {
                lines = new SortedSet<string>();
                this[yVal] = lines;
            }
            lines.Add(ln);

            return true;
        }

        public void Write(StreamWriter writer)
        {
            double lastKey = double.MaxValue;
            
            foreach (var kv in this)
            {
                if (kv.Key > lastKey)
                    throw new Exception("FATAL SORTING ERROR!");
                lastKey = kv.Key;
                foreach (var ln in kv.Value)
                {
                    writer.WriteLine(ln);
                }
                writer.Flush();
            }
        }
    }
}
