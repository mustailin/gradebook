using System;
using System.Security.Cryptography;
using System.Text;

namespace GradeBook.Common
{
    public class Hasher
    {
        private const string _salt = "KPIIPSAS";

        public static string PBKDF2Hash(string input)
        {
            var saltBytes = Encoding.ASCII.GetBytes(_salt);
            // Generate the hash
            var pbkdf2 = new Rfc2898DeriveBytes(input, saltBytes, iterations: 5000);
            var result = pbkdf2.GetBytes(20); //20 bytes length is 160 bits
            return BitConverter.ToString(result).Replace("-", "");
        }
    }
}
