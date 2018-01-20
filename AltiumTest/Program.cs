using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BigFileSorting
{
    class Program
    {
        static int slicesize = FileManager.SliceSize;


        static Int32 filesize = FileManager.FileSize;

        public static void Main()
        {
    
            FileManager.CreateWorkingDirs(FileManager.WorkPath);
            
            string FileName = FileManager.WorkPath + "\\out_small.txt";
            //FileManager.CreateFileFromListsByAppending();
            
            // MARK: - timer starts...
            var sWatch = System.Diagnostics.Stopwatch.StartNew();
            
            // MARK: - if slice size is less than file size, we go with split/external sort/merge procedure
            if (slicesize <filesize)
            {
                int Scounter = FileManager.FileSplit(FileName, filesize, slicesize);
                Console.WriteLine("File sorting state: {0}", Scounter);            
                FileManager.MergeByQueues(FileManager.WorkPath+"\\splits\\", 
                    FileName, 
                    FileManager.WorkPath+"\\out_merged_sorted.txt");
            }

            // MARK: - else we just sort file in RAM
            else
            {                
               string[] stringbuf = File.ReadAllLines(FileName);             
                List<string> trSorted = 
                    SortingMethods.TextRecordSortedInStrings(stringbuf.ToList<string>());              
                FileName = FileManager.WorkPath+"\\out_small_sorted.txt";
                FileManager.CreateFileFromListInRAM(FileName, trSorted, true);               
            }

            FileManager.DeleteTemporaryDirs(FileManager.WorkPath);
            // MARK: - timer stops.
            sWatch.Stop();
            Console.WriteLine("time spent: {0}", sWatch.Elapsed);
            Console.WriteLine("strings in file: {0}", FileManager.FileSize);
            Console.ReadKey();           
        }
    }
}
