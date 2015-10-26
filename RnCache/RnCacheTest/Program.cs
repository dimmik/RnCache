using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RnCache;

namespace RnCacheTest
{
    internal class Program
    {
        private static void Main()
        {
            IRnCache c = new MemCacheRnCache();
            var r = new Random();
            c.InitEntityCache("list", () =>
            {
                Thread.Sleep(4000);
                var s = r.Next(100);
                return (new[] {s, s + 1, s + 2}).ToList();
            }, true);
            c.UpdateEntityCache("list", true);
            var list = c.GetCachedEntity<List<int>>("list");
            Console.WriteLine("c: " + (list == null ? "null" : "" + String.Join(",", list.ToArray())));
            var t = c.UpdateEntityCache("list");
            Thread.Sleep(1000);
            list = c.GetCachedEntity<List<int>>("list");
            Console.WriteLine("c: " + (list == null ? "null" : "" + String.Join(",", list.ToArray())));
            t.Wait();
            list = c.GetCachedEntity<List<int>>("list");
            Console.WriteLine("c: " + (list == null ? "null" : "" + String.Join(",", list.ToArray())));

            //Func<object> a = (Func<string>)(() => "aaaaa");
            Console.ReadKey();
        }
    }
}
