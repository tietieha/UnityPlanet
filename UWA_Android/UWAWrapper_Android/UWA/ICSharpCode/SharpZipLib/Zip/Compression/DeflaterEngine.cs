using System;
using System.Runtime.InteropServices;
using UWA.ICSharpCode.SharpZipLib.Checksums;

namespace UWA.ICSharpCode.SharpZipLib.Zip.Compression
{
	// Token: 0x02000082 RID: 130
	[ComVisible(false)]
	public class DeflaterEngine : DeflaterConstants
	{
		// Token: 0x060005B5 RID: 1461 RVA: 0x000274BC File Offset: 0x000256BC
		public DeflaterEngine(DeflaterPending pending)
		{
			this.pending = pending;
			this.huffman = new DeflaterHuffman(pending);
			this.adler = new UWA.ICSharpCode.SharpZipLib.Checksums.Adler32();
			this.window = new byte[65536];
			this.head = new short[32768];
			this.prev = new short[32768];
			this.blockStart = (this.strstart = 1);
		}

		// Token: 0x060005B6 RID: 1462 RVA: 0x00027534 File Offset: 0x00025734
		public bool Deflate(bool flush, bool finish)
		{
			for (;;)
			{
				this.FillWindow();
				bool flush2 = flush && this.inputOff == this.inputEnd;
				bool flag;
				switch (this.compressionFunction)
				{
				case 0:
					flag = this.DeflateStored(flush2, finish);
					goto IL_79;
				case 1:
					flag = this.DeflateFast(flush2, finish);
					goto IL_79;
				case 2:
					flag = this.DeflateSlow(flush2, finish);
					goto IL_79;
				}
				break;
				IL_79:
				if (!this.pending.IsFlushed || !flag)
				{
					return flag;
				}
			}
			throw new InvalidOperationException("unknown compressionFunction");
		}

		// Token: 0x060005B7 RID: 1463 RVA: 0x000275E0 File Offset: 0x000257E0
		public void SetInput(byte[] buffer, int offset, int count)
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
			bool flag4 = this.inputOff < this.inputEnd;
			if (flag4)
			{
				throw new InvalidOperationException("Old input was not completely processed");
			}
			int num = offset + count;
			bool flag5 = offset > num || num > buffer.Length;
			if (flag5)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			this.inputBuf = buffer;
			this.inputOff = offset;
			this.inputEnd = num;
		}

		// Token: 0x060005B8 RID: 1464 RVA: 0x000276A0 File Offset: 0x000258A0
		public bool NeedsInput()
		{
			return this.inputEnd == this.inputOff;
		}

		// Token: 0x060005B9 RID: 1465 RVA: 0x000276C8 File Offset: 0x000258C8
		public void SetDictionary(byte[] buffer, int offset, int length)
		{
			this.adler.Update(buffer, offset, length);
			bool flag = length < 3;
			if (!flag)
			{
				bool flag2 = length > 32506;
				if (flag2)
				{
					offset += length - 32506;
					length = 32506;
				}
				Array.Copy(buffer, offset, this.window, this.strstart, length);
				this.UpdateHash();
				length--;
				while (--length > 0)
				{
					this.InsertString();
					this.strstart++;
				}
				this.strstart += 2;
				this.blockStart = this.strstart;
			}
		}

		// Token: 0x060005BA RID: 1466 RVA: 0x00027784 File Offset: 0x00025984
		public void Reset()
		{
			this.huffman.Reset();
			this.adler.Reset();
			this.blockStart = (this.strstart = 1);
			this.lookahead = 0;
			this.totalIn = 0L;
			this.prevAvailable = false;
			this.matchLen = 2;
			for (int i = 0; i < 32768; i++)
			{
				this.head[i] = 0;
			}
			for (int j = 0; j < 32768; j++)
			{
				this.prev[j] = 0;
			}
		}

		// Token: 0x060005BB RID: 1467 RVA: 0x00027824 File Offset: 0x00025A24
		public void ResetAdler()
		{
			this.adler.Reset();
		}

		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x060005BC RID: 1468 RVA: 0x00027834 File Offset: 0x00025A34
		public int Adler
		{
			get
			{
				return (int)this.adler.Value;
			}
		}

		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x060005BD RID: 1469 RVA: 0x0002785C File Offset: 0x00025A5C
		public long TotalIn
		{
			get
			{
				return this.totalIn;
			}
		}

		// Token: 0x170000D6 RID: 214
		// (get) Token: 0x060005BE RID: 1470 RVA: 0x0002787C File Offset: 0x00025A7C
		// (set) Token: 0x060005BF RID: 1471 RVA: 0x0002789C File Offset: 0x00025A9C
		public DeflateStrategy Strategy
		{
			get
			{
				return this.strategy;
			}
			set
			{
				this.strategy = value;
			}
		}

		// Token: 0x060005C0 RID: 1472 RVA: 0x000278A8 File Offset: 0x00025AA8
		public void SetLevel(int level)
		{
			bool flag = level < 0 || level > 9;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("level");
			}
			this.goodLength = DeflaterConstants.GOOD_LENGTH[level];
			this.max_lazy = DeflaterConstants.MAX_LAZY[level];
			this.niceLength = DeflaterConstants.NICE_LENGTH[level];
			this.max_chain = DeflaterConstants.MAX_CHAIN[level];
			bool flag2 = DeflaterConstants.COMPR_FUNC[level] != this.compressionFunction;
			if (flag2)
			{
				switch (this.compressionFunction)
				{
				case 0:
				{
					bool flag3 = this.strstart > this.blockStart;
					if (flag3)
					{
						this.huffman.FlushStoredBlock(this.window, this.blockStart, this.strstart - this.blockStart, false);
						this.blockStart = this.strstart;
					}
					this.UpdateHash();
					break;
				}
				case 1:
				{
					bool flag4 = this.strstart > this.blockStart;
					if (flag4)
					{
						this.huffman.FlushBlock(this.window, this.blockStart, this.strstart - this.blockStart, false);
						this.blockStart = this.strstart;
					}
					break;
				}
				case 2:
				{
					bool flag5 = this.prevAvailable;
					if (flag5)
					{
						this.huffman.TallyLit((int)(this.window[this.strstart - 1] & byte.MaxValue));
					}
					bool flag6 = this.strstart > this.blockStart;
					if (flag6)
					{
						this.huffman.FlushBlock(this.window, this.blockStart, this.strstart - this.blockStart, false);
						this.blockStart = this.strstart;
					}
					this.prevAvailable = false;
					this.matchLen = 2;
					break;
				}
				}
				this.compressionFunction = DeflaterConstants.COMPR_FUNC[level];
			}
		}

		// Token: 0x060005C1 RID: 1473 RVA: 0x00027A94 File Offset: 0x00025C94
		public void FillWindow()
		{
			bool flag = this.strstart >= 65274;
			if (flag)
			{
				this.SlideWindow();
			}
			while (this.lookahead < 262 && this.inputOff < this.inputEnd)
			{
				int num = 65536 - this.lookahead - this.strstart;
				bool flag2 = num > this.inputEnd - this.inputOff;
				if (flag2)
				{
					num = this.inputEnd - this.inputOff;
				}
				Array.Copy(this.inputBuf, this.inputOff, this.window, this.strstart + this.lookahead, num);
				this.adler.Update(this.inputBuf, this.inputOff, num);
				this.inputOff += num;
				this.totalIn += (long)num;
				this.lookahead += num;
			}
			bool flag3 = this.lookahead >= 3;
			if (flag3)
			{
				this.UpdateHash();
			}
		}

		// Token: 0x060005C2 RID: 1474 RVA: 0x00027BBC File Offset: 0x00025DBC
		private void UpdateHash()
		{
			this.ins_h = ((int)this.window[this.strstart] << 5 ^ (int)this.window[this.strstart + 1]);
		}

		// Token: 0x060005C3 RID: 1475 RVA: 0x00027BE4 File Offset: 0x00025DE4
		private int InsertString()
		{
			int num = (this.ins_h << 5 ^ (int)this.window[this.strstart + 2]) & 32767;
			short num2 = this.prev[this.strstart & 32767] = this.head[num];
			this.head[num] = (short)this.strstart;
			this.ins_h = num;
			return (int)num2 & 65535;
		}

		// Token: 0x060005C4 RID: 1476 RVA: 0x00027C58 File Offset: 0x00025E58
		private void SlideWindow()
		{
			Array.Copy(this.window, 32768, this.window, 0, 32768);
			this.matchStart -= 32768;
			this.strstart -= 32768;
			this.blockStart -= 32768;
			for (int i = 0; i < 32768; i++)
			{
				int num = (int)this.head[i] & 65535;
				this.head[i] = (short)((num >= 32768) ? (num - 32768) : 0);
			}
			for (int j = 0; j < 32768; j++)
			{
				int num2 = (int)this.prev[j] & 65535;
				this.prev[j] = (short)((num2 >= 32768) ? (num2 - 32768) : 0);
			}
		}

		// Token: 0x060005C5 RID: 1477 RVA: 0x00027D54 File Offset: 0x00025F54
		private bool FindLongestMatch(int curMatch)
		{
			int num = this.max_chain;
			int num2 = this.niceLength;
			short[] array = this.prev;
			int num3 = this.strstart;
			int num4 = this.strstart + this.matchLen;
			int num5 = Math.Max(this.matchLen, 2);
			int num6 = Math.Max(this.strstart - 32506, 0);
			int num7 = this.strstart + 258 - 1;
			byte b = this.window[num4 - 1];
			byte b2 = this.window[num4];
			bool flag = num5 >= this.goodLength;
			if (flag)
			{
				num >>= 2;
			}
			bool flag2 = num2 > this.lookahead;
			if (flag2)
			{
				num2 = this.lookahead;
			}
			do
			{
				bool flag3 = this.window[curMatch + num5] != b2 || this.window[curMatch + num5 - 1] != b || this.window[curMatch] != this.window[num3] || this.window[curMatch + 1] != this.window[num3 + 1];
				if (!flag3)
				{
					int num8 = curMatch + 2;
					num3 += 2;
					while (this.window[++num3] == this.window[++num8] && this.window[++num3] == this.window[++num8] && this.window[++num3] == this.window[++num8] && this.window[++num3] == this.window[++num8] && this.window[++num3] == this.window[++num8] && this.window[++num3] == this.window[++num8] && this.window[++num3] == this.window[++num8] && this.window[++num3] == this.window[++num8] && num3 < num7)
					{
					}
					bool flag4 = num3 > num4;
					if (flag4)
					{
						this.matchStart = curMatch;
						num4 = num3;
						num5 = num3 - this.strstart;
						bool flag5 = num5 >= num2;
						if (flag5)
						{
							break;
						}
						b = this.window[num4 - 1];
						b2 = this.window[num4];
					}
					num3 = this.strstart;
				}
			}
			while ((curMatch = ((int)array[curMatch & 32767] & 65535)) > num6 && --num != 0);
			this.matchLen = Math.Min(num5, this.lookahead);
			return this.matchLen >= 3;
		}

		// Token: 0x060005C6 RID: 1478 RVA: 0x0002804C File Offset: 0x0002624C
		private bool DeflateStored(bool flush, bool finish)
		{
			bool flag = !flush && this.lookahead == 0;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				this.strstart += this.lookahead;
				this.lookahead = 0;
				int num = this.strstart - this.blockStart;
				bool flag2 = num >= DeflaterConstants.MAX_BLOCK_SIZE || (this.blockStart < 32768 && num >= 32506) || flush;
				if (flag2)
				{
					bool flag3 = finish;
					bool flag4 = num > DeflaterConstants.MAX_BLOCK_SIZE;
					if (flag4)
					{
						num = DeflaterConstants.MAX_BLOCK_SIZE;
						flag3 = false;
					}
					this.huffman.FlushStoredBlock(this.window, this.blockStart, num, flag3);
					this.blockStart += num;
					result = !flag3;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		// Token: 0x060005C7 RID: 1479 RVA: 0x00028144 File Offset: 0x00026344
		private bool DeflateFast(bool flush, bool finish)
		{
			bool flag = this.lookahead < 262 && !flush;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				while (this.lookahead >= 262 || flush)
				{
					bool flag2 = this.lookahead == 0;
					if (flag2)
					{
						this.huffman.FlushBlock(this.window, this.blockStart, this.strstart - this.blockStart, finish);
						this.blockStart = this.strstart;
						return false;
					}
					bool flag3 = this.strstart > 65274;
					if (flag3)
					{
						this.SlideWindow();
					}
					int num;
					bool flag4 = this.lookahead >= 3 && (num = this.InsertString()) != 0 && this.strategy != DeflateStrategy.HuffmanOnly && this.strstart - num <= 32506 && this.FindLongestMatch(num);
					if (flag4)
					{
						bool flag5 = this.huffman.TallyDist(this.strstart - this.matchStart, this.matchLen);
						this.lookahead -= this.matchLen;
						bool flag6 = this.matchLen <= this.max_lazy && this.lookahead >= 3;
						if (flag6)
						{
							for (;;)
							{
								int num2 = this.matchLen - 1;
								this.matchLen = num2;
								if (num2 <= 0)
								{
									break;
								}
								this.strstart++;
								this.InsertString();
							}
							this.strstart++;
						}
						else
						{
							this.strstart += this.matchLen;
							bool flag7 = this.lookahead >= 2;
							if (flag7)
							{
								this.UpdateHash();
							}
						}
						this.matchLen = 2;
						bool flag8 = !flag5;
						if (flag8)
						{
							continue;
						}
					}
					else
					{
						this.huffman.TallyLit((int)(this.window[this.strstart] & byte.MaxValue));
						this.strstart++;
						this.lookahead--;
					}
					bool flag9 = this.huffman.IsFull();
					if (flag9)
					{
						bool flag10 = finish && this.lookahead == 0;
						this.huffman.FlushBlock(this.window, this.blockStart, this.strstart - this.blockStart, flag10);
						this.blockStart = this.strstart;
						return !flag10;
					}
				}
				result = true;
			}
			return result;
		}

		// Token: 0x060005C8 RID: 1480 RVA: 0x0002840C File Offset: 0x0002660C
		private bool DeflateSlow(bool flush, bool finish)
		{
			bool flag = this.lookahead < 262 && !flush;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				while (this.lookahead >= 262 || flush)
				{
					bool flag2 = this.lookahead == 0;
					if (flag2)
					{
						bool flag3 = this.prevAvailable;
						if (flag3)
						{
							this.huffman.TallyLit((int)(this.window[this.strstart - 1] & byte.MaxValue));
						}
						this.prevAvailable = false;
						this.huffman.FlushBlock(this.window, this.blockStart, this.strstart - this.blockStart, finish);
						this.blockStart = this.strstart;
						return false;
					}
					bool flag4 = this.strstart >= 65274;
					if (flag4)
					{
						this.SlideWindow();
					}
					int num = this.matchStart;
					int num2 = this.matchLen;
					bool flag5 = this.lookahead >= 3;
					if (flag5)
					{
						int num3 = this.InsertString();
						bool flag6 = this.strategy != DeflateStrategy.HuffmanOnly && num3 != 0 && this.strstart - num3 <= 32506 && this.FindLongestMatch(num3);
						if (flag6)
						{
							bool flag7 = this.matchLen <= 5 && (this.strategy == DeflateStrategy.Filtered || (this.matchLen == 3 && this.strstart - this.matchStart > 4096));
							if (flag7)
							{
								this.matchLen = 2;
							}
						}
					}
					bool flag8 = num2 >= 3 && this.matchLen <= num2;
					if (flag8)
					{
						this.huffman.TallyDist(this.strstart - 1 - num, num2);
						num2 -= 2;
						do
						{
							this.strstart++;
							this.lookahead--;
							bool flag9 = this.lookahead >= 3;
							if (flag9)
							{
								this.InsertString();
							}
						}
						while (--num2 > 0);
						this.strstart++;
						this.lookahead--;
						this.prevAvailable = false;
						this.matchLen = 2;
					}
					else
					{
						bool flag10 = this.prevAvailable;
						if (flag10)
						{
							this.huffman.TallyLit((int)(this.window[this.strstart - 1] & byte.MaxValue));
						}
						this.prevAvailable = true;
						this.strstart++;
						this.lookahead--;
					}
					bool flag11 = this.huffman.IsFull();
					if (flag11)
					{
						int num4 = this.strstart - this.blockStart;
						bool flag12 = this.prevAvailable;
						if (flag12)
						{
							num4--;
						}
						bool flag13 = finish && this.lookahead == 0 && !this.prevAvailable;
						this.huffman.FlushBlock(this.window, this.blockStart, num4, flag13);
						this.blockStart += num4;
						return !flag13;
					}
				}
				result = true;
			}
			return result;
		}

		// Token: 0x04000371 RID: 881
		private const int TooFar = 4096;

		// Token: 0x04000372 RID: 882
		private int ins_h;

		// Token: 0x04000373 RID: 883
		private short[] head;

		// Token: 0x04000374 RID: 884
		private short[] prev;

		// Token: 0x04000375 RID: 885
		private int matchStart;

		// Token: 0x04000376 RID: 886
		private int matchLen;

		// Token: 0x04000377 RID: 887
		private bool prevAvailable;

		// Token: 0x04000378 RID: 888
		private int blockStart;

		// Token: 0x04000379 RID: 889
		private int strstart;

		// Token: 0x0400037A RID: 890
		private int lookahead;

		// Token: 0x0400037B RID: 891
		private byte[] window;

		// Token: 0x0400037C RID: 892
		private DeflateStrategy strategy;

		// Token: 0x0400037D RID: 893
		private int max_chain;

		// Token: 0x0400037E RID: 894
		private int max_lazy;

		// Token: 0x0400037F RID: 895
		private int niceLength;

		// Token: 0x04000380 RID: 896
		private int goodLength;

		// Token: 0x04000381 RID: 897
		private int compressionFunction;

		// Token: 0x04000382 RID: 898
		private byte[] inputBuf;

		// Token: 0x04000383 RID: 899
		private long totalIn;

		// Token: 0x04000384 RID: 900
		private int inputOff;

		// Token: 0x04000385 RID: 901
		private int inputEnd;

		// Token: 0x04000386 RID: 902
		private DeflaterPending pending;

		// Token: 0x04000387 RID: 903
		private DeflaterHuffman huffman;

		// Token: 0x04000388 RID: 904
		private UWA.ICSharpCode.SharpZipLib.Checksums.Adler32 adler;
	}
}
