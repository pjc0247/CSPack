using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cspack
{
    public static partial class CSPack
    {
        // pointer to a null-terminated string
        public class Packer_p : PackerBase
        {
            public static object Unpack(Byte[] src, int offset, out int consumed)
            {
                for (var i = offset; i < src.Length; i++)
                {
                    if (src[i] == 0)
                    {
                        consumed = i - offset + 1;
                        return Encoding.UTF8.GetString(src, offset, i - offset + 1);
                    }
                }

                throw new InvalidOperationException();
            }
            public static Byte[] Pack(object obj)
            {
                return Encoding.UTF8.GetBytes((String)obj + "\0");
            }
        }
    }
}
