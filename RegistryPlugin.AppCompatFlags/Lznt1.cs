using System;

namespace RegistryPlugin.AppCompatFlags
{
    /// <summary>LZNT1 decompression ([MS-XCA] 2.5), so the plugin needs no other assemblies</summary>
    public static class Lznt1
    {
        private const int ChunkSize = 0x1000;

        /// <summary>Decompresses count bytes of data from offset into output; returns how many bytes were written</summary>
        public static int Decompress(byte[] data, int offset, int count, byte[] output)
        {
            var end = Math.Min(data.Length, offset + Math.Max(0, count));
            var pos = offset;
            var outPos = 0;

            while (pos + 2 <= end)
            {
                var header = BitConverter.ToUInt16(data, pos);
                pos += 2;

                //a zero header ends the data
                if (header == 0) break;

                var chunkEnd = Math.Min(end, pos + (header & 0x0FFF) + 1);
                var chunkStart = outPos;

                if ((header & 0x8000) == 0)
                {
                    var length = Math.Min(chunkEnd - pos, output.Length - outPos);
                    Buffer.BlockCopy(data, pos, output, outPos, length);
                    outPos += length;
                    pos = chunkEnd;
                    continue;
                }

                while (pos < chunkEnd && outPos < output.Length)
                {
                    var flags = data[pos++];

                    for (var bit = 0; bit < 8 && pos < chunkEnd && outPos < output.Length; bit++, flags >>= 1)
                    {
                        if ((flags & 1) == 0)
                        {
                            output[outPos++] = data[pos++];
                            continue;
                        }

                        if (pos + 2 > chunkEnd) return outPos;

                        var token = BitConverter.ToUInt16(data, pos);
                        pos += 2;

                        //the further into the chunk, the more bits the displacement has (4 to 12)
                        var lengthBits = 12;
                        for (var i = outPos - chunkStart - 1; i >= 0x10; i >>= 1)
                        {
                            lengthBits--;
                        }

                        var displacement = (token >> lengthBits) + 1;
                        var copyLength = (token & ((1 << lengthBits) - 1)) + 3;

                        if (displacement > outPos - chunkStart) return outPos;

                        //byte by byte, as the copy can overlap what it writes
                        for (var i = 0; i < copyLength && outPos < output.Length && outPos - chunkStart < ChunkSize; i++)
                        {
                            output[outPos] = output[outPos - displacement];
                            outPos++;
                        }
                    }
                }

                pos = chunkEnd;
            }

            return outPos;
        }
    }
}
