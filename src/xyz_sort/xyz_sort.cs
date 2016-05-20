using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;

namespace System
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
               "   xyz_sort <source.xyz> [sorted.xyz]\n");
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


            try
            {
                using (var xyzLines = new XYZGridLines(srcFilePath))
                {
                    Console.Write("Loading source file...   ");
                    var lnCount = xyzLines.InitReader();
                    Console.WriteLine(lnCount.ToString() + " lines loaded. ");

                    Console.WriteLine("Writing sorted file...");
                    xyzLines.DuplicateFound += XyzReader_DuplicateFound;
                    xyzLines.SortAndWriteLines(dstFilePath);

                    var summaryText = xyzLines.WritedLines.ToString() + " lines saved. ";
                    if (xyzLines.Duplicates > 0)
                        summaryText += "(" + xyzLines.Duplicates.ToString() + " duplicates filtered out)";
                    Console.WriteLine(summaryText);
                }
            }
            catch (LineException lex)
            {
                Console.WriteLine("Invalid file format, line number " + lex.LineNumber.ToString() + ": ");
                Console.WriteLine(lex.LineText);
                Console.WriteLine(lex.Message);
                if (lex.InnerException != null)
                    Console.WriteLine(lex.InnerException.Message);
                Console.ReadKey();
                return;
            }
        }

        private static void XyzReader_DuplicateFound(object sender, XLineEventArgs e)
        {
            var duplicatesAreTheSame = true;
            var firstLn = e.Lines.First();

            foreach (var lnItem in e.Lines)
            {
                duplicatesAreTheSame = duplicatesAreTheSame && (firstLn.Text == lnItem.Text);
            }

            if (!duplicatesAreTheSame)
            {
                Console.WriteLine("Multiple lines with the same XY coordinates found:");
                foreach (var lnItem in e.Lines)
                {
                    Console.WriteLine("#{0, -10}: {1}", lnItem.LineNumber, lnItem.Text);
                }
                Console.WriteLine("Saved line : " + e.UsedLine);
                Console.WriteLine();
            }
        }
    }

    
}
