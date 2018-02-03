using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;

namespace BigFileSorting
{
    class Program
    {
        static int slicesize = FileManager.SliceSize;
        static Int32 filesize = FileManager.FileSize;

        public static void Main()
        {                       
            string FileName = FileManager.WorkPath + "\\" + FileManager.InputFileName;         
            try
            {
                FileManager.CreateWorkingDirs(FileManager.WorkPath);

                //MARK: - uncomment if file to create. if file created, comment again, and rename file to out_small.txt.
                //FileManager.CreateFileFromListsByAppending();

                // MARK: - timer starts...
                var sWatch = System.Diagnostics.Stopwatch.StartNew();

                // MARK: - if slice size is less than file size, we go with split/external sort/merge procedure
                if (slicesize < filesize)
                {
                    FileManager.FileSplit(FileName, filesize, slicesize);                    
                    FileManager.MergeByQueues(FileManager.WorkPath + "\\splits\\",
                        FileName,
                        FileManager.WorkPath + "\\out_merged_sorted.txt");
                }

                // MARK: - else we just sort file in RAM
                else
                {
                    string[] stringbuf = File.ReadAllLines(FileName);
                    List<string> trSorted =
                        SortingMethods.TextRecordSortedInStrings(stringbuf.ToList<string>());
                    FileName = FileManager.WorkPath + "\\out_small_sorted.txt";
                    FileManager.CreateFileFromListInRAM(FileName, trSorted, true);
                }

                // MARK: - timer stops.
                sWatch.Stop();
                Console.WriteLine("time spent: {0}", sWatch.Elapsed);
                Console.WriteLine("strings in file: {0}", FileManager.FileSize);
                FileManager.DeleteTemporaryDirs(FileManager.WorkPath);
            }

            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }
            Console.ReadKey();           
        }
    }
}
