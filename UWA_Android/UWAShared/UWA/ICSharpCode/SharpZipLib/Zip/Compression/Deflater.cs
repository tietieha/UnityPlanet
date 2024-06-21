using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip.Compression
{
	// Token: 0x0200008E RID: 142
	[ComVisible(false)]
	public class Deflater
	{
		// Token: 0x0600067B RID: 1659 RVA: 0x00033BE8 File Offset: 0x00031DE8
		public Deflater() : this(-1, false)
		{
		}

		// Token: 0x0600067C RID: 1660 RVA: 0x00033BF4 File Offset: 0x00031DF4
		public Deflater(int level) : this(level, false)
		{
		}

		// Token: 0x0600067D RID: 1661 RVA: 0x00033C00 File Offset: 0x00031E00
		public Deflater(int level, bool noZlibHeaderOrFooter)
		{
			bool flag = level == -1;
			if (flag)
			{
				level = 6;
			}
			else
			{
				bool flag2 = level < 0 || level > 9;
				if (flag2)
				{
					throw new ArgumentOutOfRangeException("level");
				}
			}
			this.pending = new DeflaterPending();
			this.engine = new DeflaterEngine(this.pending);
			this.noZlibHeaderOrFooter = noZlibHeaderOrFooter;
			this.SetStrategy(DeflateStrategy.Default);
			this.SetLevel(level);
			this.Reset();
		}

		// Token: 0x0600067E RID: 1662 RVA: 0x00033C90 File Offset: 0x00031E90
		public void Reset()
		{
			this.state = (this.noZlibHeaderOrFooter ? 16 : 0);
			this.totalOut = 0L;
			this.pending.Reset();
			this.engine.Reset();
		}

		// Token: 0x17000123 RID: 291
		// (get) Token: 0x0600067F RID: 1663 RVA: 0x00033CCC File Offset: 0x00031ECC
		public int Adler
		{
			get
			{
				return this.engine.Adler;
			}
		}

		// Token: 0x17000124 RID: 292
		// (get) Token: 0x06000680 RID: 1664 RVA: 0x00033CF0 File Offset: 0x00031EF0
		public long TotalIn
		{
			get
			{
				return this.engine.TotalIn;
			}
		}

		// Token: 0x17000125 RID: 293
		// (get) Token: 0x06000681 RID: 1665 RVA: 0x00033D14 File Offset: 0x00031F14
		public long TotalOut
		{
			get
			{
				return this.totalOut;
			}
		}

		// Token: 0x06000682 RID: 1666 RVA: 0x00033D34 File Offset: 0x00031F34
		public void Flush()
		{
			this.state |= 4;
		}

		// Token: 0x06000683 RID: 1667 RVA: 0x00033D48 File Offset: 0x00031F48
		public void Finish()
		{
			this.state |= 12;
		}

		// Token: 0x17000126 RID: 294
		// (get) Token: 0x06000684 RID: 1668 RVA: 0x00033D5C File Offset: 0x00031F5C
		public bool IsFinished
		{
			get
			{
				return this.state == 30 && this.pending.IsFlushed;
			}
		}

		// Token: 0x17000127 RID: 295
		// (get) Token: 0x06000685 RID: 1669 RVA: 0x00033D94 File Offset: 0x00031F94
		public bool IsNeedingInput
		{
			get
			{
				return this.engine.NeedsInput();
			}
		}

		// Token: 0x06000686 RID: 1670 RVA: 0x00033DB8 File Offset: 0x00031FB8
		public void SetInput(byte[] input)
		{
			this.SetInput(input, 0, input.Length);
		}

		// Token: 0x06000687 RID: 1671 RVA: 0x00033DC8 File Offset: 0x00031FC8
		public void SetInput(byte[] input, int offset, int count)
		{
			bool flag = (this.state & 8) != 0;
			if (flag)
			{
				throw new InvalidOperationException("Finish() already called");
			}
			this.engine.SetInput(input, offset, count);
		}

		// Token: 0x06000688 RID: 1672 RVA: 0x00033E08 File Offset: 0x00032008
		public void SetLevel(int level)
		{
			bool flag = level == -1;
			if (flag)
			{
				level = 6;
			}
			else
			{
				bool flag2 = level < 0 || level > 9;
				if (flag2)
				{
					throw new ArgumentOutOfRangeException("level");
				}
			}
			bool flag3 = this.level != level;
			if (flag3)
			{
				this.level = level;
				this.engine.SetLevel(level);
			}
		}

		// Token: 0x06000689 RID: 1673 RVA: 0x00033E80 File Offset: 0x00032080
		public int GetLevel()
		{
			return this.level;
		}

		// Token: 0x0600068A RID: 1674 RVA: 0x00033EA0 File Offset: 0x000320A0
		public void SetStrategy(DeflateStrategy strategy)
		{
			this.engine.Strategy = strategy;
		}

		// Token: 0x0600068B RID: 1675 RVA: 0x00033EB0 File Offset: 0x000320B0
		public int Deflate(byte[] output)
		{
			return this.Deflate(output, 0, output.Length);
		}

		// Token: 0x0600068C RID: 1676 RVA: 0x00033ED4 File Offset: 0x000320D4
		public int Deflate(byte[] output, int offset, int length)
		{
			int num = length;
			bool flag = this.state == 127;
			if (flag)
			{
				throw new InvalidOperationException("Deflater closed");
			}
			bool flag2 = this.state < 16;
			if (flag2)
			{
				int num2 = 30720;
				int num3 = this.level - 1 >> 1;
				bool flag3 = num3 < 0 || num3 > 3;
				if (flag3)
				{
					num3 = 3;
				}
				num2 |= num3 << 6;
				bool flag4 = (this.state & 1) != 0;
				if (flag4)
				{
					num2 |= 32;
				}
				num2 += 31 - num2 % 31;
				this.pending.WriteShortMSB(num2);
				bool flag5 = (this.state & 1) != 0;
				if (flag5)
				{
					int adler = this.engine.Adler;
					this.engine.ResetAdler();
					this.pending.WriteShortMSB(adler >> 16);
					this.pending.WriteShortMSB(adler & 65535);
				}
				this.state = (16 | (this.state & 12));
			}
			for (;;)
			{
				int num4 = this.pending.Flush(output, offset, length);
				offset += num4;
				this.totalOut += (long)num4;
				length -= num4;
				bool flag6 = length == 0 || this.state == 30;
				if (flag6)
				{
					break;
				}
				bool flag7 = !this.engine.Deflate((this.state & 4) != 0, (this.state & 8) != 0);
				if (flag7)
				{
					bool flag8 = this.state == 16;
					if (flag8)
					{
						goto Block_10;
					}
					bool flag9 = this.state == 20;
					if (flag9)
					{
						bool flag10 = this.level != 0;
						if (flag10)
						{
							for (int i = 8 + (-this.pending.BitCount & 7); i > 0; i -= 10)
							{
								this.pending.WriteBits(2, 10);
							}
						}
						this.state = 16;
					}
					else
					{
						bool flag11 = this.state == 28;
						if (flag11)
						{
							this.pending.AlignToByte();
							bool flag12 = !this.noZlibHeaderOrFooter;
							if (flag12)
							{
								int adler2 = this.engine.Adler;
								this.pending.WriteShortMSB(adler2 >> 16);
								this.pending.WriteShortMSB(adler2 & 65535);
							}
							this.state = 30;
						}
					}
				}
			}
			return num - length;
			Block_10:
			return num - length;
		}

		// Token: 0x0600068D RID: 1677 RVA: 0x00034180 File Offset: 0x00032380
		public void SetDictionary(byte[] dictionary)
		{
			this.SetDictionary(dictionary, 0, dictionary.Length);
		}

		// Token: 0x0600068E RID: 1678 RVA: 0x00034190 File Offset: 0x00032390
		public void SetDictionary(byte[] dictionary, int index, int count)
		{
			bool flag = this.state != 0;
			if (flag)
			{
				throw new InvalidOperationException();
			}
			this.state = 1;
			this.engine.SetDictionary(dictionary, index, count);
		}

		// Token: 0x040003B0 RID: 944
		public const int BEST_COMPRESSION = 9;

		// Token: 0x040003B1 RID: 945
		public const int BEST_SPEED = 1;

		// Token: 0x040003B2 RID: 946
		public const int DEFAULT_COMPRESSION = -1;

		// Token: 0x040003B3 RID: 947
		public const int NO_COMPRESSION = 0;

		// Token: 0x040003B4 RID: 948
		public const int DEFLATED = 8;

		// Token: 0x040003B5 RID: 949
		private const int IS_SETDICT = 1;

		// Token: 0x040003B6 RID: 950
		private const int IS_FLUSHING = 4;

		// Token: 0x040003B7 RID: 951
		private const int IS_FINISHING = 8;

		// Token: 0x040003B8 RID: 952
		private const int INIT_STATE = 0;

		// Token: 0x040003B9 RID: 953
		private const int SETDICT_STATE = 1;

		// Token: 0x040003BA RID: 954
		private const int BUSY_STATE = 16;

		// Token: 0x040003BB RID: 955
		private const int FLUSHING_STATE = 20;

		// Token: 0x040003BC RID: 956
		private const int FINISHING_STATE = 28;

		// Token: 0x040003BD RID: 957
		private const int FINISHED_STATE = 30;

		// Token: 0x040003BE RID: 958
		private const int CLOSED_STATE = 127;

		// Token: 0x040003BF RID: 959
		private int level;

		// Token: 0x040003C0 RID: 960
		private bool noZlibHeaderOrFooter;

		// Token: 0x040003C1 RID: 961
		private int state;

		// Token: 0x040003C2 RID: 962
		private long totalOut;

		// Token: 0x040003C3 RID: 963
		private DeflaterPending pending;

		// Token: 0x040003C4 RID: 964
		private DeflaterEngine engine;
	}
}
