using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip.Compression
{
	// Token: 0x0200007F RID: 127
	[ComVisible(false)]
	public class Deflater
	{
		// Token: 0x0600059F RID: 1439 RVA: 0x00026E30 File Offset: 0x00025030
		public Deflater() : this(-1, false)
		{
		}

		// Token: 0x060005A0 RID: 1440 RVA: 0x00026E3C File Offset: 0x0002503C
		public Deflater(int level) : this(level, false)
		{
		}

		// Token: 0x060005A1 RID: 1441 RVA: 0x00026E48 File Offset: 0x00025048
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

		// Token: 0x060005A2 RID: 1442 RVA: 0x00026ED8 File Offset: 0x000250D8
		public void Reset()
		{
			this.state = (this.noZlibHeaderOrFooter ? 16 : 0);
			this.totalOut = 0L;
			this.pending.Reset();
			this.engine.Reset();
		}

		// Token: 0x170000CF RID: 207
		// (get) Token: 0x060005A3 RID: 1443 RVA: 0x00026F14 File Offset: 0x00025114
		public int Adler
		{
			get
			{
				return this.engine.Adler;
			}
		}

		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x060005A4 RID: 1444 RVA: 0x00026F38 File Offset: 0x00025138
		public long TotalIn
		{
			get
			{
				return this.engine.TotalIn;
			}
		}

		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x060005A5 RID: 1445 RVA: 0x00026F5C File Offset: 0x0002515C
		public long TotalOut
		{
			get
			{
				return this.totalOut;
			}
		}

		// Token: 0x060005A6 RID: 1446 RVA: 0x00026F7C File Offset: 0x0002517C
		public void Flush()
		{
			this.state |= 4;
		}

		// Token: 0x060005A7 RID: 1447 RVA: 0x00026F90 File Offset: 0x00025190
		public void Finish()
		{
			this.state |= 12;
		}

		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x060005A8 RID: 1448 RVA: 0x00026FA4 File Offset: 0x000251A4
		public bool IsFinished
		{
			get
			{
				return this.state == 30 && this.pending.IsFlushed;
			}
		}

		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x060005A9 RID: 1449 RVA: 0x00026FDC File Offset: 0x000251DC
		public bool IsNeedingInput
		{
			get
			{
				return this.engine.NeedsInput();
			}
		}

		// Token: 0x060005AA RID: 1450 RVA: 0x00027000 File Offset: 0x00025200
		public void SetInput(byte[] input)
		{
			this.SetInput(input, 0, input.Length);
		}

		// Token: 0x060005AB RID: 1451 RVA: 0x00027010 File Offset: 0x00025210
		public void SetInput(byte[] input, int offset, int count)
		{
			bool flag = (this.state & 8) != 0;
			if (flag)
			{
				throw new InvalidOperationException("Finish() already called");
			}
			this.engine.SetInput(input, offset, count);
		}

		// Token: 0x060005AC RID: 1452 RVA: 0x00027050 File Offset: 0x00025250
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

		// Token: 0x060005AD RID: 1453 RVA: 0x000270C8 File Offset: 0x000252C8
		public int GetLevel()
		{
			return this.level;
		}

		// Token: 0x060005AE RID: 1454 RVA: 0x000270E8 File Offset: 0x000252E8
		public void SetStrategy(DeflateStrategy strategy)
		{
			this.engine.Strategy = strategy;
		}

		// Token: 0x060005AF RID: 1455 RVA: 0x000270F8 File Offset: 0x000252F8
		public int Deflate(byte[] output)
		{
			return this.Deflate(output, 0, output.Length);
		}

		// Token: 0x060005B0 RID: 1456 RVA: 0x0002711C File Offset: 0x0002531C
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

		// Token: 0x060005B1 RID: 1457 RVA: 0x000273C8 File Offset: 0x000255C8
		public void SetDictionary(byte[] dictionary)
		{
			this.SetDictionary(dictionary, 0, dictionary.Length);
		}

		// Token: 0x060005B2 RID: 1458 RVA: 0x000273D8 File Offset: 0x000255D8
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

		// Token: 0x0400033D RID: 829
		public const int BEST_COMPRESSION = 9;

		// Token: 0x0400033E RID: 830
		public const int BEST_SPEED = 1;

		// Token: 0x0400033F RID: 831
		public const int DEFAULT_COMPRESSION = -1;

		// Token: 0x04000340 RID: 832
		public const int NO_COMPRESSION = 0;

		// Token: 0x04000341 RID: 833
		public const int DEFLATED = 8;

		// Token: 0x04000342 RID: 834
		private const int IS_SETDICT = 1;

		// Token: 0x04000343 RID: 835
		private const int IS_FLUSHING = 4;

		// Token: 0x04000344 RID: 836
		private const int IS_FINISHING = 8;

		// Token: 0x04000345 RID: 837
		private const int INIT_STATE = 0;

		// Token: 0x04000346 RID: 838
		private const int SETDICT_STATE = 1;

		// Token: 0x04000347 RID: 839
		private const int BUSY_STATE = 16;

		// Token: 0x04000348 RID: 840
		private const int FLUSHING_STATE = 20;

		// Token: 0x04000349 RID: 841
		private const int FINISHING_STATE = 28;

		// Token: 0x0400034A RID: 842
		private const int FINISHED_STATE = 30;

		// Token: 0x0400034B RID: 843
		private const int CLOSED_STATE = 127;

		// Token: 0x0400034C RID: 844
		private int level;

		// Token: 0x0400034D RID: 845
		private bool noZlibHeaderOrFooter;

		// Token: 0x0400034E RID: 846
		private int state;

		// Token: 0x0400034F RID: 847
		private long totalOut;

		// Token: 0x04000350 RID: 848
		private DeflaterPending pending;

		// Token: 0x04000351 RID: 849
		private DeflaterEngine engine;
	}
}
