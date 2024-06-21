using System;
using System.IO;
using System.Runtime.InteropServices;
using UWA.ICSharpCode.SharpZipLib.Checksums;

namespace UWA.ICSharpCode.SharpZipLib.BZip2
{
	// Token: 0x020000CE RID: 206
	[ComVisible(false)]
	public class BZip2InputStream : Stream
	{
		// Token: 0x060008F0 RID: 2288 RVA: 0x000410D0 File Offset: 0x0003F2D0
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

		// Token: 0x170001B2 RID: 434
		// (get) Token: 0x060008F1 RID: 2289 RVA: 0x00041210 File Offset: 0x0003F410
		// (set) Token: 0x060008F2 RID: 2290 RVA: 0x00041230 File Offset: 0x0003F430
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

		// Token: 0x170001B3 RID: 435
		// (get) Token: 0x060008F3 RID: 2291 RVA: 0x0004123C File Offset: 0x0003F43C
		public override bool CanRead
		{
			get
			{
				return this.baseStream.CanRead;
			}
		}

		// Token: 0x170001B4 RID: 436
		// (get) Token: 0x060008F4 RID: 2292 RVA: 0x00041260 File Offset: 0x0003F460
		public override bool CanSeek
		{
			get
			{
				return this.baseStream.CanSeek;
			}
		}

		// Token: 0x170001B5 RID: 437
		// (get) Token: 0x060008F5 RID: 2293 RVA: 0x00041284 File Offset: 0x0003F484
		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170001B6 RID: 438
		// (get) Token: 0x060008F6 RID: 2294 RVA: 0x000412A0 File Offset: 0x0003F4A0
		public override long Length
		{
			get
			{
				return this.baseStream.Length;
			}
		}

		// Token: 0x170001B7 RID: 439
		// (get) Token: 0x060008F7 RID: 2295 RVA: 0x000412C4 File Offset: 0x0003F4C4
		// (set) Token: 0x060008F8 RID: 2296 RVA: 0x000412E8 File Offset: 0x0003F4E8
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

		// Token: 0x060008F9 RID: 2297 RVA: 0x000412F8 File Offset: 0x0003F4F8
		public override void Flush()
		{
			bool flag = this.baseStream != null;
			if (flag)
			{
				this.baseStream.Flush();
			}
		}

		// Token: 0x060008FA RID: 2298 RVA: 0x00041328 File Offset: 0x0003F528
		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException("BZip2InputStream Seek not supported");
		}

		// Token: 0x060008FB RID: 2299 RVA: 0x00041338 File Offset: 0x0003F538
		public override void SetLength(long value)
		{
			throw new NotSupportedException("BZip2InputStream SetLength not supported");
		}

		// Token: 0x060008FC RID: 2300 RVA: 0x00041348 File Offset: 0x0003F548
		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException("BZip2InputStream Write not supported");
		}

		// Token: 0x060008FD RID: 2301 RVA: 0x00041358 File Offset: 0x0003F558
		public override void WriteByte(byte value)
		{
			throw new NotSupportedException("BZip2InputStream WriteByte not supported");
		}

		// Token: 0x060008FE RID: 2302 RVA: 0x00041368 File Offset: 0x0003F568
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

		// Token: 0x060008FF RID: 2303 RVA: 0x000413D4 File Offset: 0x0003F5D4
		public override void Close()
		{
			bool flag = this.IsStreamOwner && this.baseStream != null;
			if (flag)
			{
				this.baseStream.Close();
			}
		}

		// Token: 0x06000900 RID: 2304 RVA: 0x00041418 File Offset: 0x0003F618
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

		// Token: 0x06000901 RID: 2305 RVA: 0x000414BC File Offset: 0x0003F6BC
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

		// Token: 0x06000902 RID: 2306 RVA: 0x0004152C File Offset: 0x0003F72C
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

		// Token: 0x06000903 RID: 2307 RVA: 0x000415B0 File Offset: 0x0003F7B0
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

		// Token: 0x06000904 RID: 2308 RVA: 0x000416C8 File Offset: 0x0003F8C8
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

		// Token: 0x06000905 RID: 2309 RVA: 0x0004173C File Offset: 0x0003F93C
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

		// Token: 0x06000906 RID: 2310 RVA: 0x00041784 File Offset: 0x0003F984
		private void BsSetStream(Stream stream)
		{
			this.baseStream = stream;
			this.bsLive = 0;
			this.bsBuff = 0;
		}

		// Token: 0x06000907 RID: 2311 RVA: 0x0004179C File Offset: 0x0003F99C
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

		// Token: 0x06000908 RID: 2312 RVA: 0x00041814 File Offset: 0x0003FA14
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

		// Token: 0x06000909 RID: 2313 RVA: 0x00041874 File Offset: 0x0003FA74
		private char BsGetUChar()
		{
			return (char)this.BsR(8);
		}

		// Token: 0x0600090A RID: 2314 RVA: 0x00041898 File Offset: 0x0003FA98
		private int BsGetIntVS(int numBits)
		{
			return this.BsR(numBits);
		}

		// Token: 0x0600090B RID: 2315 RVA: 0x000418B8 File Offset: 0x0003FAB8
		private int BsGetInt32()
		{
			int num = this.BsR(8);
			num = (num << 8 | this.BsR(8));
			num = (num << 8 | this.BsR(8));
			return num << 8 | this.BsR(8);
		}

		// Token: 0x0600090C RID: 2316 RVA: 0x00041900 File Offset: 0x0003FB00
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

		// Token: 0x0600090D RID: 2317 RVA: 0x00041C44 File Offset: 0x0003FE44
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

		// Token: 0x0600090E RID: 2318 RVA: 0x0004217C File Offset: 0x0004037C
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

		// Token: 0x0600090F RID: 2319 RVA: 0x00042284 File Offset: 0x00040484
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

		// Token: 0x06000910 RID: 2320 RVA: 0x000423B0 File Offset: 0x000405B0
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

		// Token: 0x06000911 RID: 2321 RVA: 0x00042460 File Offset: 0x00040660
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

		// Token: 0x06000912 RID: 2322 RVA: 0x0004259C File Offset: 0x0004079C
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

		// Token: 0x06000913 RID: 2323 RVA: 0x0004261C File Offset: 0x0004081C
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

		// Token: 0x06000914 RID: 2324 RVA: 0x000426D8 File Offset: 0x000408D8
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

		// Token: 0x06000915 RID: 2325 RVA: 0x00042758 File Offset: 0x00040958
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

		// Token: 0x06000916 RID: 2326 RVA: 0x000427E0 File Offset: 0x000409E0
		private static void CompressedStreamEOF()
		{
			throw new EndOfStreamException("BZip2 input stream end of compressed stream");
		}

		// Token: 0x06000917 RID: 2327 RVA: 0x000427F0 File Offset: 0x000409F0
		private static void BlockOverrun()
		{
			throw new BZip2Exception("BZip2 input stream block overrun");
		}

		// Token: 0x06000918 RID: 2328 RVA: 0x00042800 File Offset: 0x00040A00
		private static void BadBlockHeader()
		{
			throw new BZip2Exception("BZip2 input stream bad block header");
		}

		// Token: 0x06000919 RID: 2329 RVA: 0x00042810 File Offset: 0x00040A10
		private static void CrcError()
		{
			throw new BZip2Exception("BZip2 input stream crc error");
		}

		// Token: 0x0600091A RID: 2330 RVA: 0x00042820 File Offset: 0x00040A20
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

		// Token: 0x0400054A RID: 1354
		private const int START_BLOCK_STATE = 1;

		// Token: 0x0400054B RID: 1355
		private const int RAND_PART_A_STATE = 2;

		// Token: 0x0400054C RID: 1356
		private const int RAND_PART_B_STATE = 3;

		// Token: 0x0400054D RID: 1357
		private const int RAND_PART_C_STATE = 4;

		// Token: 0x0400054E RID: 1358
		private const int NO_RAND_PART_A_STATE = 5;

		// Token: 0x0400054F RID: 1359
		private const int NO_RAND_PART_B_STATE = 6;

		// Token: 0x04000550 RID: 1360
		private const int NO_RAND_PART_C_STATE = 7;

		// Token: 0x04000551 RID: 1361
		private int last;

		// Token: 0x04000552 RID: 1362
		private int origPtr;

		// Token: 0x04000553 RID: 1363
		private int blockSize100k;

		// Token: 0x04000554 RID: 1364
		private bool blockRandomised;

		// Token: 0x04000555 RID: 1365
		private int bsBuff;

		// Token: 0x04000556 RID: 1366
		private int bsLive;

		// Token: 0x04000557 RID: 1367
		private IChecksum mCrc = new StrangeCRC();

		// Token: 0x04000558 RID: 1368
		private bool[] inUse = new bool[256];

		// Token: 0x04000559 RID: 1369
		private int nInUse;

		// Token: 0x0400055A RID: 1370
		private byte[] seqToUnseq = new byte[256];

		// Token: 0x0400055B RID: 1371
		private byte[] unseqToSeq = new byte[256];

		// Token: 0x0400055C RID: 1372
		private byte[] selector = new byte[18002];

		// Token: 0x0400055D RID: 1373
		private byte[] selectorMtf = new byte[18002];

		// Token: 0x0400055E RID: 1374
		private int[] tt;

		// Token: 0x0400055F RID: 1375
		private byte[] ll8;

		// Token: 0x04000560 RID: 1376
		private int[] unzftab = new int[256];

		// Token: 0x04000561 RID: 1377
		private int[][] limit = new int[6][];

		// Token: 0x04000562 RID: 1378
		private int[][] baseArray = new int[6][];

		// Token: 0x04000563 RID: 1379
		private int[][] perm = new int[6][];

		// Token: 0x04000564 RID: 1380
		private int[] minLens = new int[6];

		// Token: 0x04000565 RID: 1381
		private Stream baseStream;

		// Token: 0x04000566 RID: 1382
		private bool streamEnd;

		// Token: 0x04000567 RID: 1383
		private int currentChar = -1;

		// Token: 0x04000568 RID: 1384
		private int currentState = 1;

		// Token: 0x04000569 RID: 1385
		private int storedBlockCRC;

		// Token: 0x0400056A RID: 1386
		private int storedCombinedCRC;

		// Token: 0x0400056B RID: 1387
		private int computedBlockCRC;

		// Token: 0x0400056C RID: 1388
		private uint computedCombinedCRC;

		// Token: 0x0400056D RID: 1389
		private int count;

		// Token: 0x0400056E RID: 1390
		private int chPrev;

		// Token: 0x0400056F RID: 1391
		private int ch2;

		// Token: 0x04000570 RID: 1392
		private int tPos;

		// Token: 0x04000571 RID: 1393
		private int rNToGo;

		// Token: 0x04000572 RID: 1394
		private int rTPos;

		// Token: 0x04000573 RID: 1395
		private int i2;

		// Token: 0x04000574 RID: 1396
		private int j2;

		// Token: 0x04000575 RID: 1397
		private byte z;

		// Token: 0x04000576 RID: 1398
		private bool isStreamOwner = true;
	}
}
