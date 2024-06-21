using System;
using UWA.ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace UWA.ICSharpCode.SharpZipLib.Zip.Compression
{
	// Token: 0x02000095 RID: 149
	internal class InflaterDynHeader
	{
		// Token: 0x060006CA RID: 1738 RVA: 0x00036C00 File Offset: 0x00034E00
		public bool Decode(StreamManipulator input)
		{
			for (;;)
			{
				for (;;)
				{
					switch (this.mode)
					{
					case 0:
						goto IL_34;
					case 1:
						goto IL_7F;
					case 2:
						goto IL_EC;
					case 3:
						goto IL_149;
					case 4:
						goto IL_1DA;
					case 5:
						goto IL_2AE;
					}
				}
				IL_2AE:
				int bitCount = InflaterDynHeader.repBits[this.repSymbol];
				int num = input.PeekBits(bitCount);
				bool flag = num < 0;
				if (flag)
				{
					goto Block_11;
				}
				input.DropBits(bitCount);
				num += InflaterDynHeader.repMin[this.repSymbol];
				bool flag2 = this.ptr + num > this.num;
				if (flag2)
				{
					goto Block_12;
				}
				while (num-- > 0)
				{
					byte[] array = this.litdistLens;
					int num2 = this.ptr;
					this.ptr = num2 + 1;
					array[num2] = this.lastLen;
				}
				bool flag3 = this.ptr == this.num;
				if (flag3)
				{
					goto Block_14;
				}
				this.mode = 4;
				continue;
				IL_1DA:
				int symbol;
				while (((symbol = this.blTree.GetSymbol(input)) & -16) == 0)
				{
					byte[] array2 = this.litdistLens;
					int num2 = this.ptr;
					this.ptr = num2 + 1;
					array2[num2] = (this.lastLen = (byte)symbol);
					bool flag4 = this.ptr == this.num;
					if (flag4)
					{
						goto Block_6;
					}
				}
				bool flag5 = symbol < 0;
				if (flag5)
				{
					goto Block_8;
				}
				bool flag6 = symbol >= 17;
				if (flag6)
				{
					this.lastLen = 0;
				}
				else
				{
					bool flag7 = this.ptr == 0;
					if (flag7)
					{
						goto Block_10;
					}
				}
				this.repSymbol = symbol - 16;
				this.mode = 5;
				goto IL_2AE;
				IL_149:
				while (this.ptr < this.blnum)
				{
					int num3 = input.PeekBits(3);
					bool flag8 = num3 < 0;
					if (flag8)
					{
						goto Block_4;
					}
					input.DropBits(3);
					this.blLens[InflaterDynHeader.BL_ORDER[this.ptr]] = (byte)num3;
					this.ptr++;
				}
				this.blTree = new InflaterHuffmanTree(this.blLens);
				this.blLens = null;
				this.ptr = 0;
				this.mode = 4;
				goto IL_1DA;
				IL_EC:
				this.blnum = input.PeekBits(4);
				bool flag9 = this.blnum < 0;
				if (flag9)
				{
					goto Block_3;
				}
				this.blnum += 4;
				input.DropBits(4);
				this.blLens = new byte[19];
				this.ptr = 0;
				this.mode = 3;
				goto IL_149;
				IL_7F:
				this.dnum = input.PeekBits(5);
				bool flag10 = this.dnum < 0;
				if (flag10)
				{
					goto Block_2;
				}
				this.dnum++;
				input.DropBits(5);
				this.num = this.lnum + this.dnum;
				this.litdistLens = new byte[this.num];
				this.mode = 2;
				goto IL_EC;
				IL_34:
				this.lnum = input.PeekBits(5);
				bool flag11 = this.lnum < 0;
				if (flag11)
				{
					break;
				}
				this.lnum += 257;
				input.DropBits(5);
				this.mode = 1;
				goto IL_7F;
			}
			return false;
			Block_2:
			return false;
			Block_3:
			return false;
			Block_4:
			return false;
			Block_6:
			return true;
			Block_8:
			return false;
			Block_10:
			throw new SharpZipBaseException();
			Block_11:
			return false;
			Block_12:
			throw new SharpZipBaseException();
			Block_14:
			return true;
		}

		// Token: 0x060006CB RID: 1739 RVA: 0x00036F94 File Offset: 0x00035194
		public InflaterHuffmanTree BuildLitLenTree()
		{
			byte[] array = new byte[this.lnum];
			Array.Copy(this.litdistLens, 0, array, 0, this.lnum);
			return new InflaterHuffmanTree(array);
		}

		// Token: 0x060006CC RID: 1740 RVA: 0x00036FD4 File Offset: 0x000351D4
		public InflaterHuffmanTree BuildDistTree()
		{
			byte[] array = new byte[this.dnum];
			Array.Copy(this.litdistLens, this.lnum, array, 0, this.dnum);
			return new InflaterHuffmanTree(array);
		}

		// Token: 0x04000433 RID: 1075
		private const int LNUM = 0;

		// Token: 0x04000434 RID: 1076
		private const int DNUM = 1;

		// Token: 0x04000435 RID: 1077
		private const int BLNUM = 2;

		// Token: 0x04000436 RID: 1078
		private const int BLLENS = 3;

		// Token: 0x04000437 RID: 1079
		private const int LENS = 4;

		// Token: 0x04000438 RID: 1080
		private const int REPS = 5;

		// Token: 0x04000439 RID: 1081
		private static readonly int[] repMin = new int[]
		{
			3,
			3,
			11
		};

		// Token: 0x0400043A RID: 1082
		private static readonly int[] repBits = new int[]
		{
			2,
			3,
			7
		};

		// Token: 0x0400043B RID: 1083
		private static readonly int[] BL_ORDER = new int[]
		{
			16,
			17,
			18,
			0,
			8,
			7,
			9,
			6,
			10,
			5,
			11,
			4,
			12,
			3,
			13,
			2,
			14,
			1,
			15
		};

		// Token: 0x0400043C RID: 1084
		private byte[] blLens;

		// Token: 0x0400043D RID: 1085
		private byte[] litdistLens;

		// Token: 0x0400043E RID: 1086
		private InflaterHuffmanTree blTree;

		// Token: 0x0400043F RID: 1087
		private int mode;

		// Token: 0x04000440 RID: 1088
		private int lnum;

		// Token: 0x04000441 RID: 1089
		private int dnum;

		// Token: 0x04000442 RID: 1090
		private int blnum;

		// Token: 0x04000443 RID: 1091
		private int num;

		// Token: 0x04000444 RID: 1092
		private int repSymbol;

		// Token: 0x04000445 RID: 1093
		private byte lastLen;

		// Token: 0x04000446 RID: 1094
		private int ptr;
	}
}
