using System;
using System.IO;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.LZW
{
	// Token: 0x020000A8 RID: 168
	[ComVisible(false)]
	public class LzwInputStream : Stream
	{
		// Token: 0x17000186 RID: 390
		// (get) Token: 0x0600081B RID: 2075 RVA: 0x0003D47C File Offset: 0x0003B67C
		// (set) Token: 0x0600081C RID: 2076 RVA: 0x0003D49C File Offset: 0x0003B69C
		public bool IsStreamOwner
		{
			get
			{
				return this.isStreamOwner;
			}
			set
			{
				this.isStreamOwner = value;
			}
		}

		// Token: 0x0600081D RID: 2077 RVA: 0x0003D4A8 File Offset: 0x0003B6A8
		public LzwInputStream(Stream baseInputStream)
		{
			this.baseInputStream = baseInputStream;
		}

		// Token: 0x0600081E RID: 2078 RVA: 0x0003D4FC File Offset: 0x0003B6FC
		public override int ReadByte()
		{
			int num = this.Read(this.one, 0, 1);
			bool flag = num == 1;
			int result;
			if (flag)
			{
				result = (int)(this.one[0] & byte.MaxValue);
			}
			else
			{
				result = -1;
			}
			return result;
		}

		// Token: 0x0600081F RID: 2079 RVA: 0x0003D544 File Offset: 0x0003B744
		public override int Read(byte[] buffer, int offset, int count)
		{
			bool flag = !this.headerParsed;
			if (flag)
			{
				this.ParseHeader();
			}
			bool flag2 = this.eof;
			int result;
			if (flag2)
			{
				result = -1;
			}
			else
			{
				int num = offset;
				int[] array = this.tabPrefix;
				byte[] array2 = this.tabSuffix;
				byte[] array3 = this.stack;
				int num2 = this.nBits;
				int num3 = this.maxCode;
				int num4 = this.maxMaxCode;
				int num5 = this.bitMask;
				int num6 = this.oldCode;
				byte b = this.finChar;
				int num7 = this.stackP;
				int num8 = this.freeEnt;
				byte[] array4 = this.data;
				int i = this.bitPos;
				int num9 = array3.Length - num7;
				bool flag3 = num9 > 0;
				if (flag3)
				{
					int num10 = (num9 >= count) ? count : num9;
					Array.Copy(array3, num7, buffer, offset, num10);
					offset += num10;
					count -= num10;
					num7 += num10;
				}
				bool flag4 = count == 0;
				if (flag4)
				{
					this.stackP = num7;
					result = offset - num;
				}
				else
				{
					int j;
					for (;;)
					{
						for (;;)
						{
							bool flag5 = this.end < 64;
							if (flag5)
							{
								this.Fill();
							}
							int num11 = (this.got > 0) ? (this.end - this.end % num2 << 3) : ((this.end << 3) - (num2 - 1));
							while (i < num11)
							{
								bool flag6 = count == 0;
								if (flag6)
								{
									goto Block_8;
								}
								bool flag7 = num8 > num3;
								if (flag7)
								{
									goto Block_9;
								}
								int num12 = i >> 3;
								j = (((int)(array4[num12] & byte.MaxValue) | (int)(array4[num12 + 1] & byte.MaxValue) << 8 | (int)(array4[num12 + 2] & byte.MaxValue) << 16) >> (i & 7) & num5);
								i += num2;
								bool flag8 = num6 == -1;
								if (flag8)
								{
									bool flag9 = j >= 256;
									if (flag9)
									{
										goto Block_12;
									}
									b = (byte)(num6 = j);
									buffer[offset++] = b;
									count--;
								}
								else
								{
									bool flag10 = j == 256 && this.blockMode;
									if (flag10)
									{
										goto Block_14;
									}
									int num13 = j;
									num7 = array3.Length;
									bool flag11 = j >= num8;
									if (flag11)
									{
										bool flag12 = j > num8;
										if (flag12)
										{
											goto Block_16;
										}
										array3[--num7] = b;
										j = num6;
									}
									while (j >= 256)
									{
										array3[--num7] = array2[j];
										j = array[j];
									}
									b = array2[j];
									buffer[offset++] = b;
									count--;
									num9 = array3.Length - num7;
									int num14 = (num9 >= count) ? count : num9;
									Array.Copy(array3, num7, buffer, offset, num14);
									offset += num14;
									count -= num14;
									num7 += num14;
									bool flag13 = num8 < num4;
									if (flag13)
									{
										array[num8] = num6;
										array2[num8] = b;
										num8++;
									}
									num6 = num13;
									bool flag14 = count == 0;
									if (flag14)
									{
										goto Block_20;
									}
								}
							}
							i = this.ResetBuf(i);
							if (this.got <= 0)
							{
								goto Block_22;
							}
						}
						Block_9:
						int num15 = num2 << 3;
						i = i - 1 + num15 - (i - 1 + num15) % num15;
						num2++;
						num3 = ((num2 == this.maxBits) ? num4 : ((1 << num2) - 1));
						num5 = (1 << num2) - 1;
						i = this.ResetBuf(i);
						continue;
						Block_14:
						Array.Copy(this.zeros, 0, array, 0, this.zeros.Length);
						num8 = 256;
						int num16 = num2 << 3;
						i = i - 1 + num16 - (i - 1 + num16) % num16;
						num2 = 9;
						num3 = (1 << num2) - 1;
						num5 = num3;
						i = this.ResetBuf(i);
					}
					Block_8:
					this.nBits = num2;
					this.maxCode = num3;
					this.maxMaxCode = num4;
					this.bitMask = num5;
					this.oldCode = num6;
					this.finChar = b;
					this.stackP = num7;
					this.freeEnt = num8;
					this.bitPos = i;
					return offset - num;
					Block_12:
					throw new LzwException("corrupt input: " + j.ToString() + " > 255");
					Block_16:
					throw new LzwException("corrupt input: code=" + j.ToString() + ", freeEnt=" + num8.ToString());
					Block_20:
					this.nBits = num2;
					this.maxCode = num3;
					this.bitMask = num5;
					this.oldCode = num6;
					this.finChar = b;
					this.stackP = num7;
					this.freeEnt = num8;
					this.bitPos = i;
					return offset - num;
					Block_22:
					this.nBits = num2;
					this.maxCode = num3;
					this.bitMask = num5;
					this.oldCode = num6;
					this.finChar = b;
					this.stackP = num7;
					this.freeEnt = num8;
					this.bitPos = i;
					this.eof = true;
					result = offset - num;
				}
			}
			return result;
		}

		// Token: 0x06000820 RID: 2080 RVA: 0x0003DA74 File Offset: 0x0003BC74
		private int ResetBuf(int bitPosition)
		{
			int num = bitPosition >> 3;
			Array.Copy(this.data, num, this.data, 0, this.end - num);
			this.end -= num;
			return 0;
		}

		// Token: 0x06000821 RID: 2081 RVA: 0x0003DABC File Offset: 0x0003BCBC
		private void Fill()
		{
			this.got = this.baseInputStream.Read(this.data, this.end, this.data.Length - 1 - this.end);
			bool flag = this.got > 0;
			if (flag)
			{
				this.end += this.got;
			}
		}

		// Token: 0x06000822 RID: 2082 RVA: 0x0003DB24 File Offset: 0x0003BD24
		private void ParseHeader()
		{
			this.headerParsed = true;
			byte[] array = new byte[3];
			int num = this.baseInputStream.Read(array, 0, array.Length);
			bool flag = num < 0;
			if (flag)
			{
				throw new LzwException("Failed to read LZW header");
			}
			bool flag2 = array[0] != 31 || array[1] != 157;
			if (flag2)
			{
				throw new LzwException(string.Format("Wrong LZW header. Magic bytes don't match. 0x{0:x2} 0x{1:x2}", array[0], array[1]));
			}
			this.blockMode = ((array[2] & 128) > 0);
			this.maxBits = (int)(array[2] & 31);
			bool flag3 = this.maxBits > 16;
			if (flag3)
			{
				throw new LzwException(string.Concat(new string[]
				{
					"Stream compressed with ",
					this.maxBits.ToString(),
					" bits, but decompression can only handle ",
					16.ToString(),
					" bits."
				}));
			}
			bool flag4 = (array[2] & 96) > 0;
			if (flag4)
			{
				throw new LzwException("Unsupported bits set in the header.");
			}
			this.maxMaxCode = 1 << this.maxBits;
			this.nBits = 9;
			this.maxCode = (1 << this.nBits) - 1;
			this.bitMask = this.maxCode;
			this.oldCode = -1;
			this.finChar = 0;
			this.freeEnt = (this.blockMode ? 257 : 256);
			this.tabPrefix = new int[1 << this.maxBits];
			this.tabSuffix = new byte[1 << this.maxBits];
			this.stack = new byte[1 << this.maxBits];
			this.stackP = this.stack.Length;
			for (int i = 255; i >= 0; i--)
			{
				this.tabSuffix[i] = (byte)i;
			}
		}

		// Token: 0x17000187 RID: 391
		// (get) Token: 0x06000823 RID: 2083 RVA: 0x0003DD28 File Offset: 0x0003BF28
		public override bool CanRead
		{
			get
			{
				return this.baseInputStream.CanRead;
			}
		}

		// Token: 0x17000188 RID: 392
		// (get) Token: 0x06000824 RID: 2084 RVA: 0x0003DD4C File Offset: 0x0003BF4C
		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000189 RID: 393
		// (get) Token: 0x06000825 RID: 2085 RVA: 0x0003DD68 File Offset: 0x0003BF68
		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700018A RID: 394
		// (get) Token: 0x06000826 RID: 2086 RVA: 0x0003DD84 File Offset: 0x0003BF84
		public override long Length
		{
			get
			{
				return (long)this.got;
			}
		}

		// Token: 0x1700018B RID: 395
		// (get) Token: 0x06000827 RID: 2087 RVA: 0x0003DDA4 File Offset: 0x0003BFA4
		// (set) Token: 0x06000828 RID: 2088 RVA: 0x0003DDC8 File Offset: 0x0003BFC8
		public override long Position
		{
			get
			{
				return this.baseInputStream.Position;
			}
			set
			{
				throw new NotSupportedException("InflaterInputStream Position not supported");
			}
		}

		// Token: 0x06000829 RID: 2089 RVA: 0x0003DDD8 File Offset: 0x0003BFD8
		public override void Flush()
		{
			this.baseInputStream.Flush();
		}

		// Token: 0x0600082A RID: 2090 RVA: 0x0003DDE8 File Offset: 0x0003BFE8
		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException("Seek not supported");
		}

		// Token: 0x0600082B RID: 2091 RVA: 0x0003DDF8 File Offset: 0x0003BFF8
		public override void SetLength(long value)
		{
			throw new NotSupportedException("InflaterInputStream SetLength not supported");
		}

		// Token: 0x0600082C RID: 2092 RVA: 0x0003DE08 File Offset: 0x0003C008
		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException("InflaterInputStream Write not supported");
		}

		// Token: 0x0600082D RID: 2093 RVA: 0x0003DE18 File Offset: 0x0003C018
		public override void WriteByte(byte value)
		{
			throw new NotSupportedException("InflaterInputStream WriteByte not supported");
		}

		// Token: 0x0600082E RID: 2094 RVA: 0x0003DE28 File Offset: 0x0003C028
		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			throw new NotSupportedException("InflaterInputStream BeginWrite not supported");
		}

		// Token: 0x0600082F RID: 2095 RVA: 0x0003DE38 File Offset: 0x0003C038
		public override void Close()
		{
			bool flag = !this.isClosed;
			if (flag)
			{
				this.isClosed = true;
				bool flag2 = this.isStreamOwner;
				if (flag2)
				{
					this.baseInputStream.Close();
				}
			}
		}

		// Token: 0x040004E1 RID: 1249
		private Stream baseInputStream;

		// Token: 0x040004E2 RID: 1250
		private bool isStreamOwner = true;

		// Token: 0x040004E3 RID: 1251
		private bool isClosed;

		// Token: 0x040004E4 RID: 1252
		private readonly byte[] one = new byte[1];

		// Token: 0x040004E5 RID: 1253
		private bool headerParsed;

		// Token: 0x040004E6 RID: 1254
		private const int TBL_CLEAR = 256;

		// Token: 0x040004E7 RID: 1255
		private const int TBL_FIRST = 257;

		// Token: 0x040004E8 RID: 1256
		private int[] tabPrefix;

		// Token: 0x040004E9 RID: 1257
		private byte[] tabSuffix;

		// Token: 0x040004EA RID: 1258
		private readonly int[] zeros = new int[256];

		// Token: 0x040004EB RID: 1259
		private byte[] stack;

		// Token: 0x040004EC RID: 1260
		private bool blockMode;

		// Token: 0x040004ED RID: 1261
		private int nBits;

		// Token: 0x040004EE RID: 1262
		private int maxBits;

		// Token: 0x040004EF RID: 1263
		private int maxMaxCode;

		// Token: 0x040004F0 RID: 1264
		private int maxCode;

		// Token: 0x040004F1 RID: 1265
		private int bitMask;

		// Token: 0x040004F2 RID: 1266
		private int oldCode;

		// Token: 0x040004F3 RID: 1267
		private byte finChar;

		// Token: 0x040004F4 RID: 1268
		private int stackP;

		// Token: 0x040004F5 RID: 1269
		private int freeEnt;

		// Token: 0x040004F6 RID: 1270
		private readonly byte[] data = new byte[8192];

		// Token: 0x040004F7 RID: 1271
		private int bitPos;

		// Token: 0x040004F8 RID: 1272
		private int end;

		// Token: 0x040004F9 RID: 1273
		private int got;

		// Token: 0x040004FA RID: 1274
		private bool eof;

		// Token: 0x040004FB RID: 1275
		private const int EXTRA = 64;
	}
}
