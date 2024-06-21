using System;
using System.IO;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.LZW
{
	// Token: 0x02000099 RID: 153
	[ComVisible(false)]
	public class LzwInputStream : Stream
	{
		// Token: 0x17000132 RID: 306
		// (get) Token: 0x0600073F RID: 1855 RVA: 0x000306C4 File Offset: 0x0002E8C4
		// (set) Token: 0x06000740 RID: 1856 RVA: 0x000306E4 File Offset: 0x0002E8E4
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

		// Token: 0x06000741 RID: 1857 RVA: 0x000306F0 File Offset: 0x0002E8F0
		public LzwInputStream(Stream baseInputStream)
		{
			this.baseInputStream = baseInputStream;
		}

		// Token: 0x06000742 RID: 1858 RVA: 0x00030744 File Offset: 0x0002E944
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

		// Token: 0x06000743 RID: 1859 RVA: 0x0003078C File Offset: 0x0002E98C
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

		// Token: 0x06000744 RID: 1860 RVA: 0x00030CBC File Offset: 0x0002EEBC
		private int ResetBuf(int bitPosition)
		{
			int num = bitPosition >> 3;
			Array.Copy(this.data, num, this.data, 0, this.end - num);
			this.end -= num;
			return 0;
		}

		// Token: 0x06000745 RID: 1861 RVA: 0x00030D04 File Offset: 0x0002EF04
		private void Fill()
		{
			this.got = this.baseInputStream.Read(this.data, this.end, this.data.Length - 1 - this.end);
			bool flag = this.got > 0;
			if (flag)
			{
				this.end += this.got;
			}
		}

		// Token: 0x06000746 RID: 1862 RVA: 0x00030D6C File Offset: 0x0002EF6C
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

		// Token: 0x17000133 RID: 307
		// (get) Token: 0x06000747 RID: 1863 RVA: 0x00030F70 File Offset: 0x0002F170
		public override bool CanRead
		{
			get
			{
				return this.baseInputStream.CanRead;
			}
		}

		// Token: 0x17000134 RID: 308
		// (get) Token: 0x06000748 RID: 1864 RVA: 0x00030F94 File Offset: 0x0002F194
		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000135 RID: 309
		// (get) Token: 0x06000749 RID: 1865 RVA: 0x00030FB0 File Offset: 0x0002F1B0
		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000136 RID: 310
		// (get) Token: 0x0600074A RID: 1866 RVA: 0x00030FCC File Offset: 0x0002F1CC
		public override long Length
		{
			get
			{
				return (long)this.got;
			}
		}

		// Token: 0x17000137 RID: 311
		// (get) Token: 0x0600074B RID: 1867 RVA: 0x00030FEC File Offset: 0x0002F1EC
		// (set) Token: 0x0600074C RID: 1868 RVA: 0x00031010 File Offset: 0x0002F210
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

		// Token: 0x0600074D RID: 1869 RVA: 0x00031020 File Offset: 0x0002F220
		public override void Flush()
		{
			this.baseInputStream.Flush();
		}

		// Token: 0x0600074E RID: 1870 RVA: 0x00031030 File Offset: 0x0002F230
		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException("Seek not supported");
		}

		// Token: 0x0600074F RID: 1871 RVA: 0x00031040 File Offset: 0x0002F240
		public override void SetLength(long value)
		{
			throw new NotSupportedException("InflaterInputStream SetLength not supported");
		}

		// Token: 0x06000750 RID: 1872 RVA: 0x00031050 File Offset: 0x0002F250
		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException("InflaterInputStream Write not supported");
		}

		// Token: 0x06000751 RID: 1873 RVA: 0x00031060 File Offset: 0x0002F260
		public override void WriteByte(byte value)
		{
			throw new NotSupportedException("InflaterInputStream WriteByte not supported");
		}

		// Token: 0x06000752 RID: 1874 RVA: 0x00031070 File Offset: 0x0002F270
		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			throw new NotSupportedException("InflaterInputStream BeginWrite not supported");
		}

		// Token: 0x06000753 RID: 1875 RVA: 0x00031080 File Offset: 0x0002F280
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

		// Token: 0x0400046E RID: 1134
		private Stream baseInputStream;

		// Token: 0x0400046F RID: 1135
		private bool isStreamOwner = true;

		// Token: 0x04000470 RID: 1136
		private bool isClosed;

		// Token: 0x04000471 RID: 1137
		private readonly byte[] one = new byte[1];

		// Token: 0x04000472 RID: 1138
		private bool headerParsed;

		// Token: 0x04000473 RID: 1139
		private const int TBL_CLEAR = 256;

		// Token: 0x04000474 RID: 1140
		private const int TBL_FIRST = 257;

		// Token: 0x04000475 RID: 1141
		private int[] tabPrefix;

		// Token: 0x04000476 RID: 1142
		private byte[] tabSuffix;

		// Token: 0x04000477 RID: 1143
		private readonly int[] zeros = new int[256];

		// Token: 0x04000478 RID: 1144
		private byte[] stack;

		// Token: 0x04000479 RID: 1145
		private bool blockMode;

		// Token: 0x0400047A RID: 1146
		private int nBits;

		// Token: 0x0400047B RID: 1147
		private int maxBits;

		// Token: 0x0400047C RID: 1148
		private int maxMaxCode;

		// Token: 0x0400047D RID: 1149
		private int maxCode;

		// Token: 0x0400047E RID: 1150
		private int bitMask;

		// Token: 0x0400047F RID: 1151
		private int oldCode;

		// Token: 0x04000480 RID: 1152
		private byte finChar;

		// Token: 0x04000481 RID: 1153
		private int stackP;

		// Token: 0x04000482 RID: 1154
		private int freeEnt;

		// Token: 0x04000483 RID: 1155
		private readonly byte[] data = new byte[8192];

		// Token: 0x04000484 RID: 1156
		private int bitPos;

		// Token: 0x04000485 RID: 1157
		private int end;

		// Token: 0x04000486 RID: 1158
		private int got;

		// Token: 0x04000487 RID: 1159
		private bool eof;

		// Token: 0x04000488 RID: 1160
		private const int EXTRA = 64;
	}
}
