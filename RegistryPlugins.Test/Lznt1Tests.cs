using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using RegistryPlugin.AppCompatFlags;

namespace RegistryPlugins.Test;

/// <summary>The AppCompatFlags plugin's LZNT1 decompression, on data compressed by hand ([MS-XCA] 2.5)</summary>
[TestFixture]
public class Lznt1Tests
{
    private static byte[] Chunk(bool compressed, params byte[] data)
    {
        var header = (ushort)((data.Length - 1) | 0x3000 | (compressed ? 0x8000 : 0));
        return BitConverter.GetBytes(header).Concat(data).ToArray();
    }

    [Test]
    public void Literals()
    {
        var data = Chunk(true, 0x00, (byte)'a', (byte)'b', (byte)'c');
        var output = new byte[3];

        Lznt1.Decompress(data, 0, data.Length, output).Should().Be(3);
        output.Should().Equal((byte)'a', (byte)'b', (byte)'c');
    }

    [Test]
    public void OverlappingCopy()
    {
        //"ab", then a copy of 6 from 2 back: "abababab"; at position 2 the token has 12 length bits
        var token = BitConverter.GetBytes((ushort)(((2 - 1) << 12) | (6 - 3)));
        var data = Chunk(true, 0x04, (byte)'a', (byte)'b', token[0], token[1]);
        var output = new byte[8];

        Lznt1.Decompress(data, 0, data.Length, output).Should().Be(8);
        System.Text.Encoding.ASCII.GetString(output).Should().Be("abababab");
    }

    [Test]
    public void DisplacementBitsGrowWithThePosition()
    {
        //17 literals, then a copy of 3 from 17 back: at position 17 the displacement has 5 bits (11 length bits)
        var literals = Enumerable.Range(0, 17).Select(t => (byte)('A' + t)).ToArray();
        var token = BitConverter.GetBytes((ushort)(((17 - 1) << 11) | (3 - 3)));
        var body = new byte[] { 0x00 }.Concat(literals.Take(8)).Concat(new byte[] { 0x00 }).Concat(literals.Skip(8).Take(8))
            .Concat(new byte[] { 0x02, literals[16], token[0], token[1] }).ToArray();
        var data = Chunk(true, body);
        var output = new byte[20];

        Lznt1.Decompress(data, 0, data.Length, output).Should().Be(20);
        output.Skip(17).Should().Equal((byte)'A', (byte)'B', (byte)'C');
    }

    [Test]
    public void UncompressedChunkAndEnd()
    {
        var data = Chunk(false, 1, 2, 3).Concat(new byte[] { 0, 0, 0xFF }).ToArray();
        var output = new byte[10];

        Lznt1.Decompress(data, 0, data.Length, output).Should().Be(3);
    }

    [Test]
    public void BadDataDoesNotThrow()
    {
        var random = new Random(1);
        for (var n = 0; n < 2000; n++)
        {
            var data = new byte[random.Next(0, 200)];
            random.NextBytes(data);

            var act = () => Lznt1.Decompress(data, 0, data.Length + random.Next(-10, 10), new byte[random.Next(0, 5000)]);
            act.Should().NotThrow();
        }

        //a copy from before the start of the chunk stops
        var token = BitConverter.GetBytes((ushort)((5 - 1) << 12));
        var bad = Chunk(true, 0x02, (byte)'a', token[0], token[1]);
        Lznt1.Decompress(bad, 0, bad.Length, new byte[10]).Should().Be(1);
    }
}
