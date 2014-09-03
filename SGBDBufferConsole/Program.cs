using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SGBDBufferConsole
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IList<object> l = new List<object>();
            l.Add(3);
            l.Add(5);
            l.Add(10);
            l.Add(18);
            l.Add("01");
            l.Add("claudio");
            l.Add("blumenau");
            l.Add("brasil");

            l.ToArray();

            byte[] b = BitConverter.GetBytes(short.Parse("0".ToString()));

            Console.WriteLine(BitConverter.ToInt16(b, 0));

            Console.WriteLine("Byte: {0}", BitConverter.ToString(BitConverter.GetBytes(byte.MaxValue)));
            Console.WriteLine("Short: {0}", BitConverter.ToString(BitConverter.GetBytes(short.MaxValue)));
            Console.ReadKey();
        }
    }
}
