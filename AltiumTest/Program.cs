using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AltiumTest
{
    class Program
    {        
        public static void Main()
        {
            string fileName = "c:\\temp\\out_small.txt";

            //создаем случайный список и пишем его в файл
            //List<string> strBlock = FileManager.StringListRandomizer(102);  
            
            //FileManager.FileFromList(fileName, strBlock, false);

            var sWatch = System.Diagnostics.Stopwatch.StartNew();
            //считываем все строки файла в массив
            string[] stringbuf = File.ReadAllLines(fileName);

            List<string> trSorted = Sorting.TRSortedtoStrings(stringbuf.ToList<string>());

            //пишем сортированные данные в файл
            fileName = "c:\\temp\\out_small_sorted.txt";
           
            FileManager.FileFromList(fileName, trSorted, true);
            sWatch.Stop();

            //делим файл на куски заданного размера
            int counter = FileManager.FileSplit("c:\\temp\\out_small.txt", 30);

            //просеиваем строки в кусках и пишем в итоговый файл
            FileManager.MergeSortedFile("c:\\temp\\splits\\out_slice", counter, fileName, "c:\\temp\\out_merged_sorted.txt");

            Console.WriteLine("затрачено времени: {0}", sWatch.Elapsed);
            Console.WriteLine("количество строк в файле: {0}", FileManager.FileSizeinStrings(fileName));

            Console.ReadKey();           
        }
    }
}
