using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.IO;

namespace AltiumTest
{
    // MARK: - files processing
    public class FileManager
    {
        public static string WorkPath = "c:\\temp"; // MARK: - working dir
        public static int StringRange = 1024; // MARK: - max Description size
        public static Int32 FileSize = 900000; // MARK: - file size in strings
        public static Int32 SliceSize = 65000; // MARK: - slice size in strings
        public static ulong TotalRam = new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory; // MARK: - RAM volume

        // MARK: - working directories creating
        public static void CreateWorkingDirs(string path)
        {
            if (!Directory.Exists(path))
            {               
                Directory.CreateDirectory(path);
                Directory.CreateDirectory(path + "\\splits");
            }
        }

        // MARK: - creating random list
        public static List<string> StringListRandomizer(int length)
        {
            int codeRandom, descriptionRandom;
            Random rndCode = new Random();          

            // MARK: - value ranges
            int codeRangeRight = Int32.MaxValue;
            int codeRangeLeft = Int32.MinValue;
            uint codeRandomUInt = 0;
            int stringRange = StringRange;

            //repeaters for CodeID and Description
            string copier = "";
            UInt32 dubcode = 0;

            List<string> strBlocks = new List<string>();

            for (Int32 i = 0; i < length; i++)
            {               
                codeRandom = rndCode.Next(codeRangeLeft, codeRangeRight);
                codeRandomUInt = (uint)(codeRandom + codeRangeRight);
                descriptionRandom = rndCode.Next(0, stringRange);
                string desc = KeyGenerator.GetUniqueKeySimply(descriptionRandom);

                // MARK: - copying strings from time to time
                if (codeRandom % (descriptionRandom + 1) != 0)
                {
                    copier = desc;
                }
                else
                {
                    desc = KeyGenerator.GetUniqueKeySimply(descriptionRandom); 
                }
                dubcode = codeRandomUInt;
                strBlocks.Add(dubcode.ToString() + "." + copier);
            }
            return strBlocks;
        }

        // MARK: - list to file method, with creating option
        public static void FileFromList(string path, List<string>data, bool create)
        {
            if (create)
            {
                if (File.Exists(path)) File.Delete(path);              
            }
            File.WriteAllLines(path, data);           
        }

        public static void CreateFileFromLists(string path)
        {
            int FilePosition = 0;
            List<string> StringList = new List<string>();
            string fileName = FileManager.WorkPath + "\\out_small.txt";

            // MARK: - appending slices to file, while current position is before output file size
            while (FilePosition < FileSize)
            {
                if (FileSize % SliceSize == 0)
                {
                    FilePosition += SliceSize;
                    StringList = FileManager.StringListRandomizer(SliceSize);
                    File.AppendAllLines(fileName, StringList);
                }
                else if (FileSize - FilePosition < SliceSize)
                {
                    FilePosition += FileSize % SliceSize;
                    StringList = FileManager.StringListRandomizer(FileSize % SliceSize);
                    File.AppendAllLines(fileName, StringList);
                }
                else
                {
                    FilePosition += SliceSize;
                    StringList = FileManager.StringListRandomizer(SliceSize);
                    File.AppendAllLines(fileName, StringList);
                }
            }
        }

        // MARK: - file splitting/sorting/output to temp files
        public static int FileSplit(string path, Int32 FileSize, Int32 ListSize) 
        {
            var FileToProcess = new StreamReader(path);
            List<string> strSlice = new List<string>();
            int counter = 0;
            string tempPath = "";

            if (ListSize < FileSize)
            {
                counter = FileSize / ListSize;
                for (int i = 0; i < counter; i++)
                {
                    tempPath = WorkPath+"\\splits\\out_slice" + i + ".txt"; 
                    for (int j = 0; j < ListSize; j++)
                    {
                        strSlice.Add(FileToProcess.ReadLine());
                    }

                    // MARK: - random equitable file: using Quick Sort
                    //strSlice = Sorting.TextRecordSortedInStrings(strSlice); 

                    // MARK: - nearly sorted file: using Insertion Sort
                    strSlice = SortingMethods.TRSortedtoStringsByInserts(strSlice);

                    // MARK: - filling temp file with sorted list
                    FileFromList(tempPath, strSlice, true); 
                    strSlice.Clear();
                }

                if (FileSize % ListSize != 0)
                {
                    tempPath = WorkPath+"\\splits\\out_slice" + counter + ".txt";
                    while (FileToProcess.Peek() != -1)
                    {
                        strSlice.Add(FileToProcess.ReadLine());
                    }
                    strSlice = SortingMethods.TextRecordSortedInStrings(strSlice);
                    FileFromList(tempPath, strSlice, false);
                    strSlice.Clear();
                }
                return counter + 1;
            }
            else if (ListSize >= FileSize)
            {
                return 1;
            }
            else return -1;
        }

        // MARK: - merging with queues
        public static void MergeByQueues(string tempPath, string inPath, string outPath)
        {
            string[] TempPaths = Directory.GetFiles(tempPath, "out_slice*.txt");
            int Slices = TempPaths.Length; // MARK: - slices count
            int RecordSize = StringRange + UInt32.MaxValue.ToString().Length + 1; // MARK: - max string length
            int Records = FileSize; // MARK: - file size
            Int64 MaxUsage = Convert.ToInt64(TotalRam / 4); // MARK: - RAM volume, cut by 4
            Int64 BufferSize = MaxUsage / Slices; // MARK: - bytes per slice
            double RecordOverHead = 7.5; // MARK: - bytes to strings count
            int BufferLength = Convert.ToInt32(BufferSize / (RecordSize * RecordOverHead)); // MARK: - records count in queue
                       
            // MARK: - stream readers opening
            StreamReader[] readers = new StreamReader[Slices];
            for (int i = 0; i < Slices; i++)
                readers[i] = new StreamReader(TempPaths[i]);

            // MARK: - queues creating
            Queue<string>[] queues = new Queue<string>[Slices];
            for (int i = 0; i < Slices; i++)
                queues[i] = new Queue<string>(BufferLength);

            // MARK: - queues loading
            for (int i = 0; i < Slices; i++)
                LoadQueue(queues[i], readers[i], BufferLength);

            // MARK: - file merge
            StreamWriter sw = new StreamWriter(outPath);
            bool done = false;
            int LowestValueStringIndex, j, progress = 0; 
            uint   LowestStringCodeID, CurrentStringCodeID;
            string LowestValueString;

            while (!done)
            {
                // MARK: - progress on screen
                if (++progress % 5000 == 0)
                    Console.Write("{0:f2}%   \r",
                      100.0 * progress / Records);
                               
                LowestValueStringIndex = -1;
                LowestValueString = "";               
                for (j = 0; j < Slices; j++)
                {
                    if (queues[j] != null)
                    {
                        // MARK: - searching for 1st or lowest value                       
                        LowestStringCodeID = Convert.ToUInt32(LowestValueString.Substring(0, LowestValueString.IndexOf(".")));
                        CurrentStringCodeID = Convert.ToUInt32(queues[j].Peek().Substring(0, queues[j].Peek().IndexOf(".")));
                        if (LowestValueStringIndex < 0 || CurrentStringCodeID < LowestStringCodeID)
                        {
                            LowestValueStringIndex = j;
                            LowestValueString = queues[j].Peek();
                        }                      
                    }
                }
                             
                // MARK: - break if we are done 
                if (LowestValueStringIndex == -1) { done = true; break; }
                else
                {
                    // MARK: - output to file
                    sw.WriteLine(LowestValueString);

                    // MARK: - queue record deleting
                    queues[LowestValueStringIndex].Dequeue();

                    // MARK: - shifting queue 
                    if (queues[LowestValueStringIndex].Count == 0)
                    {
                        LoadQueue(queues[LowestValueStringIndex],
                          readers[LowestValueStringIndex], BufferLength);

                        // MARK: - queues records amount checking
                        if (queues[LowestValueStringIndex].Count == 0)
                        {
                            queues[LowestValueStringIndex] = null;
                        }
                    }               
                }
            }
            sw.Close();

            // MARK: - closing and deleting temporary files
            for (int i = 0; i < Slices; i++)
            {
                readers[i].Close();
                File.Delete(TempPaths[i]);
            }
        }

        // MARK: - loading queue with string read from file
        static void LoadQueue(Queue<string> queue,
            StreamReader file, int records)
        {
            for (int i = 0; i < records; i++)
            {
                if (file.Peek() < 0) break;
                queue.Enqueue(file.ReadLine());
            }
        }       
    }
}
