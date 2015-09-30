using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Reflection;

namespace cspack
{
    public static partial class CSPack
    {
        public class PackerBase
        {
            public static List<String> suffix = new List<String>();

            public static object Unpack(Byte[] src, ref int offset, out int consumed) { consumed = 0; return null; }
            public static Byte[] Pack(object src) { return null; }

            protected enum Endian
            {
                Little,
                Big
            }
            protected static void ReorderBytes(Byte[] src, int offset, int length, Endian endian)
            {
                if (BitConverter.IsLittleEndian && endian == Endian.Little)
                    Array.Reverse(src);
                if (!BitConverter.IsLittleEndian && endian == Endian.Big)
                    Array.Reverse(src);
            }
            protected static void ReorderBytes(Byte[] src, Endian endian)
            {
                ReorderBytes(src, 0, src.Length, endian);
            }
        }

        private static MatchCollection ParseFormatString(String fmts)
        {
            var regex = new Regex("([@a-zA-Z-[xX]]?)([^a-zA-Z0-9@]*)([0-9]*)");
            var matches = regex.Matches(fmts);

            return matches;
        }
        private static Type FindPacker(String type, String suffix)
        {
            var p = from candidate in typeof(CSPack).GetNestedTypes()
                    where candidate.Name.EndsWith(type) ||
                          (candidate.Name.EndsWith(type + "_") &&
                           ((List<String>)(candidate.GetField("suffix").GetValue(null))).Contains(suffix))
                    select candidate;
            var packer = p.FirstOrDefault();

            return packer;
        }
        private static object InvokePackerMethod(Type packer, String method, object[] args)
        {
            return packer.InvokeMember(
                    method,
                    BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod,
                    null, null, args);
        }

        public static Byte[] Pack(this List<object> src, String fmts)
        {
            var results = new List<Byte>();

            foreach (Match match in ParseFormatString(fmts))
            {
                var type = match.Groups[1].Value;
                var suffix = match.Groups[2].Value;
                var num = match.Groups[3].Value;

                if (type == "" && suffix == "" && num == "")
                    continue;

                // misc directives
                // moves to absolute position
                if (type == "@")
                    throw new NotImplementedException();
                // null byte
                else if (type == "x")
                {
                    results.Add(0);
                    continue;
                }
                // back up a byte
                else if (type == "X")
                {
                    results.RemoveAt(results.Count - 1);
                    continue;
                }

                var packer = FindPacker(type, suffix);
                if (packer == null)
                    continue;

                var bytes = (Byte[])InvokePackerMethod(
                    packer, "Pack", new object[] { src.First() });
                src.RemoveAt(0);

                results.AddRange(bytes);
            }

            return results.ToArray();
        }
        public static List<object> Unpack(this Byte[] src, String fmts)
        {
            var results = new List<object>();
            var offset = 0;

            foreach (Match match in ParseFormatString(fmts))
            {
                var type = match.Groups[1].Value;
                var suffix = match.Groups[2].Value;
                var num = match.Groups[3].Value;

                if (type == "" && suffix == "" && num == "")
                    continue;

                // misc directives
                // moves to absolute position
                if (type == "@")
                {
                    throw new NotImplementedException();
                }
                // null byte
                else if (type == "x")
                {
                    offset += 1;
                    continue;
                }
                // back up a byte
                else if (type == "X")
                {
                    offset -= 1;
                    continue;
                }

                var packer = FindPacker(type, suffix);
                if (packer == null)
                    continue;

                var args = new object[] { src, offset, 0 };
                var obj = InvokePackerMethod(
                    packer, "Unpack", args);
                var consumed = (int)args[2];

                offset += consumed;


                results.Add(obj);
            }

            return results;
        }
    }
}
