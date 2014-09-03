using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SGBDBuffer
{
    public class Buffer
    {
        internal Memory BufferMemory { get; private set; }

        public int ChangeIndex { get; private set; }

        private IDictionary<string, int> accessControl;
        private static object _syncLock = new object();

        public Buffer()
        {
            BufferMemory = new Memory();
            ChangeIndex = -1;
            accessControl = new Dictionary<string, int>();
        }

        public void ChangeAlgorithm(string selected)
        {
            switch (selected.ToUpper())
            {
                case "LRU":
                    BufferMemory.ChooserPage = new LRU();
                    break;
                case "MRU":
                    BufferMemory.ChooserPage = new MRU();
                    break;
            }
        }

        public void LoadPageInMemory()
        {
            string threadId = Thread.CurrentThread.ManagedThreadId.ToString();
            int index;

            lock (_syncLock)
            {
                if (accessControl.Any(x => x.Key.Equals(threadId)))
                    index = accessControl[threadId];
                else
                {
                    index = new Random().Next(0, 19);
                    accessControl.Add(threadId, index);
                }
            }

            lock (_syncLock)
            {
                BufferMemory.LoadPage(index);
            }
        }

        public void ChangePageInMemory()
        {
            string threadId = Thread.CurrentThread.ManagedThreadId.ToString();
            int index;

            lock (_syncLock)
            {
                if (accessControl.Any(x => x.Key.Equals(threadId)))
                    index = accessControl[threadId];
                else
                    return;
            }

            lock (_syncLock)
            {
                if (new Random().Next(0, 2) % 2 == 0)
                    BufferMemory.ChangePage(index, BuildNewLine());
            }
        }

        public void ReleasePageInMemory()
        {
            string threadId = Thread.CurrentThread.ManagedThreadId.ToString();
            int index;

            lock (_syncLock)
            {
                if (accessControl.Any(x => x.Key.Equals(threadId)))
                {
                    index = accessControl[threadId];
                    accessControl.Remove(threadId);
                }
                else
                    return;
            }

            lock (_syncLock)
            {
                BufferMemory.ReleasePage(index);
            }
        }

        public string PrintData()
        {
            var data = new StringBuilder();

            foreach (var pages in BufferMemory.ListPages())
            {
                if (pages != null)
                    data.AppendLine(pages.ToString());
            }

            return data.ToString();
        }

        private char[] BuildNewLine()
        {
            char[] c = new char[128];
            int caracterCode = new Random().Next(97, 122);

            for (int i = 0; i < c.Length; i++)
                c[i] = (char)caracterCode;

            return c;
        }
    }
}
