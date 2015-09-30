using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cspack
{
    public static partial class CSPack
    {
        // signed int, native endian
        public class Packer_i : PackerBase
        {
            public static object Unpack(Byte[] src, int offset, out int consumed)
            {
                consumed = sizeof(int);
                return BitConverter.ToInt32(src, offset);
            }
            public static Byte[] Pack(object obj)
            {
                return BitConverter.GetBytes((int)obj);
            }
        }
        public class Packer_i_ : Packer_i
        {
            public static List<String> suffix = new List<String> { "_", "!" };
        }

        // signed long, native endian
        public class Packer_l_ : PackerBase
        {
            public static List<String> suffix = new List<String> { "_", "!" };

            public static object Unpack(Byte[] src, int offset, out int consumed)
            {
                consumed = sizeof(long);
                return BitConverter.ToInt64(src, offset);
            }
            public static Byte[] Pack(object obj)
            {
                return BitConverter.GetBytes((long)obj);
            }
        }

        // 32-bit unsigned, network (big-endian) byte order
        public class Packer_N : PackerBase
        {
            public static object Unpack(Byte[] src, int offset, out int consumed)
            {
                consumed = sizeof(uint);
                ReorderBytes(src, offset, consumed, Endian.Big);
                return BitConverter.ToUInt32(src, offset);
            }
            public static Byte[] Pack(object obj)
            {
                var bytes = BitConverter.GetBytes((uint)obj);
                ReorderBytes(bytes, Endian.Big);
                return bytes;
            }
        }

        // 32-bit unsigned, VAX (little-endian) byte order
        public class Packer_V : PackerBase
        {
            public static object Unpack(Byte[] src, int offset, out int consumed)
            {
                consumed = sizeof(uint);
                ReorderBytes(src, offset, consumed, Endian.Little);
                return BitConverter.ToUInt32(src, offset);
            }
            public static Byte[] Pack(object obj)
            {
                var bytes = BitConverter.GetBytes((uint)obj);
                ReorderBytes(bytes, Endian.Little);
                return bytes;
            }
        }
    }
}
