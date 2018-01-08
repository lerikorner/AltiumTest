using System;
using System.Text;

namespace AltiumTest
{
    public class KeyGenerator
    {
        //создает случайную строку заданной длины 
        public static string GetUniqueKeySimply(Int32 stringSize)
        {
            int startRange = 32, endRange = 126;           
            StringBuilder builder = new StringBuilder();           
            Random rand = new Random();
            for (Int32 i = 0; i < stringSize; i++)
                builder.Append((char)rand.Next(startRange, endRange));         
            return builder.ToString();
        }
    }
}
