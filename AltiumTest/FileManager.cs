using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.IO;

namespace AltiumTest
{
    public class FileManager
    {
        public static int StringRange = 1024;
        public static Int32 FileSize = 29000;
        public static Int32 SliceSize = 6500;
        public static ulong TotalRam = new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory;


        //создание случайного списка
        public static List<string> StringListRandomizer(int length)
        {
            int CodeRandom, DescriptionRandom;
            Random rndCode = new Random();          

            //диапазоны значений
            int codeRangeRight = Int32.MaxValue;
            int codeRangeLeft = Int32.MinValue;
            uint codeRandomUInt = 0;
            int stringRange = StringRange;

            //повторители для Code и Description
            string copier = "";
            UInt32 dubcode = 0;

            List<string> strBlocks = new List<string>();

            for (Int32 i = 0; i < length; i++)
            {               
                CodeRandom = rndCode.Next(codeRangeLeft, codeRangeRight);
                codeRandomUInt = (uint)(CodeRandom + codeRangeRight);
                DescriptionRandom = rndCode.Next(0, stringRange);
                string desc = KeyGenerator.GetUniqueKeySimply(DescriptionRandom);

                if (CodeRandom % (DescriptionRandom + 1) != 0)
                {
                    copier = desc;
                }
                else
                {
                    desc = KeyGenerator.GetUniqueKeySimply(DescriptionRandom);
                }

                dubcode = codeRandomUInt;
                strBlocks.Add(dubcode.ToString() + "." + copier);
            }
            return strBlocks;
        }

        //перенос списка в файл (с опцией создания файла)
        public static void FileFromList(string path, List<string>data, bool create)
        {
            if (create)
            {
                if (File.Exists(path)) File.Delete(path);
                //File.Create(path);
            }
            File.WriteAllLines(path, data);           
        }

        //Разрезание файла на куски, которые сразу сортируются в оперативной памяти
        public static int FileSplit(string path, Int32 FileSize, Int32 ListSize)
        {
            var FiletoProcess = new StreamReader(path);
            List<string> strSlice = new List<string>();
            int counter = 0;
            string tempPath = "";

            if (ListSize < FileSize)
            {
                counter = FileSize / ListSize;
                for (int i = 0; i < counter; i++)
                {
                    tempPath = "c:\\temp\\splits\\out_slice" + i + ".txt";
                    for (int j = 0; j < ListSize; j++)
                    {
                        strSlice.Add(FiletoProcess.ReadLine());
                    }
                    strSlice = Sorting.TRSortedtoStrings(strSlice);
                    FileFromList(tempPath, strSlice, true);
                    strSlice.Clear();
                }

                if (FileSize % ListSize != 0)
                {
                    tempPath = "c:\\temp\\splits\\out_slice" + counter + ".txt";
                    while (FiletoProcess.Peek() != -1)
                    {
                        strSlice.Add(FiletoProcess.ReadLine());
                    }
                    strSlice = Sorting.TRSortedtoStrings(strSlice);
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

        //Лепка готового файла с помощью очередей
        public static void MergeByQueues(string tempPath, string inPath, string outPath)
        {
            string[] paths = Directory.GetFiles(tempPath, "out_slice*.txt");
            int slices = paths.Length; // количество кусков
            int recordsize = StringRange + UInt32.MaxValue.ToString().Length + 1; // оценочная длина записи
            int records = FileSize; // ожидаемое количество записей в файле
            Int64 maxusage = Convert.ToInt64(TotalRam / 4); // максимальное использование памяти, ограничиваем весь пул в 4 раза
            Int64 buffersize = maxusage / slices; // байт на каждый кусок
            double recordoverhead = 7.5; // The overhead of using Queue<> - как я понял, тут коэффициент превращения байт в строки, с небольшим запасом
            int bufferlen = Convert.ToInt32(buffersize / (recordsize * recordoverhead)); //количество записей в очереди
            //int bufferlen = records / slices + 1;

            Console.WriteLine("количество записей в очереди: {0}", bufferlen);
            Console.WriteLine("объем доступной оперативной памяти: {0} Мбайт", TotalRam/(1024*1024));
            Console.WriteLine("оценочная максимальная длина строки: {0}", recordsize);

            // Открытие временных файлов на чтение
            StreamReader[] readers = new StreamReader[slices];
            for (int i = 0; i < slices; i++)
                readers[i] = new StreamReader(paths[i]);

            // Создание очередей
            Queue<string>[] queues = new Queue<string>[slices];
            for (int i = 0; i < slices; i++)
                queues[i] = new Queue<string>(bufferlen);

            // Загрузка очередей
            for (int i = 0; i < slices; i++)
                LoadQueue(queues[i], readers[i], bufferlen);

            // Лепка файла
            StreamWriter sw = new StreamWriter(outPath);
            bool done = false;
            int lowest_index, j, progress = 0;
            string lowest_value;

            while (!done)
            {
                // Прогресс на экране
                if (++progress % 5000 == 0)
                    Console.Write("{0:f2}%   \r",
                      100.0 * progress / records);

                // Находим очередь с наименьшим значением, ближайшим к выводу               
                lowest_index = -1;
                lowest_value = "";
                bool flag;
                for (j = 0; j < slices; j++)
                {
                    if (queues[j] != null)
                    {                      
                        if (lowest_index < 0)
                        {
                            lowest_index = j;
                            lowest_value = queues[j].Peek();
                        }
                        else 
                        if 
                          //  ((String.CompareOrdinal(
                          //  queues[j].Peek().Substring(queues[j].Peek().IndexOf("."), queues[j].Peek().Length -
                          //  queues[j].Peek().IndexOf(".")),
                          //  lowest_value.Substring(lowest_value.IndexOf("."), lowest_value.Length -
                          //  lowest_value.IndexOf("."))) < 0)
                          //  &
                           ( (Convert.ToUInt32(queues[j].Peek().Substring(0, queues[j].Peek().IndexOf("."))) <
                            Convert.ToUInt32(lowest_value.Substring(0, lowest_value.IndexOf(".")))))
                        {
                            lowest_index = j;
                            lowest_value = queues[j].Peek();
                        }
                    }
                }
                             
                // Выход из цикла, если в очереди ничего 
                if (lowest_index == -1) { done = true; break; }
                else
                {
                    // Вывод в файл
                    sw.WriteLine(lowest_value);

                    // Удаление элемента из очереди
                    queues[lowest_index].Dequeue();
                    // Продвижение следующего элемента в очередь
                    if (queues[lowest_index].Count == 0)
                    {
                        LoadQueue(queues[lowest_index],
                          readers[lowest_index], bufferlen);

                        // Проверка на наличие элементов в очереди
                        if (queues[lowest_index].Count == 0)
                        {
                            queues[lowest_index] = null;
                        }
                    }               
                }
            }
            sw.Close();

            // Закрываем и удаляем временные файлы
            for (int i = 0; i < slices; i++)
            {
                readers[i].Close();
                File.Delete(paths[i]);
            }
        }

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
