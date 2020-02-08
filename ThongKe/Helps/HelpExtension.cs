using System;
using System.Collections.Generic;
using System.Linq;
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
}
