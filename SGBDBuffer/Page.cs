using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SGBDBuffer
{
    internal class Page
    {
        internal struct PageSlotConfig
        {
            public short InitByte;
            public short BytesLenght;
        }

        private byte[] _data;
        internal string Data
        {
            get { return Encoding.ASCII.GetString(_data); }
            set
            {
                _data = Encoding.ASCII.GetBytes(value);
            }
        }

        internal bool Dirt { get; set; }
        internal int PinCount { get; set; }
        internal int PageNumber { get; set; }
        internal DateTime LastAccess { get; set; }

        internal short RegistryCount { get { return GetRegistryCount(); } }
        internal short BytesUsageCount { get { return GetBytesUsageCount(); } }

        internal IDictionary<PageSlotConfig, Slot> PageSlots;

        internal Page()
        {
            PageSlots = new Dictionary<PageSlotConfig, Slot>();

            //aqui faz load dos slots e inicializa as counters
            var lastByte = Memory.PageLenght - 5;
                byte[] bytes;
            for (int i = 0, j = RegistryCount; i < j; i++)
            {
                bytes = new byte[] { _data[lastByte], _data[lastByte - 1] };
                var count = BitConverter.ToInt16(bytes, 0);

                lastByte -= 2;

                bytes = new byte[] { _data[lastByte], _data[lastByte - 1] };
                var ini = BitConverter.ToInt16(bytes, 0);

                var pageSlotConfig = new PageSlotConfig { BytesLenght = count, InitByte = ini };
                var newArray = new byte[count];
                Array.Copy(_data, 0, newArray, 0, count);
                var slot = new Slot(newArray);

                PageSlots.Add(pageSlotConfig, slot);
            }
        }

        private void BuildData()
        {
            int counter = 0;
            foreach (var item in PageSlots)
            {
                var slot = item.Value as Slot;
                foreach (var bytes in slot.GetSlotBytes())
                {
                    _data[counter] = bytes;
                    counter++;
                }
            }
        }

        private short GetRegistryCount()
        {
            if (_data.Length == 0)
                return 0;

            var lastByte = Memory.PageLenght - 3;
            byte[] bytes = new byte[] { _data[lastByte], _data[lastByte - 1] };

            return BitConverter.ToInt16(bytes, 0);
        }

        private short GetBytesUsageCount()
        {
            if (_data.Length == 0)
                return 0;

            var lastByte = Memory.PageLenght - 1;
            byte[] bytes = new byte[] { _data[lastByte], _data[lastByte - 1] };

            return BitConverter.ToInt16(bytes, 0);
        }

        public override string ToString()
        {
            return string.Format("Page: {0}, Pin-Count: {1}, Dirt: {2}", PageNumber, PinCount, Dirt);
        }
    }
}
