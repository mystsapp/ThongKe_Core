using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace ThongKe.Helps
{
    public static class HelpExtension
    {
        //extension method
        public static object[,] To2DArray<T>(this List<T> lines, params Func<T, object>[] lambdas)
        {
            var array = new object[lines.Count(), lambdas.Count()];
            var lineCounter = 0;
            lines.ForEach(line =>
            {
                for (var i = 0; i < lambdas.Length; i++)
                {
                    array[lineCounter, i] = lambdas[i](line);
                }
                lineCounter++;
            });
            return array;
        }
        //public static void ForEach(this IEnumerable<T> enumeration, Action action)
        //{
        //    foreach (T item in enumeration)
        //    {
        //        action(item);
        //    }
        //}
    }

    public class MaHoaSHA1
    {
        public string EncodeSHA1(string pass)
        {
            SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(pass);
            bs = sha1.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x1").ToUpper());
            }
            pass = s.ToString();
            return pass;
        }
    }

    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);

            return value == null ? default(T) :
                JsonConvert.DeserializeObject<T>(value);
        }
    }
}
