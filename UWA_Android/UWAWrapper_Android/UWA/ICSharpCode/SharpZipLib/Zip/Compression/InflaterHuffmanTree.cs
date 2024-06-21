using System;
using System.Runtime.InteropServices;
using UWA.ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace UWA.ICSharpCode.SharpZipLib.Zip.Compression
{
	// Token: 0x02000087 RID: 135
	[ComVisible(false)]
	public class InflaterHuffmanTree
	{
		// Token: 0x060005F2 RID: 1522 RVA: 0x0002A2B8 File Offset: 0x000284B8
		static InflaterHuffmanTree()
		{
			try
			{
				byte[] array = new byte[288];
				int i = 0;
				while (i < 144)
				{
					array[i++] = 8;
				}
				while (i < 256)
				{
					array[i++] = 9;
				}
				while (i < 280)
				{
					array[i++] = 7;
				}
				while (i < 288)
				{
					array[i++] = 8;
				}
				InflaterHuffmanTree.defLitLenTree = new InflaterHuffmanTree(array);
				array = new byte[32];
				i = 0;
				while (i < 32)
				{
					array[i++] = 5;
				}
				InflaterHuffmanTree.defDistTree = new InflaterHuffmanTree(array);
			}
			catch (Exception)
			{
				throw new SharpZipBaseException("InflaterHuffmanTree: static tree length illegal");
			}
		}

		// Token: 0x060005F3 RID: 1523 RVA: 0x0002A3A4 File Offset: 0x000285A4
		public InflaterHuffmanTree(byte[] codeLengths)
		{
			this.BuildTree(codeLengths);
		}

		// Token: 0x060005F4 RID: 1524 RVA: 0x0002A3B8 File Offset: 0x000285B8
		private void BuildTree(byte[] codeLengths)
		{
			int[] array = new int[16];
			int[] array2 = new int[16];
			foreach (int num in codeLengths)
			{
				bool flag = num > 0;
				if (flag)
				{
					array[num]++;
				}
			}
			int num2 = 0;
			int num3 = 512;
			for (int j = 1; j <= 15; j++)
			{
				array2[j] = num2;
				num2 += array[j] << 16 - j;
				bool flag2 = j >= 10;
				if (flag2)
				{
					int num4 = array2[j] & 130944;
					int num5 = num2 & 130944;
					num3 += num5 - num4 >> 16 - j;
				}
			}
			this.tree = new short[num3];
			int num6 = 512;
			for (int k = 15; k >= 10; k--)
			{
				int num7 = num2 & 130944;
				num2 -= array[k] << 16 - k;
				int num8 = num2 & 130944;
				for (int l = num8; l < num7; l += 128)
				{
					this.tree[(int)DeflaterHuffman.BitReverse(l)] = (short)(-num6 << 4 | k);
					num6 += 1 << k - 9;
				}
			}
			for (int m = 0; m < codeLengths.Length; m++)
			{
				int num9 = (int)codeLengths[m];
				bool flag3 = num9 == 0;
				if (!flag3)
				{
					num2 = array2[num9];
					int num10 = (int)DeflaterHuffman.BitReverse(num2);
					bool flag4 = num9 <= 9;
					if (flag4)
					{
						do
						{
							this.tree[num10] = (short)(m << 4 | num9);
							num10 += 1 << num9;
						}
						while (num10 < 512);
					}
					else
					{
						int num11 = (int)this.tree[num10 & 511];
						int num12 = 1 << (num11 & 15);
						num11 = -(num11 >> 4);
						do
						{
							this.tree[num11 | num10 >> 9] = (short)(m << 4 | num9);
							num10 += 1 << num9;
						}
						while (num10 < num12);
					}
					array2[num9] = num2 + (1 << 16 - num9);
				}
			}
		}

		// Token: 0x060005F5 RID: 1525 RVA: 0x0002A62C File Offset: 0x0002882C
		public int GetSymbol(UWA.ICSharpCode.SharpZipLib.Zip.Compression.Streams.StreamManipulator input)
		{
			int num;
			bool flag = (num = input.PeekBits(9)) >= 0;
			int result;
			if (flag)
			{
				int num2;
				bool flag2 = (num2 = (int)this.tree[num]) >= 0;
				if (flag2)
				{
					input.DropBits(num2 & 15);
					result = num2 >> 4;
				}
				else
				{
					int num3 = -(num2 >> 4);
					int bitCount = num2 & 15;
					bool flag3 = (num = input.PeekBits(bitCount)) >= 0;
					if (flag3)
					{
						num2 = (int)this.tree[num3 | num >> 9];
						input.DropBits(num2 & 15);
						result = num2 >> 4;
					}
					else
					{
						int availableBits = input.AvailableBits;
						num = input.PeekBits(availableBits);
						num2 = (int)this.tree[num3 | num >> 9];
						bool flag4 = (num2 & 15) <= availableBits;
						if (flag4)
						{
							input.DropBits(num2 & 15);
							result = num2 >> 4;
						}
						else
						{
							result = -1;
						}
					}
				}
			}
			else
			{
				int availableBits2 = input.AvailableBits;
				num = input.PeekBits(availableBits2);
				int num2 = (int)this.tree[num];
				bool flag5 = num2 >= 0 && (num2 & 15) <= availableBits2;
				if (flag5)
				{
					input.DropBits(num2 & 15);
					result = num2 >> 4;
				}
				else
				{
					result = -1;
				}
			}
			return result;
		}

		// Token: 0x040003D4 RID: 980
		private const int MAX_BITLEN = 15;

		// Token: 0x040003D5 RID: 981
		private short[] tree;

		// Token: 0x040003D6 RID: 982
		public static InflaterHuffmanTree defLitLenTree;

		// Token: 0x040003D7 RID: 983
		public static InflaterHuffmanTree defDistTree;
	}
}
