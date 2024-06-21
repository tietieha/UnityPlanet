using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace UWA.ICSharpCode.SharpZipLib.Tar
{
	// Token: 0x02000095 RID: 149
	[ComVisible(false)]
	public class TarInputStream : Stream
	{
		// Token: 0x06000704 RID: 1796 RVA: 0x0002F62C File Offset: 0x0002D82C
		public TarInputStream(Stream inputStream) : this(inputStream, 20)
		{
		}

		// Token: 0x06000705 RID: 1797 RVA: 0x0002F63C File Offset: 0x0002D83C
		public TarInputStream(Stream inputStream, int blockFactor)
		{
			this.inputStream = inputStream;
			this.tarBuffer = TarBuffer.CreateInputTarBuffer(inputStream, blockFactor);
		}

		// Token: 0x17000121 RID: 289
		// (get) Token: 0x06000706 RID: 1798 RVA: 0x0002F65C File Offset: 0x0002D85C
		// (set) Token: 0x06000707 RID: 1799 RVA: 0x0002F680 File Offset: 0x0002D880
		public bool IsStreamOwner
		{
			get
			{
				return this.tarBuffer.IsStreamOwner;
			}
			set
			{
				this.tarBuffer.IsStreamOwner = value;
			}
		}

		// Token: 0x17000122 RID: 290
		// (get) Token: 0x06000708 RID: 1800 RVA: 0x0002F690 File Offset: 0x0002D890
		public override bool CanRead
		{
			get
			{
				return this.inputStream.CanRead;
			}
		}

		// Token: 0x17000123 RID: 291
		// (get) Token: 0x06000709 RID: 1801 RVA: 0x0002F6B4 File Offset: 0x0002D8B4
		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000124 RID: 292
		// (get) Token: 0x0600070A RID: 1802 RVA: 0x0002F6D0 File Offset: 0x0002D8D0
		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000125 RID: 293
		// (get) Token: 0x0600070B RID: 1803 RVA: 0x0002F6EC File Offset: 0x0002D8EC
		public override long Length
		{
			get
			{
				return this.inputStream.Length;
			}
		}

		// Token: 0x17000126 RID: 294
		// (get) Token: 0x0600070C RID: 1804 RVA: 0x0002F710 File Offset: 0x0002D910
		// (set) Token: 0x0600070D RID: 1805 RVA: 0x0002F734 File Offset: 0x0002D934
		public override long Position
		{
			get
			{
				return this.inputStream.Position;
			}
			set
			{
				throw new NotSupportedException("TarInputStream Seek not supported");
			}
		}

		// Token: 0x0600070E RID: 1806 RVA: 0x0002F744 File Offset: 0x0002D944
		public override void Flush()
		{
			this.inputStream.Flush();
		}

		// Token: 0x0600070F RID: 1807 RVA: 0x0002F754 File Offset: 0x0002D954
		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException("TarInputStream Seek not supported");
		}

		// Token: 0x06000710 RID: 1808 RVA: 0x0002F764 File Offset: 0x0002D964
		public override void SetLength(long value)
		{
			throw new NotSupportedException("TarInputStream SetLength not supported");
		}

		// Token: 0x06000711 RID: 1809 RVA: 0x0002F774 File Offset: 0x0002D974
		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException("TarInputStream Write not supported");
		}

		// Token: 0x06000712 RID: 1810 RVA: 0x0002F784 File Offset: 0x0002D984
		public override void WriteByte(byte value)
		{
			throw new NotSupportedException("TarInputStream WriteByte not supported");
		}

		// Token: 0x06000713 RID: 1811 RVA: 0x0002F794 File Offset: 0x0002D994
		public override int ReadByte()
		{
			byte[] array = new byte[1];
			int num = this.Read(array, 0, 1);
			bool flag = num <= 0;
			int result;
			if (flag)
			{
				result = -1;
			}
			else
			{
				result = (int)array[0];
			}
			return result;
		}

		// Token: 0x06000714 RID: 1812 RVA: 0x0002F7D8 File Offset: 0x0002D9D8
		public override int Read(byte[] buffer, int offset, int count)
		{
			bool flag = buffer == null;
			if (flag)
			{
				throw new ArgumentNullException("buffer");
			}
			int num = 0;
			bool flag2 = this.entryOffset >= this.entrySize;
			int result;
			if (flag2)
			{
				result = 0;
			}
			else
			{
				long num2 = (long)count;
				bool flag3 = num2 + this.entryOffset > this.entrySize;
				if (flag3)
				{
					num2 = this.entrySize - this.entryOffset;
				}
				bool flag4 = this.readBuffer != null;
				if (flag4)
				{
					int num3 = (num2 > (long)this.readBuffer.Length) ? this.readBuffer.Length : ((int)num2);
					Array.Copy(this.readBuffer, 0, buffer, offset, num3);
					bool flag5 = num3 >= this.readBuffer.Length;
					if (flag5)
					{
						this.readBuffer = null;
					}
					else
					{
						int num4 = this.readBuffer.Length - num3;
						byte[] destinationArray = new byte[num4];
						Array.Copy(this.readBuffer, num3, destinationArray, 0, num4);
						this.readBuffer = destinationArray;
					}
					num += num3;
					num2 -= (long)num3;
					offset += num3;
				}
				while (num2 > 0L)
				{
					byte[] array = this.tarBuffer.ReadBlock();
					bool flag6 = array == null;
					if (flag6)
					{
						throw new TarException("unexpected EOF with " + num2.ToString() + " bytes unread");
					}
					int num5 = (int)num2;
					int num6 = array.Length;
					bool flag7 = num6 > num5;
					if (flag7)
					{
						Array.Copy(array, 0, buffer, offset, num5);
						this.readBuffer = new byte[num6 - num5];
						Array.Copy(array, num5, this.readBuffer, 0, num6 - num5);
					}
					else
					{
						num5 = num6;
						Array.Copy(array, 0, buffer, offset, num6);
					}
					num += num5;
					num2 -= (long)num5;
					offset += num5;
				}
				this.entryOffset += (long)num;
				result = num;
			}
			return result;
		}

		// Token: 0x06000715 RID: 1813 RVA: 0x0002F9E4 File Offset: 0x0002DBE4
		public override void Close()
		{
			this.tarBuffer.Close();
		}

		// Token: 0x06000716 RID: 1814 RVA: 0x0002F9F4 File Offset: 0x0002DBF4
		public void SetEntryFactory(TarInputStream.IEntryFactory factory)
		{
			this.entryFactory = factory;
		}

		// Token: 0x17000127 RID: 295
		// (get) Token: 0x06000717 RID: 1815 RVA: 0x0002FA00 File Offset: 0x0002DC00
		public int RecordSize
		{
			get
			{
				return this.tarBuffer.RecordSize;
			}
		}

		// Token: 0x06000718 RID: 1816 RVA: 0x0002FA24 File Offset: 0x0002DC24
		[Obsolete("Use RecordSize property instead")]
		public int GetRecordSize()
		{
			return this.tarBuffer.RecordSize;
		}

		// Token: 0x17000128 RID: 296
		// (get) Token: 0x06000719 RID: 1817 RVA: 0x0002FA48 File Offset: 0x0002DC48
		public long Available
		{
			get
			{
				return this.entrySize - this.entryOffset;
			}
		}

		// Token: 0x0600071A RID: 1818 RVA: 0x0002FA70 File Offset: 0x0002DC70
		public void Skip(long skipCount)
		{
			byte[] array = new byte[8192];
			int num2;
			for (long num = skipCount; num > 0L; num -= (long)num2)
			{
				int count = (num > (long)array.Length) ? array.Length : ((int)num);
				num2 = this.Read(array, 0, count);
				bool flag = num2 == -1;
				if (flag)
				{
					break;
				}
			}
		}

		// Token: 0x17000129 RID: 297
		// (get) Token: 0x0600071B RID: 1819 RVA: 0x0002FAD8 File Offset: 0x0002DCD8
		public bool IsMarkSupported
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600071C RID: 1820 RVA: 0x0002FAF4 File Offset: 0x0002DCF4
		public void Mark(int markLimit)
		{
		}

		// Token: 0x0600071D RID: 1821 RVA: 0x0002FAF8 File Offset: 0x0002DCF8
		public void Reset()
		{
		}

		// Token: 0x0600071E RID: 1822 RVA: 0x0002FAFC File Offset: 0x0002DCFC
		public TarEntry GetNextEntry()
		{
			bool flag = this.hasHitEOF;
			TarEntry result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = this.currentEntry != null;
				if (flag2)
				{
					this.SkipToNextEntry();
				}
				byte[] array = this.tarBuffer.ReadBlock();
				bool flag3 = array == null;
				if (flag3)
				{
					this.hasHitEOF = true;
				}
				else
				{
					bool flag4 = TarBuffer.IsEndOfArchiveBlock(array);
					if (flag4)
					{
						this.hasHitEOF = true;
					}
				}
				bool flag5 = this.hasHitEOF;
				if (flag5)
				{
					this.currentEntry = null;
				}
				else
				{
					try
					{
						TarHeader tarHeader = new TarHeader();
						tarHeader.ParseBuffer(array);
						bool flag6 = !tarHeader.IsChecksumValid;
						if (flag6)
						{
							throw new TarException("Header checksum is invalid");
						}
						this.entryOffset = 0L;
						this.entrySize = tarHeader.Size;
						StringBuilder stringBuilder = null;
						bool flag7 = tarHeader.TypeFlag == 76;
						if (flag7)
						{
							byte[] array2 = new byte[512];
							long num = this.entrySize;
							stringBuilder = new StringBuilder();
							while (num > 0L)
							{
								int num2 = this.Read(array2, 0, (num > (long)array2.Length) ? array2.Length : ((int)num));
								bool flag8 = num2 == -1;
								if (flag8)
								{
									throw new InvalidHeaderException("Failed to read long name entry");
								}
								stringBuilder.Append(TarHeader.ParseName(array2, 0, num2).ToString());
								num -= (long)num2;
							}
							this.SkipToNextEntry();
							array = this.tarBuffer.ReadBlock();
						}
						else
						{
							bool flag9 = tarHeader.TypeFlag == 103;
							if (flag9)
							{
								this.SkipToNextEntry();
								array = this.tarBuffer.ReadBlock();
							}
							else
							{
								bool flag10 = tarHeader.TypeFlag == 120;
								if (flag10)
								{
									this.SkipToNextEntry();
									array = this.tarBuffer.ReadBlock();
								}
								else
								{
									bool flag11 = tarHeader.TypeFlag == 86;
									if (flag11)
									{
										this.SkipToNextEntry();
										array = this.tarBuffer.ReadBlock();
									}
									else
									{
										bool flag12 = tarHeader.TypeFlag != 48 && tarHeader.TypeFlag != 0 && tarHeader.TypeFlag != 53;
										if (flag12)
										{
											this.SkipToNextEntry();
											array = this.tarBuffer.ReadBlock();
										}
									}
								}
							}
						}
						bool flag13 = this.entryFactory == null;
						if (flag13)
						{
							this.currentEntry = new TarEntry(array);
							bool flag14 = stringBuilder != null;
							if (flag14)
							{
								this.currentEntry.Name = stringBuilder.ToString();
							}
						}
						else
						{
							this.currentEntry = this.entryFactory.CreateEntry(array);
						}
						this.entryOffset = 0L;
						this.entrySize = this.currentEntry.Size;
					}
					catch (InvalidHeaderException ex)
					{
						this.entrySize = 0L;
						this.entryOffset = 0L;
						this.currentEntry = null;
						string message = string.Format("Bad header in record {0} block {1} {2}", this.tarBuffer.CurrentRecord, this.tarBuffer.CurrentBlock, ex.Message);
						throw new InvalidHeaderException(message);
					}
				}
				result = this.currentEntry;
			}
			return result;
		}

		// Token: 0x0600071F RID: 1823 RVA: 0x0002FE68 File Offset: 0x0002E068
		public void CopyEntryContents(Stream outputStream)
		{
			byte[] array = new byte[32768];
			for (;;)
			{
				int num = this.Read(array, 0, array.Length);
				bool flag = num <= 0;
				if (flag)
				{
					break;
				}
				outputStream.Write(array, 0, num);
			}
		}

		// Token: 0x06000720 RID: 1824 RVA: 0x0002FEBC File Offset: 0x0002E0BC
		private void SkipToNextEntry()
		{
			long num = this.entrySize - this.entryOffset;
			bool flag = num > 0L;
			if (flag)
			{
				this.Skip(num);
			}
			this.readBuffer = null;
		}

		// Token: 0x04000456 RID: 1110
		protected bool hasHitEOF;

		// Token: 0x04000457 RID: 1111
		protected long entrySize;

		// Token: 0x04000458 RID: 1112
		protected long entryOffset;

		// Token: 0x04000459 RID: 1113
		protected byte[] readBuffer;

		// Token: 0x0400045A RID: 1114
		protected TarBuffer tarBuffer;

		// Token: 0x0400045B RID: 1115
		private TarEntry currentEntry;

		// Token: 0x0400045C RID: 1116
		protected TarInputStream.IEntryFactory entryFactory;

		// Token: 0x0400045D RID: 1117
		private readonly Stream inputStream;

		// Token: 0x02000122 RID: 290
		public interface IEntryFactory
		{
			// Token: 0x060009B5 RID: 2485
			TarEntry CreateEntry(string name);

			// Token: 0x060009B6 RID: 2486
			TarEntry CreateEntryFromFile(string fileName);

			// Token: 0x060009B7 RID: 2487
			TarEntry CreateEntry(byte[] headerBuffer);
		}

		// Token: 0x02000123 RID: 291
		public class EntryFactoryAdapter : TarInputStream.IEntryFactory
		{
			// Token: 0x060009B8 RID: 2488 RVA: 0x0003EF10 File Offset: 0x0003D110
			public TarEntry CreateEntry(string name)
			{
				return TarEntry.CreateTarEntry(name);
			}

			// Token: 0x060009B9 RID: 2489 RVA: 0x0003EF30 File Offset: 0x0003D130
			public TarEntry CreateEntryFromFile(string fileName)
			{
				return TarEntry.CreateEntryFromFile(fileName);
			}

			// Token: 0x060009BA RID: 2490 RVA: 0x0003EF50 File Offset: 0x0003D150
			public TarEntry CreateEntry(byte[] headerBuffer)
			{
				return new TarEntry(headerBuffer);
			}
		}
	}
}
