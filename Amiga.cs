/* Date: 12.8.2015, Time: 2:16 */
using System;
using System.Diagnostics;
using System.IO;

namespace IllidanS4.Amiga
{
	public static class AmigaTools
	{
		[DebuggerStepThrough]
		public static int ReadInt32BE(this BinaryReader reader)
		{
			if(BitConverter.IsLittleEndian)
			{
				int i = reader.ReadInt32();
				unchecked{
					return (
						(int)((i & 0xFF000000) >> 24) |
						((i & 0x00FF0000) >> 08) |
						((i & 0x0000FF00) << 08) |
						((i & 0x000000FF) << 24)
					);
				}
			}else{
				return reader.ReadInt32();
			}
		}
		
		[DebuggerStepThrough]
		public static short ReadInt16BE(this BinaryReader reader)
		{
			if(BitConverter.IsLittleEndian)
			{
				short i = reader.ReadInt16();
				unchecked{
					return (short)(
						(ushort)((i & 0xFF00) >> 8) |
						((i & 0x00FF) << 8)
					);
				}
			}else{
				return reader.ReadInt16();
			}
		}
		
		[DebuggerStepThrough]
		public static ushort ReadUInt16BE(this BinaryReader reader)
		{
			return unchecked((ushort)ReadInt16BE(reader));
		}
	}
}
