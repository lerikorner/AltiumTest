using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.IO;

namespace BigFileSorting
{
    // MARK: - files processing
    public class FileManager
    {
        public static string WorkPath = "c:\\temp"; // MARK: - working dir
        public static int DescriptionRange = 1024; // MARK: - max Description size
        public static Int32 FileSize = 200000; // MARK: - file size in strings
        public static Int32 SliceSize = 100000; // MARK: - slice size in strings
        public static ulong TotalRam = new 
            Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory; // MARK: - RAM volume

        // MARK: - working directories creating
        public static void CreateWorkingDirs(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (!Directory.Exists(path + "\\splits"))
            {       
                Directory.CreateDirectory(path + "\\splits");
            }
        }
        public static void DeleteTemporaryDirs(string path)
        {           
            if (Directory.Exists(path + "\\splits"))
            {
                Directory.Delete(path + "\\splits");
            }
        }


        // MARK: - creating random list
        public static List<string> StringListRandomizer(int length)
        {
            int codeIDRandom, descriptionRandom;
            Random rndCode = new Random();                     
            int codeIDRangeRight = Int32.MaxValue;
            int codeIDRangeLeft = Int32.MinValue;
            uint codeIDRandomUInt = 0;
            int stringRange = DescriptionRange;            
            string copier = "";
            UInt32 dubcode = 0;
            List<string> strBlocks = new List<string>();

            for (Int32 i = 0; i < length; i++)
            {               
                codeIDRandom = rndCode.Next(codeIDRangeLeft, codeIDRangeRight);
                codeIDRandomUInt = (uint)(codeIDRandom + codeIDRangeRight);
                descriptionRandom = rndCode.Next(0, stringRange);
                string desc = KeyGenerator.GetUniqueKeySimply(descriptionRandom);
               
                if (codeIDRandom % (descriptionRandom + 1) != 0)
                {
                    copier = desc;
                }
                else
                {
                    desc = KeyGenerator.GetUniqueKeySimply(descriptionRandom); 
                }
                dubcode = codeIDRandomUInt;
                strBlocks.Add(dubcode.ToString() + "." + copier);
            }
            return strBlocks;
        }

        // MARK: - list to file method, with creating option
        public static void CreateFileFromListInRAM(string path, 
            List<string>data, bool create)
        {
            if (create)
            {
                if (File.Exists(path)) File.Delete(path);              
            }
            File.WriteAllLines(path, data);           
        }

        public static void CreateFileFromListsByAppending()
        {
            int FilePosition = 0;
            List<string> StringList = new List<string>();
            string fileName = FileManager.WorkPath + "\\out_small.txt";
            
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
                    StringList = 
                        FileManager.StringListRandomizer(FileSize % SliceSize);
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
        public static void FileSplit(string path, Int32 FileSize, Int32 ListSize) 
        {
            var FileToProcess = new StreamReader(path);
            List<string> strSlice = new List<string>();
            int FileCounter = 0;
            int PartSequenceSize = 0, //MARK: - defines Sorting state of file: counts partial increasing sequences in slice
                                      //SliceSortCounter~=0: File random. SLiceSortCounter~= |ListSize|: File sorted.
                                      //Number of deviations to switch between modes is: ...              
                SequenceCounter = 1;
            
            uint pivot = 0, current, left = 0;
            string tempPath = "";

            if (ListSize < FileSize)
            {
                FileCounter = FileSize / ListSize;
                for (int i = 0; i < FileCounter; i++)
                {
                    tempPath = WorkPath + "\\splits\\out_slice" + i + ".txt";
                    for (int j = 0; j < ListSize; j++)
                    {
                        strSlice.Add(FileToProcess.ReadLine());
                        current = Convert.ToUInt32(strSlice[j].Substring(0, strSlice[j].IndexOf(".")));

                        if ( j == 0)
                        {
                            left = current;                            
                        }
                        else left = Convert.ToUInt32(strSlice[j-1].Substring(0, strSlice[j-1].IndexOf(".")));

                        if (current >= left)
                        {                            
                            PartSequenceSize++;
                        }
                        else
                        {
                            SequenceCounter++;
                            PartSequenceSize--;
                        }                       
                    }
                    Console.WriteLine(PartSequenceSize);
                    Console.WriteLine(SequenceCounter);
                    // MARK: - random equitable file: using Quick Sort
                    if (PartSequenceSize < ListSize - SequenceCounter)                    
                        strSlice = SortingMethods.TextRecordSortedInStrings(strSlice);

                    // MARK: - partly sorted file: using Insertion Sort
                    else
                        strSlice = SortingMethods.TRSortedtoStringsByInserts(strSlice);
                    
                    PartSequenceSize = 0;
                    SequenceCounter = 1;
                    CreateFileFromListInRAM(tempPath, strSlice, true); 
                    strSlice.Clear();
                }

                if (FileSize % ListSize != 0)
                {
                    tempPath = WorkPath+"\\splits\\out_slice" + FileCounter + ".txt";
                    while (FileToProcess.Peek() != -1)
                    {
                        strSlice.Add(FileToProcess.ReadLine());
                    }

                    strSlice = SortingMethods.TextRecordSortedInStrings(strSlice);
                    CreateFileFromListInRAM(tempPath, strSlice, false);
                    strSlice.Clear();
                }              
            }                       
        }

        // MARK: - merging with queues
        public static void MergeByQueues(string tempPath, string inPath, string outPath)
        {
            string[] TempPaths = Directory.GetFiles(tempPath, "out_slice*.txt");
            int Slices = TempPaths.Length; 
            int RecordSize = DescriptionRange +
                UInt32.MaxValue.ToString().Length + 1;
            int Records = FileSize; 
            Int64 MaxUsage = Convert.ToInt64(TotalRam / 4); 
            Int64 BufferSize = MaxUsage / Slices; 
            double RecordOverHead = 7.5; 
            int BufferLength = Convert.ToInt32(BufferSize / 
                (RecordSize * RecordOverHead)); 

            List<string> OutputList = new List<string>();
            int StringCounter = 0;
                                  
            StreamReader[] readers = new StreamReader[Slices];
            for (int i = 0; i < Slices; i++)
                readers[i] = new StreamReader(TempPaths[i]);

            Queue<string>[] queues = new Queue<string>[Slices];
            for (int i = 0; i < Slices; i++)
                queues[i] = new Queue<string>(BufferLength);

            for (int i = 0; i < Slices; i++)
                LoadQueue(queues[i], readers[i], BufferLength);

            StreamWriter sw = new StreamWriter(outPath);
            bool done = false;
            int LowestValueStringIndex, j, progress = 0;
            uint LowestStringCodeID = 0, CurrentStringCodeID=0;
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
                        if (LowestValueStringIndex >= 0)
                        {
                            LowestStringCodeID = Convert.ToUInt32(LowestValueString.Substring(0, 
                                LowestValueString.IndexOf(".")));
                            CurrentStringCodeID = Convert.ToUInt32(queues[j].Peek().Substring(0, 
                                queues[j].Peek().IndexOf(".")));
                        }                           
                        if (LowestValueStringIndex < 0 || CurrentStringCodeID < LowestStringCodeID)
                        {
                            LowestValueStringIndex = j;
                            LowestValueString = queues[j].Peek();
                        }                      
                    }
                }

                if (LowestValueStringIndex == -1) { done = true; break; }
                else
                {

                    // MARK: - strings commented below show alternate method of output file appending.
                    // MARK: - instead of line-by-line method, we use appending list of strings, to reach less HDD requests.

                    //StringCounter++;
                    //OutputList.Add(LowestValueString);
                    //if ((OutputList.Count == FileManager.SliceSize) || (StringCounter == Records))
                    //{
                    //    File.AppendAllLines(outPath, OutputList);
                    //    OutputList.Clear();
                    //}


                    sw.WriteLine(LowestValueString);

                    queues[LowestValueStringIndex].Dequeue();

                    if (queues[LowestValueStringIndex].Count == 0)
                    {
                        LoadQueue(queues[LowestValueStringIndex],
                          readers[LowestValueStringIndex], BufferLength);

                        if (queues[LowestValueStringIndex].Count == 0)
                        {
                            queues[LowestValueStringIndex] = null;
                        }
                    }               
                }
            }
            sw.Close();

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
