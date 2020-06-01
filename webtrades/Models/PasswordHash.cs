using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace webtrades.Models
{
    public class PasswordHash
    {

        public static string CreateSalt()//Создаем соль
        {

            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[16];
            rng.GetBytes(buff);


            return Convert.ToBase64String(buff);
        }

        public static string GetHash(string input)//Хэшируем строку+соль методом md5
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));

            return Convert.ToBase64String(hash);
        }
    }
}
