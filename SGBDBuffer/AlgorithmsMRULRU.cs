using System;
using System.Linq;
using System.Collections.Generic;

namespace SGBDBuffer
{
    interface IAlgorithms
    {
        int ChoosePageToRemove(IList<Page> pages);
    }

    class LRU : IAlgorithms
    {
        public int ChoosePageToRemove(IList<Page> pages)
        {
            return pages.IndexOf(pages.Where(x => x.PinCount == 0).OrderBy(x => x.LastAccess).First());
        }
    }

    class MRU : IAlgorithms
    {
        public int ChoosePageToRemove(IList<Page> pages)
        {
            return pages.IndexOf(pages.Where(x => x.PinCount == 0).OrderByDescending(x => x.LastAccess).First());
        }
    }
}