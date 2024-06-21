using System;
using UWA.ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace UWA.ICSharpCode.SharpZipLib.Zip.Compression
{
	// Token: 0x02000086 RID: 134
	internal class InflaterDynHeader
	{
		// Token: 0x060005EE RID: 1518 RVA: 0x00029E48 File Offset: 0x00028048
		public bool Decode(UWA.ICSharpCode.SharpZipLib.Zip.Compression.Streams.StreamManipulator input)
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

		// Token: 0x060005EF RID: 1519 RVA: 0x0002A1DC File Offset: 0x000283DC
		public InflaterHuffmanTree BuildLitLenTree()
		{
			byte[] array = new byte[this.lnum];
			Array.Copy(this.litdistLens, 0, array, 0, this.lnum);
			return new InflaterHuffmanTree(array);
		}

		// Token: 0x060005F0 RID: 1520 RVA: 0x0002A21C File Offset: 0x0002841C
		public InflaterHuffmanTree BuildDistTree()
		{
			byte[] array = new byte[this.dnum];
			Array.Copy(this.litdistLens, this.lnum, array, 0, this.dnum);
			return new InflaterHuffmanTree(array);
		}

		// Token: 0x040003C0 RID: 960
		private const int LNUM = 0;

		// Token: 0x040003C1 RID: 961
		private const int DNUM = 1;

		// Token: 0x040003C2 RID: 962
		private const int BLNUM = 2;

		// Token: 0x040003C3 RID: 963
		private const int BLLENS = 3;

		// Token: 0x040003C4 RID: 964
		private const int LENS = 4;

		// Token: 0x040003C5 RID: 965
		private const int REPS = 5;

		// Token: 0x040003C6 RID: 966
		private static readonly int[] repMin = new int[]
		{
			3,
			3,
			11
		};

		// Token: 0x040003C7 RID: 967
		private static readonly int[] repBits = new int[]
		{
			2,
			3,
			7
		};

		// Token: 0x040003C8 RID: 968
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

		// Token: 0x040003C9 RID: 969
		private byte[] blLens;

		// Token: 0x040003CA RID: 970
		private byte[] litdistLens;

		// Token: 0x040003CB RID: 971
		private InflaterHuffmanTree blTree;

		// Token: 0x040003CC RID: 972
		private int mode;

		// Token: 0x040003CD RID: 973
		private int lnum;

		// Token: 0x040003CE RID: 974
		private int dnum;

		// Token: 0x040003CF RID: 975
		private int blnum;

		// Token: 0x040003D0 RID: 976
		private int num;

		// Token: 0x040003D1 RID: 977
		private int repSymbol;

		// Token: 0x040003D2 RID: 978
		private byte lastLen;

		// Token: 0x040003D3 RID: 979
		private int ptr;
	}
}
