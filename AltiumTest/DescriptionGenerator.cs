using System;
using System.Text;

namespace AltiumTest
{
    public class KeyGenerator
    {
        //создает случайную строку заданной длины 
        public static string GetUniqueKeySimply(Int32 stringSize)
        {
            StringBuilder builder = new StringBuilder();           
            Random rand = new Random();
            for (Int32 i = 0; i < stringSize; i++)
                builder.Append((char)rand.Next(33, 126));         
            return builder.ToString();
        }
    }
}
