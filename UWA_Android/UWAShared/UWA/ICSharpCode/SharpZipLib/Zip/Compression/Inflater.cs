using System;
using System.Runtime.InteropServices;
using UWA.ICSharpCode.SharpZipLib.Checksums;
using UWA.ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace UWA.ICSharpCode.SharpZipLib.Zip.Compression
{
	// Token: 0x02000094 RID: 148
	[ComVisible(false)]
	public class Inflater
	{
		// Token: 0x060006B3 RID: 1715 RVA: 0x00035E88 File Offset: 0x00034088
		public Inflater() : this(false)
		{
		}

		// Token: 0x060006B4 RID: 1716 RVA: 0x00035E94 File Offset: 0x00034094
		public Inflater(bool noHeader)
		{
			this.noHeader = noHeader;
			this.adler = new Adler32();
			this.input = new StreamManipulator();
			this.outputWindow = new OutputWindow();
			this.mode = (noHeader ? 2 : 0);
		}

		// Token: 0x060006B5 RID: 1717 RVA: 0x00035EE8 File Offset: 0x000340E8
		public void Reset()
		{
			this.mode = (this.noHeader ? 2 : 0);
			this.totalIn = 0L;
			this.totalOut = 0L;
			this.input.Reset();
			this.outputWindow.Reset();
			this.dynHeader = null;
			this.litlenTree = null;
			this.distTree = null;
			this.isLastBlock = false;
			this.adler.Reset();
		}

		// Token: 0x060006B6 RID: 1718 RVA: 0x00035F64 File Offset: 0x00034164
		private bool DecodeHeader()
		{
			int num = this.input.PeekBits(16);
			bool flag = num < 0;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				this.input.DropBits(16);
				num = ((num << 8 | num >> 8) & 65535);
				bool flag2 = num % 31 != 0;
				if (flag2)
				{
					throw new SharpZipBaseException("Header checksum illegal");
				}
				bool flag3 = (num & 3840) != 2048;
				if (flag3)
				{
					throw new SharpZipBaseException("Compression Method unknown");
				}
				bool flag4 = (num & 32) == 0;
				if (flag4)
				{
					this.mode = 2;
				}
				else
				{
					this.mode = 1;
					this.neededBits = 32;
				}
				result = true;
			}
			return result;
		}

		// Token: 0x060006B7 RID: 1719 RVA: 0x0003602C File Offset: 0x0003422C
		private bool DecodeDict()
		{
			while (this.neededBits > 0)
			{
				int num = this.input.PeekBits(8);
				bool flag = num < 0;
				if (flag)
				{
					return false;
				}
				this.input.DropBits(8);
				this.readAdler = (this.readAdler << 8 | num);
				this.neededBits -= 8;
			}
			return false;
		}

		// Token: 0x060006B8 RID: 1720 RVA: 0x000360A8 File Offset: 0x000342A8
		private bool DecodeHuffman()
		{
			int i = this.outputWindow.GetFreeSpace();
			while (i >= 258)
			{
				int symbol;
				switch (this.mode)
				{
				case 7:
				{
					while (((symbol = this.litlenTree.GetSymbol(this.input)) & -256) == 0)
					{
						this.outputWindow.Write(symbol);
						bool flag = --i < 258;
						if (flag)
						{
							return true;
						}
					}
					bool flag2 = symbol < 257;
					if (!flag2)
					{
						try
						{
							this.repLength = Inflater.CPLENS[symbol - 257];
							this.neededBits = Inflater.CPLEXT[symbol - 257];
						}
						catch (Exception)
						{
							throw new SharpZipBaseException("Illegal rep length code");
						}
						goto IL_113;
					}
					bool flag3 = symbol < 0;
					if (flag3)
					{
						return false;
					}
					this.distTree = null;
					this.litlenTree = null;
					this.mode = 2;
					return true;
				}
				case 8:
					goto IL_113;
				case 9:
					goto IL_186;
				case 10:
					break;
				default:
					throw new SharpZipBaseException("Inflater unknown mode");
				}
				IL_1E2:
				bool flag4 = this.neededBits > 0;
				if (flag4)
				{
					this.mode = 10;
					int num = this.input.PeekBits(this.neededBits);
					bool flag5 = num < 0;
					if (flag5)
					{
						return false;
					}
					this.input.DropBits(this.neededBits);
					this.repDist += num;
				}
				this.outputWindow.Repeat(this.repLength, this.repDist);
				i -= this.repLength;
				this.mode = 7;
				continue;
				IL_186:
				symbol = this.distTree.GetSymbol(this.input);
				bool flag6 = symbol < 0;
				if (flag6)
				{
					return false;
				}
				try
				{
					this.repDist = Inflater.CPDIST[symbol];
					this.neededBits = Inflater.CPDEXT[symbol];
				}
				catch (Exception)
				{
					throw new SharpZipBaseException("Illegal rep dist code");
				}
				goto IL_1E2;
				IL_113:
				bool flag7 = this.neededBits > 0;
				if (flag7)
				{
					this.mode = 8;
					int num2 = this.input.PeekBits(this.neededBits);
					bool flag8 = num2 < 0;
					if (flag8)
					{
						return false;
					}
					this.input.DropBits(this.neededBits);
					this.repLength += num2;
				}
				this.mode = 9;
				goto IL_186;
			}
			return true;
		}

		// Token: 0x060006B9 RID: 1721 RVA: 0x00036374 File Offset: 0x00034574
		private bool DecodeChksum()
		{
			while (this.neededBits > 0)
			{
				int num = this.input.PeekBits(8);
				bool flag = num < 0;
				if (flag)
				{
					return false;
				}
				this.input.DropBits(8);
				this.readAdler = (this.readAdler << 8 | num);
				this.neededBits -= 8;
			}
			bool flag2 = (int)this.adler.Value != this.readAdler;
			if (flag2)
			{
				throw new SharpZipBaseException("Adler chksum doesn't match: " + ((int)this.adler.Value).ToString() + " vs. " + this.readAdler.ToString());
			}
			this.mode = 12;
			return false;
		}

		// Token: 0x060006BA RID: 1722 RVA: 0x0003644C File Offset: 0x0003464C
		private bool Decode()
		{
			switch (this.mode)
			{
			case 0:
				return this.DecodeHeader();
			case 1:
				return this.DecodeDict();
			case 2:
			{
				bool flag = this.isLastBlock;
				if (flag)
				{
					bool flag2 = this.noHeader;
					if (flag2)
					{
						this.mode = 12;
						return false;
					}
					this.input.SkipToByteBoundary();
					this.neededBits = 32;
					this.mode = 11;
					return true;
				}
				else
				{
					int num = this.input.PeekBits(3);
					bool flag3 = num < 0;
					if (flag3)
					{
						return false;
					}
					this.input.DropBits(3);
					bool flag4 = (num & 1) != 0;
					if (flag4)
					{
						this.isLastBlock = true;
					}
					switch (num >> 1)
					{
					case 0:
						this.input.SkipToByteBoundary();
						this.mode = 3;
						break;
					case 1:
						this.litlenTree = InflaterHuffmanTree.defLitLenTree;
						this.distTree = InflaterHuffmanTree.defDistTree;
						this.mode = 7;
						break;
					case 2:
						this.dynHeader = new InflaterDynHeader();
						this.mode = 6;
						break;
					default:
						throw new SharpZipBaseException("Unknown block type " + num.ToString());
					}
					return true;
				}
				break;
			}
			case 3:
			{
				bool flag5 = (this.uncomprLen = this.input.PeekBits(16)) < 0;
				if (flag5)
				{
					return false;
				}
				this.input.DropBits(16);
				this.mode = 4;
				break;
			}
			case 4:
				break;
			case 5:
				goto IL_24A;
			case 6:
			{
				bool flag6 = !this.dynHeader.Decode(this.input);
				if (flag6)
				{
					return false;
				}
				this.litlenTree = this.dynHeader.BuildLitLenTree();
				this.distTree = this.dynHeader.BuildDistTree();
				this.mode = 7;
				goto IL_2FB;
			}
			case 7:
			case 8:
			case 9:
			case 10:
				goto IL_2FB;
			case 11:
				return this.DecodeChksum();
			case 12:
				return false;
			default:
				throw new SharpZipBaseException("Inflater.Decode unknown mode");
			}
			int num2 = this.input.PeekBits(16);
			bool flag7 = num2 < 0;
			if (flag7)
			{
				return false;
			}
			this.input.DropBits(16);
			bool flag8 = num2 != (this.uncomprLen ^ 65535);
			if (flag8)
			{
				throw new SharpZipBaseException("broken uncompressed block");
			}
			this.mode = 5;
			IL_24A:
			int num3 = this.outputWindow.CopyStored(this.input, this.uncomprLen);
			this.uncomprLen -= num3;
			bool flag9 = this.uncomprLen == 0;
			if (flag9)
			{
				this.mode = 2;
				return true;
			}
			return !this.input.IsNeedingInput;
			IL_2FB:
			return this.DecodeHuffman();
		}

		// Token: 0x060006BB RID: 1723 RVA: 0x00036778 File Offset: 0x00034978
		public void SetDictionary(byte[] buffer)
		{
			this.SetDictionary(buffer, 0, buffer.Length);
		}

		// Token: 0x060006BC RID: 1724 RVA: 0x00036788 File Offset: 0x00034988
		public void SetDictionary(byte[] buffer, int index, int count)
		{
			bool flag = buffer == null;
			if (flag)
			{
				throw new ArgumentNullException("buffer");
			}
			bool flag2 = index < 0;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			bool flag3 = count < 0;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			bool flag4 = !this.IsNeedingDictionary;
			if (flag4)
			{
				throw new InvalidOperationException("Dictionary is not needed");
			}
			this.adler.Update(buffer, index, count);
			bool flag5 = (int)this.adler.Value != this.readAdler;
			if (flag5)
			{
				throw new SharpZipBaseException("Wrong adler checksum");
			}
			this.adler.Reset();
			this.outputWindow.CopyDict(buffer, index, count);
			this.mode = 2;
		}

		// Token: 0x060006BD RID: 1725 RVA: 0x00036858 File Offset: 0x00034A58
		public void SetInput(byte[] buffer)
		{
			this.SetInput(buffer, 0, buffer.Length);
		}

		// Token: 0x060006BE RID: 1726 RVA: 0x00036868 File Offset: 0x00034A68
		public void SetInput(byte[] buffer, int index, int count)
		{
			this.input.SetInput(buffer, index, count);
			this.totalIn += (long)count;
		}

		// Token: 0x060006BF RID: 1727 RVA: 0x0003688C File Offset: 0x00034A8C
		public int Inflate(byte[] buffer)
		{
			bool flag = buffer == null;
			if (flag)
			{
				throw new ArgumentNullException("buffer");
			}
			return this.Inflate(buffer, 0, buffer.Length);
		}

		// Token: 0x060006C0 RID: 1728 RVA: 0x000368C8 File Offset: 0x00034AC8
		public int Inflate(byte[] buffer, int offset, int count)
		{
			bool flag = buffer == null;
			if (flag)
			{
				throw new ArgumentNullException("buffer");
			}
			bool flag2 = count < 0;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("count", "count cannot be negative");
			}
			bool flag3 = offset < 0;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException("offset", "offset cannot be negative");
			}
			bool flag4 = offset + count > buffer.Length;
			if (flag4)
			{
				throw new ArgumentException("count exceeds buffer bounds");
			}
			bool flag5 = count == 0;
			int result;
			if (flag5)
			{
				bool flag6 = !this.IsFinished;
				if (flag6)
				{
					this.Decode();
				}
				result = 0;
			}
			else
			{
				int num = 0;
				for (;;)
				{
					bool flag7 = this.mode != 11;
					if (flag7)
					{
						int num2 = this.outputWindow.CopyOutput(buffer, offset, count);
						bool flag8 = num2 > 0;
						if (flag8)
						{
							this.adler.Update(buffer, offset, num2);
							offset += num2;
							num += num2;
							this.totalOut += (long)num2;
							count -= num2;
							bool flag9 = count == 0;
							if (flag9)
							{
								break;
							}
						}
					}
					if (!this.Decode() && (this.outputWindow.GetAvailable() <= 0 || this.mode == 11))
					{
						goto Block_12;
					}
				}
				return num;
				Block_12:
				result = num;
			}
			return result;
		}

		// Token: 0x1700012B RID: 299
		// (get) Token: 0x060006C1 RID: 1729 RVA: 0x00036A48 File Offset: 0x00034C48
		public bool IsNeedingInput
		{
			get
			{
				return this.input.IsNeedingInput;
			}
		}

		// Token: 0x1700012C RID: 300
		// (get) Token: 0x060006C2 RID: 1730 RVA: 0x00036A6C File Offset: 0x00034C6C
		public bool IsNeedingDictionary
		{
			get
			{
				return this.mode == 1 && this.neededBits == 0;
			}
		}

		// Token: 0x1700012D RID: 301
		// (get) Token: 0x060006C3 RID: 1731 RVA: 0x00036AA0 File Offset: 0x00034CA0
		public bool IsFinished
		{
			get
			{
				return this.mode == 12 && this.outputWindow.GetAvailable() == 0;
			}
		}

		// Token: 0x1700012E RID: 302
		// (get) Token: 0x060006C4 RID: 1732 RVA: 0x00036ADC File Offset: 0x00034CDC
		public int Adler
		{
			get
			{
				return this.IsNeedingDictionary ? this.readAdler : ((int)this.adler.Value);
			}
		}

		// Token: 0x1700012F RID: 303
		// (get) Token: 0x060006C5 RID: 1733 RVA: 0x00036B18 File Offset: 0x00034D18
		public long TotalOut
		{
			get
			{
				return this.totalOut;
			}
		}

		// Token: 0x17000130 RID: 304
		// (get) Token: 0x060006C6 RID: 1734 RVA: 0x00036B38 File Offset: 0x00034D38
		public long TotalIn
		{
			get
			{
				return this.totalIn - (long)this.RemainingInput;
			}
		}

		// Token: 0x17000131 RID: 305
		// (get) Token: 0x060006C7 RID: 1735 RVA: 0x00036B60 File Offset: 0x00034D60
		public int RemainingInput
		{
			get
			{
				return this.input.AvailableBytes;
			}
		}

		// Token: 0x04000412 RID: 1042
		private static readonly int[] CPLENS = new int[]
		{
			3,
			4,
			5,
			6,
			7,
			8,
			9,
			10,
			11,
			13,
			15,
			17,
			19,
			23,
			27,
			31,
			35,
			43,
			51,
			59,
			67,
			83,
			99,
			115,
			131,
			163,
			195,
			227,
			258
		};

		// Token: 0x04000413 RID: 1043
		private static readonly int[] CPLEXT = new int[]
		{
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			1,
			1,
			1,
			1,
			2,
			2,
			2,
			2,
			3,
			3,
			3,
			3,
			4,
			4,
			4,
			4,
			5,
			5,
			5,
			5,
			0
		};

		// Token: 0x04000414 RID: 1044
		private static readonly int[] CPDIST = new int[]
		{
			1,
			2,
			3,
			4,
			5,
			7,
			9,
			13,
			17,
			25,
			33,
			49,
			65,
			97,
			129,
			193,
			257,
			385,
			513,
			769,
			1025,
			1537,
			2049,
			3073,
			4097,
			6145,
			8193,
			12289,
			16385,
			24577
		};

		// Token: 0x04000415 RID: 1045
		private static readonly int[] CPDEXT = new int[]
		{
			0,
			0,
			0,
			0,
			1,
			1,
			2,
			2,
			3,
			3,
			4,
			4,
			5,
			5,
			6,
			6,
			7,
			7,
			8,
			8,
			9,
			9,
			10,
			10,
			11,
			11,
			12,
			12,
			13,
			13
		};

		// Token: 0x04000416 RID: 1046
		private const int DECODE_HEADER = 0;

		// Token: 0x04000417 RID: 1047
		private const int DECODE_DICT = 1;

		// Token: 0x04000418 RID: 1048
		private const int DECODE_BLOCKS = 2;

		// Token: 0x04000419 RID: 1049
		private const int DECODE_STORED_LEN1 = 3;

		// Token: 0x0400041A RID: 1050
		private const int DECODE_STORED_LEN2 = 4;

		// Token: 0x0400041B RID: 1051
		private const int DECODE_STORED = 5;

		// Token: 0x0400041C RID: 1052
		private const int DECODE_DYN_HEADER = 6;

		// Token: 0x0400041D RID: 1053
		private const int DECODE_HUFFMAN = 7;

		// Token: 0x0400041E RID: 1054
		private const int DECODE_HUFFMAN_LENBITS = 8;

		// Token: 0x0400041F RID: 1055
		private const int DECODE_HUFFMAN_DIST = 9;

		// Token: 0x04000420 RID: 1056
		private const int DECODE_HUFFMAN_DISTBITS = 10;

		// Token: 0x04000421 RID: 1057
		private const int DECODE_CHKSUM = 11;

		// Token: 0x04000422 RID: 1058
		private const int FINISHED = 12;

		// Token: 0x04000423 RID: 1059
		private int mode;

		// Token: 0x04000424 RID: 1060
		private int readAdler;

		// Token: 0x04000425 RID: 1061
		private int neededBits;

		// Token: 0x04000426 RID: 1062
		private int repLength;

		// Token: 0x04000427 RID: 1063
		private int repDist;

		// Token: 0x04000428 RID: 1064
		private int uncomprLen;

		// Token: 0x04000429 RID: 1065
		private bool isLastBlock;

		// Token: 0x0400042A RID: 1066
		private long totalOut;

		// Token: 0x0400042B RID: 1067
		private long totalIn;

		// Token: 0x0400042C RID: 1068
		private bool noHeader;

		// Token: 0x0400042D RID: 1069
		private StreamManipulator input;

		// Token: 0x0400042E RID: 1070
		private OutputWindow outputWindow;

		// Token: 0x0400042F RID: 1071
		private InflaterDynHeader dynHeader;

		// Token: 0x04000430 RID: 1072
		private InflaterHuffmanTree litlenTree;

		// Token: 0x04000431 RID: 1073
		private InflaterHuffmanTree distTree;

		// Token: 0x04000432 RID: 1074
		private Adler32 adler;
	}
}
