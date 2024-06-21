using System;
using System.Runtime.InteropServices;
using UWA.ICSharpCode.SharpZipLib.Checksums;
using UWA.ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace UWA.ICSharpCode.SharpZipLib.Zip.Compression
{
	// Token: 0x02000085 RID: 133
	[ComVisible(false)]
	public class Inflater
	{
		// Token: 0x060005D7 RID: 1495 RVA: 0x000290D0 File Offset: 0x000272D0
		public Inflater() : this(false)
		{
		}

		// Token: 0x060005D8 RID: 1496 RVA: 0x000290DC File Offset: 0x000272DC
		public Inflater(bool noHeader)
		{
			this.noHeader = noHeader;
			this.adler = new UWA.ICSharpCode.SharpZipLib.Checksums.Adler32();
			this.input = new UWA.ICSharpCode.SharpZipLib.Zip.Compression.Streams.StreamManipulator();
			this.outputWindow = new UWA.ICSharpCode.SharpZipLib.Zip.Compression.Streams.OutputWindow();
			this.mode = (noHeader ? 2 : 0);
		}

		// Token: 0x060005D9 RID: 1497 RVA: 0x00029130 File Offset: 0x00027330
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

		// Token: 0x060005DA RID: 1498 RVA: 0x000291AC File Offset: 0x000273AC
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

		// Token: 0x060005DB RID: 1499 RVA: 0x00029274 File Offset: 0x00027474
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

		// Token: 0x060005DC RID: 1500 RVA: 0x000292F0 File Offset: 0x000274F0
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

		// Token: 0x060005DD RID: 1501 RVA: 0x000295BC File Offset: 0x000277BC
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

		// Token: 0x060005DE RID: 1502 RVA: 0x00029694 File Offset: 0x00027894
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

		// Token: 0x060005DF RID: 1503 RVA: 0x000299C0 File Offset: 0x00027BC0
		public void SetDictionary(byte[] buffer)
		{
			this.SetDictionary(buffer, 0, buffer.Length);
		}

		// Token: 0x060005E0 RID: 1504 RVA: 0x000299D0 File Offset: 0x00027BD0
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

		// Token: 0x060005E1 RID: 1505 RVA: 0x00029AA0 File Offset: 0x00027CA0
		public void SetInput(byte[] buffer)
		{
			this.SetInput(buffer, 0, buffer.Length);
		}

		// Token: 0x060005E2 RID: 1506 RVA: 0x00029AB0 File Offset: 0x00027CB0
		public void SetInput(byte[] buffer, int index, int count)
		{
			this.input.SetInput(buffer, index, count);
			this.totalIn += (long)count;
		}

		// Token: 0x060005E3 RID: 1507 RVA: 0x00029AD4 File Offset: 0x00027CD4
		public int Inflate(byte[] buffer)
		{
			bool flag = buffer == null;
			if (flag)
			{
				throw new ArgumentNullException("buffer");
			}
			return this.Inflate(buffer, 0, buffer.Length);
		}

		// Token: 0x060005E4 RID: 1508 RVA: 0x00029B10 File Offset: 0x00027D10
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

		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x060005E5 RID: 1509 RVA: 0x00029C90 File Offset: 0x00027E90
		public bool IsNeedingInput
		{
			get
			{
				return this.input.IsNeedingInput;
			}
		}

		// Token: 0x170000D8 RID: 216
		// (get) Token: 0x060005E6 RID: 1510 RVA: 0x00029CB4 File Offset: 0x00027EB4
		public bool IsNeedingDictionary
		{
			get
			{
				return this.mode == 1 && this.neededBits == 0;
			}
		}

		// Token: 0x170000D9 RID: 217
		// (get) Token: 0x060005E7 RID: 1511 RVA: 0x00029CE8 File Offset: 0x00027EE8
		public bool IsFinished
		{
			get
			{
				return this.mode == 12 && this.outputWindow.GetAvailable() == 0;
			}
		}

		// Token: 0x170000DA RID: 218
		// (get) Token: 0x060005E8 RID: 1512 RVA: 0x00029D24 File Offset: 0x00027F24
		public int Adler
		{
			get
			{
				return this.IsNeedingDictionary ? this.readAdler : ((int)this.adler.Value);
			}
		}

		// Token: 0x170000DB RID: 219
		// (get) Token: 0x060005E9 RID: 1513 RVA: 0x00029D60 File Offset: 0x00027F60
		public long TotalOut
		{
			get
			{
				return this.totalOut;
			}
		}

		// Token: 0x170000DC RID: 220
		// (get) Token: 0x060005EA RID: 1514 RVA: 0x00029D80 File Offset: 0x00027F80
		public long TotalIn
		{
			get
			{
				return this.totalIn - (long)this.RemainingInput;
			}
		}

		// Token: 0x170000DD RID: 221
		// (get) Token: 0x060005EB RID: 1515 RVA: 0x00029DA8 File Offset: 0x00027FA8
		public int RemainingInput
		{
			get
			{
				return this.input.AvailableBytes;
			}
		}

		// Token: 0x0400039F RID: 927
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

		// Token: 0x040003A0 RID: 928
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

		// Token: 0x040003A1 RID: 929
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

		// Token: 0x040003A2 RID: 930
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

		// Token: 0x040003A3 RID: 931
		private const int DECODE_HEADER = 0;

		// Token: 0x040003A4 RID: 932
		private const int DECODE_DICT = 1;

		// Token: 0x040003A5 RID: 933
		private const int DECODE_BLOCKS = 2;

		// Token: 0x040003A6 RID: 934
		private const int DECODE_STORED_LEN1 = 3;

		// Token: 0x040003A7 RID: 935
		private const int DECODE_STORED_LEN2 = 4;

		// Token: 0x040003A8 RID: 936
		private const int DECODE_STORED = 5;

		// Token: 0x040003A9 RID: 937
		private const int DECODE_DYN_HEADER = 6;

		// Token: 0x040003AA RID: 938
		private const int DECODE_HUFFMAN = 7;

		// Token: 0x040003AB RID: 939
		private const int DECODE_HUFFMAN_LENBITS = 8;

		// Token: 0x040003AC RID: 940
		private const int DECODE_HUFFMAN_DIST = 9;

		// Token: 0x040003AD RID: 941
		private const int DECODE_HUFFMAN_DISTBITS = 10;

		// Token: 0x040003AE RID: 942
		private const int DECODE_CHKSUM = 11;

		// Token: 0x040003AF RID: 943
		private const int FINISHED = 12;

		// Token: 0x040003B0 RID: 944
		private int mode;

		// Token: 0x040003B1 RID: 945
		private int readAdler;

		// Token: 0x040003B2 RID: 946
		private int neededBits;

		// Token: 0x040003B3 RID: 947
		private int repLength;

		// Token: 0x040003B4 RID: 948
		private int repDist;

		// Token: 0x040003B5 RID: 949
		private int uncomprLen;

		// Token: 0x040003B6 RID: 950
		private bool isLastBlock;

		// Token: 0x040003B7 RID: 951
		private long totalOut;

		// Token: 0x040003B8 RID: 952
		private long totalIn;

		// Token: 0x040003B9 RID: 953
		private bool noHeader;

		// Token: 0x040003BA RID: 954
		private UWA.ICSharpCode.SharpZipLib.Zip.Compression.Streams.StreamManipulator input;

		// Token: 0x040003BB RID: 955
		private UWA.ICSharpCode.SharpZipLib.Zip.Compression.Streams.OutputWindow outputWindow;

		// Token: 0x040003BC RID: 956
		private InflaterDynHeader dynHeader;

		// Token: 0x040003BD RID: 957
		private InflaterHuffmanTree litlenTree;

		// Token: 0x040003BE RID: 958
		private InflaterHuffmanTree distTree;

		// Token: 0x040003BF RID: 959
		private UWA.ICSharpCode.SharpZipLib.Checksums.Adler32 adler;
	}
}
