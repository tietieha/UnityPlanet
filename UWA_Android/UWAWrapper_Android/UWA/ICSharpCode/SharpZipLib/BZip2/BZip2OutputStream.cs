using System;
using System.IO;
using System.Runtime.InteropServices;
using UWA.ICSharpCode.SharpZipLib.Checksums;

namespace UWA.ICSharpCode.SharpZipLib.BZip2
{
	// Token: 0x020000C0 RID: 192
	[ComVisible(false)]
	public class BZip2OutputStream : Stream
	{
		// Token: 0x0600083F RID: 2111 RVA: 0x00035BDC File Offset: 0x00033DDC
		public BZip2OutputStream(Stream stream) : this(stream, 9)
		{
		}

		// Token: 0x06000840 RID: 2112 RVA: 0x00035BEC File Offset: 0x00033DEC
		public BZip2OutputStream(Stream stream, int blockSize)
		{
			this.BsSetStream(stream);
			this.workFactor = 50;
			bool flag = blockSize > 9;
			if (flag)
			{
				blockSize = 9;
			}
			bool flag2 = blockSize < 1;
			if (flag2)
			{
				blockSize = 1;
			}
			this.blockSize100k = blockSize;
			this.AllocateCompressStructures();
			this.Initialize();
			this.InitBlock();
		}

		// Token: 0x06000841 RID: 2113 RVA: 0x00035CE4 File Offset: 0x00033EE4
		~BZip2OutputStream()
		{
			this.Dispose(false);
		}

		// Token: 0x17000164 RID: 356
		// (get) Token: 0x06000842 RID: 2114 RVA: 0x00035D18 File Offset: 0x00033F18
		// (set) Token: 0x06000843 RID: 2115 RVA: 0x00035D38 File Offset: 0x00033F38
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

		// Token: 0x17000165 RID: 357
		// (get) Token: 0x06000844 RID: 2116 RVA: 0x00035D44 File Offset: 0x00033F44
		public override bool CanRead
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000166 RID: 358
		// (get) Token: 0x06000845 RID: 2117 RVA: 0x00035D60 File Offset: 0x00033F60
		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000167 RID: 359
		// (get) Token: 0x06000846 RID: 2118 RVA: 0x00035D7C File Offset: 0x00033F7C
		public override bool CanWrite
		{
			get
			{
				return this.baseStream.CanWrite;
			}
		}

		// Token: 0x17000168 RID: 360
		// (get) Token: 0x06000847 RID: 2119 RVA: 0x00035DA0 File Offset: 0x00033FA0
		public override long Length
		{
			get
			{
				return this.baseStream.Length;
			}
		}

		// Token: 0x17000169 RID: 361
		// (get) Token: 0x06000848 RID: 2120 RVA: 0x00035DC4 File Offset: 0x00033FC4
		// (set) Token: 0x06000849 RID: 2121 RVA: 0x00035DE8 File Offset: 0x00033FE8
		public override long Position
		{
			get
			{
				return this.baseStream.Position;
			}
			set
			{
				throw new NotSupportedException("BZip2OutputStream position cannot be set");
			}
		}

		// Token: 0x0600084A RID: 2122 RVA: 0x00035DF8 File Offset: 0x00033FF8
		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException("BZip2OutputStream Seek not supported");
		}

		// Token: 0x0600084B RID: 2123 RVA: 0x00035E08 File Offset: 0x00034008
		public override void SetLength(long value)
		{
			throw new NotSupportedException("BZip2OutputStream SetLength not supported");
		}

		// Token: 0x0600084C RID: 2124 RVA: 0x00035E18 File Offset: 0x00034018
		public override int ReadByte()
		{
			throw new NotSupportedException("BZip2OutputStream ReadByte not supported");
		}

		// Token: 0x0600084D RID: 2125 RVA: 0x00035E28 File Offset: 0x00034028
		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException("BZip2OutputStream Read not supported");
		}

		// Token: 0x0600084E RID: 2126 RVA: 0x00035E38 File Offset: 0x00034038
		public override void Write(byte[] buffer, int offset, int count)
		{
			bool flag = buffer == null;
			if (flag)
			{
				throw new ArgumentNullException("buffer");
			}
			bool flag2 = offset < 0;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			bool flag3 = count < 0;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			bool flag4 = buffer.Length - offset < count;
			if (flag4)
			{
				throw new ArgumentException("Offset/count out of range");
			}
			for (int i = 0; i < count; i++)
			{
				this.WriteByte(buffer[offset + i]);
			}
		}

		// Token: 0x0600084F RID: 2127 RVA: 0x00035ED4 File Offset: 0x000340D4
		public override void WriteByte(byte value)
		{
			int num = (256 + (int)value) % 256;
			bool flag = this.currentChar != -1;
			if (flag)
			{
				bool flag2 = this.currentChar == num;
				if (flag2)
				{
					this.runLength++;
					bool flag3 = this.runLength > 254;
					if (flag3)
					{
						this.WriteRun();
						this.currentChar = -1;
						this.runLength = 0;
					}
				}
				else
				{
					this.WriteRun();
					this.runLength = 1;
					this.currentChar = num;
				}
			}
			else
			{
				this.currentChar = num;
				this.runLength++;
			}
		}

		// Token: 0x06000850 RID: 2128 RVA: 0x00035F8C File Offset: 0x0003418C
		public override void Close()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x06000851 RID: 2129 RVA: 0x00035FA0 File Offset: 0x000341A0
		private void MakeMaps()
		{
			this.nInUse = 0;
			for (int i = 0; i < 256; i++)
			{
				bool flag = this.inUse[i];
				if (flag)
				{
					this.seqToUnseq[this.nInUse] = (char)i;
					this.unseqToSeq[i] = (char)this.nInUse;
					this.nInUse++;
				}
			}
		}

		// Token: 0x06000852 RID: 2130 RVA: 0x00036010 File Offset: 0x00034210
		private void WriteRun()
		{
			bool flag = this.last < this.allowableBlockSize;
			if (flag)
			{
				this.inUse[this.currentChar] = true;
				for (int i = 0; i < this.runLength; i++)
				{
					this.mCrc.Update(this.currentChar);
				}
				switch (this.runLength)
				{
				case 1:
					this.last++;
					this.block[this.last + 1] = (byte)this.currentChar;
					break;
				case 2:
					this.last++;
					this.block[this.last + 1] = (byte)this.currentChar;
					this.last++;
					this.block[this.last + 1] = (byte)this.currentChar;
					break;
				case 3:
					this.last++;
					this.block[this.last + 1] = (byte)this.currentChar;
					this.last++;
					this.block[this.last + 1] = (byte)this.currentChar;
					this.last++;
					this.block[this.last + 1] = (byte)this.currentChar;
					break;
				default:
					this.inUse[this.runLength - 4] = true;
					this.last++;
					this.block[this.last + 1] = (byte)this.currentChar;
					this.last++;
					this.block[this.last + 1] = (byte)this.currentChar;
					this.last++;
					this.block[this.last + 1] = (byte)this.currentChar;
					this.last++;
					this.block[this.last + 1] = (byte)this.currentChar;
					this.last++;
					this.block[this.last + 1] = (byte)(this.runLength - 4);
					break;
				}
			}
			else
			{
				this.EndBlock();
				this.InitBlock();
				this.WriteRun();
			}
		}

		// Token: 0x1700016A RID: 362
		// (get) Token: 0x06000853 RID: 2131 RVA: 0x00036268 File Offset: 0x00034468
		public int BytesWritten
		{
			get
			{
				return this.bytesOut;
			}
		}

		// Token: 0x06000854 RID: 2132 RVA: 0x00036288 File Offset: 0x00034488
		protected override void Dispose(bool disposing)
		{
			try
			{
				base.Dispose(disposing);
				bool flag = !this.disposed_;
				if (flag)
				{
					this.disposed_ = true;
					bool flag2 = this.runLength > 0;
					if (flag2)
					{
						this.WriteRun();
					}
					this.currentChar = -1;
					this.EndBlock();
					this.EndCompression();
					this.Flush();
				}
			}
			finally
			{
				if (disposing)
				{
					bool flag3 = this.IsStreamOwner;
					if (flag3)
					{
						this.baseStream.Close();
					}
				}
			}
		}

		// Token: 0x06000855 RID: 2133 RVA: 0x0003632C File Offset: 0x0003452C
		public override void Flush()
		{
			this.baseStream.Flush();
		}

		// Token: 0x06000856 RID: 2134 RVA: 0x0003633C File Offset: 0x0003453C
		private void Initialize()
		{
			this.bytesOut = 0;
			this.nBlocksRandomised = 0;
			this.BsPutUChar(66);
			this.BsPutUChar(90);
			this.BsPutUChar(104);
			this.BsPutUChar(48 + this.blockSize100k);
			this.combinedCRC = 0U;
		}

		// Token: 0x06000857 RID: 2135 RVA: 0x00036390 File Offset: 0x00034590
		private void InitBlock()
		{
			this.mCrc.Reset();
			this.last = -1;
			for (int i = 0; i < 256; i++)
			{
				this.inUse[i] = false;
			}
			this.allowableBlockSize = 100000 * this.blockSize100k - 20;
		}

		// Token: 0x06000858 RID: 2136 RVA: 0x000363EC File Offset: 0x000345EC
		private void EndBlock()
		{
			bool flag = this.last < 0;
			if (!flag)
			{
				this.blockCRC = (uint)this.mCrc.Value;
				this.combinedCRC = (this.combinedCRC << 1 | this.combinedCRC >> 31);
				this.combinedCRC ^= this.blockCRC;
				this.DoReversibleTransformation();
				this.BsPutUChar(49);
				this.BsPutUChar(65);
				this.BsPutUChar(89);
				this.BsPutUChar(38);
				this.BsPutUChar(83);
				this.BsPutUChar(89);
				this.BsPutint((int)this.blockCRC);
				bool flag2 = this.blockRandomised;
				if (flag2)
				{
					this.BsW(1, 1);
					this.nBlocksRandomised++;
				}
				else
				{
					this.BsW(1, 0);
				}
				this.MoveToFrontCodeAndSend();
			}
		}

		// Token: 0x06000859 RID: 2137 RVA: 0x000364DC File Offset: 0x000346DC
		private void EndCompression()
		{
			this.BsPutUChar(23);
			this.BsPutUChar(114);
			this.BsPutUChar(69);
			this.BsPutUChar(56);
			this.BsPutUChar(80);
			this.BsPutUChar(144);
			this.BsPutint((int)this.combinedCRC);
			this.BsFinishedWithStream();
		}

		// Token: 0x0600085A RID: 2138 RVA: 0x00036540 File Offset: 0x00034740
		private void BsSetStream(Stream stream)
		{
			this.baseStream = stream;
			this.bsLive = 0;
			this.bsBuff = 0;
			this.bytesOut = 0;
		}

		// Token: 0x0600085B RID: 2139 RVA: 0x00036560 File Offset: 0x00034760
		private void BsFinishedWithStream()
		{
			while (this.bsLive > 0)
			{
				int num = this.bsBuff >> 24;
				this.baseStream.WriteByte((byte)num);
				this.bsBuff <<= 8;
				this.bsLive -= 8;
				this.bytesOut++;
			}
		}

		// Token: 0x0600085C RID: 2140 RVA: 0x000365C8 File Offset: 0x000347C8
		private void BsW(int n, int v)
		{
			while (this.bsLive >= 8)
			{
				int num = this.bsBuff >> 24;
				this.baseStream.WriteByte((byte)num);
				this.bsBuff <<= 8;
				this.bsLive -= 8;
				this.bytesOut++;
			}
			this.bsBuff |= v << 32 - this.bsLive - n;
			this.bsLive += n;
		}

		// Token: 0x0600085D RID: 2141 RVA: 0x00036660 File Offset: 0x00034860
		private void BsPutUChar(int c)
		{
			this.BsW(8, c);
		}

		// Token: 0x0600085E RID: 2142 RVA: 0x0003666C File Offset: 0x0003486C
		private void BsPutint(int u)
		{
			this.BsW(8, u >> 24 & 255);
			this.BsW(8, u >> 16 & 255);
			this.BsW(8, u >> 8 & 255);
			this.BsW(8, u & 255);
		}

		// Token: 0x0600085F RID: 2143 RVA: 0x000366C4 File Offset: 0x000348C4
		private void BsPutIntVS(int numBits, int c)
		{
			this.BsW(numBits, c);
		}

		// Token: 0x06000860 RID: 2144 RVA: 0x000366D0 File Offset: 0x000348D0
		private void SendMTFValues()
		{
			char[][] array = new char[6][];
			for (int i = 0; i < 6; i++)
			{
				array[i] = new char[258];
			}
			int num = 0;
			int num2 = this.nInUse + 2;
			for (int j = 0; j < 6; j++)
			{
				for (int k = 0; k < num2; k++)
				{
					array[j][k] = '\u000f';
				}
			}
			bool flag = this.nMTF <= 0;
			if (flag)
			{
				BZip2OutputStream.Panic();
			}
			bool flag2 = this.nMTF < 200;
			int num3;
			if (flag2)
			{
				num3 = 2;
			}
			else
			{
				bool flag3 = this.nMTF < 600;
				if (flag3)
				{
					num3 = 3;
				}
				else
				{
					bool flag4 = this.nMTF < 1200;
					if (flag4)
					{
						num3 = 4;
					}
					else
					{
						bool flag5 = this.nMTF < 2400;
						if (flag5)
						{
							num3 = 5;
						}
						else
						{
							num3 = 6;
						}
					}
				}
			}
			int l = num3;
			int num4 = this.nMTF;
			int num5 = 0;
			while (l > 0)
			{
				int num6 = num4 / l;
				int num7 = 0;
				int num8 = num5 - 1;
				while (num7 < num6 && num8 < num2 - 1)
				{
					num8++;
					num7 += this.mtfFreq[num8];
				}
				bool flag6 = num8 > num5 && l != num3 && l != 1 && (num3 - l) % 2 == 1;
				if (flag6)
				{
					num7 -= this.mtfFreq[num8];
					num8--;
				}
				for (int m = 0; m < num2; m++)
				{
					bool flag7 = m >= num5 && m <= num8;
					if (flag7)
					{
						array[l - 1][m] = '\0';
					}
					else
					{
						array[l - 1][m] = '\u000f';
					}
				}
				l--;
				num5 = num8 + 1;
				num4 -= num7;
			}
			int[][] array2 = new int[6][];
			for (int n = 0; n < 6; n++)
			{
				array2[n] = new int[258];
			}
			int[] array3 = new int[6];
			short[] array4 = new short[6];
			for (int num9 = 0; num9 < 4; num9++)
			{
				for (int num10 = 0; num10 < num3; num10++)
				{
					array3[num10] = 0;
				}
				for (int num11 = 0; num11 < num3; num11++)
				{
					for (int num12 = 0; num12 < num2; num12++)
					{
						array2[num11][num12] = 0;
					}
				}
				num = 0;
				int num13 = 0;
				num5 = 0;
				for (;;)
				{
					bool flag8 = num5 >= this.nMTF;
					if (flag8)
					{
						break;
					}
					int num8 = num5 + 50 - 1;
					bool flag9 = num8 >= this.nMTF;
					if (flag9)
					{
						num8 = this.nMTF - 1;
					}
					for (int num14 = 0; num14 < num3; num14++)
					{
						array4[num14] = 0;
					}
					bool flag10 = num3 == 6;
					if (flag10)
					{
						short num20;
						short num19;
						short num18;
						short num17;
						short num16;
						short num15 = num16 = (num17 = (num18 = (num19 = (num20 = 0))));
						for (int num21 = num5; num21 <= num8; num21++)
						{
							short num22 = this.szptr[num21];
							num16 += (short)array[0][(int)num22];
							num15 += (short)array[1][(int)num22];
							num17 += (short)array[2][(int)num22];
							num18 += (short)array[3][(int)num22];
							num19 += (short)array[4][(int)num22];
							num20 += (short)array[5][(int)num22];
						}
						array4[0] = num16;
						array4[1] = num15;
						array4[2] = num17;
						array4[3] = num18;
						array4[4] = num19;
						array4[5] = num20;
					}
					else
					{
						for (int num23 = num5; num23 <= num8; num23++)
						{
							short num24 = this.szptr[num23];
							for (int num25 = 0; num25 < num3; num25++)
							{
								short[] array5 = array4;
								int num26 = num25;
								array5[num26] += (short)array[num25][(int)num24];
							}
						}
					}
					int num27 = 999999999;
					int num28 = -1;
					for (int num29 = 0; num29 < num3; num29++)
					{
						bool flag11 = (int)array4[num29] < num27;
						if (flag11)
						{
							num27 = (int)array4[num29];
							num28 = num29;
						}
					}
					num13 += num27;
					array3[num28]++;
					this.selector[num] = (char)num28;
					num++;
					for (int num30 = num5; num30 <= num8; num30++)
					{
						array2[num28][(int)this.szptr[num30]]++;
					}
					num5 = num8 + 1;
				}
				for (int num31 = 0; num31 < num3; num31++)
				{
					BZip2OutputStream.HbMakeCodeLengths(array[num31], array2[num31], num2, 20);
				}
			}
			bool flag12 = num3 >= 8;
			if (flag12)
			{
				BZip2OutputStream.Panic();
			}
			bool flag13 = num >= 32768 || num > 18002;
			if (flag13)
			{
				BZip2OutputStream.Panic();
			}
			char[] array6 = new char[6];
			for (int num32 = 0; num32 < num3; num32++)
			{
				array6[num32] = (char)num32;
			}
			for (int num33 = 0; num33 < num; num33++)
			{
				char c = this.selector[num33];
				int num34 = 0;
				char c2 = array6[num34];
				while (c != c2)
				{
					num34++;
					char c3 = c2;
					c2 = array6[num34];
					array6[num34] = c3;
				}
				array6[0] = c2;
				this.selectorMtf[num33] = (char)num34;
			}
			int[][] array7 = new int[6][];
			for (int num35 = 0; num35 < 6; num35++)
			{
				array7[num35] = new int[258];
			}
			for (int num36 = 0; num36 < num3; num36++)
			{
				int num37 = 32;
				int num38 = 0;
				for (int num39 = 0; num39 < num2; num39++)
				{
					bool flag14 = (int)array[num36][num39] > num38;
					if (flag14)
					{
						num38 = (int)array[num36][num39];
					}
					bool flag15 = (int)array[num36][num39] < num37;
					if (flag15)
					{
						num37 = (int)array[num36][num39];
					}
				}
				bool flag16 = num38 > 20;
				if (flag16)
				{
					BZip2OutputStream.Panic();
				}
				bool flag17 = num37 < 1;
				if (flag17)
				{
					BZip2OutputStream.Panic();
				}
				BZip2OutputStream.HbAssignCodes(array7[num36], array[num36], num37, num38, num2);
			}
			bool[] array8 = new bool[16];
			for (int num40 = 0; num40 < 16; num40++)
			{
				array8[num40] = false;
				for (int num41 = 0; num41 < 16; num41++)
				{
					bool flag18 = this.inUse[num40 * 16 + num41];
					if (flag18)
					{
						array8[num40] = true;
					}
				}
			}
			for (int num42 = 0; num42 < 16; num42++)
			{
				bool flag19 = array8[num42];
				if (flag19)
				{
					this.BsW(1, 1);
				}
				else
				{
					this.BsW(1, 0);
				}
			}
			for (int num43 = 0; num43 < 16; num43++)
			{
				bool flag20 = array8[num43];
				if (flag20)
				{
					for (int num44 = 0; num44 < 16; num44++)
					{
						bool flag21 = this.inUse[num43 * 16 + num44];
						if (flag21)
						{
							this.BsW(1, 1);
						}
						else
						{
							this.BsW(1, 0);
						}
					}
				}
			}
			this.BsW(3, num3);
			this.BsW(15, num);
			for (int num45 = 0; num45 < num; num45++)
			{
				for (int num46 = 0; num46 < (int)this.selectorMtf[num45]; num46++)
				{
					this.BsW(1, 1);
				}
				this.BsW(1, 0);
			}
			for (int num47 = 0; num47 < num3; num47++)
			{
				int num48 = (int)array[num47][0];
				this.BsW(5, num48);
				for (int num49 = 0; num49 < num2; num49++)
				{
					while (num48 < (int)array[num47][num49])
					{
						this.BsW(2, 2);
						num48++;
					}
					while (num48 > (int)array[num47][num49])
					{
						this.BsW(2, 3);
						num48--;
					}
					this.BsW(1, 0);
				}
			}
			int num50 = 0;
			num5 = 0;
			for (;;)
			{
				bool flag22 = num5 >= this.nMTF;
				if (flag22)
				{
					break;
				}
				int num8 = num5 + 50 - 1;
				bool flag23 = num8 >= this.nMTF;
				if (flag23)
				{
					num8 = this.nMTF - 1;
				}
				for (int num51 = num5; num51 <= num8; num51++)
				{
					this.BsW((int)array[(int)this.selector[num50]][(int)this.szptr[num51]], array7[(int)this.selector[num50]][(int)this.szptr[num51]]);
				}
				num5 = num8 + 1;
				num50++;
			}
			bool flag24 = num50 != num;
			if (flag24)
			{
				BZip2OutputStream.Panic();
			}
		}

		// Token: 0x06000861 RID: 2145 RVA: 0x000371AC File Offset: 0x000353AC
		private void MoveToFrontCodeAndSend()
		{
			this.BsPutIntVS(24, this.origPtr);
			this.GenerateMTFValues();
			this.SendMTFValues();
		}

		// Token: 0x06000862 RID: 2146 RVA: 0x000371CC File Offset: 0x000353CC
		private void SimpleSort(int lo, int hi, int d)
		{
			int num = hi - lo + 1;
			bool flag = num < 2;
			if (!flag)
			{
				int i = 0;
				while (this.increments[i] < num)
				{
					i++;
				}
				i--;
				while (i >= 0)
				{
					int num2 = this.increments[i];
					int num3 = lo + num2;
					for (;;)
					{
						bool flag2 = num3 > hi;
						if (flag2)
						{
							break;
						}
						int num4 = this.zptr[num3];
						int num5 = num3;
						while (this.FullGtU(this.zptr[num5 - num2] + d, num4 + d))
						{
							this.zptr[num5] = this.zptr[num5 - num2];
							num5 -= num2;
							bool flag3 = num5 <= lo + num2 - 1;
							if (flag3)
							{
								break;
							}
						}
						this.zptr[num5] = num4;
						num3++;
						bool flag4 = num3 > hi;
						if (flag4)
						{
							break;
						}
						num4 = this.zptr[num3];
						num5 = num3;
						while (this.FullGtU(this.zptr[num5 - num2] + d, num4 + d))
						{
							this.zptr[num5] = this.zptr[num5 - num2];
							num5 -= num2;
							bool flag5 = num5 <= lo + num2 - 1;
							if (flag5)
							{
								break;
							}
						}
						this.zptr[num5] = num4;
						num3++;
						bool flag6 = num3 > hi;
						if (flag6)
						{
							break;
						}
						num4 = this.zptr[num3];
						num5 = num3;
						while (this.FullGtU(this.zptr[num5 - num2] + d, num4 + d))
						{
							this.zptr[num5] = this.zptr[num5 - num2];
							num5 -= num2;
							bool flag7 = num5 <= lo + num2 - 1;
							if (flag7)
							{
								break;
							}
						}
						this.zptr[num5] = num4;
						num3++;
						bool flag8 = this.workDone > this.workLimit && this.firstAttempt;
						if (flag8)
						{
							goto Block_10;
						}
					}
					IL_23A:
					i--;
					continue;
					goto IL_23A;
					Block_10:
					break;
					goto IL_23A;
				}
			}
		}

		// Token: 0x06000863 RID: 2147 RVA: 0x0003742C File Offset: 0x0003562C
		private void Vswap(int p1, int p2, int n)
		{
			while (n > 0)
			{
				int num = this.zptr[p1];
				this.zptr[p1] = this.zptr[p2];
				this.zptr[p2] = num;
				p1++;
				p2++;
				n--;
			}
		}

		// Token: 0x06000864 RID: 2148 RVA: 0x00037480 File Offset: 0x00035680
		private void QSort3(int loSt, int hiSt, int dSt)
		{
			BZip2OutputStream.StackElement[] array = new BZip2OutputStream.StackElement[1000];
			int i = 0;
			array[i].ll = loSt;
			array[i].hh = hiSt;
			array[i].dd = dSt;
			i++;
			while (i > 0)
			{
				bool flag = i >= 1000;
				if (flag)
				{
					BZip2OutputStream.Panic();
				}
				i--;
				int ll = array[i].ll;
				int hh = array[i].hh;
				int dd = array[i].dd;
				bool flag2 = hh - ll < 20 || dd > 10;
				if (flag2)
				{
					this.SimpleSort(ll, hh, dd);
					bool flag3 = this.workDone > this.workLimit && this.firstAttempt;
					if (flag3)
					{
						break;
					}
				}
				else
				{
					int num = (int)BZip2OutputStream.Med3(this.block[this.zptr[ll] + dd + 1], this.block[this.zptr[hh] + dd + 1], this.block[this.zptr[ll + hh >> 1] + dd + 1]);
					int num3;
					int num2 = num3 = ll;
					int num5;
					int num4 = num5 = hh;
					for (;;)
					{
						for (;;)
						{
							bool flag4 = num3 > num5;
							if (flag4)
							{
								break;
							}
							int num6 = (int)this.block[this.zptr[num3] + dd + 1] - num;
							bool flag5 = num6 == 0;
							if (flag5)
							{
								int num7 = this.zptr[num3];
								this.zptr[num3] = this.zptr[num2];
								this.zptr[num2] = num7;
								num2++;
								num3++;
							}
							else
							{
								bool flag6 = num6 > 0;
								if (flag6)
								{
									break;
								}
								num3++;
							}
						}
						IL_1E0:
						for (;;)
						{
							bool flag7 = num3 > num5;
							if (flag7)
							{
								break;
							}
							int num6 = (int)this.block[this.zptr[num5] + dd + 1] - num;
							bool flag8 = num6 == 0;
							if (flag8)
							{
								int num8 = this.zptr[num5];
								this.zptr[num5] = this.zptr[num4];
								this.zptr[num4] = num8;
								num4--;
								num5--;
							}
							else
							{
								bool flag9 = num6 < 0;
								if (flag9)
								{
									break;
								}
								num5--;
							}
						}
						IL_280:
						bool flag10 = num3 > num5;
						if (flag10)
						{
							break;
						}
						int num9 = this.zptr[num3];
						this.zptr[num3] = this.zptr[num5];
						this.zptr[num5] = num9;
						num3++;
						num5--;
						continue;
						goto IL_280;
						goto IL_1E0;
					}
					bool flag11 = num4 < num2;
					if (flag11)
					{
						array[i].ll = ll;
						array[i].hh = hh;
						array[i].dd = dd + 1;
						i++;
					}
					else
					{
						int num6 = (num2 - ll < num3 - num2) ? (num2 - ll) : (num3 - num2);
						this.Vswap(ll, num3 - num6, num6);
						int num10 = (hh - num4 < num4 - num5) ? (hh - num4) : (num4 - num5);
						this.Vswap(num3, hh - num10 + 1, num10);
						num6 = ll + num3 - num2 - 1;
						num10 = hh - (num4 - num5) + 1;
						array[i].ll = ll;
						array[i].hh = num6;
						array[i].dd = dd;
						i++;
						array[i].ll = num6 + 1;
						array[i].hh = num10 - 1;
						array[i].dd = dd + 1;
						i++;
						array[i].ll = num10;
						array[i].hh = hh;
						array[i].dd = dd;
						i++;
					}
				}
			}
		}

		// Token: 0x06000865 RID: 2149 RVA: 0x000378BC File Offset: 0x00035ABC
		private void MainSort()
		{
			int[] array = new int[256];
			int[] array2 = new int[256];
			bool[] array3 = new bool[256];
			for (int i = 0; i < 20; i++)
			{
				this.block[this.last + i + 2] = this.block[i % (this.last + 1) + 1];
			}
			for (int i = 0; i <= this.last + 20; i++)
			{
				this.quadrant[i] = 0;
			}
			this.block[0] = this.block[this.last + 1];
			bool flag = this.last < 4000;
			if (flag)
			{
				for (int i = 0; i <= this.last; i++)
				{
					this.zptr[i] = i;
				}
				this.firstAttempt = false;
				this.workDone = (this.workLimit = 0);
				this.SimpleSort(0, this.last, 0);
			}
			else
			{
				int num = 0;
				for (int i = 0; i <= 255; i++)
				{
					array3[i] = false;
				}
				for (int i = 0; i <= 65536; i++)
				{
					this.ftab[i] = 0;
				}
				int num2 = (int)this.block[0];
				for (int i = 0; i <= this.last; i++)
				{
					int num3 = (int)this.block[i + 1];
					this.ftab[(num2 << 8) + num3]++;
					num2 = num3;
				}
				for (int i = 1; i <= 65536; i++)
				{
					this.ftab[i] += this.ftab[i - 1];
				}
				num2 = (int)this.block[1];
				int j;
				for (int i = 0; i < this.last; i++)
				{
					int num3 = (int)this.block[i + 2];
					j = (num2 << 8) + num3;
					num2 = num3;
					this.ftab[j]--;
					this.zptr[this.ftab[j]] = i;
				}
				j = ((int)this.block[this.last + 1] << 8) + (int)this.block[1];
				this.ftab[j]--;
				this.zptr[this.ftab[j]] = this.last;
				for (int i = 0; i <= 255; i++)
				{
					array[i] = i;
				}
				int num4 = 1;
				do
				{
					num4 = 3 * num4 + 1;
				}
				while (num4 <= 256);
				do
				{
					num4 /= 3;
					for (int i = num4; i <= 255; i++)
					{
						int num5 = array[i];
						j = i;
						while (this.ftab[array[j - num4] + 1 << 8] - this.ftab[array[j - num4] << 8] > this.ftab[num5 + 1 << 8] - this.ftab[num5 << 8])
						{
							array[j] = array[j - num4];
							j -= num4;
							bool flag2 = j <= num4 - 1;
							if (flag2)
							{
								break;
							}
						}
						array[j] = num5;
					}
				}
				while (num4 != 1);
				for (int i = 0; i <= 255; i++)
				{
					int num6 = array[i];
					for (j = 0; j <= 255; j++)
					{
						int num7 = (num6 << 8) + j;
						bool flag3 = (this.ftab[num7] & 2097152) != 2097152;
						if (flag3)
						{
							int num8 = this.ftab[num7] & -2097153;
							int num9 = (this.ftab[num7 + 1] & -2097153) - 1;
							bool flag4 = num9 > num8;
							if (flag4)
							{
								this.QSort3(num8, num9, 2);
								num += num9 - num8 + 1;
								bool flag5 = this.workDone > this.workLimit && this.firstAttempt;
								if (flag5)
								{
									return;
								}
							}
							this.ftab[num7] |= 2097152;
						}
					}
					array3[num6] = true;
					bool flag6 = i < 255;
					if (flag6)
					{
						int num10 = this.ftab[num6 << 8] & -2097153;
						int num11 = (this.ftab[num6 + 1 << 8] & -2097153) - num10;
						int num12 = 0;
						while (num11 >> num12 > 65534)
						{
							num12++;
						}
						for (j = 0; j < num11; j++)
						{
							int num13 = this.zptr[num10 + j];
							int num14 = j >> num12;
							this.quadrant[num13] = num14;
							bool flag7 = num13 < 20;
							if (flag7)
							{
								this.quadrant[num13 + this.last + 1] = num14;
							}
						}
						bool flag8 = num11 - 1 >> num12 > 65535;
						if (flag8)
						{
							BZip2OutputStream.Panic();
						}
					}
					for (j = 0; j <= 255; j++)
					{
						array2[j] = (this.ftab[(j << 8) + num6] & -2097153);
					}
					for (j = (this.ftab[num6 << 8] & -2097153); j < (this.ftab[num6 + 1 << 8] & -2097153); j++)
					{
						num2 = (int)this.block[this.zptr[j]];
						bool flag9 = !array3[num2];
						if (flag9)
						{
							this.zptr[array2[num2]] = ((this.zptr[j] == 0) ? this.last : (this.zptr[j] - 1));
							array2[num2]++;
						}
					}
					for (j = 0; j <= 255; j++)
					{
						this.ftab[(j << 8) + num6] |= 2097152;
					}
				}
			}
		}

		// Token: 0x06000866 RID: 2150 RVA: 0x00037F68 File Offset: 0x00036168
		private void RandomiseBlock()
		{
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < 256; i++)
			{
				this.inUse[i] = false;
			}
			for (int i = 0; i <= this.last; i++)
			{
				bool flag = num == 0;
				if (flag)
				{
					num = BZip2Constants.RandomNumbers[num2];
					num2++;
					bool flag2 = num2 == 512;
					if (flag2)
					{
						num2 = 0;
					}
				}
				num--;
				byte[] array = this.block;
				int num3 = i + 1;
				array[num3] ^= ((num == 1) ? 1 : 0);
				byte[] array2 = this.block;
				int num4 = i + 1;
				array2[num4] &= byte.MaxValue;
				this.inUse[(int)this.block[i + 1]] = true;
			}
		}

		// Token: 0x06000867 RID: 2151 RVA: 0x00038044 File Offset: 0x00036244
		private void DoReversibleTransformation()
		{
			this.workLimit = this.workFactor * this.last;
			this.workDone = 0;
			this.blockRandomised = false;
			this.firstAttempt = true;
			this.MainSort();
			bool flag = this.workDone > this.workLimit && this.firstAttempt;
			if (flag)
			{
				this.RandomiseBlock();
				this.workLimit = (this.workDone = 0);
				this.blockRandomised = true;
				this.firstAttempt = false;
				this.MainSort();
			}
			this.origPtr = -1;
			for (int i = 0; i <= this.last; i++)
			{
				bool flag2 = this.zptr[i] == 0;
				if (flag2)
				{
					this.origPtr = i;
					break;
				}
			}
			bool flag3 = this.origPtr == -1;
			if (flag3)
			{
				BZip2OutputStream.Panic();
			}
		}

		// Token: 0x06000868 RID: 2152 RVA: 0x00038138 File Offset: 0x00036338
		private bool FullGtU(int i1, int i2)
		{
			byte b = this.block[i1 + 1];
			byte b2 = this.block[i2 + 1];
			bool flag = b != b2;
			bool result;
			if (flag)
			{
				result = (b > b2);
			}
			else
			{
				i1++;
				i2++;
				b = this.block[i1 + 1];
				b2 = this.block[i2 + 1];
				bool flag2 = b != b2;
				if (flag2)
				{
					result = (b > b2);
				}
				else
				{
					i1++;
					i2++;
					b = this.block[i1 + 1];
					b2 = this.block[i2 + 1];
					bool flag3 = b != b2;
					if (flag3)
					{
						result = (b > b2);
					}
					else
					{
						i1++;
						i2++;
						b = this.block[i1 + 1];
						b2 = this.block[i2 + 1];
						bool flag4 = b != b2;
						if (flag4)
						{
							result = (b > b2);
						}
						else
						{
							i1++;
							i2++;
							b = this.block[i1 + 1];
							b2 = this.block[i2 + 1];
							bool flag5 = b != b2;
							if (flag5)
							{
								result = (b > b2);
							}
							else
							{
								i1++;
								i2++;
								b = this.block[i1 + 1];
								b2 = this.block[i2 + 1];
								bool flag6 = b != b2;
								if (flag6)
								{
									result = (b > b2);
								}
								else
								{
									i1++;
									i2++;
									int num = this.last + 1;
									int num2;
									int num3;
									for (;;)
									{
										b = this.block[i1 + 1];
										b2 = this.block[i2 + 1];
										bool flag7 = b != b2;
										if (flag7)
										{
											break;
										}
										num2 = this.quadrant[i1];
										num3 = this.quadrant[i2];
										bool flag8 = num2 != num3;
										if (flag8)
										{
											goto Block_8;
										}
										i1++;
										i2++;
										b = this.block[i1 + 1];
										b2 = this.block[i2 + 1];
										bool flag9 = b != b2;
										if (flag9)
										{
											goto Block_9;
										}
										num2 = this.quadrant[i1];
										num3 = this.quadrant[i2];
										bool flag10 = num2 != num3;
										if (flag10)
										{
											goto Block_10;
										}
										i1++;
										i2++;
										b = this.block[i1 + 1];
										b2 = this.block[i2 + 1];
										bool flag11 = b != b2;
										if (flag11)
										{
											goto Block_11;
										}
										num2 = this.quadrant[i1];
										num3 = this.quadrant[i2];
										bool flag12 = num2 != num3;
										if (flag12)
										{
											goto Block_12;
										}
										i1++;
										i2++;
										b = this.block[i1 + 1];
										b2 = this.block[i2 + 1];
										bool flag13 = b != b2;
										if (flag13)
										{
											goto Block_13;
										}
										num2 = this.quadrant[i1];
										num3 = this.quadrant[i2];
										bool flag14 = num2 != num3;
										if (flag14)
										{
											goto Block_14;
										}
										i1++;
										i2++;
										bool flag15 = i1 > this.last;
										if (flag15)
										{
											i1 -= this.last;
											i1--;
										}
										bool flag16 = i2 > this.last;
										if (flag16)
										{
											i2 -= this.last;
											i2--;
										}
										num -= 4;
										this.workDone++;
										if (num < 0)
										{
											goto Block_17;
										}
									}
									return b > b2;
									Block_8:
									return num2 > num3;
									Block_9:
									return b > b2;
									Block_10:
									return num2 > num3;
									Block_11:
									return b > b2;
									Block_12:
									return num2 > num3;
									Block_13:
									return b > b2;
									Block_14:
									return num2 > num3;
									Block_17:
									result = false;
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000869 RID: 2153 RVA: 0x000384E4 File Offset: 0x000366E4
		private void AllocateCompressStructures()
		{
			int num = 100000 * this.blockSize100k;
			this.block = new byte[num + 1 + 20];
			this.quadrant = new int[num + 20];
			this.zptr = new int[num];
			this.ftab = new int[65537];
			bool flag = this.block == null || this.quadrant == null || this.zptr == null || this.ftab == null;
			if (flag)
			{
			}
			this.szptr = new short[2 * num];
		}

		// Token: 0x0600086A RID: 2154 RVA: 0x00038588 File Offset: 0x00036788
		private void GenerateMTFValues()
		{
			char[] array = new char[256];
			this.MakeMaps();
			int num = this.nInUse + 1;
			for (int i = 0; i <= num; i++)
			{
				this.mtfFreq[i] = 0;
			}
			int num2 = 0;
			int num3 = 0;
			for (int i = 0; i < this.nInUse; i++)
			{
				array[i] = (char)i;
			}
			for (int i = 0; i <= this.last; i++)
			{
				char c = this.unseqToSeq[(int)this.block[this.zptr[i]]];
				int num4 = 0;
				char c2 = array[num4];
				while (c != c2)
				{
					num4++;
					char c3 = c2;
					c2 = array[num4];
					array[num4] = c3;
				}
				array[0] = c2;
				bool flag = num4 == 0;
				if (flag)
				{
					num3++;
				}
				else
				{
					bool flag2 = num3 > 0;
					if (flag2)
					{
						num3--;
						for (;;)
						{
							int num5 = num3 % 2;
							int num6 = num5;
							if (num6 != 0)
							{
								if (num6 == 1)
								{
									this.szptr[num2] = 1;
									num2++;
									this.mtfFreq[1]++;
								}
							}
							else
							{
								this.szptr[num2] = 0;
								num2++;
								this.mtfFreq[0]++;
							}
							bool flag3 = num3 < 2;
							if (flag3)
							{
								break;
							}
							num3 = (num3 - 2) / 2;
						}
						num3 = 0;
					}
					this.szptr[num2] = (short)(num4 + 1);
					num2++;
					this.mtfFreq[num4 + 1]++;
				}
			}
			bool flag4 = num3 > 0;
			if (flag4)
			{
				num3--;
				for (;;)
				{
					int num7 = num3 % 2;
					int num8 = num7;
					if (num8 != 0)
					{
						if (num8 == 1)
						{
							this.szptr[num2] = 1;
							num2++;
							this.mtfFreq[1]++;
						}
					}
					else
					{
						this.szptr[num2] = 0;
						num2++;
						this.mtfFreq[0]++;
					}
					bool flag5 = num3 < 2;
					if (flag5)
					{
						break;
					}
					num3 = (num3 - 2) / 2;
				}
			}
			this.szptr[num2] = (short)num;
			num2++;
			this.mtfFreq[num]++;
			this.nMTF = num2;
		}

		// Token: 0x0600086B RID: 2155 RVA: 0x00038844 File Offset: 0x00036A44
		private static void Panic()
		{
			throw new BZip2Exception("BZip2 output stream panic");
		}

		// Token: 0x0600086C RID: 2156 RVA: 0x00038854 File Offset: 0x00036A54
		private static void HbMakeCodeLengths(char[] len, int[] freq, int alphaSize, int maxLen)
		{
			int[] array = new int[260];
			int[] array2 = new int[516];
			int[] array3 = new int[516];
			for (int i = 0; i < alphaSize; i++)
			{
				array2[i + 1] = ((freq[i] == 0) ? 1 : freq[i]) << 8;
			}
			for (;;)
			{
				int num = alphaSize;
				int j = 0;
				array[0] = 0;
				array2[0] = 0;
				array3[0] = -2;
				for (int k = 1; k <= alphaSize; k++)
				{
					array3[k] = -1;
					j++;
					array[j] = k;
					int num2 = j;
					int num3 = array[num2];
					while (array2[num3] < array2[array[num2 >> 1]])
					{
						array[num2] = array[num2 >> 1];
						num2 >>= 1;
					}
					array[num2] = num3;
				}
				bool flag = j >= 260;
				if (flag)
				{
					BZip2OutputStream.Panic();
				}
				while (j > 1)
				{
					int num4 = array[1];
					array[1] = array[j];
					j--;
					int num5 = 1;
					int num6 = array[num5];
					for (;;)
					{
						int num7 = num5 << 1;
						bool flag2 = num7 > j;
						if (flag2)
						{
							break;
						}
						bool flag3 = num7 < j && array2[array[num7 + 1]] < array2[array[num7]];
						if (flag3)
						{
							num7++;
						}
						bool flag4 = array2[num6] < array2[array[num7]];
						if (flag4)
						{
							break;
						}
						array[num5] = array[num7];
						num5 = num7;
					}
					IL_193:
					array[num5] = num6;
					int num8 = array[1];
					array[1] = array[j];
					j--;
					num5 = 1;
					num6 = array[num5];
					for (;;)
					{
						int num7 = num5 << 1;
						bool flag5 = num7 > j;
						if (flag5)
						{
							break;
						}
						bool flag6 = num7 < j && array2[array[num7 + 1]] < array2[array[num7]];
						if (flag6)
						{
							num7++;
						}
						bool flag7 = array2[num6] < array2[array[num7]];
						if (flag7)
						{
							break;
						}
						array[num5] = array[num7];
						num5 = num7;
					}
					IL_235:
					array[num5] = num6;
					num++;
					array3[num4] = (array3[num8] = num);
					array2[num] = ((int)(((long)array2[num4] & (long)((ulong)-256)) + ((long)array2[num8] & (long)((ulong)-256))) | 1 + (((array2[num4] & 255) > (array2[num8] & 255)) ? (array2[num4] & 255) : (array2[num8] & 255)));
					array3[num] = -1;
					j++;
					array[j] = num;
					num5 = j;
					num6 = array[num5];
					while (array2[num6] < array2[array[num5 >> 1]])
					{
						array[num5] = array[num5 >> 1];
						num5 >>= 1;
					}
					array[num5] = num6;
					continue;
					goto IL_235;
					goto IL_193;
				}
				bool flag8 = num >= 516;
				if (flag8)
				{
					BZip2OutputStream.Panic();
				}
				bool flag9 = false;
				for (int l = 1; l <= alphaSize; l++)
				{
					int num9 = 0;
					int num10 = l;
					while (array3[num10] >= 0)
					{
						num10 = array3[num10];
						num9++;
					}
					len[l - 1] = (char)num9;
					bool flag10 = num9 > maxLen;
					if (flag10)
					{
						flag9 = true;
					}
				}
				bool flag11 = !flag9;
				if (flag11)
				{
					break;
				}
				for (int m = 1; m < alphaSize; m++)
				{
					int num9 = array2[m] >> 8;
					num9 = 1 + num9 / 2;
					array2[m] = num9 << 8;
				}
			}
		}

		// Token: 0x0600086D RID: 2157 RVA: 0x00038C38 File Offset: 0x00036E38
		private static void HbAssignCodes(int[] code, char[] length, int minLen, int maxLen, int alphaSize)
		{
			int num = 0;
			for (int i = minLen; i <= maxLen; i++)
			{
				for (int j = 0; j < alphaSize; j++)
				{
					bool flag = (int)length[j] == i;
					if (flag)
					{
						code[j] = num;
						num++;
					}
				}
				num <<= 1;
			}
		}

		// Token: 0x0600086E RID: 2158 RVA: 0x00038C9C File Offset: 0x00036E9C
		private static byte Med3(byte a, byte b, byte c)
		{
			bool flag = a > b;
			if (flag)
			{
				byte b2 = a;
				a = b;
				b = b2;
			}
			bool flag2 = b > c;
			if (flag2)
			{
				byte b2 = b;
				b = c;
				c = b2;
			}
			bool flag3 = a > b;
			if (flag3)
			{
				b = a;
			}
			return b;
		}

		// Token: 0x04000504 RID: 1284
		private const int SETMASK = 2097152;

		// Token: 0x04000505 RID: 1285
		private const int CLEARMASK = -2097153;

		// Token: 0x04000506 RID: 1286
		private const int GREATER_ICOST = 15;

		// Token: 0x04000507 RID: 1287
		private const int LESSER_ICOST = 0;

		// Token: 0x04000508 RID: 1288
		private const int SMALL_THRESH = 20;

		// Token: 0x04000509 RID: 1289
		private const int DEPTH_THRESH = 10;

		// Token: 0x0400050A RID: 1290
		private const int QSORT_STACK_SIZE = 1000;

		// Token: 0x0400050B RID: 1291
		private readonly int[] increments = new int[]
		{
			1,
			4,
			13,
			40,
			121,
			364,
			1093,
			3280,
			9841,
			29524,
			88573,
			265720,
			797161,
			2391484
		};

		// Token: 0x0400050C RID: 1292
		private bool isStreamOwner = true;

		// Token: 0x0400050D RID: 1293
		private int last;

		// Token: 0x0400050E RID: 1294
		private int origPtr;

		// Token: 0x0400050F RID: 1295
		private int blockSize100k;

		// Token: 0x04000510 RID: 1296
		private bool blockRandomised;

		// Token: 0x04000511 RID: 1297
		private int bytesOut;

		// Token: 0x04000512 RID: 1298
		private int bsBuff;

		// Token: 0x04000513 RID: 1299
		private int bsLive;

		// Token: 0x04000514 RID: 1300
		private UWA.ICSharpCode.SharpZipLib.Checksums.IChecksum mCrc = new UWA.ICSharpCode.SharpZipLib.Checksums.StrangeCRC();

		// Token: 0x04000515 RID: 1301
		private bool[] inUse = new bool[256];

		// Token: 0x04000516 RID: 1302
		private int nInUse;

		// Token: 0x04000517 RID: 1303
		private char[] seqToUnseq = new char[256];

		// Token: 0x04000518 RID: 1304
		private char[] unseqToSeq = new char[256];

		// Token: 0x04000519 RID: 1305
		private char[] selector = new char[18002];

		// Token: 0x0400051A RID: 1306
		private char[] selectorMtf = new char[18002];

		// Token: 0x0400051B RID: 1307
		private byte[] block;

		// Token: 0x0400051C RID: 1308
		private int[] quadrant;

		// Token: 0x0400051D RID: 1309
		private int[] zptr;

		// Token: 0x0400051E RID: 1310
		private short[] szptr;

		// Token: 0x0400051F RID: 1311
		private int[] ftab;

		// Token: 0x04000520 RID: 1312
		private int nMTF;

		// Token: 0x04000521 RID: 1313
		private int[] mtfFreq = new int[258];

		// Token: 0x04000522 RID: 1314
		private int workFactor;

		// Token: 0x04000523 RID: 1315
		private int workDone;

		// Token: 0x04000524 RID: 1316
		private int workLimit;

		// Token: 0x04000525 RID: 1317
		private bool firstAttempt;

		// Token: 0x04000526 RID: 1318
		private int nBlocksRandomised;

		// Token: 0x04000527 RID: 1319
		private int currentChar = -1;

		// Token: 0x04000528 RID: 1320
		private int runLength;

		// Token: 0x04000529 RID: 1321
		private uint blockCRC;

		// Token: 0x0400052A RID: 1322
		private uint combinedCRC;

		// Token: 0x0400052B RID: 1323
		private int allowableBlockSize;

		// Token: 0x0400052C RID: 1324
		private Stream baseStream;

		// Token: 0x0400052D RID: 1325
		private bool disposed_;

		// Token: 0x02000125 RID: 293
		private struct StackElement
		{
			// Token: 0x04000716 RID: 1814
			public int ll;

			// Token: 0x04000717 RID: 1815
			public int hh;

			// Token: 0x04000718 RID: 1816
			public int dd;
		}
	}
}
