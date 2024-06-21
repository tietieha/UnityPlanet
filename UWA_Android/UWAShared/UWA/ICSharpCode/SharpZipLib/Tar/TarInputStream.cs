using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace UWA.ICSharpCode.SharpZipLib.Tar
{
	// Token: 0x020000A4 RID: 164
	[ComVisible(false)]
	public class TarInputStream : Stream
	{
		// Token: 0x060007E0 RID: 2016 RVA: 0x0003C3E4 File Offset: 0x0003A5E4
		public TarInputStream(Stream inputStream) : this(inputStream, 20)
		{
		}

		// Token: 0x060007E1 RID: 2017 RVA: 0x0003C3F4 File Offset: 0x0003A5F4
		public TarInputStream(Stream inputStream, int blockFactor)
		{
			this.inputStream = inputStream;
			this.tarBuffer = TarBuffer.CreateInputTarBuffer(inputStream, blockFactor);
		}

		// Token: 0x17000175 RID: 373
		// (get) Token: 0x060007E2 RID: 2018 RVA: 0x0003C414 File Offset: 0x0003A614
		// (set) Token: 0x060007E3 RID: 2019 RVA: 0x0003C438 File Offset: 0x0003A638
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

		// Token: 0x17000176 RID: 374
		// (get) Token: 0x060007E4 RID: 2020 RVA: 0x0003C448 File Offset: 0x0003A648
		public override bool CanRead
		{
			get
			{
				return this.inputStream.CanRead;
			}
		}

		// Token: 0x17000177 RID: 375
		// (get) Token: 0x060007E5 RID: 2021 RVA: 0x0003C46C File Offset: 0x0003A66C
		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000178 RID: 376
		// (get) Token: 0x060007E6 RID: 2022 RVA: 0x0003C488 File Offset: 0x0003A688
		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000179 RID: 377
		// (get) Token: 0x060007E7 RID: 2023 RVA: 0x0003C4A4 File Offset: 0x0003A6A4
		public override long Length
		{
			get
			{
				return this.inputStream.Length;
			}
		}

		// Token: 0x1700017A RID: 378
		// (get) Token: 0x060007E8 RID: 2024 RVA: 0x0003C4C8 File Offset: 0x0003A6C8
		// (set) Token: 0x060007E9 RID: 2025 RVA: 0x0003C4EC File Offset: 0x0003A6EC
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

		// Token: 0x060007EA RID: 2026 RVA: 0x0003C4FC File Offset: 0x0003A6FC
		public override void Flush()
		{
			this.inputStream.Flush();
		}

		// Token: 0x060007EB RID: 2027 RVA: 0x0003C50C File Offset: 0x0003A70C
		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException("TarInputStream Seek not supported");
		}

		// Token: 0x060007EC RID: 2028 RVA: 0x0003C51C File Offset: 0x0003A71C
		public override void SetLength(long value)
		{
			throw new NotSupportedException("TarInputStream SetLength not supported");
		}

		// Token: 0x060007ED RID: 2029 RVA: 0x0003C52C File Offset: 0x0003A72C
		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException("TarInputStream Write not supported");
		}

		// Token: 0x060007EE RID: 2030 RVA: 0x0003C53C File Offset: 0x0003A73C
		public override void WriteByte(byte value)
		{
			throw new NotSupportedException("TarInputStream WriteByte not supported");
		}

		// Token: 0x060007EF RID: 2031 RVA: 0x0003C54C File Offset: 0x0003A74C
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

		// Token: 0x060007F0 RID: 2032 RVA: 0x0003C590 File Offset: 0x0003A790
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

		// Token: 0x060007F1 RID: 2033 RVA: 0x0003C79C File Offset: 0x0003A99C
		public override void Close()
		{
			this.tarBuffer.Close();
		}

		// Token: 0x060007F2 RID: 2034 RVA: 0x0003C7AC File Offset: 0x0003A9AC
		public void SetEntryFactory(TarInputStream.IEntryFactory factory)
		{
			this.entryFactory = factory;
		}

		// Token: 0x1700017B RID: 379
		// (get) Token: 0x060007F3 RID: 2035 RVA: 0x0003C7B8 File Offset: 0x0003A9B8
		public int RecordSize
		{
			get
			{
				return this.tarBuffer.RecordSize;
			}
		}

		// Token: 0x060007F4 RID: 2036 RVA: 0x0003C7DC File Offset: 0x0003A9DC
		[Obsolete("Use RecordSize property instead")]
		public int GetRecordSize()
		{
			return this.tarBuffer.RecordSize;
		}

		// Token: 0x1700017C RID: 380
		// (get) Token: 0x060007F5 RID: 2037 RVA: 0x0003C800 File Offset: 0x0003AA00
		public long Available
		{
			get
			{
				return this.entrySize - this.entryOffset;
			}
		}

		// Token: 0x060007F6 RID: 2038 RVA: 0x0003C828 File Offset: 0x0003AA28
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

		// Token: 0x1700017D RID: 381
		// (get) Token: 0x060007F7 RID: 2039 RVA: 0x0003C890 File Offset: 0x0003AA90
		public bool IsMarkSupported
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060007F8 RID: 2040 RVA: 0x0003C8AC File Offset: 0x0003AAAC
		public void Mark(int markLimit)
		{
		}

		// Token: 0x060007F9 RID: 2041 RVA: 0x0003C8B0 File Offset: 0x0003AAB0
		public void Reset()
		{
		}

		// Token: 0x060007FA RID: 2042 RVA: 0x0003C8B4 File Offset: 0x0003AAB4
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

		// Token: 0x060007FB RID: 2043 RVA: 0x0003CC20 File Offset: 0x0003AE20
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

		// Token: 0x060007FC RID: 2044 RVA: 0x0003CC74 File Offset: 0x0003AE74
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

		// Token: 0x040004C9 RID: 1225
		protected bool hasHitEOF;

		// Token: 0x040004CA RID: 1226
		protected long entrySize;

		// Token: 0x040004CB RID: 1227
		protected long entryOffset;

		// Token: 0x040004CC RID: 1228
		protected byte[] readBuffer;

		// Token: 0x040004CD RID: 1229
		protected TarBuffer tarBuffer;

		// Token: 0x040004CE RID: 1230
		private TarEntry currentEntry;

		// Token: 0x040004CF RID: 1231
		protected TarInputStream.IEntryFactory entryFactory;

		// Token: 0x040004D0 RID: 1232
		private readonly Stream inputStream;

		// Token: 0x02000158 RID: 344
		public interface IEntryFactory
		{
			// Token: 0x06000B0B RID: 2827
			TarEntry CreateEntry(string name);

			// Token: 0x06000B0C RID: 2828
			TarEntry CreateEntryFromFile(string fileName);

			// Token: 0x06000B0D RID: 2829
			TarEntry CreateEntry(byte[] headerBuffer);
		}

		// Token: 0x02000159 RID: 345
		public class EntryFactoryAdapter : TarInputStream.IEntryFactory
		{
			// Token: 0x06000B0E RID: 2830 RVA: 0x0004C674 File Offset: 0x0004A874
			public TarEntry CreateEntry(string name)
			{
				return TarEntry.CreateTarEntry(name);
			}

			// Token: 0x06000B0F RID: 2831 RVA: 0x0004C694 File Offset: 0x0004A894
			public TarEntry CreateEntryFromFile(string fileName)
			{
				return TarEntry.CreateEntryFromFile(fileName);
			}

			// Token: 0x06000B10 RID: 2832 RVA: 0x0004C6B4 File Offset: 0x0004A8B4
			public TarEntry CreateEntry(byte[] headerBuffer)
			{
				return new TarEntry(headerBuffer);
			}
		}
	}
}
