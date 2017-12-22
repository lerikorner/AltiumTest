using System;
using System.Text;
using System.Security.Cryptography;

namespace AltiumTest
{
    public class KeyGenerator
    {
        public static int GetRandomStringLength(int left, int right)
        {
            Random rnd = new Random();
            return rnd.Next(left, right);
        }       
        public static string GetUniqueKeySimply(int maxSize)
        {
            string str="";
            char[] alphabet="abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890,.!?()@#%^-_ ".ToCharArray();
            Random rnd = new Random();
            for (int i = 0;i< maxSize; i++)
            {
                str += alphabet[rnd.Next(0, alphabet.Length)];
            }
            return str;
        }
    }
}
