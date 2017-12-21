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
        public static string GetUniqueKey(int maxSize)
        {
            char[] chars = new char[75];
            chars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890,.!?()@#%^-_ ".ToCharArray();
            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[maxSize];
                crypto.GetNonZeroBytes(data);
            }
            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
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
