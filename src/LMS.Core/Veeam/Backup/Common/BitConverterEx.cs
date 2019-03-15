using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Common
{
  public static class BitConverterEx
  {
    public static int FillBytes(byte[] buff, int offset, byte value)
    {
      if (offset >= buff.Length)
        throw new IndexOutOfRangeException(nameof (offset));
      buff[offset] = value;
      return 1;
    }

    public static int FillBytes(byte[] buff, int offset, sbyte value)
    {
      if (offset >= buff.Length)
        throw new IndexOutOfRangeException(nameof (offset));
      buff[offset] = (byte) value;
      return 1;
    }

    public static int FillBytes(byte[] buff, int offset, bool value)
    {
      if (buff.Length - offset < 1)
        throw new IndexOutOfRangeException(nameof (offset));
      buff[offset] = value ? (byte) 1 : (byte) 0;
      return 1;
    }

    public static Guid ToGuid(byte[] value, int offset)
    {
      if (value.Length - offset < 16)
        throw new IndexOutOfRangeException(nameof (offset));
      return new Guid((int) value[offset + 3] << 24 | (int) value[2] << 16 | (int) value[offset + 1] << 8 | (int) value[offset], (short) ((int) value[offset + 5] << 8 | (int) value[offset + 4]), (short) ((int) value[offset + 7] << 8 | (int) value[offset + 6]), value[offset + 8], value[offset + 9], value[offset + 10], value[offset + 11], value[offset + 12], value[offset + 13], value[offset + 14], value[offset + 15]);
    }

    public static string DynToString(byte[] value, ref int offset)
    {
      int count = (int) value[offset];
      string str = Encoding.ASCII.GetString(value, offset + 1, count);
      offset += count + 1;
      return str;
    }

    public static ulong DynToUInt64(byte[] value, int offset, out int length)
    {
      length = (int) value[offset];
      ulong num = 0;
      for (int index = 0; index != length; ++index)
        num = num << 8 | (ulong) value[offset + index + 1];
      ++length;
      return num;
    }

    public static ulong DynToUInt64(byte[] value, ref int offset)
    {
      int length;
      ulong uint64 = BitConverterEx.DynToUInt64(value, offset, out length);
      offset += length;
      return uint64;
    }

    public static Guid LittleEndianToGuid(byte[] value, int offset)
    {
      return new Guid(BitConverter.ToUInt32(value, offset), BitConverter.ToUInt16(value, offset + 4), BitConverter.ToUInt16(value, offset + 6), value[offset + 8], value[offset + 9], value[offset + 10], value[offset + 11], value[offset + 12], value[offset + 13], value[offset + 14], value[offset + 15]);
    }

    public static Guid BigEndianToGuid(byte[] value, int offset)
    {
      return new Guid(BitConverterEx.BigEndianToUInt32(value, offset), BitConverterEx.BigEndianToUInt16(value, offset + 4), BitConverterEx.BigEndianToUInt16(value, offset + 6), value[offset + 8], value[offset + 9], value[offset + 10], value[offset + 11], value[offset + 12], value[offset + 13], value[offset + 14], value[offset + 15]);
    }

    public static ushort BigEndianToUInt16(byte[] value, int offset)
    {
      if (BitConverter.IsLittleEndian)
        return BitConverterEx.ReverseBytes(BitConverter.ToUInt16(value, offset));
      return BitConverter.ToUInt16(value, offset);
    }

    public static uint BigEndianToUInt32(byte[] value, int offset)
    {
      if (BitConverter.IsLittleEndian)
        return BitConverterEx.ReverseBytes(BitConverter.ToUInt32(value, offset));
      return BitConverter.ToUInt32(value, offset);
    }

    public static ulong BigEndianToUInt64(byte[] value, int offset)
    {
      if (BitConverter.IsLittleEndian)
        return BitConverterEx.ReverseBytes(BitConverter.ToUInt64(value, offset));
      return BitConverter.ToUInt64(value, offset);
    }

    public static ushort ReverseBytes(ushort value)
    {
      return (ushort) ((uint) (((int) value & (int) byte.MaxValue) << 8) | ((uint) value & 65280U) >> 8);
    }

    public static uint ReverseBytes(uint value)
    {
      return (uint) (((int) value & (int) byte.MaxValue) << 24 | ((int) value & 65280) << 8) | (value & 16711680U) >> 8 | (value & 4278190080U) >> 24;
    }

    public static ulong ReverseBytes(ulong value)
    {
      return (ulong) (((long) value & (long) byte.MaxValue) << 56 | ((long) value & 65280L) << 40 | ((long) value & 16711680L) << 24 | ((long) value & 4278190080L) << 8) | (value & 1095216660480UL) >> 8 | (value & 280375465082880UL) >> 24 | (value & 71776119061217280UL) >> 40 | (value & 18374686479671623680UL) >> 56;
    }

    public static ushort ReverseBits(ushort value)
    {
      ushort num = 0;
      for (int index = 0; index < 16; ++index)
      {
        num = (ushort) ((uint) (ushort) ((uint) num << 1) | (uint) (ushort) ((uint) value & 1U));
        value >>= 1;
      }
      return num;
    }

    public static uint ReverseBits(uint value)
    {
      uint num = 0;
      for (int index = 0; index < 32; ++index)
      {
        num = num << 1 | value & 1U;
        value >>= 1;
      }
      return num;
    }

    public static ulong ReverseBits(ulong value)
    {
      ulong num = 0;
      for (int index = 0; index < 64; ++index)
      {
        num = num << 1 | value & 1UL;
        value >>= 1;
      }
      return num;
    }
  }
}
