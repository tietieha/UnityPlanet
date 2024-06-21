using System;
using System.IO;
using System.Runtime.InteropServices;
using UWA.ICSharpCode.SharpZipLib.Checksums;

namespace UWA.ICSharpCode.SharpZipLib.BZip2
{
	// Token: 0x020000BF RID: 191
	[ComVisible(false)]
	public class BZip2InputStream : Stream
	{
		// Token: 0x06000814 RID: 2068 RVA: 0x00034318 File Offset: 0x00032518
		public BZip2InputStream(Stream stream)
		{
			for (int i = 0; i < 6; i++)
			{
				this.limit[i] = new int[258];
				this.baseArray[i] = new int[258];
				this.perm[i] = new int[258];
			}
			this.BsSetStream(stream);
			this.Initialize();
			this.InitBlock();
			this.SetupBlock();
		}

		// Token: 0x1700015E RID: 350
		// (get) Token: 0x06000815 RID: 2069 RVA: 0x00034458 File Offset: 0x00032658
		// (set) Token: 0x06000816 RID: 2070 RVA: 0x00034478 File Offset: 0x00032678
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

		// Token: 0x1700015F RID: 351
		// (get) Token: 0x06000817 RID: 2071 RVA: 0x00034484 File Offset: 0x00032684
		public override bool CanRead
		{
			get
			{
				return this.baseStream.CanRead;
			}
		}

		// Token: 0x17000160 RID: 352
		// (get) Token: 0x06000818 RID: 2072 RVA: 0x000344A8 File Offset: 0x000326A8
		public override bool CanSeek
		{
			get
			{
				return this.baseStream.CanSeek;
			}
		}

		// Token: 0x17000161 RID: 353
		// (get) Token: 0x06000819 RID: 2073 RVA: 0x000344CC File Offset: 0x000326CC
		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000162 RID: 354
		// (get) Token: 0x0600081A RID: 2074 RVA: 0x000344E8 File Offset: 0x000326E8
		public override long Length
		{
			get
			{
				return this.baseStream.Length;
			}
		}

		// Token: 0x17000163 RID: 355
		// (get) Token: 0x0600081B RID: 2075 RVA: 0x0003450C File Offset: 0x0003270C
		// (set) Token: 0x0600081C RID: 2076 RVA: 0x00034530 File Offset: 0x00032730
		public override long Position
		{
			get
			{
				return this.baseStream.Position;
			}
			set
			{
				throw new NotSupportedException("BZip2InputStream position cannot be set");
			}
		}

		// Token: 0x0600081D RID: 2077 RVA: 0x00034540 File Offset: 0x00032740
		public override void Flush()
		{
			bool flag = this.baseStream != null;
			if (flag)
			{
				this.baseStream.Flush();
			}
		}

		// Token: 0x0600081E RID: 2078 RVA: 0x00034570 File Offset: 0x00032770
		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException("BZip2InputStream Seek not supported");
		}

		// Token: 0x0600081F RID: 2079 RVA: 0x00034580 File Offset: 0x00032780
		public override void SetLength(long value)
		{
			throw new NotSupportedException("BZip2InputStream SetLength not supported");
		}

		// Token: 0x06000820 RID: 2080 RVA: 0x00034590 File Offset: 0x00032790
		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException("BZip2InputStream Write not supported");
		}

		// Token: 0x06000821 RID: 2081 RVA: 0x000345A0 File Offset: 0x000327A0
		public override void WriteByte(byte value)
		{
			throw new NotSupportedException("BZip2InputStream WriteByte not supported");
		}

		// Token: 0x06000822 RID: 2082 RVA: 0x000345B0 File Offset: 0x000327B0
		public override int Read(byte[] buffer, int offset, int count)
		{
			bool flag = buffer == null;
			if (flag)
			{
				throw new ArgumentNullException("buffer");
			}
			for (int i = 0; i < count; i++)
			{
				int num = this.ReadByte();
				bool flag2 = num == -1;
				if (flag2)
				{
					return i;
				}
				buffer[offset + i] = (byte)num;
			}
			return count;
		}

		// Token: 0x06000823 RID: 2083 RVA: 0x0003461C File Offset: 0x0003281C
		public override void Close()
		{
			bool flag = this.IsStreamOwner && this.baseStream != null;
			if (flag)
			{
				this.baseStream.Close();
			}
		}

		// Token: 0x06000824 RID: 2084 RVA: 0x00034660 File Offset: 0x00032860
		public override int ReadByte()
		{
			bool flag = this.streamEnd;
			int result;
			if (flag)
			{
				result = -1;
			}
			else
			{
				int num = this.currentChar;
				switch (this.currentState)
				{
				case 3:
					this.SetupRandPartB();
					break;
				case 4:
					this.SetupRandPartC();
					break;
				case 6:
					this.SetupNoRandPartB();
					break;
				case 7:
					this.SetupNoRandPartC();
					break;
				}
				result = num;
			}
			return result;
		}

		// Token: 0x06000825 RID: 2085 RVA: 0x00034704 File Offset: 0x00032904
		private void MakeMaps()
		{
			this.nInUse = 0;
			for (int i = 0; i < 256; i++)
			{
				bool flag = this.inUse[i];
				if (flag)
				{
					this.seqToUnseq[this.nInUse] = (byte)i;
					this.unseqToSeq[i] = (byte)this.nInUse;
					this.nInUse++;
				}
			}
		}

		// Token: 0x06000826 RID: 2086 RVA: 0x00034774 File Offset: 0x00032974
		private void Initialize()
		{
			char c = this.BsGetUChar();
			char c2 = this.BsGetUChar();
			char c3 = this.BsGetUChar();
			char c4 = this.BsGetUChar();
			bool flag = c != 'B' || c2 != 'Z' || c3 != 'h' || c4 < '1' || c4 > '9';
			if (flag)
			{
				this.streamEnd = true;
			}
			else
			{
				this.SetDecompressStructureSizes((int)(c4 - '0'));
				this.computedCombinedCRC = 0U;
			}
		}

		// Token: 0x06000827 RID: 2087 RVA: 0x000347F8 File Offset: 0x000329F8
		private void InitBlock()
		{
			char c = this.BsGetUChar();
			char c2 = this.BsGetUChar();
			char c3 = this.BsGetUChar();
			char c4 = this.BsGetUChar();
			char c5 = this.BsGetUChar();
			char c6 = this.BsGetUChar();
			bool flag = c == '\u0017' && c2 == 'r' && c3 == 'E' && c4 == '8' && c5 == 'P' && c6 == '\u0090';
			if (flag)
			{
				this.Complete();
			}
			else
			{
				bool flag2 = c != '1' || c2 != 'A' || c3 != 'Y' || c4 != '&' || c5 != 'S' || c6 != 'Y';
				if (flag2)
				{
					BZip2InputStream.BadBlockHeader();
					this.streamEnd = true;
				}
				else
				{
					this.storedBlockCRC = this.BsGetInt32();
					this.blockRandomised = (this.BsR(1) == 1);
					this.GetAndMoveToFrontDecode();
					this.mCrc.Reset();
					this.currentState = 1;
				}
			}
		}

		// Token: 0x06000828 RID: 2088 RVA: 0x00034910 File Offset: 0x00032B10
		private void EndBlock()
		{
			this.computedBlockCRC = (int)this.mCrc.Value;
			bool flag = this.storedBlockCRC != this.computedBlockCRC;
			if (flag)
			{
				BZip2InputStream.CrcError();
			}
			this.computedCombinedCRC = ((this.computedCombinedCRC << 1 & uint.MaxValue) | this.computedCombinedCRC >> 31);
			this.computedCombinedCRC ^= (uint)this.computedBlockCRC;
		}

		// Token: 0x06000829 RID: 2089 RVA: 0x00034984 File Offset: 0x00032B84
		private void Complete()
		{
			this.storedCombinedCRC = this.BsGetInt32();
			bool flag = this.storedCombinedCRC != (int)this.computedCombinedCRC;
			if (flag)
			{
				BZip2InputStream.CrcError();
			}
			this.streamEnd = true;
		}

		// Token: 0x0600082A RID: 2090 RVA: 0x000349CC File Offset: 0x00032BCC
		private void BsSetStream(Stream stream)
		{
			this.baseStream = stream;
			this.bsLive = 0;
			this.bsBuff = 0;
		}

		// Token: 0x0600082B RID: 2091 RVA: 0x000349E4 File Offset: 0x00032BE4
		private void FillBuffer()
		{
			int num = 0;
			try
			{
				num = this.baseStream.ReadByte();
			}
			catch (Exception)
			{
				BZip2InputStream.CompressedStreamEOF();
			}
			bool flag = num == -1;
			if (flag)
			{
				BZip2InputStream.CompressedStreamEOF();
			}
			this.bsBuff = (this.bsBuff << 8 | (num & 255));
			this.bsLive += 8;
		}

		// Token: 0x0600082C RID: 2092 RVA: 0x00034A5C File Offset: 0x00032C5C
		private int BsR(int n)
		{
			while (this.bsLive < n)
			{
				this.FillBuffer();
			}
			int result = this.bsBuff >> this.bsLive - n & (1 << n) - 1;
			this.bsLive -= n;
			return result;
		}

		// Token: 0x0600082D RID: 2093 RVA: 0x00034ABC File Offset: 0x00032CBC
		private char BsGetUChar()
		{
			return (char)this.BsR(8);
		}

		// Token: 0x0600082E RID: 2094 RVA: 0x00034AE0 File Offset: 0x00032CE0
		private int BsGetIntVS(int numBits)
		{
			return this.BsR(numBits);
		}

		// Token: 0x0600082F RID: 2095 RVA: 0x00034B00 File Offset: 0x00032D00
		private int BsGetInt32()
		{
			int num = this.BsR(8);
			num = (num << 8 | this.BsR(8));
			num = (num << 8 | this.BsR(8));
			return num << 8 | this.BsR(8);
		}

		// Token: 0x06000830 RID: 2096 RVA: 0x00034B48 File Offset: 0x00032D48
		private void RecvDecodingTables()
		{
			char[][] array = new char[6][];
			for (int i = 0; i < 6; i++)
			{
				array[i] = new char[258];
			}
			bool[] array2 = new bool[16];
			for (int j = 0; j < 16; j++)
			{
				array2[j] = (this.BsR(1) == 1);
			}
			for (int k = 0; k < 16; k++)
			{
				bool flag = array2[k];
				if (flag)
				{
					for (int l = 0; l < 16; l++)
					{
						this.inUse[k * 16 + l] = (this.BsR(1) == 1);
					}
				}
				else
				{
					for (int m = 0; m < 16; m++)
					{
						this.inUse[k * 16 + m] = false;
					}
				}
			}
			this.MakeMaps();
			int num = this.nInUse + 2;
			int num2 = this.BsR(3);
			int num3 = this.BsR(15);
			for (int n = 0; n < num3; n++)
			{
				int num4 = 0;
				while (this.BsR(1) == 1)
				{
					num4++;
				}
				this.selectorMtf[n] = (byte)num4;
			}
			byte[] array3 = new byte[6];
			for (int num5 = 0; num5 < num2; num5++)
			{
				array3[num5] = (byte)num5;
			}
			for (int num6 = 0; num6 < num3; num6++)
			{
				int num7 = (int)this.selectorMtf[num6];
				byte b = array3[num7];
				while (num7 > 0)
				{
					array3[num7] = array3[num7 - 1];
					num7--;
				}
				array3[0] = b;
				this.selector[num6] = b;
			}
			for (int num8 = 0; num8 < num2; num8++)
			{
				int num9 = this.BsR(5);
				for (int num10 = 0; num10 < num; num10++)
				{
					while (this.BsR(1) == 1)
					{
						bool flag2 = this.BsR(1) == 0;
						if (flag2)
						{
							num9++;
						}
						else
						{
							num9--;
						}
					}
					array[num8][num10] = (char)num9;
				}
			}
			for (int num11 = 0; num11 < num2; num11++)
			{
				int num12 = 32;
				int num13 = 0;
				for (int num14 = 0; num14 < num; num14++)
				{
					num13 = Math.Max(num13, (int)array[num11][num14]);
					num12 = Math.Min(num12, (int)array[num11][num14]);
				}
				BZip2InputStream.HbCreateDecodeTables(this.limit[num11], this.baseArray[num11], this.perm[num11], array[num11], num12, num13, num);
				this.minLens[num11] = num12;
			}
		}

		// Token: 0x06000831 RID: 2097 RVA: 0x00034E8C File Offset: 0x0003308C
		private void GetAndMoveToFrontDecode()
		{
			byte[] array = new byte[256];
			int num = 100000 * this.blockSize100k;
			this.origPtr = this.BsGetIntVS(24);
			this.RecvDecodingTables();
			int num2 = this.nInUse + 1;
			int num3 = -1;
			int num4 = 0;
			for (int i = 0; i <= 255; i++)
			{
				this.unzftab[i] = 0;
			}
			for (int j = 0; j <= 255; j++)
			{
				array[j] = (byte)j;
			}
			this.last = -1;
			bool flag = num4 == 0;
			if (flag)
			{
				num3++;
				num4 = 50;
			}
			num4--;
			int num5 = (int)this.selector[num3];
			int num6 = this.minLens[num5];
			int k;
			int num7;
			for (k = this.BsR(num6); k > this.limit[num5][num6]; k = (k << 1 | num7))
			{
				bool flag2 = num6 > 20;
				if (flag2)
				{
					throw new BZip2Exception("Bzip data error");
				}
				num6++;
				while (this.bsLive < 1)
				{
					this.FillBuffer();
				}
				num7 = (this.bsBuff >> this.bsLive - 1 & 1);
				this.bsLive--;
			}
			bool flag3 = k - this.baseArray[num5][num6] < 0 || k - this.baseArray[num5][num6] >= 258;
			if (flag3)
			{
				throw new BZip2Exception("Bzip data error");
			}
			int num8 = this.perm[num5][k - this.baseArray[num5][num6]];
			for (;;)
			{
				bool flag4 = num8 == num2;
				if (flag4)
				{
					break;
				}
				bool flag5 = num8 == 0 || num8 == 1;
				if (flag5)
				{
					int l = -1;
					int num9 = 1;
					do
					{
						bool flag6 = num8 == 0;
						if (flag6)
						{
							l += num9;
						}
						else
						{
							bool flag7 = num8 == 1;
							if (flag7)
							{
								l += 2 * num9;
							}
						}
						num9 <<= 1;
						bool flag8 = num4 == 0;
						if (flag8)
						{
							num3++;
							num4 = 50;
						}
						num4--;
						num5 = (int)this.selector[num3];
						num6 = this.minLens[num5];
						for (k = this.BsR(num6); k > this.limit[num5][num6]; k = (k << 1 | num7))
						{
							num6++;
							while (this.bsLive < 1)
							{
								this.FillBuffer();
							}
							num7 = (this.bsBuff >> this.bsLive - 1 & 1);
							this.bsLive--;
						}
						num8 = this.perm[num5][k - this.baseArray[num5][num6]];
					}
					while (num8 == 0 || num8 == 1);
					l++;
					byte b = this.seqToUnseq[(int)array[0]];
					this.unzftab[(int)b] += l;
					while (l > 0)
					{
						this.last++;
						this.ll8[this.last] = b;
						l--;
					}
					bool flag9 = this.last >= num;
					if (flag9)
					{
						BZip2InputStream.BlockOverrun();
					}
				}
				else
				{
					this.last++;
					bool flag10 = this.last >= num;
					if (flag10)
					{
						BZip2InputStream.BlockOverrun();
					}
					byte b2 = array[num8 - 1];
					this.unzftab[(int)this.seqToUnseq[(int)b2]]++;
					this.ll8[this.last] = this.seqToUnseq[(int)b2];
					for (int m = num8 - 1; m > 0; m--)
					{
						array[m] = array[m - 1];
					}
					array[0] = b2;
					bool flag11 = num4 == 0;
					if (flag11)
					{
						num3++;
						num4 = 50;
					}
					num4--;
					num5 = (int)this.selector[num3];
					num6 = this.minLens[num5];
					for (k = this.BsR(num6); k > this.limit[num5][num6]; k = (k << 1 | num7))
					{
						num6++;
						while (this.bsLive < 1)
						{
							this.FillBuffer();
						}
						num7 = (this.bsBuff >> this.bsLive - 1 & 1);
						this.bsLive--;
					}
					num8 = this.perm[num5][k - this.baseArray[num5][num6]];
				}
			}
		}

		// Token: 0x06000832 RID: 2098 RVA: 0x000353C4 File Offset: 0x000335C4
		private void SetupBlock()
		{
			int[] array = new int[257];
			array[0] = 0;
			Array.Copy(this.unzftab, 0, array, 1, 256);
			for (int i = 1; i <= 256; i++)
			{
				array[i] += array[i - 1];
			}
			for (int j = 0; j <= this.last; j++)
			{
				byte b = this.ll8[j];
				this.tt[array[(int)b]] = j;
				array[(int)b]++;
			}
			this.tPos = this.tt[this.origPtr];
			this.count = 0;
			this.i2 = 0;
			this.ch2 = 256;
			bool flag = this.blockRandomised;
			if (flag)
			{
				this.rNToGo = 0;
				this.rTPos = 0;
				this.SetupRandPartA();
			}
			else
			{
				this.SetupNoRandPartA();
			}
		}

		// Token: 0x06000833 RID: 2099 RVA: 0x000354CC File Offset: 0x000336CC
		private void SetupRandPartA()
		{
			bool flag = this.i2 <= this.last;
			if (flag)
			{
				this.chPrev = this.ch2;
				this.ch2 = (int)this.ll8[this.tPos];
				this.tPos = this.tt[this.tPos];
				bool flag2 = this.rNToGo == 0;
				if (flag2)
				{
					this.rNToGo = BZip2Constants.RandomNumbers[this.rTPos];
					this.rTPos++;
					bool flag3 = this.rTPos == 512;
					if (flag3)
					{
						this.rTPos = 0;
					}
				}
				this.rNToGo--;
				this.ch2 ^= ((this.rNToGo == 1) ? 1 : 0);
				this.i2++;
				this.currentChar = this.ch2;
				this.currentState = 3;
				this.mCrc.Update(this.ch2);
			}
			else
			{
				this.EndBlock();
				this.InitBlock();
				this.SetupBlock();
			}
		}

		// Token: 0x06000834 RID: 2100 RVA: 0x000355F8 File Offset: 0x000337F8
		private void SetupNoRandPartA()
		{
			bool flag = this.i2 <= this.last;
			if (flag)
			{
				this.chPrev = this.ch2;
				this.ch2 = (int)this.ll8[this.tPos];
				this.tPos = this.tt[this.tPos];
				this.i2++;
				this.currentChar = this.ch2;
				this.currentState = 6;
				this.mCrc.Update(this.ch2);
			}
			else
			{
				this.EndBlock();
				this.InitBlock();
				this.SetupBlock();
			}
		}

		// Token: 0x06000835 RID: 2101 RVA: 0x000356A8 File Offset: 0x000338A8
		private void SetupRandPartB()
		{
			bool flag = this.ch2 != this.chPrev;
			if (flag)
			{
				this.currentState = 2;
				this.count = 1;
				this.SetupRandPartA();
			}
			else
			{
				this.count++;
				bool flag2 = this.count >= 4;
				if (flag2)
				{
					this.z = this.ll8[this.tPos];
					this.tPos = this.tt[this.tPos];
					bool flag3 = this.rNToGo == 0;
					if (flag3)
					{
						this.rNToGo = BZip2Constants.RandomNumbers[this.rTPos];
						this.rTPos++;
						bool flag4 = this.rTPos == 512;
						if (flag4)
						{
							this.rTPos = 0;
						}
					}
					this.rNToGo--;
					this.z ^= ((this.rNToGo == 1) ? 1 : 0);
					this.j2 = 0;
					this.currentState = 4;
					this.SetupRandPartC();
				}
				else
				{
					this.currentState = 2;
					this.SetupRandPartA();
				}
			}
		}

		// Token: 0x06000836 RID: 2102 RVA: 0x000357E4 File Offset: 0x000339E4
		private void SetupRandPartC()
		{
			bool flag = this.j2 < (int)this.z;
			if (flag)
			{
				this.currentChar = this.ch2;
				this.mCrc.Update(this.ch2);
				this.j2++;
			}
			else
			{
				this.currentState = 2;
				this.i2++;
				this.count = 0;
				this.SetupRandPartA();
			}
		}

		// Token: 0x06000837 RID: 2103 RVA: 0x00035864 File Offset: 0x00033A64
		private void SetupNoRandPartB()
		{
			bool flag = this.ch2 != this.chPrev;
			if (flag)
			{
				this.currentState = 5;
				this.count = 1;
				this.SetupNoRandPartA();
			}
			else
			{
				this.count++;
				bool flag2 = this.count >= 4;
				if (flag2)
				{
					this.z = this.ll8[this.tPos];
					this.tPos = this.tt[this.tPos];
					this.currentState = 7;
					this.j2 = 0;
					this.SetupNoRandPartC();
				}
				else
				{
					this.currentState = 5;
					this.SetupNoRandPartA();
				}
			}
		}

		// Token: 0x06000838 RID: 2104 RVA: 0x00035920 File Offset: 0x00033B20
		private void SetupNoRandPartC()
		{
			bool flag = this.j2 < (int)this.z;
			if (flag)
			{
				this.currentChar = this.ch2;
				this.mCrc.Update(this.ch2);
				this.j2++;
			}
			else
			{
				this.currentState = 5;
				this.i2++;
				this.count = 0;
				this.SetupNoRandPartA();
			}
		}

		// Token: 0x06000839 RID: 2105 RVA: 0x000359A0 File Offset: 0x00033BA0
		private void SetDecompressStructureSizes(int newSize100k)
		{
			bool flag = 0 > newSize100k || newSize100k > 9 || 0 > this.blockSize100k || this.blockSize100k > 9;
			if (flag)
			{
				throw new BZip2Exception("Invalid block size");
			}
			this.blockSize100k = newSize100k;
			bool flag2 = newSize100k == 0;
			if (!flag2)
			{
				int num = 100000 * newSize100k;
				this.ll8 = new byte[num];
				this.tt = new int[num];
			}
		}

		// Token: 0x0600083A RID: 2106 RVA: 0x00035A28 File Offset: 0x00033C28
		private static void CompressedStreamEOF()
		{
			throw new EndOfStreamException("BZip2 input stream end of compressed stream");
		}

		// Token: 0x0600083B RID: 2107 RVA: 0x00035A38 File Offset: 0x00033C38
		private static void BlockOverrun()
		{
			throw new BZip2Exception("BZip2 input stream block overrun");
		}

		// Token: 0x0600083C RID: 2108 RVA: 0x00035A48 File Offset: 0x00033C48
		private static void BadBlockHeader()
		{
			throw new BZip2Exception("BZip2 input stream bad block header");
		}

		// Token: 0x0600083D RID: 2109 RVA: 0x00035A58 File Offset: 0x00033C58
		private static void CrcError()
		{
			throw new BZip2Exception("BZip2 input stream crc error");
		}

		// Token: 0x0600083E RID: 2110 RVA: 0x00035A68 File Offset: 0x00033C68
		private static void HbCreateDecodeTables(int[] limit, int[] baseArray, int[] perm, char[] length, int minLen, int maxLen, int alphaSize)
		{
			int num = 0;
			for (int i = minLen; i <= maxLen; i++)
			{
				for (int j = 0; j < alphaSize; j++)
				{
					bool flag = (int)length[j] == i;
					if (flag)
					{
						perm[num] = j;
						num++;
					}
				}
			}
			for (int k = 0; k < 23; k++)
			{
				baseArray[k] = 0;
			}
			for (int l = 0; l < alphaSize; l++)
			{
				baseArray[(int)(length[l] + '\u0001')]++;
			}
			for (int m = 1; m < 23; m++)
			{
				baseArray[m] += baseArray[m - 1];
			}
			for (int n = 0; n < 23; n++)
			{
				limit[n] = 0;
			}
			int num2 = 0;
			for (int num3 = minLen; num3 <= maxLen; num3++)
			{
				num2 += baseArray[num3 + 1] - baseArray[num3];
				limit[num3] = num2 - 1;
				num2 <<= 1;
			}
			for (int num4 = minLen + 1; num4 <= maxLen; num4++)
			{
				baseArray[num4] = (limit[num4 - 1] + 1 << 1) - baseArray[num4];
			}
		}

		// Token: 0x040004D7 RID: 1239
		private const int START_BLOCK_STATE = 1;

		// Token: 0x040004D8 RID: 1240
		private const int RAND_PART_A_STATE = 2;

		// Token: 0x040004D9 RID: 1241
		private const int RAND_PART_B_STATE = 3;

		// Token: 0x040004DA RID: 1242
		private const int RAND_PART_C_STATE = 4;

		// Token: 0x040004DB RID: 1243
		private const int NO_RAND_PART_A_STATE = 5;

		// Token: 0x040004DC RID: 1244
		private const int NO_RAND_PART_B_STATE = 6;

		// Token: 0x040004DD RID: 1245
		private const int NO_RAND_PART_C_STATE = 7;

		// Token: 0x040004DE RID: 1246
		private int last;

		// Token: 0x040004DF RID: 1247
		private int origPtr;

		// Token: 0x040004E0 RID: 1248
		private int blockSize100k;

		// Token: 0x040004E1 RID: 1249
		private bool blockRandomised;

		// Token: 0x040004E2 RID: 1250
		private int bsBuff;

		// Token: 0x040004E3 RID: 1251
		private int bsLive;

		// Token: 0x040004E4 RID: 1252
		private UWA.ICSharpCode.SharpZipLib.Checksums.IChecksum mCrc = new UWA.ICSharpCode.SharpZipLib.Checksums.StrangeCRC();

		// Token: 0x040004E5 RID: 1253
		private bool[] inUse = new bool[256];

		// Token: 0x040004E6 RID: 1254
		private int nInUse;

		// Token: 0x040004E7 RID: 1255
		private byte[] seqToUnseq = new byte[256];

		// Token: 0x040004E8 RID: 1256
		private byte[] unseqToSeq = new byte[256];

		// Token: 0x040004E9 RID: 1257
		private byte[] selector = new byte[18002];

		// Token: 0x040004EA RID: 1258
		private byte[] selectorMtf = new byte[18002];

		// Token: 0x040004EB RID: 1259
		private int[] tt;

		// Token: 0x040004EC RID: 1260
		private byte[] ll8;

		// Token: 0x040004ED RID: 1261
		private int[] unzftab = new int[256];

		// Token: 0x040004EE RID: 1262
		private int[][] limit = new int[6][];

		// Token: 0x040004EF RID: 1263
		private int[][] baseArray = new int[6][];

		// Token: 0x040004F0 RID: 1264
		private int[][] perm = new int[6][];

		// Token: 0x040004F1 RID: 1265
		private int[] minLens = new int[6];

		// Token: 0x040004F2 RID: 1266
		private Stream baseStream;

		// Token: 0x040004F3 RID: 1267
		private bool streamEnd;

		// Token: 0x040004F4 RID: 1268
		private int currentChar = -1;

		// Token: 0x040004F5 RID: 1269
		private int currentState = 1;

		// Token: 0x040004F6 RID: 1270
		private int storedBlockCRC;

		// Token: 0x040004F7 RID: 1271
		private int storedCombinedCRC;

		// Token: 0x040004F8 RID: 1272
		private int computedBlockCRC;

		// Token: 0x040004F9 RID: 1273
		private uint computedCombinedCRC;

		// Token: 0x040004FA RID: 1274
		private int count;

		// Token: 0x040004FB RID: 1275
		private int chPrev;

		// Token: 0x040004FC RID: 1276
		private int ch2;

		// Token: 0x040004FD RID: 1277
		private int tPos;

		// Token: 0x040004FE RID: 1278
		private int rNToGo;

		// Token: 0x040004FF RID: 1279
		private int rTPos;

		// Token: 0x04000500 RID: 1280
		private int i2;

		// Token: 0x04000501 RID: 1281
		private int j2;

		// Token: 0x04000502 RID: 1282
		private byte z;

		// Token: 0x04000503 RID: 1283
		private bool isStreamOwner = true;
	}
}
