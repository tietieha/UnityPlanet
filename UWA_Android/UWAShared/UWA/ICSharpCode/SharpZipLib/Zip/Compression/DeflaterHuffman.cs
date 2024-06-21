using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip.Compression
{
	// Token: 0x02000092 RID: 146
	[ComVisible(false)]
	public class DeflaterHuffman
	{
		// Token: 0x060006A5 RID: 1701 RVA: 0x00035540 File Offset: 0x00033740
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

		// Token: 0x060006A6 RID: 1702 RVA: 0x000356B8 File Offset: 0x000338B8
		public DeflaterHuffman(DeflaterPending pending)
		{
			this.pending = pending;
			this.literalTree = new DeflaterHuffman.Tree(this, 286, 257, 15);
			this.distTree = new DeflaterHuffman.Tree(this, 30, 1, 15);
			this.blTree = new DeflaterHuffman.Tree(this, 19, 4, 7);
			this.d_buf = new short[16384];
			this.l_buf = new byte[16384];
		}

		// Token: 0x060006A7 RID: 1703 RVA: 0x00035734 File Offset: 0x00033934
		public void Reset()
		{
			this.last_lit = 0;
			this.extra_bits = 0;
			this.literalTree.Reset();
			this.distTree.Reset();
			this.blTree.Reset();
		}

		// Token: 0x060006A8 RID: 1704 RVA: 0x0003576C File Offset: 0x0003396C
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

		// Token: 0x060006A9 RID: 1705 RVA: 0x00035844 File Offset: 0x00033A44
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

		// Token: 0x060006AA RID: 1706 RVA: 0x00035968 File Offset: 0x00033B68
		public void FlushStoredBlock(byte[] stored, int storedOffset, int storedLength, bool lastBlock)
		{
			this.pending.WriteBits(lastBlock ? 1 : 0, 3);
			this.pending.AlignToByte();
			this.pending.WriteShort(storedLength);
			this.pending.WriteShort(~storedLength);
			this.pending.WriteBlock(stored, storedOffset, storedLength);
			this.Reset();
		}

		// Token: 0x060006AB RID: 1707 RVA: 0x000359D4 File Offset: 0x00033BD4
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

		// Token: 0x060006AC RID: 1708 RVA: 0x00035C20 File Offset: 0x00033E20
		public bool IsFull()
		{
			return this.last_lit >= 16384;
		}

		// Token: 0x060006AD RID: 1709 RVA: 0x00035C4C File Offset: 0x00033E4C
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

		// Token: 0x060006AE RID: 1710 RVA: 0x00035CAC File Offset: 0x00033EAC
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

		// Token: 0x060006AF RID: 1711 RVA: 0x00035D94 File Offset: 0x00033F94
		public static short BitReverse(int toReverse)
		{
			return (short)((int)DeflaterHuffman.bit4Reverse[toReverse & 15] << 12 | (int)DeflaterHuffman.bit4Reverse[toReverse >> 4 & 15] << 8 | (int)DeflaterHuffman.bit4Reverse[toReverse >> 8 & 15] << 4 | (int)DeflaterHuffman.bit4Reverse[toReverse >> 12]);
		}

		// Token: 0x060006B0 RID: 1712 RVA: 0x00035DE4 File Offset: 0x00033FE4
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

		// Token: 0x060006B1 RID: 1713 RVA: 0x00035E3C File Offset: 0x0003403C
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

		// Token: 0x040003FC RID: 1020
		private const int BUFSIZE = 16384;

		// Token: 0x040003FD RID: 1021
		private const int LITERAL_NUM = 286;

		// Token: 0x040003FE RID: 1022
		private const int DIST_NUM = 30;

		// Token: 0x040003FF RID: 1023
		private const int BITLEN_NUM = 19;

		// Token: 0x04000400 RID: 1024
		private const int REP_3_6 = 16;

		// Token: 0x04000401 RID: 1025
		private const int REP_3_10 = 17;

		// Token: 0x04000402 RID: 1026
		private const int REP_11_138 = 18;

		// Token: 0x04000403 RID: 1027
		private const int EOF_SYMBOL = 256;

		// Token: 0x04000404 RID: 1028
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

		// Token: 0x04000405 RID: 1029
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

		// Token: 0x04000406 RID: 1030
		private static short[] staticLCodes = new short[286];

		// Token: 0x04000407 RID: 1031
		private static byte[] staticLLength = new byte[286];

		// Token: 0x04000408 RID: 1032
		private static short[] staticDCodes;

		// Token: 0x04000409 RID: 1033
		private static byte[] staticDLength;

		// Token: 0x0400040A RID: 1034
		public DeflaterPending pending;

		// Token: 0x0400040B RID: 1035
		private DeflaterHuffman.Tree literalTree;

		// Token: 0x0400040C RID: 1036
		private DeflaterHuffman.Tree distTree;

		// Token: 0x0400040D RID: 1037
		private DeflaterHuffman.Tree blTree;

		// Token: 0x0400040E RID: 1038
		private short[] d_buf;

		// Token: 0x0400040F RID: 1039
		private byte[] l_buf;

		// Token: 0x04000410 RID: 1040
		private int last_lit;

		// Token: 0x04000411 RID: 1041
		private int extra_bits;

		// Token: 0x02000157 RID: 343
		private class Tree
		{
			// Token: 0x06000B00 RID: 2816 RVA: 0x0004BB9C File Offset: 0x00049D9C
			public Tree(DeflaterHuffman dh, int elems, int minCodes, int maxLength)
			{
				this.dh = dh;
				this.minNumCodes = minCodes;
				this.maxLength = maxLength;
				this.freqs = new short[elems];
				this.bl_counts = new int[maxLength];
			}

			// Token: 0x06000B01 RID: 2817 RVA: 0x0004BBD8 File Offset: 0x00049DD8
			public void Reset()
			{
				for (int i = 0; i < this.freqs.Length; i++)
				{
					this.freqs[i] = 0;
				}
				this.codes = null;
				this.length = null;
			}

			// Token: 0x06000B02 RID: 2818 RVA: 0x0004BC20 File Offset: 0x00049E20
			public void WriteSymbol(int code)
			{
				this.dh.pending.WriteBits((int)this.codes[code] & 65535, (int)this.length[code]);
			}

			// Token: 0x06000B03 RID: 2819 RVA: 0x0004BC4C File Offset: 0x00049E4C
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

			// Token: 0x06000B04 RID: 2820 RVA: 0x0004BCAC File Offset: 0x00049EAC
			public void SetStaticCodes(short[] staticCodes, byte[] staticLengths)
			{
				this.codes = staticCodes;
				this.length = staticLengths;
			}

			// Token: 0x06000B05 RID: 2821 RVA: 0x0004BCC0 File Offset: 0x00049EC0
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

			// Token: 0x06000B06 RID: 2822 RVA: 0x0004BD98 File Offset: 0x00049F98
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

			// Token: 0x06000B07 RID: 2823 RVA: 0x0004C0D0 File Offset: 0x0004A2D0
			public int GetEncodedLength()
			{
				int num = 0;
				for (int i = 0; i < this.freqs.Length; i++)
				{
					num += (int)(this.freqs[i] * (short)this.length[i]);
				}
				return num;
			}

			// Token: 0x06000B08 RID: 2824 RVA: 0x0004C11C File Offset: 0x0004A31C
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

			// Token: 0x06000B09 RID: 2825 RVA: 0x0004C280 File Offset: 0x0004A480
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

			// Token: 0x06000B0A RID: 2826 RVA: 0x0004C408 File Offset: 0x0004A608
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

			// Token: 0x040007C6 RID: 1990
			public short[] freqs;

			// Token: 0x040007C7 RID: 1991
			public byte[] length;

			// Token: 0x040007C8 RID: 1992
			public int minNumCodes;

			// Token: 0x040007C9 RID: 1993
			public int numCodes;

			// Token: 0x040007CA RID: 1994
			private short[] codes;

			// Token: 0x040007CB RID: 1995
			private int[] bl_counts;

			// Token: 0x040007CC RID: 1996
			private int maxLength;

			// Token: 0x040007CD RID: 1997
			private DeflaterHuffman dh;
		}
	}
}
