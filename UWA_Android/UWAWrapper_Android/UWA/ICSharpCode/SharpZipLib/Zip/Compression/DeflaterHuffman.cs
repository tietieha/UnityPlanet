using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip.Compression
{
	// Token: 0x02000083 RID: 131
	[ComVisible(false)]
	public class DeflaterHuffman
	{
		// Token: 0x060005C9 RID: 1481 RVA: 0x00028788 File Offset: 0x00026988
		static DeflaterHuffman()
		{
			int i = 0;
			while (i < 144)
			{
				DeflaterHuffman.staticLCodes[i] = DeflaterHuffman.BitReverse(48 + i << 8);
				DeflaterHuffman.staticLLength[i++] = 8;
			}
			while (i < 256)
			{
				DeflaterHuffman.staticLCodes[i] = DeflaterHuffman.BitReverse(256 + i << 7);
				DeflaterHuffman.staticLLength[i++] = 9;
			}
			while (i < 280)
			{
				DeflaterHuffman.staticLCodes[i] = DeflaterHuffman.BitReverse(-256 + i << 9);
				DeflaterHuffman.staticLLength[i++] = 7;
			}
			while (i < 286)
			{
				DeflaterHuffman.staticLCodes[i] = DeflaterHuffman.BitReverse(-88 + i << 8);
				DeflaterHuffman.staticLLength[i++] = 8;
			}
			DeflaterHuffman.staticDCodes = new short[30];
			DeflaterHuffman.staticDLength = new byte[30];
			for (i = 0; i < 30; i++)
			{
				DeflaterHuffman.staticDCodes[i] = DeflaterHuffman.BitReverse(i << 11);
				DeflaterHuffman.staticDLength[i] = 5;
			}
		}

		// Token: 0x060005CA RID: 1482 RVA: 0x00028900 File Offset: 0x00026B00
		public DeflaterHuffman(DeflaterPending pending)
		{
			this.pending = pending;
			this.literalTree = new DeflaterHuffman.Tree(this, 286, 257, 15);
			this.distTree = new DeflaterHuffman.Tree(this, 30, 1, 15);
			this.blTree = new DeflaterHuffman.Tree(this, 19, 4, 7);
			this.d_buf = new short[16384];
			this.l_buf = new byte[16384];
		}

		// Token: 0x060005CB RID: 1483 RVA: 0x0002897C File Offset: 0x00026B7C
		public void Reset()
		{
			this.last_lit = 0;
			this.extra_bits = 0;
			this.literalTree.Reset();
			this.distTree.Reset();
			this.blTree.Reset();
		}

		// Token: 0x060005CC RID: 1484 RVA: 0x000289B4 File Offset: 0x00026BB4
		public void SendAllTrees(int blTreeCodes)
		{
			this.blTree.BuildCodes();
			this.literalTree.BuildCodes();
			this.distTree.BuildCodes();
			this.pending.WriteBits(this.literalTree.numCodes - 257, 5);
			this.pending.WriteBits(this.distTree.numCodes - 1, 5);
			this.pending.WriteBits(blTreeCodes - 4, 4);
			for (int i = 0; i < blTreeCodes; i++)
			{
				this.pending.WriteBits((int)this.blTree.length[DeflaterHuffman.BL_ORDER[i]], 3);
			}
			this.literalTree.WriteTree(this.blTree);
			this.distTree.WriteTree(this.blTree);
		}

		// Token: 0x060005CD RID: 1485 RVA: 0x00028A8C File Offset: 0x00026C8C
		public void CompressBlock()
		{
			for (int i = 0; i < this.last_lit; i++)
			{
				int num = (int)(this.l_buf[i] & byte.MaxValue);
				int num2 = (int)this.d_buf[i];
				bool flag = num2-- != 0;
				if (flag)
				{
					int num3 = DeflaterHuffman.Lcode(num);
					this.literalTree.WriteSymbol(num3);
					int num4 = (num3 - 261) / 4;
					bool flag2 = num4 > 0 && num4 <= 5;
					if (flag2)
					{
						this.pending.WriteBits(num & (1 << num4) - 1, num4);
					}
					int num5 = DeflaterHuffman.Dcode(num2);
					this.distTree.WriteSymbol(num5);
					num4 = num5 / 2 - 1;
					bool flag3 = num4 > 0;
					if (flag3)
					{
						this.pending.WriteBits(num2 & (1 << num4) - 1, num4);
					}
				}
				else
				{
					this.literalTree.WriteSymbol(num);
				}
			}
			this.literalTree.WriteSymbol(256);
		}

		// Token: 0x060005CE RID: 1486 RVA: 0x00028BB0 File Offset: 0x00026DB0
		public void FlushStoredBlock(byte[] stored, int storedOffset, int storedLength, bool lastBlock)
		{
			this.pending.WriteBits(lastBlock ? 1 : 0, 3);
			this.pending.AlignToByte();
			this.pending.WriteShort(storedLength);
			this.pending.WriteShort(~storedLength);
			this.pending.WriteBlock(stored, storedOffset, storedLength);
			this.Reset();
		}

		// Token: 0x060005CF RID: 1487 RVA: 0x00028C1C File Offset: 0x00026E1C
		public void FlushBlock(byte[] stored, int storedOffset, int storedLength, bool lastBlock)
		{
			short[] freqs = this.literalTree.freqs;
			int num = 256;
			freqs[num] += 1;
			this.literalTree.BuildTree();
			this.distTree.BuildTree();
			this.literalTree.CalcBLFreq(this.blTree);
			this.distTree.CalcBLFreq(this.blTree);
			this.blTree.BuildTree();
			int num2 = 4;
			for (int i = 18; i > num2; i--)
			{
				bool flag = this.blTree.length[DeflaterHuffman.BL_ORDER[i]] > 0;
				if (flag)
				{
					num2 = i + 1;
				}
			}
			int num3 = 14 + num2 * 3 + this.blTree.GetEncodedLength() + this.literalTree.GetEncodedLength() + this.distTree.GetEncodedLength() + this.extra_bits;
			int num4 = this.extra_bits;
			for (int j = 0; j < 286; j++)
			{
				num4 += (int)(this.literalTree.freqs[j] * (short)DeflaterHuffman.staticLLength[j]);
			}
			for (int k = 0; k < 30; k++)
			{
				num4 += (int)(this.distTree.freqs[k] * (short)DeflaterHuffman.staticDLength[k]);
			}
			bool flag2 = num3 >= num4;
			if (flag2)
			{
				num3 = num4;
			}
			bool flag3 = storedOffset >= 0 && storedLength + 4 < num3 >> 3;
			if (flag3)
			{
				this.FlushStoredBlock(stored, storedOffset, storedLength, lastBlock);
			}
			else
			{
				bool flag4 = num3 == num4;
				if (flag4)
				{
					this.pending.WriteBits(2 + (lastBlock ? 1 : 0), 3);
					this.literalTree.SetStaticCodes(DeflaterHuffman.staticLCodes, DeflaterHuffman.staticLLength);
					this.distTree.SetStaticCodes(DeflaterHuffman.staticDCodes, DeflaterHuffman.staticDLength);
					this.CompressBlock();
					this.Reset();
				}
				else
				{
					this.pending.WriteBits(4 + (lastBlock ? 1 : 0), 3);
					this.SendAllTrees(num2);
					this.CompressBlock();
					this.Reset();
				}
			}
		}

		// Token: 0x060005D0 RID: 1488 RVA: 0x00028E68 File Offset: 0x00027068
		public bool IsFull()
		{
			return this.last_lit >= 16384;
		}

		// Token: 0x060005D1 RID: 1489 RVA: 0x00028E94 File Offset: 0x00027094
		public bool TallyLit(int literal)
		{
			this.d_buf[this.last_lit] = 0;
			byte[] array = this.l_buf;
			int num = this.last_lit;
			this.last_lit = num + 1;
			array[num] = (byte)literal;
			short[] freqs = this.literalTree.freqs;
			freqs[literal] += 1;
			return this.IsFull();
		}

		// Token: 0x060005D2 RID: 1490 RVA: 0x00028EF4 File Offset: 0x000270F4
		public bool TallyDist(int distance, int length)
		{
			this.d_buf[this.last_lit] = (short)distance;
			byte[] array = this.l_buf;
			int num = this.last_lit;
			this.last_lit = num + 1;
			array[num] = (byte)(length - 3);
			int num2 = DeflaterHuffman.Lcode(length - 3);
			short[] freqs = this.literalTree.freqs;
			int num3 = num2;
			freqs[num3] += 1;
			bool flag = num2 >= 265 && num2 < 285;
			if (flag)
			{
				this.extra_bits += (num2 - 261) / 4;
			}
			int num4 = DeflaterHuffman.Dcode(distance - 1);
			short[] freqs2 = this.distTree.freqs;
			int num5 = num4;
			freqs2[num5] += 1;
			bool flag2 = num4 >= 4;
			if (flag2)
			{
				this.extra_bits += num4 / 2 - 1;
			}
			return this.IsFull();
		}

		// Token: 0x060005D3 RID: 1491 RVA: 0x00028FDC File Offset: 0x000271DC
		public static short BitReverse(int toReverse)
		{
			return (short)((int)DeflaterHuffman.bit4Reverse[toReverse & 15] << 12 | (int)DeflaterHuffman.bit4Reverse[toReverse >> 4 & 15] << 8 | (int)DeflaterHuffman.bit4Reverse[toReverse >> 8 & 15] << 4 | (int)DeflaterHuffman.bit4Reverse[toReverse >> 12]);
		}

		// Token: 0x060005D4 RID: 1492 RVA: 0x0002902C File Offset: 0x0002722C
		private static int Lcode(int length)
		{
			bool flag = length == 255;
			int result;
			if (flag)
			{
				result = 285;
			}
			else
			{
				int num = 257;
				while (length >= 8)
				{
					num += 4;
					length >>= 1;
				}
				result = num + length;
			}
			return result;
		}

		// Token: 0x060005D5 RID: 1493 RVA: 0x00029084 File Offset: 0x00027284
		private static int Dcode(int distance)
		{
			int num = 0;
			while (distance >= 4)
			{
				num += 2;
				distance >>= 1;
			}
			return num + distance;
		}

		// Token: 0x04000389 RID: 905
		private const int BUFSIZE = 16384;

		// Token: 0x0400038A RID: 906
		private const int LITERAL_NUM = 286;

		// Token: 0x0400038B RID: 907
		private const int DIST_NUM = 30;

		// Token: 0x0400038C RID: 908
		private const int BITLEN_NUM = 19;

		// Token: 0x0400038D RID: 909
		private const int REP_3_6 = 16;

		// Token: 0x0400038E RID: 910
		private const int REP_3_10 = 17;

		// Token: 0x0400038F RID: 911
		private const int REP_11_138 = 18;

		// Token: 0x04000390 RID: 912
		private const int EOF_SYMBOL = 256;

		// Token: 0x04000391 RID: 913
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

		// Token: 0x04000392 RID: 914
		private static readonly byte[] bit4Reverse = new byte[]
		{
			0,
			8,
			4,
			12,
			2,
			10,
			6,
			14,
			1,
			9,
			5,
			13,
			3,
			11,
			7,
			15
		};

		// Token: 0x04000393 RID: 915
		private static short[] staticLCodes = new short[286];

		// Token: 0x04000394 RID: 916
		private static byte[] staticLLength = new byte[286];

		// Token: 0x04000395 RID: 917
		private static short[] staticDCodes;

		// Token: 0x04000396 RID: 918
		private static byte[] staticDLength;

		// Token: 0x04000397 RID: 919
		public DeflaterPending pending;

		// Token: 0x04000398 RID: 920
		private DeflaterHuffman.Tree literalTree;

		// Token: 0x04000399 RID: 921
		private DeflaterHuffman.Tree distTree;

		// Token: 0x0400039A RID: 922
		private DeflaterHuffman.Tree blTree;

		// Token: 0x0400039B RID: 923
		private short[] d_buf;

		// Token: 0x0400039C RID: 924
		private byte[] l_buf;

		// Token: 0x0400039D RID: 925
		private int last_lit;

		// Token: 0x0400039E RID: 926
		private int extra_bits;

		// Token: 0x02000121 RID: 289
		private class Tree
		{
			// Token: 0x060009AA RID: 2474 RVA: 0x0003E438 File Offset: 0x0003C638
			public Tree(DeflaterHuffman dh, int elems, int minCodes, int maxLength)
			{
				this.dh = dh;
				this.minNumCodes = minCodes;
				this.maxLength = maxLength;
				this.freqs = new short[elems];
				this.bl_counts = new int[maxLength];
			}

			// Token: 0x060009AB RID: 2475 RVA: 0x0003E474 File Offset: 0x0003C674
			public void Reset()
			{
				for (int i = 0; i < this.freqs.Length; i++)
				{
					this.freqs[i] = 0;
				}
				this.codes = null;
				this.length = null;
			}

			// Token: 0x060009AC RID: 2476 RVA: 0x0003E4BC File Offset: 0x0003C6BC
			public void WriteSymbol(int code)
			{
				this.dh.pending.WriteBits((int)this.codes[code] & 65535, (int)this.length[code]);
			}

			// Token: 0x060009AD RID: 2477 RVA: 0x0003E4E8 File Offset: 0x0003C6E8
			public void CheckEmpty()
			{
				bool flag = true;
				for (int i = 0; i < this.freqs.Length; i++)
				{
					bool flag2 = this.freqs[i] != 0;
					if (flag2)
					{
						flag = false;
					}
				}
				bool flag3 = !flag;
				if (flag3)
				{
					throw new SharpZipBaseException("!Empty");
				}
			}

			// Token: 0x060009AE RID: 2478 RVA: 0x0003E548 File Offset: 0x0003C748
			public void SetStaticCodes(short[] staticCodes, byte[] staticLengths)
			{
				this.codes = staticCodes;
				this.length = staticLengths;
			}

			// Token: 0x060009AF RID: 2479 RVA: 0x0003E55C File Offset: 0x0003C75C
			public void BuildCodes()
			{
				int num = this.freqs.Length;
				int[] array = new int[this.maxLength];
				int num2 = 0;
				this.codes = new short[this.freqs.Length];
				for (int i = 0; i < this.maxLength; i++)
				{
					array[i] = num2;
					num2 += this.bl_counts[i] << 15 - i;
				}
				for (int j = 0; j < this.numCodes; j++)
				{
					int num3 = (int)this.length[j];
					bool flag = num3 > 0;
					if (flag)
					{
						this.codes[j] = DeflaterHuffman.BitReverse(array[num3 - 1]);
						array[num3 - 1] += 1 << 16 - num3;
					}
				}
			}

			// Token: 0x060009B0 RID: 2480 RVA: 0x0003E634 File Offset: 0x0003C834
			public void BuildTree()
			{
				int num = this.freqs.Length;
				int[] array = new int[num];
				int i = 0;
				int num2 = 0;
				for (int j = 0; j < num; j++)
				{
					int num3 = (int)this.freqs[j];
					bool flag = num3 != 0;
					if (flag)
					{
						int num4 = i++;
						int num5;
						while (num4 > 0 && (int)this.freqs[array[num5 = (num4 - 1) / 2]] > num3)
						{
							array[num4] = array[num5];
							num4 = num5;
						}
						array[num4] = j;
						num2 = j;
					}
				}
				while (i < 2)
				{
					int num6 = (num2 < 2) ? (++num2) : 0;
					array[i++] = num6;
				}
				this.numCodes = Math.Max(num2 + 1, this.minNumCodes);
				int num7 = i;
				int[] array2 = new int[4 * i - 2];
				int[] array3 = new int[2 * i - 1];
				int num8 = num7;
				for (int k = 0; k < i; k++)
				{
					int num9 = array[k];
					array2[2 * k] = num9;
					array2[2 * k + 1] = -1;
					array3[k] = (int)this.freqs[num9] << 8;
					array[k] = k;
				}
				do
				{
					int num10 = array[0];
					int num11 = array[--i];
					int num12 = 0;
					int l;
					for (l = 1; l < i; l = l * 2 + 1)
					{
						bool flag2 = l + 1 < i && array3[array[l]] > array3[array[l + 1]];
						if (flag2)
						{
							l++;
						}
						array[num12] = array[l];
						num12 = l;
					}
					int num13 = array3[num11];
					while ((l = num12) > 0 && array3[array[num12 = (l - 1) / 2]] > num13)
					{
						array[l] = array[num12];
					}
					array[l] = num11;
					int num14 = array[0];
					num11 = num8++;
					array2[2 * num11] = num10;
					array2[2 * num11 + 1] = num14;
					int num15 = Math.Min(array3[num10] & 255, array3[num14] & 255);
					num13 = (array3[num11] = array3[num10] + array3[num14] - num15 + 1);
					num12 = 0;
					for (l = 1; l < i; l = num12 * 2 + 1)
					{
						bool flag3 = l + 1 < i && array3[array[l]] > array3[array[l + 1]];
						if (flag3)
						{
							l++;
						}
						array[num12] = array[l];
						num12 = l;
					}
					while ((l = num12) > 0 && array3[array[num12 = (l - 1) / 2]] > num13)
					{
						array[l] = array[num12];
					}
					array[l] = num11;
				}
				while (i > 1);
				bool flag4 = array[0] != array2.Length / 2 - 1;
				if (flag4)
				{
					throw new SharpZipBaseException("Heap invariant violated");
				}
				this.BuildLength(array2);
			}

			// Token: 0x060009B1 RID: 2481 RVA: 0x0003E96C File Offset: 0x0003CB6C
			public int GetEncodedLength()
			{
				int num = 0;
				for (int i = 0; i < this.freqs.Length; i++)
				{
					num += (int)(this.freqs[i] * (short)this.length[i]);
				}
				return num;
			}

			// Token: 0x060009B2 RID: 2482 RVA: 0x0003E9B8 File Offset: 0x0003CBB8
			public void CalcBLFreq(DeflaterHuffman.Tree blTree)
			{
				int num = -1;
				int i = 0;
				while (i < this.numCodes)
				{
					int num2 = 1;
					int num3 = (int)this.length[i];
					bool flag = num3 == 0;
					int num4;
					int num5;
					if (flag)
					{
						num4 = 138;
						num5 = 3;
					}
					else
					{
						num4 = 6;
						num5 = 3;
						bool flag2 = num != num3;
						if (flag2)
						{
							short[] array = blTree.freqs;
							int num6 = num3;
							array[num6] += 1;
							num2 = 0;
						}
					}
					num = num3;
					i++;
					while (i < this.numCodes && num == (int)this.length[i])
					{
						i++;
						bool flag3 = ++num2 >= num4;
						if (flag3)
						{
							break;
						}
					}
					bool flag4 = num2 < num5;
					if (flag4)
					{
						short[] array2 = blTree.freqs;
						int num7 = num;
						array2[num7] += (short)num2;
					}
					else
					{
						bool flag5 = num != 0;
						if (flag5)
						{
							short[] array3 = blTree.freqs;
							int num8 = 16;
							array3[num8] += 1;
						}
						else
						{
							bool flag6 = num2 <= 10;
							if (flag6)
							{
								short[] array4 = blTree.freqs;
								int num9 = 17;
								array4[num9] += 1;
							}
							else
							{
								short[] array5 = blTree.freqs;
								int num10 = 18;
								array5[num10] += 1;
							}
						}
					}
				}
			}

			// Token: 0x060009B3 RID: 2483 RVA: 0x0003EB1C File Offset: 0x0003CD1C
			public void WriteTree(DeflaterHuffman.Tree blTree)
			{
				int num = -1;
				int i = 0;
				while (i < this.numCodes)
				{
					int num2 = 1;
					int num3 = (int)this.length[i];
					bool flag = num3 == 0;
					int num4;
					int num5;
					if (flag)
					{
						num4 = 138;
						num5 = 3;
					}
					else
					{
						num4 = 6;
						num5 = 3;
						bool flag2 = num != num3;
						if (flag2)
						{
							blTree.WriteSymbol(num3);
							num2 = 0;
						}
					}
					num = num3;
					i++;
					while (i < this.numCodes && num == (int)this.length[i])
					{
						i++;
						bool flag3 = ++num2 >= num4;
						if (flag3)
						{
							break;
						}
					}
					bool flag4 = num2 < num5;
					if (flag4)
					{
						while (num2-- > 0)
						{
							blTree.WriteSymbol(num);
						}
					}
					else
					{
						bool flag5 = num != 0;
						if (flag5)
						{
							blTree.WriteSymbol(16);
							this.dh.pending.WriteBits(num2 - 3, 2);
						}
						else
						{
							bool flag6 = num2 <= 10;
							if (flag6)
							{
								blTree.WriteSymbol(17);
								this.dh.pending.WriteBits(num2 - 3, 3);
							}
							else
							{
								blTree.WriteSymbol(18);
								this.dh.pending.WriteBits(num2 - 11, 7);
							}
						}
					}
				}
			}

			// Token: 0x060009B4 RID: 2484 RVA: 0x0003ECA4 File Offset: 0x0003CEA4
			private void BuildLength(int[] childs)
			{
				this.length = new byte[this.freqs.Length];
				int num = childs.Length / 2;
				int num2 = (num + 1) / 2;
				int num3 = 0;
				for (int i = 0; i < this.maxLength; i++)
				{
					this.bl_counts[i] = 0;
				}
				int[] array = new int[num];
				array[num - 1] = 0;
				for (int j = num - 1; j >= 0; j--)
				{
					bool flag = childs[2 * j + 1] != -1;
					if (flag)
					{
						int num4 = array[j] + 1;
						bool flag2 = num4 > this.maxLength;
						if (flag2)
						{
							num4 = this.maxLength;
							num3++;
						}
						array[childs[2 * j]] = (array[childs[2 * j + 1]] = num4);
					}
					else
					{
						int num5 = array[j];
						this.bl_counts[num5 - 1]++;
						this.length[childs[2 * j]] = (byte)array[j];
					}
				}
				bool flag3 = num3 == 0;
				if (!flag3)
				{
					int num6 = this.maxLength - 1;
					do
					{
						while (this.bl_counts[--num6] == 0)
						{
						}
						do
						{
							this.bl_counts[num6]--;
							this.bl_counts[++num6]++;
							num3 -= 1 << this.maxLength - 1 - num6;
						}
						while (num3 > 0 && num6 < this.maxLength - 1);
					}
					while (num3 > 0);
					this.bl_counts[this.maxLength - 1] += num3;
					this.bl_counts[this.maxLength - 2] -= num3;
					int num7 = 2 * num2;
					for (int k = this.maxLength; k != 0; k--)
					{
						int l = this.bl_counts[k - 1];
						while (l > 0)
						{
							int num8 = 2 * childs[num7++];
							bool flag4 = childs[num8 + 1] == -1;
							if (flag4)
							{
								this.length[childs[num8]] = (byte)k;
								l--;
							}
						}
					}
				}
			}

			// Token: 0x04000709 RID: 1801
			public short[] freqs;

			// Token: 0x0400070A RID: 1802
			public byte[] length;

			// Token: 0x0400070B RID: 1803
			public int minNumCodes;

			// Token: 0x0400070C RID: 1804
			public int numCodes;

			// Token: 0x0400070D RID: 1805
			private short[] codes;

			// Token: 0x0400070E RID: 1806
			private int[] bl_counts;

			// Token: 0x0400070F RID: 1807
			private int maxLength;

			// Token: 0x04000710 RID: 1808
			private DeflaterHuffman dh;
		}
	}
}
