using Microsoft.AspNet.SignalR.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Microsoft.AspNet.SignalR.Tests
{
    public class MemoryPoolTextWriterFacts
    {
        [Fact]
        public void CanEncodingSurrogatePairsCorrectly()
        {
            var bytes = new List<byte>();
            var writer = new BinaryMemoryPoolTextWriter(new MemoryPool());

            writer.Write("\U00024B62"[0]);
            writer.Write("\U00024B62"[1]);
            writer.Flush();

            var expected = new byte[] { 0xF0, 0xA4, 0xAD, 0xA2 };

            var buffer = writer.Buffer;
            Assert.Equal(4, buffer.Count);
            Assert.Equal(expected, new ArraySegment<byte>(buffer.Array, 0, buffer.Count));
        }

        [Fact]
        public void CanInterleaveStringsAndRawBinary()
        {
            var writer = new BinaryMemoryPoolTextWriter(new MemoryPool());

            var encoding = new UTF8Encoding();

            writer.Write('H');
            writer.Write('e');
            writer.Write("llo ");
            writer.Write(new ArraySegment<byte>(encoding.GetBytes("World")));
            writer.Flush();

            var buffer = writer.Buffer;
            Assert.Equal(11, buffer.Count);
            var s = encoding.GetString(buffer.Array, 0, buffer.Count);

            Assert.Equal("Hello World", s);
        }

        [Fact]
        public void StringOverrideBehavesAsCharArray()
        {
            const string testTxt = "some random text";

            // act 1
            var writer1 = new BinaryMemoryPoolTextWriter(new MemoryPool());
            for (int i = 0; i < 50; i++)
                writer1.Write(testTxt);
            writer1.Flush();
            var test1 = writer1.Encoding.GetString(writer1.Buffer.Array, 0, writer1.Buffer.Count);
            writer1.Dispose();

            // act 2
            var writer2 = new BinaryMemoryPoolTextWriter(new MemoryPool());
            for (int i = 0; i < 50; i++)
                writer2.Write(testTxt.ToCharArray(), 0, testTxt.Length);
            writer2.Flush();
            var test2 = writer2.Encoding.GetString(writer2.Buffer.Array, 0, writer2.Buffer.Count);
            writer2.Dispose();

            Assert.Equal(test1, test2);
        }
    }
}