using System;

namespace AltiumTest
{
    public class KeyGenerator
    {
        //создает случайную строку заданной длины 
        public static string GetUniqueKeySimply(int maxSize, Random rnd)
        {
            string str="";
            char[] alphabet="abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890,.!?()@#%^-_ ".ToCharArray();          
            for (int i = 0;i< maxSize; i++)
            {
                str += alphabet[rnd.Next(0, alphabet.Length)];
            }
            return str;
        }
    }
}
