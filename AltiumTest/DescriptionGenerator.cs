using System;

namespace AltiumTest
{
    public class KeyGenerator
    {
        //создает случайную строку заданной длины 
        public static string GetUniqueKeySimply(int stringSize)
        {
            string str = "";
            Random rand = new Random();
            for (int i = 0; i < stringSize; i++)
                str += (char)rand.Next(33, 126);
            return str;
           ;
        }
    }
}
