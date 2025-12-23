using Newtonsoft.Json.Linq;
using System.Buffers.Binary;
using System.Text;

namespace DarksideApi.DarkOrbit
{
    public class ByteArrayBuffer
    {
        public uint Length { get; private set; }
        public bool IsReady { get; private set; }

        public void Reset()
        {
            this.Length = 0;
            this.IsReady = false;
        }

        public void SetLength(uint length)
        {
            this.Length = length;
            this.IsReady = length > 0;// && length <= available;
        }
    }

    public class ByteArray
    {
        private uint position;
        private readonly List<byte> buffer;
        public long DataAvailable => this.buffer.Count - this.position;

        public byte this[int i] => this.buffer.ElementAt(i);

        public ByteArray(byte[] bytes)
        {
            this.buffer = [.. bytes];
        }

        public ByteArray()
        {
            this.buffer = [];
        }
        #region Read values
        public ushort ReadUShort()
        {
            return BinaryPrimitives.ReadUInt16BigEndian(this.GetBytes(2));
        }

        public short ReadShort()
        {
            return BinaryPrimitives.ReadInt16BigEndian(this.GetBytes(2));
        }

        public int ReadInt()
        {
            return BinaryPrimitives.ReadInt32BigEndian(this.GetBytes(4));
        }

        public long ReadInt64()
        {
            return BinaryPrimitives.ReadInt64BigEndian(this.GetBytes(8));
        }

        public string ReadUTF()
        {
            return Encoding.UTF8.GetString(this.GetBytes(this.ReadUShort()));
        }

        public string ReadUTF(uint length)
        {
            return Encoding.UTF8.GetString(this.GetBytes(length));
        }

        public float ReadFloat()
        {
            return BinaryPrimitives.ReadSingleBigEndian(this.GetBytes(4));
        }

        public double ReadDouble()
        {
            return BinaryPrimitives.ReadDoubleBigEndian(this.GetBytes(8));
        }

        public bool ReadBool()
        {
            return this.GetBytes(1)[0] != 0;
        }

        public byte ReadByte()
        {
            return this.GetBytes(1)[0];
        }

        public char ReadChar()
        {
            return (char)this.ReadByte();
        }

        public byte[] ReadBytes(uint length)
        {
            return this.GetBytes(length);
        }

        #endregion
        #region Write values
        public void WriteShort(short v)
        {
            var bytes = new byte[2];
            BinaryPrimitives.WriteInt16BigEndian(bytes, v);
            this.buffer.AddRange(bytes);
        }

        public void WriteUShort(ushort v)
        {
            var bytes = new byte[2];
            BinaryPrimitives.WriteUInt16BigEndian(bytes, v);
            this.buffer.AddRange(bytes);
        }

        public void WriteInt(int v)
        {
            var bytes = new byte[4];
            BinaryPrimitives.WriteInt32BigEndian(bytes, v);
            this.buffer.AddRange(bytes);
        }

        public void WriteInt64(long v)
        {
            var bytes = new byte[8];
            BinaryPrimitives.WriteInt64BigEndian(bytes, v);
            this.buffer.AddRange(bytes);
        }

        public void WriteFloat(float v)
        {
            var bytes = new byte[4];
            BinaryPrimitives.WriteSingleBigEndian(bytes, v);
            this.buffer.AddRange(bytes);
        }

        public void WriteDouble(double v)
        {
            var bytes = new byte[8];
            BinaryPrimitives.WriteDoubleBigEndian(bytes, v);
            this.buffer.AddRange(bytes);
        }

        public void WriteUTF(string v)
        {
            this.WriteUShort((ushort)v.Length);
            this.WriteByteArray(Encoding.UTF8.GetBytes(v));
        }

        public void WriteByte(byte v)
        {
            this.buffer.Add(v);
        }

        public void WriteByteArray(byte[] bytes)
        {
            this.buffer.AddRange(bytes);
        }

        public void WriteChar(char v)
        {
            this.WriteByte((byte)v);
        }

        public void WriteBool(bool v)
        {
            this.buffer.Add((byte)(v ? 1 : 0));
        }
        #endregion
        #region Get
        public byte[] ToArray()
        {
            return [.. this.buffer];
        }

        public int Length()
        {
            return this.buffer.Count;
        }

        private byte[] GetBytes(uint size)
        {
            if (this.position + size > this.Length())
            {
                throw new ArgumentOutOfRangeException($"Byte Array Size Exception\nLength={this.Length()}\nPosition={this.position}\nRequested Size={size}");
            }
            var bytes = new byte[size];

            for (int x = (int)this.position, i = 0; x < this.position + size; x++, i++)
            {
                bytes[i] = this.buffer[x];
            }

            this.position += size;
            return bytes;
        }
        #endregion
    }
}