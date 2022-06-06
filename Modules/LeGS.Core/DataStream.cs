using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace LEGS
{
	/// <summary>
	/// Types that support serialization by <see cref="DataStream"/>
	/// </summary>
	public enum DataStreamType : byte
	{
		Char = 1,
		Byte,
		Int,
		Float,
		String,
		Boolean,
		ByteArray,
		Short,
		UShort,
		UInt,
		Long,
		ULong,
		Double,
	}

	public class DataStream
	{
		private const int MaxStreamSize = 1024 * 1024;
		private readonly DataStreamType[] LengthDefinedTypes = { DataStreamType.ByteArray, DataStreamType.String };

		private int m_Index = 0;
		private byte[] m_Buffer;

		public bool isReading { get; private set; } = true;
		public bool isWriting => !isReading;

		/// <summary>
		/// Length, in bytes, of the current stream.
		/// If writing, this is equal to the amount of data written.
		/// </summary>
		public int Length => isReading ? m_Buffer.Length : m_Index;

		public DataStreamType NextType => (DataStreamType)m_Buffer[m_Index];

		/// <summary>
		/// Constructor for new, writing stream
		/// </summary>
		public DataStream() : this(new byte[] { }) { }

		/// <summary>
		/// Constructor for existing data. Sets <see cref="isReading"/> to true if <paramref name="data"/> is not null.
		/// </summary>
		public DataStream(IEnumerable<byte> data, int offset = 0) : this(data?.ToArray(), offset) { }

		/// <summary>
		/// Constructor for existing data. Sets <see cref="isReading"/> to true if <paramref name="data"/> is not null.
		/// </summary>
		public DataStream(byte[] data, int offset = 0)
		{
			m_Index = offset;
			isReading = data != null && data.Length > 0;
			m_Buffer = isReading ? data : new byte[MaxStreamSize];
		}

		public byte[] Data
		{
			get
			{
				if (isReading)
					return m_Buffer;

				byte[] d = new byte[Length];
				for (int i = 0; i < d.Length; i++)
					d[i] = m_Buffer[i];
				return d;
			}
		}

		public void SetReading()
		{
			if (isReading) return;
			isReading = true;
			m_Index = 0;
		}

		public void SetWriting()
		{
			if (isWriting) return;
			isReading = false;
			m_Index = 0;
		}

		public void ResetPosition() => m_Index = 0;

		private void WriteRange(byte[] data)
		{
			for (int i = 0; i < data.Length; i++)
				m_Buffer[m_Index++] = data[i];
		}

		private void InternalWrite(DataStreamType type, byte[] data)
		{
			m_Buffer[m_Index++] = (byte)type;
			if (LengthDefinedTypes.Contains(type))
				WriteRange(BitConverter.GetBytes(data.Length));
			WriteRange(data);
		}

		private int CheckType(DataStreamType expectedType)
		{
			if (!isReading) throw new Exception("Tried reading NetStream when it's not set to read!");
			if (m_Index + 1 > m_Buffer.Length)
				throw new Exception($"Could not check type - Read past type buffer");
			DataStreamType type = (DataStreamType)m_Buffer[m_Index++];
			if (type != expectedType)
				throw new Exception($"Could not check type - Incorrect type, got '{type}' but expected '{expectedType}' (index: {m_Index - 1}/{m_Buffer.Length})");
			int length = 0;
			if (LengthDefinedTypes.Contains(type))
			{
				if (m_Index + sizeof(int) > m_Buffer.Length)
					throw new Exception($"Could not check type - Read past length buffer (index: {m_Index}/{m_Buffer.Length}");

				length = BitConverter.ToInt32(m_Buffer, m_Index);
				m_Index += sizeof(int);
			}
			return length;
		}

		public void Append(DataStream stream) => WriteRange(stream.m_Buffer);

		public void Prepend(DataStream stream)
		{
			byte[] originalData = m_Buffer;
			m_Buffer = new byte[originalData.Length + stream.m_Buffer.Length];
			WriteRange(stream.m_Buffer);
			WriteRange(originalData);
		}

		#region Write
		public DataStream Write(byte[] b) { InternalWrite(DataStreamType.ByteArray, b); return this; }
		public DataStream Write(byte b) { InternalWrite(DataStreamType.Byte, new byte[] { b }); return this; }
		public DataStream Write(int i) { InternalWrite(DataStreamType.Int, BitConverter.GetBytes(i)); return this; }
		public DataStream Write(char c) { InternalWrite(DataStreamType.Char, BitConverter.GetBytes(c)); return this; }
		public DataStream Write(float f) { InternalWrite(DataStreamType.Float, BitConverter.GetBytes(f)); return this; }
		public DataStream Write(string s) { InternalWrite(DataStreamType.String, Encoding.UTF8.GetBytes(s)); return this; }
		public DataStream Write(bool b) { InternalWrite(DataStreamType.Boolean, new byte[] { (byte)(b ? 1 : 0) }); return this; }
		public DataStream Write(short s) { InternalWrite(DataStreamType.Short, BitConverter.GetBytes(s)); return this; }
		public DataStream Write(ushort s) { InternalWrite(DataStreamType.UShort, BitConverter.GetBytes(s)); return this; }
		public DataStream Write(uint i) { InternalWrite(DataStreamType.UInt, BitConverter.GetBytes(i)); return this; }
		public DataStream Write(long l) { InternalWrite(DataStreamType.Long, BitConverter.GetBytes(l)); return this; }
		public DataStream Write(ulong l) { InternalWrite(DataStreamType.ULong, BitConverter.GetBytes(l)); return this; }
		public DataStream Write(double d) { InternalWrite(DataStreamType.Double, BitConverter.GetBytes(d)); return this; }

		public DataStream Write<T>(T value)
		{
			if (value is byte) return Write((byte)Convert.ChangeType(value, typeof(byte)));
			else if (value is byte[]) return Write((byte[])Convert.ChangeType(value, typeof(byte[])));
			else if (value is string) return Write((string)Convert.ChangeType(value, typeof(string)));
			else if (value is int) return Write((int)Convert.ChangeType(value, typeof(int)));
			else if (value is uint) return Write((uint)Convert.ChangeType(value, typeof(uint)));
			else if (value is float) return Write((float)Convert.ChangeType(value, typeof(float)));
			else if (value is bool) return Write((bool)Convert.ChangeType(value, typeof(bool)));
			else if (value is short) return Write((short)Convert.ChangeType(value, typeof(short)));
			else if (value is ushort) return Write((ushort)Convert.ChangeType(value, typeof(ushort)));
			else if (value is long) return Write((long)Convert.ChangeType(value, typeof(long)));
			else if (value is ulong) return Write((ulong)Convert.ChangeType(value, typeof(ulong)));
			else if (value is double) return Write((double)Convert.ChangeType(value, typeof(double)));
			else throw new Exception($"Tried writing unsupported type '{typeof(T).Name}'");
		}
		#endregion

		#region Read
		public byte ReadByte()
		{
			CheckType(DataStreamType.Byte);
			if (m_Index + 1 > m_Buffer.Length) throw new Exception("Failed to read BYTE - Index past buffer");
			return m_Buffer[m_Index++];
		}

		public byte[] ReadByteArray()
		{
			int length = CheckType(DataStreamType.ByteArray);
			if (length < 0) throw new Exception("Invalid type, tried reading BYTE_ARRAY");
			if (m_Index + length > m_Buffer.Length)
				throw new Exception("Failed to read BYTE_ARRAY - Index past buffer");
			byte[] data = m_Buffer.Skip(m_Index).Take(length).ToArray();
			m_Index += length;
			return data;
		}

		public string ReadString()
		{
			int length = CheckType(DataStreamType.String);
			if (length < 0) throw new Exception("Invalid type, tried reading STRING");
			if (m_Index + length > m_Buffer.Length)
				throw new Exception("Failed to read STRING - Index past buffer");
			string s = Encoding.UTF8.GetString(m_Buffer.Skip(m_Index).Take(length).ToArray());
			m_Index += length;
			return s;
		}

		public char ReadChar()
		{
			CheckType(DataStreamType.Char);
			m_Index += sizeof(char);
			if (m_Index > m_Buffer.Length) throw new Exception("Failed to read CHAR - Index past buffer");
			return BitConverter.ToChar(m_Buffer, m_Index - sizeof(char));
		}

		public int ReadInt()
		{
			CheckType(DataStreamType.Int);
			m_Index += sizeof(int);
			if (m_Index > m_Buffer.Length) throw new Exception("Failed to read INT - Index past buffer");
			return BitConverter.ToInt32(m_Buffer, m_Index - sizeof(int));
		}

		public uint ReadUInt()
		{
			CheckType(DataStreamType.UInt);
			m_Index += sizeof(uint);
			if (m_Index > m_Buffer.Length) throw new Exception("Failed to read UINT - Index past buffer");
			return BitConverter.ToUInt32(m_Buffer, m_Index - sizeof(uint));
		}

		public float ReadFloat()
		{
			CheckType(DataStreamType.Float);
			m_Index += sizeof(float);
			if (m_Index > m_Buffer.Length) throw new Exception("Failed to read FLOAT - Index past buffer");
			return BitConverter.ToSingle(m_Buffer, m_Index - sizeof(float));
		}

		public bool ReadBoolean()
		{
			CheckType(DataStreamType.Boolean);
			if (m_Index + 1 > m_Buffer.Length) throw new Exception("Failed to read BOOL - Index past buffer");
			return m_Buffer[m_Index++] == (byte)1;
		}

		public short ReadShort()
		{
			CheckType(DataStreamType.Short);
			m_Index += sizeof(short);
			if (m_Index > m_Buffer.Length) throw new Exception("Failed to read SHORT - Index past buffer");
			return BitConverter.ToInt16(m_Buffer, m_Index - sizeof(short));
		}

		public ushort ReadUShort()
		{
			CheckType(DataStreamType.UShort);
			m_Index += sizeof(ushort);
			if (m_Index > m_Buffer.Length) throw new Exception("Failed to read USHORT - Index past buffer");
			return BitConverter.ToUInt16(m_Buffer, m_Index - sizeof(ushort));
		}

		public long ReadLong()
		{
			CheckType(DataStreamType.Long);
			m_Index += sizeof(long);
			if (m_Index > m_Buffer.Length) throw new Exception("Failed to read LONG - Index past buffer");
			return BitConverter.ToInt64(m_Buffer, m_Index - sizeof(long));
		}

		public ulong ReadULong()
		{
			CheckType(DataStreamType.Long);
			m_Index += sizeof(ulong);
			if (m_Index > m_Buffer.Length) throw new Exception("Failed to read ULONG - Index past buffer");
			return BitConverter.ToUInt64(m_Buffer, m_Index - sizeof(ulong));
		}

		public double ReadDouble()
		{
			CheckType(DataStreamType.Double);
			m_Index += sizeof(ulong);
			if (m_Index > m_Buffer.Length) throw new Exception("Failed to read DOUBLE - Index past buffer");
			return BitConverter.ToDouble(m_Buffer, m_Index - sizeof(ulong));
		}

		public T Read<T>()
		{
			Type type = typeof(T);
			return (T)Convert.ChangeType(new Func<object>(() =>
			{
				if		(type == typeof(byte)) return ReadByte();
				else if (type == typeof(byte[])) return ReadByteArray();
				else if (type == typeof(string)) return ReadString();
				else if (type == typeof(int)) return ReadInt();
				else if (type == typeof(uint)) return ReadUInt();
				else if (type == typeof(float)) return ReadFloat();
				else if (type == typeof(bool)) return ReadBoolean();
				else if (type == typeof(short)) return ReadShort();
				else if (type == typeof(ushort)) return ReadUShort();
				else if (type == typeof(long)) return ReadLong();
				else if (type == typeof(ulong)) return ReadULong();
				else if (type == typeof(double)) return ReadDouble();
				throw new Exception("Tried reading unsupported type");
			}).Invoke(), typeof(T));
		}
		#endregion

		/// <summary>
		/// Reads or writes to stream, depending on values of <see cref="isReading"/> & <see cref="isWriting"/>.
		/// </summary>
		/// <typeparam name="T">Type to serialize, must be one of <see cref="DataStreamType"/></typeparam>
		public void Serialize<T>(ref T value) { if (isWriting) Write(value); else value = Read<T>(); }
	}
}