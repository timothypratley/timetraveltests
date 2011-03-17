using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.FSharp.Collections;
using System.Diagnostics;

namespace PerformanceTests
{
    [TestClass]
    public class CompareMaps
    {
        public class Item
        {
            public int A { get; set; }
            public string B { get; set; }
            public Item(int a, string b)
            {
                A = a;
                B = b;
            }
        }

        [TestMethod]
        public void Performance()
        {
            //Var.pushThreadBindings(RT.map(RT.CURRENT_NS, RT.CURRENT_NS.deref()));
            var m1 = new RecordedMapModel<string, Item>();
            //var m2 = new ClojureRecordedMapModel<string, Item>();
            var m3 = new Dictionary<string, Item>();
            var m4 = new FSharpMap<string, Item>(Enumerable.Empty<Tuple<string, Item>>());
            //var m5 = new PersistentTreeMap();

            const int size = 10000;
            var s = new Stopwatch();
            s.Start();
            for (var i = 0; i < size; i++)
                m1.Set(i.ToString(), new Item(i, i.ToString()));
            for (var i = 0; i < size * 100; i++)
                m1.Get((i % size).ToString());
            for (var i = 0; i < size; i++)
                m1.Remove(i.ToString());
            s.Stop();
            Console.WriteLine("RecordedMapModel took " + s.Elapsed);

            /*
            s.Restart();
            for (var i = 0; i < size; i++)
                m2.Set(i.ToString(), new Item(i, i.ToString()));
            for (var i = 0; i < size * 100; i++)
                m2.Get((i % size).ToString());
            for (var i = 0; i < size; i++)
                m2.Remove(i.ToString());
            s.Stop();
            Console.WriteLine("ClojureRecordedMapModel took " + s.Elapsed);
             */

            s.Restart();
            for (var i = 0; i < size; i++)
                m3[i.ToString()] = new Item(i, i.ToString());
            Item o;
            for (var i = 0; i < size * 100; i++)
                m3.TryGetValue((i % size).ToString(), out o);
            for (var i = 0; i < size; i++)
                m3.Remove(i.ToString());
            s.Stop();
            Console.WriteLine("Dictionary took " + s.Elapsed);

            m3.Clear();
            s.Restart();
            for (var i = 0; i < size; i++)
            {
                var tmp = new Dictionary<string, Item>(m3);
                tmp[i.ToString()] = new Item(i, i.ToString());
                m3 = tmp;
            }
            for (var i = 0; i < size * 100; i++)
                m3.TryGetValue((i % size).ToString(), out o);
            for (var i = 0; i < size; i++)
            {
                var tmp = new Dictionary<string, Item>(m3);
                tmp.Remove(i.ToString());
                m3 = tmp;
            }
            s.Stop();
            Console.WriteLine("Copy Dictionary took " + s.Elapsed);

            s.Restart();
            for (var i = 0; i < size; i++)
                m4 = m4.Add(i.ToString(), new Item(i, i.ToString()));
            for (var i = 0; i < size * 100; i++)
                m4.TryFind((i % size).ToString());
            for (var i = 0; i < size; i++)
                m4 = m4.Remove(i.ToString());
            s.Stop();
            Console.WriteLine("FSharpMap took " + s.Elapsed);

            /*
            s.Restart();
            for (var i = 0; i < size; i++)
                m5 = (PersistentTreeMap)m5.assoc(i.ToString(), new Item(i, i.ToString()));
            object obj;
            for (var i = 0; i < size * 100; i++)
                m5.TryGetValue((i % size).ToString(), out obj);
            for (var i = 0; i < size; i++)
                m5 = (PersistentTreeMap)RT.dissoc(m5, i.ToString());
            s.Stop();
            Console.WriteLine("PersistentTreeMap took " + s.Elapsed);
             */
        }
    }
}
