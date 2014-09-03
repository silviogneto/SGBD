using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SGBDBuffer
{

    internal class Memory
    {
        internal const int PageLenght = 128;

        private IList<Page> _pages = new Page[10];
        private string _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data.txt");

        public IAlgorithms ChooserPage { get; set; }

        public Memory()
        {
            ChooserPage = new LRU();
        }

        internal void LoadPage(int page)
        {
            int slot;

            if (!PageInBuffer(page))
            {
                slot = GetFreeIndex();

                if (slot == -1)
                {
                    slot = ChooserPage.ChoosePageToRemove(_pages);

                    if (_pages[slot].Dirt)
                    {
                        SavePage(page, _pages[slot]);
                    }
                }

                using (var stream = new StreamReader(File.Open(_filePath, FileMode.Open)))
                {
                    string content = stream.ReadToEnd();

                    var newPage = new Page
                    {
                        Dirt = false,
                        PageNumber = page,
                        PinCount = 0,
                        LastAccess = DateTime.Now,
                        Data = content.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)[page]
                    };

                    _pages[slot] = newPage;
                }
            }
            else
            {
                slot = PageIndex(page);
                _pages[slot].LastAccess = DateTime.Now;
            }

            _pages[slot].PinCount++;
        }

        internal void ReleasePage(int page)
        {
            _pages[PageIndex(page)].PinCount--;
        }

        internal void SavePage(int page, Page info)
        {
            string[] lines = File.ReadAllLines(_filePath);

            using (var writer = new StreamWriter(_filePath, false))
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == page)
                        writer.WriteLine(info.Data);
                    else
                        writer.WriteLine(lines[i]);
                }
            }
        }

        internal void ChangePage(int page, char[] buffer)
        {
            Page p = _pages[PageIndex(page)];
            p.Data = new string(buffer);
            p.Dirt = true;
            p.LastAccess = DateTime.Now;
        }

        internal IList<Page> ListPages()
        {
            return _pages;
        }

        private bool PageInBuffer(int page)
        {
            return _pages.Any(x => x != null && x.PageNumber == page);
        }

        private int PageIndex(int page)
        {
            return _pages.IndexOf(_pages.Where(x => x.PageNumber == page).First());
        }

        private int GetFreeIndex()
        {
            return _pages.IndexOf(_pages.SkipWhile(x => x != null).FirstOrDefault());
        }
    }
}
