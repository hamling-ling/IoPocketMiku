using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace IoPokeMikuClient.Model
{
    public class Utility
    {
        public static IBuffer StrHex2ByteStream(string hexStr)
        {
            var dataWriter = new DataWriter();

            var lit = new[] { " " };
            var split = hexStr.Split(new[] { ' ' });
            foreach (var item in split)
            {
                if (string.IsNullOrWhiteSpace(item))
                {
                    continue;
                }
                var strByte = Convert.ToByte(item, 16);
                dataWriter.WriteByte(strByte);
            }
            return dataWriter.DetachBuffer();
        }
    }
}
