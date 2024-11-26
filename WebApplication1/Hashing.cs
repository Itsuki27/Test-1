using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace WebApplication1
{
    public static class Hashing
    {
        public static string Hash(string value)
        {
            return Convert.ToBase64String(
                System.Security.Cryptography.SHA256.Create()
                .ComputeHash(Encoding.UTF8.GetBytes(value))
            );


        }
    }
}


//namespace WebApplication1
//{
//    public static class Hashing
//    {
//        public static string Hash(string value)
//        {
//            if (string.IsNullOrEmpty(value))
//            {
//                throw new ArgumentNullException(nameof(value), "Input string cannot be null or empty.");
//            }

//            return Convert.ToBase64String(
//                System.Security.Cryptography.SHA256.Create()
//                .ComputeHash(Encoding.UTF8.GetBytes(value))
//            );
//        }
//    }
//}




//if (String.IsNullOrEmpty(value))
//    throw new ArgumentNullException(nameof(value));

//var bytes = Encoding.UTF8.GetBytes(value);
//var hash = SHA256.Create().ComputeHash(bytes);
//return Convert.ToBase64String(hash);