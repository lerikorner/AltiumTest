using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BigFileSorting
{
    class Program
    {
        // MARK: - slice size in strings  
        static int slicesize = FileManager.SliceSize;

        // MARK: - file size in strings      
        static Int32 fileSize = FileManager.FileSize;

        public static void Main()
        {
            // MARK: - working paths creating
            FileManager.CreateWorkingDirs(FileManager.WorkPath);
            
            // MARK: - creating file of known size
            string FileName = FileManager.WorkPath + "\\out_small.txt";
            FileManager.CreateFileFromListsByAppending();
            
            // MARK: - timer starts...
            var sWatch = System.Diagnostics.Stopwatch.StartNew();

            // MARK: - file splitting, in case of slice sizes      
            // MARK: - if slice size is less than file size, we go with split/external sort/merge procedure
            if (slicesize <fileSize)
            {
                int Scounter = FileManager.FileSplit(FileName, fileSize, slicesize);
                Console.WriteLine("File sorting state: {0}", Scounter);            
                FileManager.MergeByQueues(FileManager.WorkPath+"\\splits\\", 
                    FileName, 
                    FileManager.WorkPath+"\\out_merged_sorted.txt");
            }

            // MARK: - else we just sort file in RAM
            else
            {
                // MARK: - reading all strings to array
               string[] stringbuf = File.ReadAllLines(FileName);

                // MARK: - sorting array
                List<string> trSorted = 
                    SortingMethods.TextRecordSortedInStrings(stringbuf.ToList<string>());

                // MARK: - writing array to output file
                FileName = FileManager.WorkPath+"\\out_small_sorted.txt";
                FileManager.CreateFileFromListInRAM(FileName, trSorted, true);               
            }

            // MARK: - timer stops.
            sWatch.Stop();
            Console.WriteLine("time spent: {0}", sWatch.Elapsed);
            Console.WriteLine("strings in file: {0}", FileManager.FileSize);
            Console.ReadKey();           
        }
    }
}
