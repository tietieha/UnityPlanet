using System;
using UnityEngine;
using UWA;

namespace UWACore.Util
{
	// Token: 0x0200002F RID: 47
	public class JPGEncoder
	{
		// Token: 0x06000209 RID: 521 RVA: 0x0000CCFC File Offset: 0x0000AEFC
		private void initQuantTables(int sf)
		{
			int[] array = new int[]
			{
				16,
				11,
				10,
				16,
				24,
				40,
				51,
				61,
				12,
				12,
				14,
				19,
				26,
				58,
				60,
				55,
				14,
				13,
				16,
				24,
				40,
				57,
				69,
				56,
				14,
				17,
				22,
				29,
				51,
				87,
				80,
				62,
				18,
				22,
				37,
				56,
				68,
				109,
				103,
				77,
				24,
				35,
				55,
				64,
				81,
				104,
				113,
				92,
				49,
				64,
				78,
				87,
				103,
				121,
				120,
				101,
				72,
				92,
				95,
				98,
				112,
				100,
				103,
				99
			};
			int i;
			for (i = 0; i < 64; i++)
			{
				float num = Mathf.Floor(((float)(array[i] * sf) + 50f) / 100f);
				if (num < 1f)
				{
					num = 1f;
				}
				else if (num > 255f)
				{
					num = 255f;
				}
				this.YTable[this.ZigZag[i]] = (int)num;
			}
			int[] array2 = new int[]
			{
				17,
				18,
				24,
				47,
				99,
				99,
				99,
				99,
				18,
				21,
				26,
				66,
				99,
				99,
				99,
				99,
				24,
				26,
				56,
				99,
				99,
				99,
				99,
				99,
				47,
				66,
				99,
				99,
				99,
				99,
				99,
				99,
				99,
				99,
				99,
				99,
				99,
				99,
				99,
				99,
				99,
				99,
				99,
				99,
				99,
				99,
				99,
				99,
				99,
				99,
				99,
				99,
				99,
				99,
				99,
				99,
				99,
				99,
				99,
				99,
				99,
				99,
				99,
				99
			};
			for (i = 0; i < 64; i++)
			{
				float num = Mathf.Floor(((float)(array2[i] * sf) + 50f) / 100f);
				if (num < 1f)
				{
					num = 1f;
				}
				else if (num > 255f)
				{
					num = 255f;
				}
				this.UVTable[this.ZigZag[i]] = (int)num;
			}
			float[] array3 = new float[]
			{
				1f,
				1.3870399f,
				1.306563f,
				1.1758755f,
				1f,
				0.78569496f,
				0.5411961f,
				0.27589938f
			};
			i = 0;
			for (int j = 0; j < 8; j++)
			{
				for (int k = 0; k < 8; k++)
				{
					this.fdtbl_Y[i] = 1f / ((float)this.YTable[this.ZigZag[i]] * array3[j] * array3[k] * 8f);
					this.fdtbl_UV[i] = 1f / ((float)this.UVTable[this.ZigZag[i]] * array3[j] * array3[k] * 8f);
					i++;
				}
			}
		}

		// Token: 0x0600020A RID: 522 RVA: 0x0000CE98 File Offset: 0x0000B098
		private BitString[] computeHuffmanTbl(int[] nrcodes, int[] std_table)
		{
			int num = 0;
			int num2 = 0;
			BitString[] array = new BitString[256];
			for (int i = 1; i <= 16; i++)
			{
				for (int j = 1; j <= nrcodes[i]; j++)
				{
					array[std_table[num2]] = new BitString();
					array[std_table[num2]].val = num;
					array[std_table[num2]].len = i;
					num2++;
					num++;
				}
				num *= 2;
			}
			return array;
		}

		// Token: 0x0600020B RID: 523 RVA: 0x0000CF18 File Offset: 0x0000B118
		private void initHuffmanTbl()
		{
			JPGEncoder.YDC_HT = this.computeHuffmanTbl(this.std_dc_luminance_nrcodes, this.std_dc_luminance_values);
			JPGEncoder.UVDC_HT = this.computeHuffmanTbl(this.std_dc_chrominance_nrcodes, this.std_dc_chrominance_values);
			JPGEncoder.YAC_HT = this.computeHuffmanTbl(this.std_ac_luminance_nrcodes, this.std_ac_luminance_values);
			JPGEncoder.UVAC_HT = this.computeHuffmanTbl(this.std_ac_chrominance_nrcodes, this.std_ac_chrominance_values);
		}

		// Token: 0x0600020C RID: 524 RVA: 0x0000CF88 File Offset: 0x0000B188
		private void initCategoryfloat()
		{
			int num = 1;
			int num2 = 2;
			for (int i = 1; i <= 15; i++)
			{
				for (int j = num; j < num2; j++)
				{
					this.category[32767 + j] = i;
					BitString bitString = new BitString();
					bitString.len = i;
					bitString.val = j;
					this.bitcode[32767 + j] = bitString;
				}
				for (int j = -(num2 - 1); j <= -num; j++)
				{
					this.category[32767 + j] = i;
					BitString bitString = new BitString();
					bitString.len = i;
					bitString.val = num2 - 1 + j;
					this.bitcode[32767 + j] = bitString;
				}
				num <<= 1;
				num2 <<= 1;
			}
		}

		// Token: 0x0600020D RID: 525 RVA: 0x0000D054 File Offset: 0x0000B254
		public byte[] GetBytes()
		{
			if (!this.isDone)
			{
				SharedUtils.Log("JPEGEncoder not complete, cannot get bytes!");
				return new byte[1];
			}
			return this.byteout.GetAllBytes();
		}

		// Token: 0x0600020E RID: 526 RVA: 0x0000D080 File Offset: 0x0000B280
		public void WriteTo(string path)
		{
			this.byteout.WriteTo(path);
		}

		// Token: 0x0600020F RID: 527 RVA: 0x0000D090 File Offset: 0x0000B290
		private void writeBits(BitString bs)
		{
			int val = bs.val;
			int i = bs.len - 1;
			while (i >= 0)
			{
				if ((val & (int)Convert.ToUInt32(1 << i)) != 0)
				{
					this.bytenew |= (int)Convert.ToUInt32(1 << this.bytepos);
				}
				i--;
				this.bytepos--;
				if (this.bytepos < 0)
				{
					if (this.bytenew == 255)
					{
						this.writeByte(byte.MaxValue);
						this.writeByte(0);
					}
					else
					{
						this.writeByte((byte)this.bytenew);
					}
					this.bytepos = 7;
					this.bytenew = 0;
				}
			}
		}

		// Token: 0x06000210 RID: 528 RVA: 0x0000D14C File Offset: 0x0000B34C
		private void writeByte(byte value)
		{
			this.byteout.writeByte(value);
		}

		// Token: 0x06000211 RID: 529 RVA: 0x0000D15C File Offset: 0x0000B35C
		private void writeWord(int value)
		{
			this.writeByte((byte)(value >> 8 & 255));
			this.writeByte((byte)(value & 255));
		}

		// Token: 0x06000212 RID: 530 RVA: 0x0000D17C File Offset: 0x0000B37C
		private float[] fDCTQuant(float[] data, float[] fdtbl)
		{
			int num = 0;
			for (int i = 0; i < 8; i++)
			{
				float num2 = data[num] + data[num + 7];
				float num3 = data[num] - data[num + 7];
				float num4 = data[num + 1] + data[num + 6];
				float num5 = data[num + 1] - data[num + 6];
				float num6 = data[num + 2] + data[num + 5];
				float num7 = data[num + 2] - data[num + 5];
				float num8 = data[num + 3] + data[num + 4];
				float num9 = data[num + 3] - data[num + 4];
				float num10 = num2 + num8;
				float num11 = num2 - num8;
				float num12 = num4 + num6;
				float num13 = num4 - num6;
				data[num] = num10 + num12;
				data[num + 4] = num10 - num12;
				float num14 = (num13 + num11) * 0.70710677f;
				data[num + 2] = num11 + num14;
				data[num + 6] = num11 - num14;
				num10 = num9 + num7;
				num12 = num7 + num5;
				num13 = num5 + num3;
				float num15 = (num10 - num13) * 0.38268343f;
				float num16 = 0.5411961f * num10 + num15;
				float num17 = 1.306563f * num13 + num15;
				float num18 = num12 * 0.70710677f;
				float num19 = num3 + num18;
				float num20 = num3 - num18;
				data[num + 5] = num20 + num16;
				data[num + 3] = num20 - num16;
				data[num + 1] = num19 + num17;
				data[num + 7] = num19 - num17;
				num += 8;
			}
			num = 0;
			for (int i = 0; i < 8; i++)
			{
				float num2 = data[num] + data[num + 56];
				float num3 = data[num] - data[num + 56];
				float num4 = data[num + 8] + data[num + 48];
				float num5 = data[num + 8] - data[num + 48];
				float num6 = data[num + 16] + data[num + 40];
				float num7 = data[num + 16] - data[num + 40];
				float num8 = data[num + 24] + data[num + 32];
				float num9 = data[num + 24] - data[num + 32];
				float num10 = num2 + num8;
				float num11 = num2 - num8;
				float num12 = num4 + num6;
				float num13 = num4 - num6;
				data[num] = num10 + num12;
				data[num + 32] = num10 - num12;
				float num14 = (num13 + num11) * 0.70710677f;
				data[num + 16] = num11 + num14;
				data[num + 48] = num11 - num14;
				num10 = num9 + num7;
				num12 = num7 + num5;
				num13 = num5 + num3;
				float num15 = (num10 - num13) * 0.38268343f;
				float num16 = 0.5411961f * num10 + num15;
				float num17 = 1.306563f * num13 + num15;
				float num18 = num12 * 0.70710677f;
				float num19 = num3 + num18;
				float num20 = num3 - num18;
				data[num + 40] = num20 + num16;
				data[num + 24] = num20 - num16;
				data[num + 8] = num19 + num17;
				data[num + 56] = num19 - num17;
				num++;
			}
			for (int i = 0; i < 64; i++)
			{
				data[i] = Mathf.Round(data[i] * fdtbl[i]);
			}
			return data;
		}

		// Token: 0x06000213 RID: 531 RVA: 0x0000D440 File Offset: 0x0000B640
		private void writeAPP0()
		{
			this.writeWord(65504);
			this.writeWord(16);
			this.writeByte(74);
			this.writeByte(70);
			this.writeByte(73);
			this.writeByte(70);
			this.writeByte(0);
			this.writeByte(1);
			this.writeByte(1);
			this.writeByte(0);
			this.writeWord(1);
			this.writeWord(1);
			this.writeByte(0);
			this.writeByte(0);
		}

		// Token: 0x06000214 RID: 532 RVA: 0x0000D4BC File Offset: 0x0000B6BC
		private void writeSOF0(int width, int height)
		{
			this.writeWord(65472);
			this.writeWord(17);
			this.writeByte(8);
			this.writeWord(height);
			this.writeWord(width);
			this.writeByte(3);
			this.writeByte(1);
			this.writeByte(17);
			this.writeByte(0);
			this.writeByte(2);
			this.writeByte(17);
			this.writeByte(1);
			this.writeByte(3);
			this.writeByte(17);
			this.writeByte(1);
		}

		// Token: 0x06000215 RID: 533 RVA: 0x0000D540 File Offset: 0x0000B740
		private void writeDQT()
		{
			this.writeWord(65499);
			this.writeWord(132);
			this.writeByte(0);
			for (int i = 0; i < 64; i++)
			{
				this.writeByte((byte)this.YTable[i]);
			}
			this.writeByte(1);
			for (int i = 0; i < 64; i++)
			{
				this.writeByte((byte)this.UVTable[i]);
			}
		}

		// Token: 0x06000216 RID: 534 RVA: 0x0000D5B4 File Offset: 0x0000B7B4
		private void writeDHT()
		{
			this.writeWord(65476);
			this.writeWord(418);
			this.writeByte(0);
			for (int i = 0; i < 16; i++)
			{
				this.writeByte((byte)this.std_dc_luminance_nrcodes[i + 1]);
			}
			for (int i = 0; i <= 11; i++)
			{
				this.writeByte((byte)this.std_dc_luminance_values[i]);
			}
			this.writeByte(16);
			for (int i = 0; i < 16; i++)
			{
				this.writeByte((byte)this.std_ac_luminance_nrcodes[i + 1]);
			}
			for (int i = 0; i <= 161; i++)
			{
				this.writeByte((byte)this.std_ac_luminance_values[i]);
			}
			this.writeByte(1);
			for (int i = 0; i < 16; i++)
			{
				this.writeByte((byte)this.std_dc_chrominance_nrcodes[i + 1]);
			}
			for (int i = 0; i <= 11; i++)
			{
				this.writeByte((byte)this.std_dc_chrominance_values[i]);
			}
			this.writeByte(17);
			for (int i = 0; i < 16; i++)
			{
				this.writeByte((byte)this.std_ac_chrominance_nrcodes[i + 1]);
			}
			for (int i = 0; i <= 161; i++)
			{
				this.writeByte((byte)this.std_ac_chrominance_values[i]);
			}
		}

		// Token: 0x06000217 RID: 535 RVA: 0x0000D700 File Offset: 0x0000B900
		private void writeSOS()
		{
			this.writeWord(65498);
			this.writeWord(12);
			this.writeByte(3);
			this.writeByte(1);
			this.writeByte(0);
			this.writeByte(2);
			this.writeByte(17);
			this.writeByte(3);
			this.writeByte(17);
			this.writeByte(0);
			this.writeByte(63);
			this.writeByte(0);
		}

		// Token: 0x06000218 RID: 536 RVA: 0x0000D770 File Offset: 0x0000B970
		private float processDU(float[] CDU, float[] fdtbl, float DC, BitString[] HTDC, BitString[] HTAC)
		{
			BitString bs = HTAC[0];
			BitString bs2 = HTAC[240];
			float[] array = this.fDCTQuant(CDU, fdtbl);
			for (int i = 0; i < 64; i++)
			{
				this.DU[this.ZigZag[i]] = (int)array[i];
			}
			int num = (int)((float)this.DU[0] - DC);
			DC = (float)this.DU[0];
			if (num == 0)
			{
				this.writeBits(HTDC[0]);
			}
			else
			{
				this.writeBits(HTDC[this.category[32767 + num]]);
				this.writeBits(this.bitcode[32767 + num]);
			}
			int num2 = 63;
			while (num2 > 0 && this.DU[num2] == 0)
			{
				num2--;
			}
			if (num2 == 0)
			{
				this.writeBits(bs);
				return DC;
			}
			for (int i = 1; i <= num2; i++)
			{
				int num3 = i;
				while (this.DU[i] == 0 && i <= num2)
				{
					i++;
				}
				int num4 = i - num3;
				if (num4 >= 16)
				{
					for (int j = 1; j <= num4 / 16; j++)
					{
						this.writeBits(bs2);
					}
					num4 &= 15;
				}
				this.writeBits(HTAC[num4 * 16 + this.category[32767 + this.DU[i]]]);
				this.writeBits(this.bitcode[32767 + this.DU[i]]);
			}
			if (num2 != 63)
			{
				this.writeBits(bs);
			}
			return DC;
		}

		// Token: 0x06000219 RID: 537 RVA: 0x0000D91C File Offset: 0x0000BB1C
		private void RGB2YUV(BitmapData img, int xpos, int ypos)
		{
			int num = 0;
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					Color32 pixelColor = img.getPixelColor32(xpos + j, img.height - (ypos + i));
					this.YDU[num] = 0.299f * (float)pixelColor.r + 0.587f * (float)pixelColor.g + 0.114f * (float)pixelColor.b - 128f;
					this.UDU[num] = -0.16874f * (float)pixelColor.r + -0.33126f * (float)pixelColor.g + 0.5f * (float)pixelColor.b;
					this.VDU[num] = 0.5f * (float)pixelColor.r + -0.41869f * (float)pixelColor.g + -0.08131f * (float)pixelColor.b;
					num++;
				}
			}
		}

		// Token: 0x0600021A RID: 538 RVA: 0x0000DA04 File Offset: 0x0000BC04
		public void ResetData(Color32[] pixels, int width, int height)
		{
			this.image = new BitmapData(pixels, width, height);
		}

		// Token: 0x0600021B RID: 539 RVA: 0x0000DA14 File Offset: 0x0000BC14
		public JPGEncoder(Color32[] pixels, int width, int height, float quality)
		{
			this.image = new BitmapData(pixels, width, height);
			if (quality <= 0f)
			{
				quality = 1f;
			}
			if (quality > 100f)
			{
				quality = 100f;
			}
			if (quality < 50f)
			{
				this.sf = (int)(5000f / quality);
			}
			else
			{
				this.sf = (int)(200f - quality * 2f);
			}
			this.initHuffmanTbl();
			this.initCategoryfloat();
			this.initQuantTables(this.sf);
		}

		// Token: 0x0600021C RID: 540 RVA: 0x0000DC24 File Offset: 0x0000BE24
		public void doEncoding()
		{
			this.isDone = false;
			this.encode();
			this.isDone = true;
			this.image = null;
		}

		// Token: 0x0600021D RID: 541 RVA: 0x0000DC44 File Offset: 0x0000BE44
		private void encode()
		{
			this.byteout = new ByteArray();
			this.bytenew = 0;
			this.bytepos = 7;
			this.writeWord(65496);
			this.writeAPP0();
			this.writeDQT();
			this.writeSOF0(this.image.width, this.image.height);
			this.writeDHT();
			this.writeSOS();
			float dc = 0f;
			float dc2 = 0f;
			float dc3 = 0f;
			this.bytenew = 0;
			this.bytepos = 7;
			for (int i = 0; i < this.image.height; i += 8)
			{
				for (int j = 0; j < this.image.width; j += 8)
				{
					this.RGB2YUV(this.image, j, i);
					dc = this.processDU(this.YDU, this.fdtbl_Y, dc, JPGEncoder.YDC_HT, JPGEncoder.YAC_HT);
					dc2 = this.processDU(this.UDU, this.fdtbl_UV, dc2, JPGEncoder.UVDC_HT, JPGEncoder.UVAC_HT);
					dc3 = this.processDU(this.VDU, this.fdtbl_UV, dc3, JPGEncoder.UVDC_HT, JPGEncoder.UVAC_HT);
				}
			}
			if (this.bytepos >= 0)
			{
				this.writeBits(new BitString
				{
					len = this.bytepos + 1,
					val = (1 << this.bytepos + 1) - 1
				});
			}
			this.writeWord(65497);
			this.isDone = true;
		}

		// Token: 0x04000104 RID: 260
		public int[] ZigZag = new int[]
		{
			0,
			1,
			5,
			6,
			14,
			15,
			27,
			28,
			2,
			4,
			7,
			13,
			16,
			26,
			29,
			42,
			3,
			8,
			12,
			17,
			25,
			30,
			41,
			43,
			9,
			11,
			18,
			24,
			31,
			40,
			44,
			53,
			10,
			19,
			23,
			32,
			39,
			45,
			52,
			54,
			20,
			22,
			33,
			38,
			46,
			51,
			55,
			60,
			21,
			34,
			37,
			47,
			50,
			56,
			59,
			61,
			35,
			36,
			48,
			49,
			57,
			58,
			62,
			63
		};

		// Token: 0x04000105 RID: 261
		private int[] YTable = new int[64];

		// Token: 0x04000106 RID: 262
		private int[] UVTable = new int[64];

		// Token: 0x04000107 RID: 263
		private float[] fdtbl_Y = new float[64];

		// Token: 0x04000108 RID: 264
		private float[] fdtbl_UV = new float[64];

		// Token: 0x04000109 RID: 265
		private static BitString[] YDC_HT;

		// Token: 0x0400010A RID: 266
		private static BitString[] UVDC_HT;

		// Token: 0x0400010B RID: 267
		private static BitString[] YAC_HT;

		// Token: 0x0400010C RID: 268
		private static BitString[] UVAC_HT;

		// Token: 0x0400010D RID: 269
		private int[] std_dc_luminance_nrcodes = new int[]
		{
			0,
			0,
			1,
			5,
			1,
			1,
			1,
			1,
			1,
			1,
			0,
			0,
			0,
			0,
			0,
			0,
			0
		};

		// Token: 0x0400010E RID: 270
		private int[] std_dc_luminance_values = new int[]
		{
			0,
			1,
			2,
			3,
			4,
			5,
			6,
			7,
			8,
			9,
			10,
			11
		};

		// Token: 0x0400010F RID: 271
		private int[] std_ac_luminance_nrcodes = new int[]
		{
			0,
			0,
			2,
			1,
			3,
			3,
			2,
			4,
			3,
			5,
			5,
			4,
			4,
			0,
			0,
			1,
			125
		};

		// Token: 0x04000110 RID: 272
		private int[] std_ac_luminance_values = new int[]
		{
			1,
			2,
			3,
			0,
			4,
			17,
			5,
			18,
			33,
			49,
			65,
			6,
			19,
			81,
			97,
			7,
			34,
			113,
			20,
			50,
			129,
			145,
			161,
			8,
			35,
			66,
			177,
			193,
			21,
			82,
			209,
			240,
			36,
			51,
			98,
			114,
			130,
			9,
			10,
			22,
			23,
			24,
			25,
			26,
			37,
			38,
			39,
			40,
			41,
			42,
			52,
			53,
			54,
			55,
			56,
			57,
			58,
			67,
			68,
			69,
			70,
			71,
			72,
			73,
			74,
			83,
			84,
			85,
			86,
			87,
			88,
			89,
			90,
			99,
			100,
			101,
			102,
			103,
			104,
			105,
			106,
			115,
			116,
			117,
			118,
			119,
			120,
			121,
			122,
			131,
			132,
			133,
			134,
			135,
			136,
			137,
			138,
			146,
			147,
			148,
			149,
			150,
			151,
			152,
			153,
			154,
			162,
			163,
			164,
			165,
			166,
			167,
			168,
			169,
			170,
			178,
			179,
			180,
			181,
			182,
			183,
			184,
			185,
			186,
			194,
			195,
			196,
			197,
			198,
			199,
			200,
			201,
			202,
			210,
			211,
			212,
			213,
			214,
			215,
			216,
			217,
			218,
			225,
			226,
			227,
			228,
			229,
			230,
			231,
			232,
			233,
			234,
			241,
			242,
			243,
			244,
			245,
			246,
			247,
			248,
			249,
			250
		};

		// Token: 0x04000111 RID: 273
		private int[] std_dc_chrominance_nrcodes = new int[]
		{
			0,
			0,
			3,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			0,
			0,
			0,
			0,
			0
		};

		// Token: 0x04000112 RID: 274
		private int[] std_dc_chrominance_values = new int[]
		{
			0,
			1,
			2,
			3,
			4,
			5,
			6,
			7,
			8,
			9,
			10,
			11
		};

		// Token: 0x04000113 RID: 275
		private int[] std_ac_chrominance_nrcodes = new int[]
		{
			0,
			0,
			2,
			1,
			2,
			4,
			4,
			3,
			4,
			7,
			5,
			4,
			4,
			0,
			1,
			2,
			119
		};

		// Token: 0x04000114 RID: 276
		private int[] std_ac_chrominance_values = new int[]
		{
			0,
			1,
			2,
			3,
			17,
			4,
			5,
			33,
			49,
			6,
			18,
			65,
			81,
			7,
			97,
			113,
			19,
			34,
			50,
			129,
			8,
			20,
			66,
			145,
			161,
			177,
			193,
			9,
			35,
			51,
			82,
			240,
			21,
			98,
			114,
			209,
			10,
			22,
			36,
			52,
			225,
			37,
			241,
			23,
			24,
			25,
			26,
			38,
			39,
			40,
			41,
			42,
			53,
			54,
			55,
			56,
			57,
			58,
			67,
			68,
			69,
			70,
			71,
			72,
			73,
			74,
			83,
			84,
			85,
			86,
			87,
			88,
			89,
			90,
			99,
			100,
			101,
			102,
			103,
			104,
			105,
			106,
			115,
			116,
			117,
			118,
			119,
			120,
			121,
			122,
			130,
			131,
			132,
			133,
			134,
			135,
			136,
			137,
			138,
			146,
			147,
			148,
			149,
			150,
			151,
			152,
			153,
			154,
			162,
			163,
			164,
			165,
			166,
			167,
			168,
			169,
			170,
			178,
			179,
			180,
			181,
			182,
			183,
			184,
			185,
			186,
			194,
			195,
			196,
			197,
			198,
			199,
			200,
			201,
			202,
			210,
			211,
			212,
			213,
			214,
			215,
			216,
			217,
			218,
			226,
			227,
			228,
			229,
			230,
			231,
			232,
			233,
			234,
			242,
			243,
			244,
			245,
			246,
			247,
			248,
			249,
			250
		};

		// Token: 0x04000115 RID: 277
		private BitString[] bitcode = new BitString[65535];

		// Token: 0x04000116 RID: 278
		private int[] category = new int[65535];

		// Token: 0x04000117 RID: 279
		private int bytenew;

		// Token: 0x04000118 RID: 280
		private int bytepos = 7;

		// Token: 0x04000119 RID: 281
		public ByteArray byteout = new ByteArray();

		// Token: 0x0400011A RID: 282
		private int[] DU = new int[64];

		// Token: 0x0400011B RID: 283
		private float[] YDU = new float[64];

		// Token: 0x0400011C RID: 284
		private float[] UDU = new float[64];

		// Token: 0x0400011D RID: 285
		private float[] VDU = new float[64];

		// Token: 0x0400011E RID: 286
		public bool isDone;

		// Token: 0x0400011F RID: 287
		private BitmapData image;

		// Token: 0x04000120 RID: 288
		private int sf;
	}
}
