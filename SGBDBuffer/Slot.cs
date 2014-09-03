using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SGBDBuffer
{
    /* Fixed slot format
     * ex.:
     * 
     * code:01
     * name:silvio
     * city:bnu
     */

    internal class Slot
    {
        private IList<object> _data;

        internal Slot(IList<byte> data)
        {
            _data = new List<object>();

            for (int i = 0, j = 0; i < 3; i++, j+=2)
            {
                _data.Add(BitConverter.ToInt16(new byte[] { data[j], data[j + 1] }, 0));
                //_data.Add(short.Parse(data[i].ToString()));
            }

            for (int i = 3, j = 0; i < 6; i++, j++)
            {
                var length = (short.Parse(_data[j + 1].ToString()) - short.Parse(_data[j].ToString()));
                var values = new byte[length];
                Array.Copy(data.ToArray(), short.Parse(_data[j].ToString()), values, 0, length);

                _data.Add(Encoding.ASCII.GetString(values));
            }
        }

        internal byte[] GetSlotBytes()
        {
            List<byte> ret = new List<byte>();

            for (int i = 0; i < 3; i++)
                ret.AddRange(BitConverter.GetBytes(short.Parse(_data[i].ToString())));

            for (int i = 3; i < _data.Count; i++)
                ret.AddRange(Encoding.ASCII.GetBytes(_data[i].ToString()));

            return ret.ToArray();
        }

        internal short GetSlotLength()
        {
            if (_data.Count == 0)
                return 0;

            var length = short.Parse("6");

            for (int i = 3; i < _data.Count; i++)
                length += (short)_data[i].ToString().ToCharArray().Length;

            return length;
        }

        internal short GetSlotLength(byte[] info)
        {
            if (info.Length == 0)
                return 0;

            var length = short.Parse("6") + ;

            for (int i = 3; i < info.Length; i++)
                length += (short)info[i].ToString().ToCharArray().Length;

            return length;
        }
    }
}
