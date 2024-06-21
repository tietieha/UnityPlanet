using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using UWA.ICSharpCode.SharpZipLib.Checksums;
using UWA.ICSharpCode.SharpZipLib.Core;
using UWA.ICSharpCode.SharpZipLib.Encryption;
using UWA.ICSharpCode.SharpZipLib.Zip.Compression;
using UWA.ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x0200007F RID: 127
	[ComVisible(false)]
	public class ZipFile : IEnumerable, IDisposable
	{
		// Token: 0x0600059A RID: 1434 RVA: 0x0002C078 File Offset: 0x0002A278
		private void OnKeysRequired(string fileName)
		{
			bool flag = this.KeysRequired != null;
			if (flag)
			{
				KeysRequiredEventArgs keysRequiredEventArgs = new KeysRequiredEventArgs(fileName, this.key);
				this.KeysRequired(this, keysRequiredEventArgs);
				this.key = keysRequiredEventArgs.Key;
			}
		}

		// Token: 0x170000FD RID: 253
		// (get) Token: 0x0600059B RID: 1435 RVA: 0x0002C0C4 File Offset: 0x0002A2C4
		// (set) Token: 0x0600059C RID: 1436 RVA: 0x0002C0E4 File Offset: 0x0002A2E4
		private byte[] Key
		{
			get
			{
				return this.key;
			}
			set
			{
				this.key = value;
			}
		}

		// Token: 0x170000FE RID: 254
		// (set) Token: 0x0600059D RID: 1437 RVA: 0x0002C0F0 File Offset: 0x0002A2F0
		public string Password
		{
			set
			{
				bool flag = value == null || value.Length == 0;
				if (flag)
				{
					this.key = null;
				}
				else
				{
					this.rawPassword_ = value;
					this.key = PkzipClassic.GenerateKeys(ZipConstants.ConvertToArray(value));
				}
			}
		}

		// Token: 0x170000FF RID: 255
		// (get) Token: 0x0600059E RID: 1438 RVA: 0x0002C148 File Offset: 0x0002A348
		private bool HaveKeys
		{
			get
			{
				return this.key != null;
			}
		}

		// Token: 0x0600059F RID: 1439 RVA: 0x0002C16C File Offset: 0x0002A36C
		public ZipFile(string name)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			this.name_ = name;
			this.baseStream_ = File.Open(name, FileMode.Open, FileAccess.Read, FileShare.Read);
			this.isStreamOwner = true;
			try
			{
				this.ReadEntries();
			}
			catch
			{
				this.DisposeInternal(true);
				throw;
			}
		}

		// Token: 0x060005A0 RID: 1440 RVA: 0x0002C1FC File Offset: 0x0002A3FC
		public ZipFile(FileStream file)
		{
			bool flag = file == null;
			if (flag)
			{
				throw new ArgumentNullException("file");
			}
			bool flag2 = !file.CanSeek;
			if (flag2)
			{
				throw new ArgumentException("Stream is not seekable", "file");
			}
			this.baseStream_ = file;
			this.name_ = file.Name;
			this.isStreamOwner = true;
			try
			{
				this.ReadEntries();
			}
			catch
			{
				this.DisposeInternal(true);
				throw;
			}
		}

		// Token: 0x060005A1 RID: 1441 RVA: 0x0002C2AC File Offset: 0x0002A4AC
		public ZipFile(Stream stream)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			bool flag2 = !stream.CanSeek;
			if (flag2)
			{
				throw new ArgumentException("Stream is not seekable", "stream");
			}
			this.baseStream_ = stream;
			this.isStreamOwner = true;
			bool flag3 = this.baseStream_.Length > 0L;
			if (flag3)
			{
				try
				{
					this.ReadEntries();
				}
				catch
				{
					this.DisposeInternal(true);
					throw;
				}
			}
			else
			{
				this.entries_ = new ZipEntry[0];
				this.isNewArchive_ = true;
			}
		}

		// Token: 0x060005A2 RID: 1442 RVA: 0x0002C380 File Offset: 0x0002A580
		internal ZipFile()
		{
			this.entries_ = new ZipEntry[0];
			this.isNewArchive_ = true;
		}

		// Token: 0x060005A3 RID: 1443 RVA: 0x0002C3BC File Offset: 0x0002A5BC
		~ZipFile()
		{
			this.Dispose(false);
		}

		// Token: 0x060005A4 RID: 1444 RVA: 0x0002C3F0 File Offset: 0x0002A5F0
		public void Close()
		{
			this.DisposeInternal(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x060005A5 RID: 1445 RVA: 0x0002C404 File Offset: 0x0002A604
		public static ZipFile Create(string fileName)
		{
			bool flag = fileName == null;
			if (flag)
			{
				throw new ArgumentNullException("fileName");
			}
			FileStream fileStream = File.Create(fileName);
			return new ZipFile
			{
				name_ = fileName,
				baseStream_ = fileStream,
				isStreamOwner = true
			};
		}

		// Token: 0x060005A6 RID: 1446 RVA: 0x0002C458 File Offset: 0x0002A658
		public static ZipFile Create(Stream outStream)
		{
			bool flag = outStream == null;
			if (flag)
			{
				throw new ArgumentNullException("outStream");
			}
			bool flag2 = !outStream.CanWrite;
			if (flag2)
			{
				throw new ArgumentException("Stream is not writeable", "outStream");
			}
			bool flag3 = !outStream.CanSeek;
			if (flag3)
			{
				throw new ArgumentException("Stream is not seekable", "outStream");
			}
			return new ZipFile
			{
				baseStream_ = outStream
			};
		}

		// Token: 0x17000100 RID: 256
		// (get) Token: 0x060005A7 RID: 1447 RVA: 0x0002C4DC File Offset: 0x0002A6DC
		// (set) Token: 0x060005A8 RID: 1448 RVA: 0x0002C4FC File Offset: 0x0002A6FC
		public bool IsStreamOwner
		{
			get
			{
				return this.isStreamOwner;
			}
			set
			{
				this.isStreamOwner = value;
			}
		}

		// Token: 0x17000101 RID: 257
		// (get) Token: 0x060005A9 RID: 1449 RVA: 0x0002C508 File Offset: 0x0002A708
		public bool IsEmbeddedArchive
		{
			get
			{
				return this.offsetOfFirstEntry > 0L;
			}
		}

		// Token: 0x17000102 RID: 258
		// (get) Token: 0x060005AA RID: 1450 RVA: 0x0002C52C File Offset: 0x0002A72C
		public bool IsNewArchive
		{
			get
			{
				return this.isNewArchive_;
			}
		}

		// Token: 0x17000103 RID: 259
		// (get) Token: 0x060005AB RID: 1451 RVA: 0x0002C54C File Offset: 0x0002A74C
		public string ZipFileComment
		{
			get
			{
				return this.comment_;
			}
		}

		// Token: 0x17000104 RID: 260
		// (get) Token: 0x060005AC RID: 1452 RVA: 0x0002C56C File Offset: 0x0002A76C
		public string Name
		{
			get
			{
				return this.name_;
			}
		}

		// Token: 0x17000105 RID: 261
		// (get) Token: 0x060005AD RID: 1453 RVA: 0x0002C58C File Offset: 0x0002A78C
		[Obsolete("Use the Count property instead")]
		public int Size
		{
			get
			{
				return this.entries_.Length;
			}
		}

		// Token: 0x17000106 RID: 262
		// (get) Token: 0x060005AE RID: 1454 RVA: 0x0002C5B0 File Offset: 0x0002A7B0
		public long Count
		{
			get
			{
				return (long)this.entries_.Length;
			}
		}

		// Token: 0x17000107 RID: 263
		[IndexerName("EntryByIndex")]
		public ZipEntry this[int index]
		{
			get
			{
				return (ZipEntry)this.entries_[index].Clone();
			}
		}

		// Token: 0x060005B0 RID: 1456 RVA: 0x0002C604 File Offset: 0x0002A804
		public IEnumerator GetEnumerator()
		{
			bool flag = this.isDisposed_;
			if (flag)
			{
				throw new ObjectDisposedException("ZipFile");
			}
			return new ZipFile.ZipEntryEnumerator(this.entries_);
		}

		// Token: 0x060005B1 RID: 1457 RVA: 0x0002C644 File Offset: 0x0002A844
		public int FindEntry(string name, bool ignoreCase)
		{
			bool flag = this.isDisposed_;
			if (flag)
			{
				throw new ObjectDisposedException("ZipFile");
			}
			for (int i = 0; i < this.entries_.Length; i++)
			{
				bool flag2 = string.Compare(name, this.entries_[i].Name, ignoreCase, CultureInfo.InvariantCulture) == 0;
				if (flag2)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x060005B2 RID: 1458 RVA: 0x0002C6C4 File Offset: 0x0002A8C4
		public ZipEntry GetEntry(string name)
		{
			bool flag = this.isDisposed_;
			if (flag)
			{
				throw new ObjectDisposedException("ZipFile");
			}
			int num = this.FindEntry(name, true);
			return (num >= 0) ? ((ZipEntry)this.entries_[num].Clone()) : null;
		}

		// Token: 0x060005B3 RID: 1459 RVA: 0x0002C724 File Offset: 0x0002A924
		public Stream GetInputStream(ZipEntry entry)
		{
			bool flag = entry == null;
			if (flag)
			{
				throw new ArgumentNullException("entry");
			}
			bool flag2 = this.isDisposed_;
			if (flag2)
			{
				throw new ObjectDisposedException("ZipFile");
			}
			long num = entry.ZipFileIndex;
			bool flag3 = num < 0L || num >= (long)this.entries_.Length || this.entries_[(int)(checked((IntPtr)num))].Name != entry.Name;
			if (flag3)
			{
				num = (long)this.FindEntry(entry.Name, true);
				bool flag4 = num < 0L;
				if (flag4)
				{
					throw new ZipException("Entry cannot be found");
				}
			}
			return this.GetInputStream(num);
		}

		// Token: 0x060005B4 RID: 1460 RVA: 0x0002C7EC File Offset: 0x0002A9EC
		public Stream GetInputStream(long entryIndex)
		{
			bool flag = this.isDisposed_;
			if (flag)
			{
				throw new ObjectDisposedException("ZipFile");
			}
			checked
			{
				long start = this.LocateEntry(this.entries_[(int)((IntPtr)entryIndex)]);
				CompressionMethod compressionMethod = this.entries_[(int)((IntPtr)entryIndex)].CompressionMethod;
				Stream stream = new ZipFile.PartialInputStream(this, start, this.entries_[(int)((IntPtr)entryIndex)].CompressedSize);
				bool isCrypted = this.entries_[(int)((IntPtr)entryIndex)].IsCrypted;
				if (isCrypted)
				{
					stream = this.CreateAndInitDecryptionStream(stream, this.entries_[(int)((IntPtr)entryIndex)]);
					bool flag2 = stream == null;
					if (flag2)
					{
						throw new ZipException("Unable to decrypt this entry");
					}
				}
				CompressionMethod compressionMethod2 = compressionMethod;
				CompressionMethod compressionMethod3 = compressionMethod2;
				if (compressionMethod3 != CompressionMethod.Stored)
				{
					if (compressionMethod3 != CompressionMethod.Deflated)
					{
						throw new ZipException("Unsupported compression method " + compressionMethod.ToString());
					}
					stream = new InflaterInputStream(stream, new Inflater(true));
				}
				return stream;
			}
		}

		// Token: 0x060005B5 RID: 1461 RVA: 0x0002C904 File Offset: 0x0002AB04
		public bool TestArchive(bool testData)
		{
			return this.TestArchive(testData, TestStrategy.FindFirstError, null);
		}

		// Token: 0x060005B6 RID: 1462 RVA: 0x0002C928 File Offset: 0x0002AB28
		public bool TestArchive(bool testData, TestStrategy strategy, ZipTestResultHandler resultHandler)
		{
			bool flag = this.isDisposed_;
			if (flag)
			{
				throw new ObjectDisposedException("ZipFile");
			}
			TestStatus testStatus = new TestStatus(this);
			bool flag2 = resultHandler != null;
			if (flag2)
			{
				resultHandler(testStatus, null);
			}
			ZipFile.HeaderTest tests = testData ? (ZipFile.HeaderTest.Extract | ZipFile.HeaderTest.Header) : ZipFile.HeaderTest.Header;
			bool flag3 = true;
			try
			{
				int num = 0;
				while (flag3 && (long)num < this.Count)
				{
					bool flag4 = resultHandler != null;
					if (flag4)
					{
						testStatus.SetEntry(this[num]);
						testStatus.SetOperation(TestOperation.EntryHeader);
						resultHandler(testStatus, null);
					}
					try
					{
						this.TestLocalHeader(this[num], tests);
					}
					catch (ZipException ex)
					{
						testStatus.AddError();
						bool flag5 = resultHandler != null;
						if (flag5)
						{
							resultHandler(testStatus, string.Format("Exception during test - '{0}'", ex.Message));
						}
						bool flag6 = strategy == TestStrategy.FindFirstError;
						if (flag6)
						{
							flag3 = false;
						}
					}
					bool flag7 = flag3 && testData && this[num].IsFile;
					if (flag7)
					{
						bool flag8 = resultHandler != null;
						if (flag8)
						{
							testStatus.SetOperation(TestOperation.EntryData);
							resultHandler(testStatus, null);
						}
						Crc32 crc = new Crc32();
						using (Stream inputStream = this.GetInputStream(this[num]))
						{
							byte[] array = new byte[4096];
							long num2 = 0L;
							int num3;
							while ((num3 = inputStream.Read(array, 0, array.Length)) > 0)
							{
								crc.Update(array, 0, num3);
								bool flag9 = resultHandler != null;
								if (flag9)
								{
									num2 += (long)num3;
									testStatus.SetBytesTested(num2);
									resultHandler(testStatus, null);
								}
							}
						}
						bool flag10 = this[num].Crc != crc.Value;
						if (flag10)
						{
							testStatus.AddError();
							bool flag11 = resultHandler != null;
							if (flag11)
							{
								resultHandler(testStatus, "CRC mismatch");
							}
							bool flag12 = strategy == TestStrategy.FindFirstError;
							if (flag12)
							{
								flag3 = false;
							}
						}
						bool flag13 = (this[num].Flags & 8) != 0;
						if (flag13)
						{
							ZipHelperStream zipHelperStream = new ZipHelperStream(this.baseStream_);
							DescriptorData descriptorData = new DescriptorData();
							zipHelperStream.ReadDataDescriptor(this[num].LocalHeaderRequiresZip64, descriptorData);
							bool flag14 = this[num].Crc != descriptorData.Crc;
							if (flag14)
							{
								testStatus.AddError();
							}
							bool flag15 = this[num].CompressedSize != descriptorData.CompressedSize;
							if (flag15)
							{
								testStatus.AddError();
							}
							bool flag16 = this[num].Size != descriptorData.Size;
							if (flag16)
							{
								testStatus.AddError();
							}
						}
					}
					bool flag17 = resultHandler != null;
					if (flag17)
					{
						testStatus.SetOperation(TestOperation.EntryComplete);
						resultHandler(testStatus, null);
					}
					num++;
				}
				bool flag18 = resultHandler != null;
				if (flag18)
				{
					testStatus.SetOperation(TestOperation.MiscellaneousTests);
					resultHandler(testStatus, null);
				}
			}
			catch (Exception ex2)
			{
				testStatus.AddError();
				bool flag19 = resultHandler != null;
				if (flag19)
				{
					resultHandler(testStatus, string.Format("Exception during test - '{0}'", ex2.Message));
				}
			}
			bool flag20 = resultHandler != null;
			if (flag20)
			{
				testStatus.SetOperation(TestOperation.Complete);
				testStatus.SetEntry(null);
				resultHandler(testStatus, null);
			}
			return testStatus.ErrorCount == 0;
		}

		// Token: 0x060005B7 RID: 1463 RVA: 0x0002CD44 File Offset: 0x0002AF44
		private long TestLocalHeader(ZipEntry entry, ZipFile.HeaderTest tests)
		{
			Stream obj = this.baseStream_;
			long result;
			lock (obj)
			{
				bool flag = (tests & ZipFile.HeaderTest.Header) > (ZipFile.HeaderTest)0;
				bool flag2 = (tests & ZipFile.HeaderTest.Extract) > (ZipFile.HeaderTest)0;
				this.baseStream_.Seek(this.offsetOfFirstEntry + entry.Offset, SeekOrigin.Begin);
				bool flag3 = this.ReadLEUint() != 67324752U;
				if (flag3)
				{
					throw new ZipException(string.Format("Wrong local header signature @{0:X}", this.offsetOfFirstEntry + entry.Offset));
				}
				short num = (short)this.ReadLEUshort();
				short num2 = (short)this.ReadLEUshort();
				short num3 = (short)this.ReadLEUshort();
				short num4 = (short)this.ReadLEUshort();
				short num5 = (short)this.ReadLEUshort();
				uint num6 = this.ReadLEUint();
				long num7 = (long)((ulong)this.ReadLEUint());
				long num8 = (long)((ulong)this.ReadLEUint());
				int num9 = (int)this.ReadLEUshort();
				int num10 = (int)this.ReadLEUshort();
				byte[] array = new byte[num9];
				StreamUtils.ReadFully(this.baseStream_, array);
				byte[] array2 = new byte[num10];
				StreamUtils.ReadFully(this.baseStream_, array2);
				ZipExtraData zipExtraData = new ZipExtraData(array2);
				bool flag4 = zipExtraData.Find(1);
				if (flag4)
				{
					num8 = zipExtraData.ReadLong();
					num7 = zipExtraData.ReadLong();
					bool flag5 = (num2 & 8) != 0;
					if (flag5)
					{
						bool flag6 = num8 != -1L && num8 != entry.Size;
						if (flag6)
						{
							throw new ZipException("Size invalid for descriptor");
						}
						bool flag7 = num7 != -1L && num7 != entry.CompressedSize;
						if (flag7)
						{
							throw new ZipException("Compressed size invalid for descriptor");
						}
					}
				}
				else
				{
					bool flag8 = num >= 45 && ((uint)num8 == uint.MaxValue || (uint)num7 == uint.MaxValue);
					if (flag8)
					{
						throw new ZipException("Required Zip64 extended information missing");
					}
				}
				bool flag9 = flag2;
				if (flag9)
				{
					bool isFile = entry.IsFile;
					if (isFile)
					{
						bool flag10 = !entry.IsCompressionMethodSupported();
						if (flag10)
						{
							throw new ZipException("Compression method not supported");
						}
						bool flag11 = num > 51 || (num > 20 && num < 45);
						if (flag11)
						{
							throw new ZipException(string.Format("Version required to extract this entry not supported ({0})", num));
						}
						bool flag12 = (num2 & 12384) != 0;
						if (flag12)
						{
							throw new ZipException("The library does not support the zip version required to extract this entry");
						}
					}
				}
				bool flag13 = flag;
				if (flag13)
				{
					bool flag14 = num <= 63 && num != 10 && num != 11 && num != 20 && num != 21 && num != 25 && num != 27 && num != 45 && num != 46 && num != 50 && num != 51 && num != 52 && num != 61 && num != 62 && num != 63;
					if (flag14)
					{
						throw new ZipException(string.Format("Version required to extract this entry is invalid ({0})", num));
					}
					bool flag15 = ((int)num2 & 49168) != 0;
					if (flag15)
					{
						throw new ZipException("Reserved bit flags cannot be set.");
					}
					bool flag16 = (num2 & 1) != 0 && num < 20;
					if (flag16)
					{
						throw new ZipException(string.Format("Version required to extract this entry is too low for encryption ({0})", num));
					}
					bool flag17 = (num2 & 64) != 0;
					if (flag17)
					{
						bool flag18 = (num2 & 1) == 0;
						if (flag18)
						{
							throw new ZipException("Strong encryption flag set but encryption flag is not set");
						}
						bool flag19 = num < 50;
						if (flag19)
						{
							throw new ZipException(string.Format("Version required to extract this entry is too low for encryption ({0})", num));
						}
					}
					bool flag20 = (num2 & 32) != 0 && num < 27;
					if (flag20)
					{
						throw new ZipException(string.Format("Patched data requires higher version than ({0})", num));
					}
					bool flag21 = (int)num2 != entry.Flags;
					if (flag21)
					{
						throw new ZipException("Central header/local header flags mismatch");
					}
					bool flag22 = entry.CompressionMethod != (CompressionMethod)num3;
					if (flag22)
					{
						throw new ZipException("Central header/local header compression method mismatch");
					}
					bool flag23 = entry.Version != (int)num;
					if (flag23)
					{
						throw new ZipException("Extract version mismatch");
					}
					bool flag24 = (num2 & 64) != 0;
					if (flag24)
					{
						bool flag25 = num < 62;
						if (flag25)
						{
							throw new ZipException("Strong encryption flag set but version not high enough");
						}
					}
					bool flag26 = (num2 & 8192) != 0;
					if (flag26)
					{
						bool flag27 = num4 != 0 || num5 != 0;
						if (flag27)
						{
							throw new ZipException("Header masked set but date/time values non-zero");
						}
					}
					bool flag28 = (num2 & 8) == 0;
					if (flag28)
					{
						bool flag29 = num6 != (uint)entry.Crc;
						if (flag29)
						{
							throw new ZipException("Central header/local header crc mismatch");
						}
					}
					bool flag30 = num8 == 0L && num7 == 0L;
					if (flag30)
					{
						bool flag31 = num6 > 0U;
						if (flag31)
						{
							throw new ZipException("Invalid CRC for empty entry");
						}
					}
					bool flag32 = entry.Name.Length > num9;
					if (flag32)
					{
						throw new ZipException("File name length mismatch");
					}
					string text = ZipConstants.ConvertToStringExt((int)num2, array);
					bool flag33 = text != entry.Name;
					if (flag33)
					{
						throw new ZipException("Central header and local header file name mismatch");
					}
					bool isDirectory = entry.IsDirectory;
					if (isDirectory)
					{
						bool flag34 = num8 > 0L;
						if (flag34)
						{
							throw new ZipException("Directory cannot have size");
						}
						bool isCrypted = entry.IsCrypted;
						if (isCrypted)
						{
							bool flag35 = num7 > 14L;
							if (flag35)
							{
								throw new ZipException("Directory compressed size invalid");
							}
						}
						else
						{
							bool flag36 = num7 > 2L;
							if (flag36)
							{
								throw new ZipException("Directory compressed size invalid");
							}
						}
					}
					bool flag37 = !ZipNameTransform.IsValidName(text, true);
					if (flag37)
					{
						throw new ZipException("Name is invalid");
					}
				}
				bool flag38 = (num2 & 8) == 0 || num8 > 0L || num7 > 0L;
				if (flag38)
				{
					bool flag39 = num8 != entry.Size;
					if (flag39)
					{
						throw new ZipException(string.Format("Size mismatch between central header({0}) and local header({1})", entry.Size, num8));
					}
					bool flag40 = num7 != entry.CompressedSize && num7 != (long)((ulong)-1) && num7 != -1L;
					if (flag40)
					{
						throw new ZipException(string.Format("Compressed size mismatch between central header({0}) and local header({1})", entry.CompressedSize, num7));
					}
				}
				int num11 = num9 + num10;
				result = this.offsetOfFirstEntry + entry.Offset + 30L + (long)num11;
			}
			return result;
		}

		// Token: 0x17000108 RID: 264
		// (get) Token: 0x060005B8 RID: 1464 RVA: 0x0002D494 File Offset: 0x0002B694
		// (set) Token: 0x060005B9 RID: 1465 RVA: 0x0002D4B8 File Offset: 0x0002B6B8
		public INameTransform NameTransform
		{
			get
			{
				return this.updateEntryFactory_.NameTransform;
			}
			set
			{
				this.updateEntryFactory_.NameTransform = value;
			}
		}

		// Token: 0x17000109 RID: 265
		// (get) Token: 0x060005BA RID: 1466 RVA: 0x0002D4C8 File Offset: 0x0002B6C8
		// (set) Token: 0x060005BB RID: 1467 RVA: 0x0002D4E8 File Offset: 0x0002B6E8
		public IEntryFactory EntryFactory
		{
			get
			{
				return this.updateEntryFactory_;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					this.updateEntryFactory_ = new ZipEntryFactory();
				}
				else
				{
					this.updateEntryFactory_ = value;
				}
			}
		}

		// Token: 0x1700010A RID: 266
		// (get) Token: 0x060005BC RID: 1468 RVA: 0x0002D520 File Offset: 0x0002B720
		// (set) Token: 0x060005BD RID: 1469 RVA: 0x0002D540 File Offset: 0x0002B740
		public int BufferSize
		{
			get
			{
				return this.bufferSize_;
			}
			set
			{
				bool flag = value < 1024;
				if (flag)
				{
					throw new ArgumentOutOfRangeException("value", "cannot be below 1024");
				}
				bool flag2 = this.bufferSize_ != value;
				if (flag2)
				{
					this.bufferSize_ = value;
					this.copyBuffer_ = null;
				}
			}
		}

		// Token: 0x1700010B RID: 267
		// (get) Token: 0x060005BE RID: 1470 RVA: 0x0002D598 File Offset: 0x0002B798
		public bool IsUpdating
		{
			get
			{
				return this.updates_ != null;
			}
		}

		// Token: 0x1700010C RID: 268
		// (get) Token: 0x060005BF RID: 1471 RVA: 0x0002D5BC File Offset: 0x0002B7BC
		// (set) Token: 0x060005C0 RID: 1472 RVA: 0x0002D5DC File Offset: 0x0002B7DC
		public UseZip64 UseZip64
		{
			get
			{
				return this.useZip64_;
			}
			set
			{
				this.useZip64_ = value;
			}
		}

		// Token: 0x060005C1 RID: 1473 RVA: 0x0002D5E8 File Offset: 0x0002B7E8
		public void BeginUpdate(IArchiveStorage archiveStorage, IDynamicDataSource dataSource)
		{
			bool flag = archiveStorage == null;
			if (flag)
			{
				throw new ArgumentNullException("archiveStorage");
			}
			bool flag2 = dataSource == null;
			if (flag2)
			{
				throw new ArgumentNullException("dataSource");
			}
			bool flag3 = this.isDisposed_;
			if (flag3)
			{
				throw new ObjectDisposedException("ZipFile");
			}
			bool isEmbeddedArchive = this.IsEmbeddedArchive;
			if (isEmbeddedArchive)
			{
				throw new ZipException("Cannot update embedded/SFX archives");
			}
			this.archiveStorage_ = archiveStorage;
			this.updateDataSource_ = dataSource;
			this.updateIndex_ = new Hashtable();
			this.updates_ = new ArrayList(this.entries_.Length);
			foreach (ZipEntry zipEntry in this.entries_)
			{
				int num = this.updates_.Add(new ZipFile.ZipUpdate(zipEntry));
				this.updateIndex_.Add(zipEntry.Name, num);
			}
			this.updates_.Sort(new ZipFile.UpdateComparer());
			int num2 = 0;
			foreach (object obj in this.updates_)
			{
				ZipFile.ZipUpdate zipUpdate = (ZipFile.ZipUpdate)obj;
				bool flag4 = num2 == this.updates_.Count - 1;
				if (flag4)
				{
					break;
				}
				zipUpdate.OffsetBasedSize = ((ZipFile.ZipUpdate)this.updates_[num2 + 1]).Entry.Offset - zipUpdate.Entry.Offset;
				num2++;
			}
			this.updateCount_ = (long)this.updates_.Count;
			this.contentsEdited_ = false;
			this.commentEdited_ = false;
			this.newComment_ = null;
		}

		// Token: 0x060005C2 RID: 1474 RVA: 0x0002D7CC File Offset: 0x0002B9CC
		public void BeginUpdate(IArchiveStorage archiveStorage)
		{
			this.BeginUpdate(archiveStorage, new DynamicDiskDataSource());
		}

		// Token: 0x060005C3 RID: 1475 RVA: 0x0002D7DC File Offset: 0x0002B9DC
		public void BeginUpdate()
		{
			bool flag = this.Name == null;
			if (flag)
			{
				this.BeginUpdate(new MemoryArchiveStorage(), new DynamicDiskDataSource());
			}
			else
			{
				this.BeginUpdate(new DiskArchiveStorage(this), new DynamicDiskDataSource());
			}
		}

		// Token: 0x060005C4 RID: 1476 RVA: 0x0002D82C File Offset: 0x0002BA2C
		public void CommitUpdate()
		{
			bool flag = this.isDisposed_;
			if (flag)
			{
				throw new ObjectDisposedException("ZipFile");
			}
			this.CheckUpdating();
			try
			{
				this.updateIndex_.Clear();
				this.updateIndex_ = null;
				bool flag2 = this.contentsEdited_;
				if (flag2)
				{
					this.RunUpdates();
				}
				else
				{
					bool flag3 = this.commentEdited_;
					if (flag3)
					{
						this.UpdateCommentOnly();
					}
					else
					{
						bool flag4 = this.entries_.Length == 0;
						if (flag4)
						{
							byte[] comment = (this.newComment_ != null) ? this.newComment_.RawComment : ZipConstants.ConvertToArray(this.comment_);
							using (ZipHelperStream zipHelperStream = new ZipHelperStream(this.baseStream_))
							{
								zipHelperStream.WriteEndOfCentralDirectory(0L, 0L, 0L, comment);
							}
						}
					}
				}
			}
			finally
			{
				this.PostUpdateCleanup();
			}
		}

		// Token: 0x060005C5 RID: 1477 RVA: 0x0002D944 File Offset: 0x0002BB44
		public void AbortUpdate()
		{
			this.PostUpdateCleanup();
		}

		// Token: 0x060005C6 RID: 1478 RVA: 0x0002D950 File Offset: 0x0002BB50
		public void SetComment(string comment)
		{
			bool flag = this.isDisposed_;
			if (flag)
			{
				throw new ObjectDisposedException("ZipFile");
			}
			this.CheckUpdating();
			this.newComment_ = new ZipFile.ZipString(comment);
			bool flag2 = this.newComment_.RawLength > 65535;
			if (flag2)
			{
				this.newComment_ = null;
				throw new ZipException("Comment length exceeds maximum - 65535");
			}
			this.commentEdited_ = true;
		}

		// Token: 0x060005C7 RID: 1479 RVA: 0x0002D9C4 File Offset: 0x0002BBC4
		private void AddUpdate(ZipFile.ZipUpdate update)
		{
			this.contentsEdited_ = true;
			int num = this.FindExistingUpdate(update.Entry.Name);
			bool flag = num >= 0;
			if (flag)
			{
				bool flag2 = this.updates_[num] == null;
				if (flag2)
				{
					this.updateCount_ += 1L;
				}
				this.updates_[num] = update;
			}
			else
			{
				num = this.updates_.Add(update);
				this.updateCount_ += 1L;
				this.updateIndex_.Add(update.Entry.Name, num);
			}
		}

		// Token: 0x060005C8 RID: 1480 RVA: 0x0002DA74 File Offset: 0x0002BC74
		public void Add(string fileName, CompressionMethod compressionMethod, bool useUnicodeText)
		{
			bool flag = fileName == null;
			if (flag)
			{
				throw new ArgumentNullException("fileName");
			}
			bool flag2 = this.isDisposed_;
			if (flag2)
			{
				throw new ObjectDisposedException("ZipFile");
			}
			bool flag3 = !ZipEntry.IsCompressionMethodSupported(compressionMethod);
			if (flag3)
			{
				throw new ArgumentOutOfRangeException("compressionMethod");
			}
			this.CheckUpdating();
			this.contentsEdited_ = true;
			ZipEntry zipEntry = this.EntryFactory.MakeFileEntry(fileName);
			zipEntry.IsUnicodeText = useUnicodeText;
			zipEntry.CompressionMethod = compressionMethod;
			this.AddUpdate(new ZipFile.ZipUpdate(fileName, zipEntry));
		}

		// Token: 0x060005C9 RID: 1481 RVA: 0x0002DB0C File Offset: 0x0002BD0C
		public void Add(string fileName, CompressionMethod compressionMethod)
		{
			bool flag = fileName == null;
			if (flag)
			{
				throw new ArgumentNullException("fileName");
			}
			bool flag2 = !ZipEntry.IsCompressionMethodSupported(compressionMethod);
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("compressionMethod");
			}
			this.CheckUpdating();
			this.contentsEdited_ = true;
			ZipEntry zipEntry = this.EntryFactory.MakeFileEntry(fileName);
			zipEntry.CompressionMethod = compressionMethod;
			this.AddUpdate(new ZipFile.ZipUpdate(fileName, zipEntry));
		}

		// Token: 0x060005CA RID: 1482 RVA: 0x0002DB84 File Offset: 0x0002BD84
		public void Add(string fileName)
		{
			bool flag = fileName == null;
			if (flag)
			{
				throw new ArgumentNullException("fileName");
			}
			this.CheckUpdating();
			this.AddUpdate(new ZipFile.ZipUpdate(fileName, this.EntryFactory.MakeFileEntry(fileName)));
		}

		// Token: 0x060005CB RID: 1483 RVA: 0x0002DBD0 File Offset: 0x0002BDD0
		public void Add(string fileName, string entryName)
		{
			bool flag = fileName == null;
			if (flag)
			{
				throw new ArgumentNullException("fileName");
			}
			bool flag2 = entryName == null;
			if (flag2)
			{
				throw new ArgumentNullException("entryName");
			}
			this.CheckUpdating();
			this.AddUpdate(new ZipFile.ZipUpdate(fileName, this.EntryFactory.MakeFileEntry(entryName)));
		}

		// Token: 0x060005CC RID: 1484 RVA: 0x0002DC30 File Offset: 0x0002BE30
		public void Add(IStaticDataSource dataSource, string entryName)
		{
			bool flag = dataSource == null;
			if (flag)
			{
				throw new ArgumentNullException("dataSource");
			}
			bool flag2 = entryName == null;
			if (flag2)
			{
				throw new ArgumentNullException("entryName");
			}
			this.CheckUpdating();
			this.AddUpdate(new ZipFile.ZipUpdate(dataSource, this.EntryFactory.MakeFileEntry(entryName, false)));
		}

		// Token: 0x060005CD RID: 1485 RVA: 0x0002DC94 File Offset: 0x0002BE94
		public void Add(IStaticDataSource dataSource, string entryName, CompressionMethod compressionMethod)
		{
			bool flag = dataSource == null;
			if (flag)
			{
				throw new ArgumentNullException("dataSource");
			}
			bool flag2 = entryName == null;
			if (flag2)
			{
				throw new ArgumentNullException("entryName");
			}
			this.CheckUpdating();
			ZipEntry zipEntry = this.EntryFactory.MakeFileEntry(entryName, false);
			zipEntry.CompressionMethod = compressionMethod;
			this.AddUpdate(new ZipFile.ZipUpdate(dataSource, zipEntry));
		}

		// Token: 0x060005CE RID: 1486 RVA: 0x0002DD00 File Offset: 0x0002BF00
		public void Add(IStaticDataSource dataSource, string entryName, CompressionMethod compressionMethod, bool useUnicodeText)
		{
			bool flag = dataSource == null;
			if (flag)
			{
				throw new ArgumentNullException("dataSource");
			}
			bool flag2 = entryName == null;
			if (flag2)
			{
				throw new ArgumentNullException("entryName");
			}
			this.CheckUpdating();
			ZipEntry zipEntry = this.EntryFactory.MakeFileEntry(entryName, false);
			zipEntry.IsUnicodeText = useUnicodeText;
			zipEntry.CompressionMethod = compressionMethod;
			this.AddUpdate(new ZipFile.ZipUpdate(dataSource, zipEntry));
		}

		// Token: 0x060005CF RID: 1487 RVA: 0x0002DD74 File Offset: 0x0002BF74
		public void Add(ZipEntry entry)
		{
			bool flag = entry == null;
			if (flag)
			{
				throw new ArgumentNullException("entry");
			}
			this.CheckUpdating();
			bool flag2 = entry.Size != 0L || entry.CompressedSize != 0L;
			if (flag2)
			{
				throw new ZipException("Entry cannot have any data");
			}
			this.AddUpdate(new ZipFile.ZipUpdate(ZipFile.UpdateCommand.Add, entry));
		}

		// Token: 0x060005D0 RID: 1488 RVA: 0x0002DDE0 File Offset: 0x0002BFE0
		public void AddDirectory(string directoryName)
		{
			bool flag = directoryName == null;
			if (flag)
			{
				throw new ArgumentNullException("directoryName");
			}
			this.CheckUpdating();
			ZipEntry entry = this.EntryFactory.MakeDirectoryEntry(directoryName);
			this.AddUpdate(new ZipFile.ZipUpdate(ZipFile.UpdateCommand.Add, entry));
		}

		// Token: 0x060005D1 RID: 1489 RVA: 0x0002DE2C File Offset: 0x0002C02C
		public bool Delete(string fileName)
		{
			bool flag = fileName == null;
			if (flag)
			{
				throw new ArgumentNullException("fileName");
			}
			this.CheckUpdating();
			int num = this.FindExistingUpdate(fileName);
			bool flag2 = num >= 0 && this.updates_[num] != null;
			if (flag2)
			{
				bool result = true;
				this.contentsEdited_ = true;
				this.updates_[num] = null;
				this.updateCount_ -= 1L;
				return result;
			}
			throw new ZipException("Cannot find entry to delete");
		}

		// Token: 0x060005D2 RID: 1490 RVA: 0x0002DECC File Offset: 0x0002C0CC
		public void Delete(ZipEntry entry)
		{
			bool flag = entry == null;
			if (flag)
			{
				throw new ArgumentNullException("entry");
			}
			this.CheckUpdating();
			int num = this.FindExistingUpdate(entry);
			bool flag2 = num >= 0;
			if (flag2)
			{
				this.contentsEdited_ = true;
				this.updates_[num] = null;
				this.updateCount_ -= 1L;
				return;
			}
			throw new ZipException("Cannot find entry to delete");
		}

		// Token: 0x060005D3 RID: 1491 RVA: 0x0002DF4C File Offset: 0x0002C14C
		private void WriteLEShort(int value)
		{
			this.baseStream_.WriteByte((byte)(value & 255));
			this.baseStream_.WriteByte((byte)(value >> 8 & 255));
		}

		// Token: 0x060005D4 RID: 1492 RVA: 0x0002DF7C File Offset: 0x0002C17C
		private void WriteLEUshort(ushort value)
		{
			this.baseStream_.WriteByte((byte)(value & 255));
			this.baseStream_.WriteByte((byte)(value >> 8));
		}

		// Token: 0x060005D5 RID: 1493 RVA: 0x0002DFA4 File Offset: 0x0002C1A4
		private void WriteLEInt(int value)
		{
			this.WriteLEShort(value & 65535);
			this.WriteLEShort(value >> 16);
		}

		// Token: 0x060005D6 RID: 1494 RVA: 0x0002DFC0 File Offset: 0x0002C1C0
		private void WriteLEUint(uint value)
		{
			this.WriteLEUshort((ushort)(value & 65535U));
			this.WriteLEUshort((ushort)(value >> 16));
		}

		// Token: 0x060005D7 RID: 1495 RVA: 0x0002DFE0 File Offset: 0x0002C1E0
		private void WriteLeLong(long value)
		{
			this.WriteLEInt((int)(value & (long)((ulong)-1)));
			this.WriteLEInt((int)(value >> 32));
		}

		// Token: 0x060005D8 RID: 1496 RVA: 0x0002DFFC File Offset: 0x0002C1FC
		private void WriteLEUlong(ulong value)
		{
			this.WriteLEUint((uint)(value & (ulong)-1));
			this.WriteLEUint((uint)(value >> 32));
		}

		// Token: 0x060005D9 RID: 1497 RVA: 0x0002E018 File Offset: 0x0002C218
		private void WriteLocalEntryHeader(ZipFile.ZipUpdate update)
		{
			ZipEntry outEntry = update.OutEntry;
			outEntry.Offset = this.baseStream_.Position;
			bool flag = update.Command > ZipFile.UpdateCommand.Copy;
			if (flag)
			{
				bool flag2 = outEntry.CompressionMethod == CompressionMethod.Deflated;
				if (flag2)
				{
					bool flag3 = outEntry.Size == 0L;
					if (flag3)
					{
						outEntry.CompressedSize = outEntry.Size;
						outEntry.Crc = 0L;
						outEntry.CompressionMethod = CompressionMethod.Stored;
					}
				}
				else
				{
					bool flag4 = outEntry.CompressionMethod == CompressionMethod.Stored;
					if (flag4)
					{
						outEntry.Flags &= -9;
					}
				}
				bool haveKeys = this.HaveKeys;
				if (haveKeys)
				{
					outEntry.IsCrypted = true;
					bool flag5 = outEntry.Crc < 0L;
					if (flag5)
					{
						outEntry.Flags |= 8;
					}
				}
				else
				{
					outEntry.IsCrypted = false;
				}
				switch (this.useZip64_)
				{
				case UseZip64.On:
					outEntry.ForceZip64();
					break;
				case UseZip64.Dynamic:
				{
					bool flag6 = outEntry.Size < 0L;
					if (flag6)
					{
						outEntry.ForceZip64();
					}
					break;
				}
				}
			}
			this.WriteLEInt(67324752);
			this.WriteLEShort(outEntry.Version);
			this.WriteLEShort(outEntry.Flags);
			this.WriteLEShort((int)((byte)outEntry.CompressionMethod));
			this.WriteLEInt((int)outEntry.DosTime);
			bool flag7 = !outEntry.HasCrc;
			if (flag7)
			{
				update.CrcPatchOffset = this.baseStream_.Position;
				this.WriteLEInt(0);
			}
			else
			{
				this.WriteLEInt((int)outEntry.Crc);
			}
			bool localHeaderRequiresZip = outEntry.LocalHeaderRequiresZip64;
			if (localHeaderRequiresZip)
			{
				this.WriteLEInt(-1);
				this.WriteLEInt(-1);
			}
			else
			{
				bool flag8 = outEntry.CompressedSize < 0L || outEntry.Size < 0L;
				if (flag8)
				{
					update.SizePatchOffset = this.baseStream_.Position;
				}
				this.WriteLEInt((int)outEntry.CompressedSize);
				this.WriteLEInt((int)outEntry.Size);
			}
			byte[] array = ZipConstants.ConvertToArray(outEntry.Flags, outEntry.Name);
			bool flag9 = array.Length > 65535;
			if (flag9)
			{
				throw new ZipException("Entry name too long.");
			}
			ZipExtraData zipExtraData = new ZipExtraData(outEntry.ExtraData);
			bool localHeaderRequiresZip2 = outEntry.LocalHeaderRequiresZip64;
			if (localHeaderRequiresZip2)
			{
				zipExtraData.StartNewEntry();
				zipExtraData.AddLeLong(outEntry.Size);
				zipExtraData.AddLeLong(outEntry.CompressedSize);
				zipExtraData.AddNewEntry(1);
			}
			else
			{
				zipExtraData.Delete(1);
			}
			outEntry.ExtraData = zipExtraData.GetEntryData();
			this.WriteLEShort(array.Length);
			this.WriteLEShort(outEntry.ExtraData.Length);
			bool flag10 = array.Length != 0;
			if (flag10)
			{
				this.baseStream_.Write(array, 0, array.Length);
			}
			bool localHeaderRequiresZip3 = outEntry.LocalHeaderRequiresZip64;
			if (localHeaderRequiresZip3)
			{
				bool flag11 = !zipExtraData.Find(1);
				if (flag11)
				{
					throw new ZipException("Internal error cannot find extra data");
				}
				update.SizePatchOffset = this.baseStream_.Position + (long)zipExtraData.CurrentReadIndex;
			}
			bool flag12 = outEntry.ExtraData.Length != 0;
			if (flag12)
			{
				this.baseStream_.Write(outEntry.ExtraData, 0, outEntry.ExtraData.Length);
			}
		}

		// Token: 0x060005DA RID: 1498 RVA: 0x0002E3BC File Offset: 0x0002C5BC
		private int WriteCentralDirectoryHeader(ZipEntry entry)
		{
			bool flag = entry.CompressedSize < 0L;
			if (flag)
			{
				throw new ZipException("Attempt to write central directory entry with unknown csize");
			}
			bool flag2 = entry.Size < 0L;
			if (flag2)
			{
				throw new ZipException("Attempt to write central directory entry with unknown size");
			}
			bool flag3 = entry.Crc < 0L;
			if (flag3)
			{
				throw new ZipException("Attempt to write central directory entry with unknown crc");
			}
			this.WriteLEInt(33639248);
			this.WriteLEShort(51);
			this.WriteLEShort(entry.Version);
			this.WriteLEShort(entry.Flags);
			this.WriteLEShort((int)((byte)entry.CompressionMethod));
			this.WriteLEInt((int)entry.DosTime);
			this.WriteLEInt((int)entry.Crc);
			bool flag4 = entry.IsZip64Forced() || entry.CompressedSize >= (long)((ulong)-1);
			if (flag4)
			{
				this.WriteLEInt(-1);
			}
			else
			{
				this.WriteLEInt((int)(entry.CompressedSize & (long)((ulong)-1)));
			}
			bool flag5 = entry.IsZip64Forced() || entry.Size >= (long)((ulong)-1);
			if (flag5)
			{
				this.WriteLEInt(-1);
			}
			else
			{
				this.WriteLEInt((int)entry.Size);
			}
			byte[] array = ZipConstants.ConvertToArray(entry.Flags, entry.Name);
			bool flag6 = array.Length > 65535;
			if (flag6)
			{
				throw new ZipException("Entry name is too long.");
			}
			this.WriteLEShort(array.Length);
			ZipExtraData zipExtraData = new ZipExtraData(entry.ExtraData);
			bool centralHeaderRequiresZip = entry.CentralHeaderRequiresZip64;
			if (centralHeaderRequiresZip)
			{
				zipExtraData.StartNewEntry();
				bool flag7 = entry.Size >= (long)((ulong)-1) || this.useZip64_ == UseZip64.On;
				if (flag7)
				{
					zipExtraData.AddLeLong(entry.Size);
				}
				bool flag8 = entry.CompressedSize >= (long)((ulong)-1) || this.useZip64_ == UseZip64.On;
				if (flag8)
				{
					zipExtraData.AddLeLong(entry.CompressedSize);
				}
				bool flag9 = entry.Offset >= (long)((ulong)-1);
				if (flag9)
				{
					zipExtraData.AddLeLong(entry.Offset);
				}
				zipExtraData.AddNewEntry(1);
			}
			else
			{
				zipExtraData.Delete(1);
			}
			byte[] entryData = zipExtraData.GetEntryData();
			this.WriteLEShort(entryData.Length);
			this.WriteLEShort((entry.Comment != null) ? entry.Comment.Length : 0);
			this.WriteLEShort(0);
			this.WriteLEShort(0);
			bool flag10 = entry.ExternalFileAttributes != -1;
			if (flag10)
			{
				this.WriteLEInt(entry.ExternalFileAttributes);
			}
			else
			{
				bool isDirectory = entry.IsDirectory;
				if (isDirectory)
				{
					this.WriteLEUint(16U);
				}
				else
				{
					this.WriteLEUint(0U);
				}
			}
			bool flag11 = entry.Offset >= (long)((ulong)-1);
			if (flag11)
			{
				this.WriteLEUint(uint.MaxValue);
			}
			else
			{
				this.WriteLEUint((uint)((int)entry.Offset));
			}
			bool flag12 = array.Length != 0;
			if (flag12)
			{
				this.baseStream_.Write(array, 0, array.Length);
			}
			bool flag13 = entryData.Length != 0;
			if (flag13)
			{
				this.baseStream_.Write(entryData, 0, entryData.Length);
			}
			byte[] array2 = (entry.Comment != null) ? Encoding.ASCII.GetBytes(entry.Comment) : new byte[0];
			bool flag14 = array2.Length != 0;
			if (flag14)
			{
				this.baseStream_.Write(array2, 0, array2.Length);
			}
			return 46 + array.Length + entryData.Length + array2.Length;
		}

		// Token: 0x060005DB RID: 1499 RVA: 0x0002E798 File Offset: 0x0002C998
		private void PostUpdateCleanup()
		{
			this.updateDataSource_ = null;
			this.updates_ = null;
			this.updateIndex_ = null;
			bool flag = this.archiveStorage_ != null;
			if (flag)
			{
				this.archiveStorage_.Dispose();
				this.archiveStorage_ = null;
			}
		}

		// Token: 0x060005DC RID: 1500 RVA: 0x0002E7E4 File Offset: 0x0002C9E4
		private string GetTransformedFileName(string name)
		{
			INameTransform nameTransform = this.NameTransform;
			return (nameTransform != null) ? nameTransform.TransformFile(name) : name;
		}

		// Token: 0x060005DD RID: 1501 RVA: 0x0002E818 File Offset: 0x0002CA18
		private string GetTransformedDirectoryName(string name)
		{
			INameTransform nameTransform = this.NameTransform;
			return (nameTransform != null) ? nameTransform.TransformDirectory(name) : name;
		}

		// Token: 0x060005DE RID: 1502 RVA: 0x0002E84C File Offset: 0x0002CA4C
		private byte[] GetBuffer()
		{
			bool flag = this.copyBuffer_ == null;
			if (flag)
			{
				this.copyBuffer_ = new byte[this.bufferSize_];
			}
			return this.copyBuffer_;
		}

		// Token: 0x060005DF RID: 1503 RVA: 0x0002E890 File Offset: 0x0002CA90
		private void CopyDescriptorBytes(ZipFile.ZipUpdate update, Stream dest, Stream source)
		{
			int i = this.GetDescriptorSize(update);
			bool flag = i > 0;
			if (flag)
			{
				byte[] buffer = this.GetBuffer();
				while (i > 0)
				{
					int count = Math.Min(buffer.Length, i);
					int num = source.Read(buffer, 0, count);
					bool flag2 = num > 0;
					if (!flag2)
					{
						throw new ZipException("Unxpected end of stream");
					}
					dest.Write(buffer, 0, num);
					i -= num;
				}
			}
		}

		// Token: 0x060005E0 RID: 1504 RVA: 0x0002E918 File Offset: 0x0002CB18
		private void CopyBytes(ZipFile.ZipUpdate update, Stream destination, Stream source, long bytesToCopy, bool updateCrc)
		{
			bool flag = destination == source;
			if (flag)
			{
				throw new InvalidOperationException("Destination and source are the same");
			}
			Crc32 crc = new Crc32();
			byte[] buffer = this.GetBuffer();
			long num = bytesToCopy;
			long num2 = 0L;
			int num4;
			do
			{
				int num3 = buffer.Length;
				bool flag2 = bytesToCopy < (long)num3;
				if (flag2)
				{
					num3 = (int)bytesToCopy;
				}
				num4 = source.Read(buffer, 0, num3);
				bool flag3 = num4 > 0;
				if (flag3)
				{
					if (updateCrc)
					{
						crc.Update(buffer, 0, num4);
					}
					destination.Write(buffer, 0, num4);
					bytesToCopy -= (long)num4;
					num2 += (long)num4;
				}
			}
			while (num4 > 0 && bytesToCopy > 0L);
			bool flag4 = num2 != num;
			if (flag4)
			{
				throw new ZipException(string.Format("Failed to copy bytes expected {0} read {1}", num, num2));
			}
			if (updateCrc)
			{
				update.OutEntry.Crc = crc.Value;
			}
		}

		// Token: 0x060005E1 RID: 1505 RVA: 0x0002EA30 File Offset: 0x0002CC30
		private int GetDescriptorSize(ZipFile.ZipUpdate update)
		{
			int result = 0;
			bool flag = (update.Entry.Flags & 8) != 0;
			if (flag)
			{
				result = 12;
				bool localHeaderRequiresZip = update.Entry.LocalHeaderRequiresZip64;
				if (localHeaderRequiresZip)
				{
					result = 20;
				}
			}
			return result;
		}

		// Token: 0x060005E2 RID: 1506 RVA: 0x0002EA80 File Offset: 0x0002CC80
		private void CopyDescriptorBytesDirect(ZipFile.ZipUpdate update, Stream stream, ref long destinationPosition, long sourcePosition)
		{
			int i = this.GetDescriptorSize(update);
			while (i > 0)
			{
				int count = i;
				byte[] buffer = this.GetBuffer();
				stream.Position = sourcePosition;
				int num = stream.Read(buffer, 0, count);
				bool flag = num > 0;
				if (!flag)
				{
					throw new ZipException("Unxpected end of stream");
				}
				stream.Position = destinationPosition;
				stream.Write(buffer, 0, num);
				i -= num;
				destinationPosition += (long)num;
				sourcePosition += (long)num;
			}
		}

		// Token: 0x060005E3 RID: 1507 RVA: 0x0002EB0C File Offset: 0x0002CD0C
		private void CopyEntryDataDirect(ZipFile.ZipUpdate update, Stream stream, bool updateCrc, ref long destinationPosition, ref long sourcePosition)
		{
			long num = update.Entry.CompressedSize;
			Crc32 crc = new Crc32();
			byte[] buffer = this.GetBuffer();
			long num2 = num;
			long num3 = 0L;
			int num5;
			do
			{
				int num4 = buffer.Length;
				bool flag = num < (long)num4;
				if (flag)
				{
					num4 = (int)num;
				}
				stream.Position = sourcePosition;
				num5 = stream.Read(buffer, 0, num4);
				bool flag2 = num5 > 0;
				if (flag2)
				{
					if (updateCrc)
					{
						crc.Update(buffer, 0, num5);
					}
					stream.Position = destinationPosition;
					stream.Write(buffer, 0, num5);
					destinationPosition += (long)num5;
					sourcePosition += (long)num5;
					num -= (long)num5;
					num3 += (long)num5;
				}
			}
			while (num5 > 0 && num > 0L);
			bool flag3 = num3 != num2;
			if (flag3)
			{
				throw new ZipException(string.Format("Failed to copy bytes expected {0} read {1}", num2, num3));
			}
			if (updateCrc)
			{
				update.OutEntry.Crc = crc.Value;
			}
		}

		// Token: 0x060005E4 RID: 1508 RVA: 0x0002EC38 File Offset: 0x0002CE38
		private int FindExistingUpdate(ZipEntry entry)
		{
			int result = -1;
			string transformedFileName = this.GetTransformedFileName(entry.Name);
			bool flag = this.updateIndex_.ContainsKey(transformedFileName);
			if (flag)
			{
				result = (int)this.updateIndex_[transformedFileName];
			}
			return result;
		}

		// Token: 0x060005E5 RID: 1509 RVA: 0x0002EC88 File Offset: 0x0002CE88
		private int FindExistingUpdate(string fileName)
		{
			int result = -1;
			string transformedFileName = this.GetTransformedFileName(fileName);
			bool flag = this.updateIndex_.ContainsKey(transformedFileName);
			if (flag)
			{
				result = (int)this.updateIndex_[transformedFileName];
			}
			return result;
		}

		// Token: 0x060005E6 RID: 1510 RVA: 0x0002ECD4 File Offset: 0x0002CED4
		private Stream GetOutputStream(ZipEntry entry)
		{
			Stream stream = this.baseStream_;
			bool isCrypted = entry.IsCrypted;
			if (isCrypted)
			{
				stream = this.CreateAndInitEncryptionStream(stream, entry);
			}
			CompressionMethod compressionMethod = entry.CompressionMethod;
			CompressionMethod compressionMethod2 = compressionMethod;
			if (compressionMethod2 != CompressionMethod.Stored)
			{
				if (compressionMethod2 != CompressionMethod.Deflated)
				{
					throw new ZipException("Unknown compression method " + entry.CompressionMethod.ToString());
				}
				stream = new DeflaterOutputStream(stream, new Deflater(9, true))
				{
					IsStreamOwner = false
				};
			}
			else
			{
				stream = new ZipFile.UncompressedStream(stream);
			}
			return stream;
		}

		// Token: 0x060005E7 RID: 1511 RVA: 0x0002ED84 File Offset: 0x0002CF84
		private void AddEntry(ZipFile workFile, ZipFile.ZipUpdate update)
		{
			Stream stream = null;
			bool isFile = update.Entry.IsFile;
			if (isFile)
			{
				stream = update.GetSource();
				bool flag = stream == null;
				if (flag)
				{
					stream = this.updateDataSource_.GetSource(update.Entry, update.Filename);
				}
			}
			bool flag2 = stream != null;
			if (flag2)
			{
				using (stream)
				{
					long length = stream.Length;
					bool flag3 = update.OutEntry.Size < 0L;
					if (flag3)
					{
						update.OutEntry.Size = length;
					}
					else
					{
						bool flag4 = update.OutEntry.Size != length;
						if (flag4)
						{
							throw new ZipException("Entry size/stream size mismatch");
						}
					}
					workFile.WriteLocalEntryHeader(update);
					long position = workFile.baseStream_.Position;
					using (Stream outputStream = workFile.GetOutputStream(update.OutEntry))
					{
						this.CopyBytes(update, outputStream, stream, length, true);
					}
					long position2 = workFile.baseStream_.Position;
					update.OutEntry.CompressedSize = position2 - position;
					bool flag5 = (update.OutEntry.Flags & 8) == 8;
					if (flag5)
					{
						ZipHelperStream zipHelperStream = new ZipHelperStream(workFile.baseStream_);
						zipHelperStream.WriteDataDescriptor(update.OutEntry);
					}
				}
			}
			else
			{
				workFile.WriteLocalEntryHeader(update);
				update.OutEntry.CompressedSize = 0L;
			}
		}

		// Token: 0x060005E8 RID: 1512 RVA: 0x0002EF48 File Offset: 0x0002D148
		private void ModifyEntry(ZipFile workFile, ZipFile.ZipUpdate update)
		{
			workFile.WriteLocalEntryHeader(update);
			long position = workFile.baseStream_.Position;
			bool flag = update.Entry.IsFile && update.Filename != null;
			if (flag)
			{
				using (Stream outputStream = workFile.GetOutputStream(update.OutEntry))
				{
					using (Stream inputStream = this.GetInputStream(update.Entry))
					{
						this.CopyBytes(update, outputStream, inputStream, inputStream.Length, true);
					}
				}
			}
			long position2 = workFile.baseStream_.Position;
			update.Entry.CompressedSize = position2 - position;
		}

		// Token: 0x060005E9 RID: 1513 RVA: 0x0002F020 File Offset: 0x0002D220
		private void CopyEntryDirect(ZipFile workFile, ZipFile.ZipUpdate update, ref long destinationPosition)
		{
			bool flag = false;
			bool flag2 = update.Entry.Offset == destinationPosition;
			if (flag2)
			{
				flag = true;
			}
			bool flag3 = !flag;
			if (flag3)
			{
				this.baseStream_.Position = destinationPosition;
				workFile.WriteLocalEntryHeader(update);
				destinationPosition = this.baseStream_.Position;
			}
			long num = 0L;
			long num2 = update.Entry.Offset + 26L;
			this.baseStream_.Seek(num2, SeekOrigin.Begin);
			uint num3 = (uint)this.ReadLEUshort();
			uint num4 = (uint)this.ReadLEUshort();
			num = this.baseStream_.Position + (long)((ulong)num3) + (long)((ulong)num4);
			bool flag4 = flag;
			if (flag4)
			{
				bool flag5 = update.OffsetBasedSize != -1L;
				if (flag5)
				{
					destinationPosition += update.OffsetBasedSize;
				}
				else
				{
					destinationPosition += num - num2 + 26L + update.Entry.CompressedSize + (long)this.GetDescriptorSize(update);
				}
			}
			else
			{
				bool flag6 = update.Entry.CompressedSize > 0L;
				if (flag6)
				{
					this.CopyEntryDataDirect(update, this.baseStream_, false, ref destinationPosition, ref num);
				}
				this.CopyDescriptorBytesDirect(update, this.baseStream_, ref destinationPosition, num);
			}
		}

		// Token: 0x060005EA RID: 1514 RVA: 0x0002F15C File Offset: 0x0002D35C
		private void CopyEntry(ZipFile workFile, ZipFile.ZipUpdate update)
		{
			workFile.WriteLocalEntryHeader(update);
			bool flag = update.Entry.CompressedSize > 0L;
			if (flag)
			{
				long offset = update.Entry.Offset + 26L;
				this.baseStream_.Seek(offset, SeekOrigin.Begin);
				uint num = (uint)this.ReadLEUshort();
				uint num2 = (uint)this.ReadLEUshort();
				this.baseStream_.Seek((long)((ulong)(num + num2)), SeekOrigin.Current);
				this.CopyBytes(update, workFile.baseStream_, this.baseStream_, update.Entry.CompressedSize, false);
			}
			this.CopyDescriptorBytes(update, workFile.baseStream_, this.baseStream_);
		}

		// Token: 0x060005EB RID: 1515 RVA: 0x0002F200 File Offset: 0x0002D400
		private void Reopen(Stream source)
		{
			bool flag = source == null;
			if (flag)
			{
				throw new ZipException("Failed to reopen archive - no source");
			}
			this.isNewArchive_ = false;
			this.baseStream_ = source;
			this.ReadEntries();
		}

		// Token: 0x060005EC RID: 1516 RVA: 0x0002F240 File Offset: 0x0002D440
		private void Reopen()
		{
			bool flag = this.Name == null;
			if (flag)
			{
				throw new InvalidOperationException("Name is not known cannot Reopen");
			}
			this.Reopen(File.Open(this.Name, FileMode.Open, FileAccess.Read, FileShare.Read));
		}

		// Token: 0x060005ED RID: 1517 RVA: 0x0002F284 File Offset: 0x0002D484
		private void UpdateCommentOnly()
		{
			long length = this.baseStream_.Length;
			bool flag = this.archiveStorage_.UpdateMode == FileUpdateMode.Safe;
			ZipHelperStream zipHelperStream;
			if (flag)
			{
				Stream stream = this.archiveStorage_.MakeTemporaryCopy(this.baseStream_);
				zipHelperStream = new ZipHelperStream(stream);
				zipHelperStream.IsStreamOwner = true;
				this.baseStream_.Close();
				this.baseStream_ = null;
			}
			else
			{
				bool flag2 = this.archiveStorage_.UpdateMode == FileUpdateMode.Direct;
				if (flag2)
				{
					this.baseStream_ = this.archiveStorage_.OpenForDirectUpdate(this.baseStream_);
					zipHelperStream = new ZipHelperStream(this.baseStream_);
				}
				else
				{
					this.baseStream_.Close();
					this.baseStream_ = null;
					zipHelperStream = new ZipHelperStream(this.Name);
				}
			}
			using (zipHelperStream)
			{
				long num = zipHelperStream.LocateBlockWithSignature(101010256, length, 22, 65535);
				bool flag3 = num < 0L;
				if (flag3)
				{
					throw new ZipException("Cannot find central directory");
				}
				zipHelperStream.Position += 16L;
				byte[] rawComment = this.newComment_.RawComment;
				zipHelperStream.WriteLEShort(rawComment.Length);
				zipHelperStream.Write(rawComment, 0, rawComment.Length);
				zipHelperStream.SetLength(zipHelperStream.Position);
			}
			bool flag4 = this.archiveStorage_.UpdateMode == FileUpdateMode.Safe;
			if (flag4)
			{
				this.Reopen(this.archiveStorage_.ConvertTemporaryToFinal());
			}
			else
			{
				this.ReadEntries();
			}
		}

		// Token: 0x060005EE RID: 1518 RVA: 0x0002F42C File Offset: 0x0002D62C
		private void RunUpdates()
		{
			long num = 0L;
			long length = 0L;
			bool flag = false;
			long position = 0L;
			bool isNewArchive = this.IsNewArchive;
			ZipFile zipFile;
			if (isNewArchive)
			{
				zipFile = this;
				zipFile.baseStream_.Position = 0L;
				flag = true;
			}
			else
			{
				bool flag2 = this.archiveStorage_.UpdateMode == FileUpdateMode.Direct;
				if (flag2)
				{
					zipFile = this;
					zipFile.baseStream_.Position = 0L;
					flag = true;
					this.updates_.Sort(new ZipFile.UpdateComparer());
				}
				else
				{
					zipFile = ZipFile.Create(this.archiveStorage_.GetTemporaryOutput());
					zipFile.UseZip64 = this.UseZip64;
					bool flag3 = this.key != null;
					if (flag3)
					{
						zipFile.key = (byte[])this.key.Clone();
					}
				}
			}
			try
			{
				foreach (object obj in this.updates_)
				{
					ZipFile.ZipUpdate zipUpdate = (ZipFile.ZipUpdate)obj;
					bool flag4 = zipUpdate != null;
					if (flag4)
					{
						switch (zipUpdate.Command)
						{
						case ZipFile.UpdateCommand.Copy:
						{
							bool flag5 = flag;
							if (flag5)
							{
								this.CopyEntryDirect(zipFile, zipUpdate, ref position);
							}
							else
							{
								this.CopyEntry(zipFile, zipUpdate);
							}
							break;
						}
						case ZipFile.UpdateCommand.Modify:
							this.ModifyEntry(zipFile, zipUpdate);
							break;
						case ZipFile.UpdateCommand.Add:
						{
							bool flag6 = !this.IsNewArchive && flag;
							if (flag6)
							{
								zipFile.baseStream_.Position = position;
							}
							this.AddEntry(zipFile, zipUpdate);
							bool flag7 = flag;
							if (flag7)
							{
								position = zipFile.baseStream_.Position;
							}
							break;
						}
						}
					}
				}
				bool flag8 = !this.IsNewArchive && flag;
				if (flag8)
				{
					zipFile.baseStream_.Position = position;
				}
				long position2 = zipFile.baseStream_.Position;
				foreach (object obj2 in this.updates_)
				{
					ZipFile.ZipUpdate zipUpdate2 = (ZipFile.ZipUpdate)obj2;
					bool flag9 = zipUpdate2 != null;
					if (flag9)
					{
						num += (long)zipFile.WriteCentralDirectoryHeader(zipUpdate2.OutEntry);
					}
				}
				byte[] comment = (this.newComment_ != null) ? this.newComment_.RawComment : ZipConstants.ConvertToArray(this.comment_);
				using (ZipHelperStream zipHelperStream = new ZipHelperStream(zipFile.baseStream_))
				{
					zipHelperStream.WriteEndOfCentralDirectory(this.updateCount_, num, position2, comment);
				}
				length = zipFile.baseStream_.Position;
				foreach (object obj3 in this.updates_)
				{
					ZipFile.ZipUpdate zipUpdate3 = (ZipFile.ZipUpdate)obj3;
					bool flag10 = zipUpdate3 != null;
					if (flag10)
					{
						bool flag11 = zipUpdate3.CrcPatchOffset > 0L && zipUpdate3.OutEntry.CompressedSize > 0L;
						if (flag11)
						{
							zipFile.baseStream_.Position = zipUpdate3.CrcPatchOffset;
							zipFile.WriteLEInt((int)zipUpdate3.OutEntry.Crc);
						}
						bool flag12 = zipUpdate3.SizePatchOffset > 0L;
						if (flag12)
						{
							zipFile.baseStream_.Position = zipUpdate3.SizePatchOffset;
							bool localHeaderRequiresZip = zipUpdate3.OutEntry.LocalHeaderRequiresZip64;
							if (localHeaderRequiresZip)
							{
								zipFile.WriteLeLong(zipUpdate3.OutEntry.Size);
								zipFile.WriteLeLong(zipUpdate3.OutEntry.CompressedSize);
							}
							else
							{
								zipFile.WriteLEInt((int)zipUpdate3.OutEntry.CompressedSize);
								zipFile.WriteLEInt((int)zipUpdate3.OutEntry.Size);
							}
						}
					}
				}
			}
			catch
			{
				zipFile.Close();
				bool flag13 = !flag && zipFile.Name != null;
				if (flag13)
				{
					File.Delete(zipFile.Name);
				}
				throw;
			}
			bool flag14 = flag;
			if (flag14)
			{
				zipFile.baseStream_.SetLength(length);
				zipFile.baseStream_.Flush();
				this.isNewArchive_ = false;
				this.ReadEntries();
			}
			else
			{
				this.baseStream_.Close();
				this.Reopen(this.archiveStorage_.ConvertTemporaryToFinal());
			}
		}

		// Token: 0x060005EF RID: 1519 RVA: 0x0002F980 File Offset: 0x0002DB80
		private void CheckUpdating()
		{
			bool flag = this.updates_ == null;
			if (flag)
			{
				throw new InvalidOperationException("BeginUpdate has not been called");
			}
		}

		// Token: 0x060005F0 RID: 1520 RVA: 0x0002F9B0 File Offset: 0x0002DBB0
		void IDisposable.Dispose()
		{
			this.Close();
		}

		// Token: 0x060005F1 RID: 1521 RVA: 0x0002F9BC File Offset: 0x0002DBBC
		private void DisposeInternal(bool disposing)
		{
			bool flag = !this.isDisposed_;
			if (flag)
			{
				this.isDisposed_ = true;
				this.entries_ = new ZipEntry[0];
				bool flag2 = this.IsStreamOwner && this.baseStream_ != null;
				if (flag2)
				{
					Stream obj = this.baseStream_;
					lock (obj)
					{
						this.baseStream_.Close();
					}
				}
				this.PostUpdateCleanup();
			}
		}

		// Token: 0x060005F2 RID: 1522 RVA: 0x0002FA54 File Offset: 0x0002DC54
		protected virtual void Dispose(bool disposing)
		{
			this.DisposeInternal(disposing);
		}

		// Token: 0x060005F3 RID: 1523 RVA: 0x0002FA60 File Offset: 0x0002DC60
		private ushort ReadLEUshort()
		{
			int num = this.baseStream_.ReadByte();
			bool flag = num < 0;
			if (flag)
			{
				throw new EndOfStreamException("End of stream");
			}
			int num2 = this.baseStream_.ReadByte();
			bool flag2 = num2 < 0;
			if (flag2)
			{
				throw new EndOfStreamException("End of stream");
			}
			return (ushort)num | (ushort)(num2 << 8);
		}

		// Token: 0x060005F4 RID: 1524 RVA: 0x0002FACC File Offset: 0x0002DCCC
		private uint ReadLEUint()
		{
			return (uint)((int)this.ReadLEUshort() | (int)this.ReadLEUshort() << 16);
		}

		// Token: 0x060005F5 RID: 1525 RVA: 0x0002FAF8 File Offset: 0x0002DCF8
		private ulong ReadLEUlong()
		{
			return (ulong)this.ReadLEUint() | (ulong)this.ReadLEUint() << 32;
		}

		// Token: 0x060005F6 RID: 1526 RVA: 0x0002FB24 File Offset: 0x0002DD24
		private long LocateBlockWithSignature(int signature, long endLocation, int minimumBlockSize, int maximumVariableData)
		{
			long result;
			using (ZipHelperStream zipHelperStream = new ZipHelperStream(this.baseStream_))
			{
				result = zipHelperStream.LocateBlockWithSignature(signature, endLocation, minimumBlockSize, maximumVariableData);
			}
			return result;
		}

		// Token: 0x060005F7 RID: 1527 RVA: 0x0002FB70 File Offset: 0x0002DD70
		private void ReadEntries()
		{
			bool flag = !this.baseStream_.CanSeek;
			if (flag)
			{
				throw new ZipException("ZipFile stream must be seekable");
			}
			long num = this.LocateBlockWithSignature(101010256, this.baseStream_.Length, 22, 65535);
			bool flag2 = num < 0L;
			if (flag2)
			{
				throw new ZipException("Cannot find central directory");
			}
			ushort num2 = this.ReadLEUshort();
			ushort num3 = this.ReadLEUshort();
			ulong num4 = (ulong)this.ReadLEUshort();
			ulong num5 = (ulong)this.ReadLEUshort();
			ulong num6 = (ulong)this.ReadLEUint();
			long num7 = (long)((ulong)this.ReadLEUint());
			uint num8 = (uint)this.ReadLEUshort();
			bool flag3 = num8 > 0U;
			if (flag3)
			{
				byte[] array = new byte[num8];
				StreamUtils.ReadFully(this.baseStream_, array);
				this.comment_ = ZipConstants.ConvertToString(array);
			}
			else
			{
				this.comment_ = string.Empty;
			}
			bool flag4 = false;
			bool flag5 = num2 == ushort.MaxValue || num3 == ushort.MaxValue || num4 == 65535UL || num5 == 65535UL || num6 == (ulong)-1 || num7 == (long)((ulong)-1);
			if (flag5)
			{
				flag4 = true;
				long num9 = this.LocateBlockWithSignature(117853008, num, 0, 4096);
				bool flag6 = num9 < 0L;
				if (flag6)
				{
					throw new ZipException("Cannot find Zip64 locator");
				}
				this.ReadLEUint();
				ulong num10 = this.ReadLEUlong();
				uint num11 = this.ReadLEUint();
				this.baseStream_.Position = (long)num10;
				long num12 = (long)((ulong)this.ReadLEUint());
				bool flag7 = num12 != 101075792L;
				if (flag7)
				{
					throw new ZipException(string.Format("Invalid Zip64 Central directory signature at {0:X}", num10));
				}
				ulong num13 = this.ReadLEUlong();
				int num14 = (int)this.ReadLEUshort();
				int num15 = (int)this.ReadLEUshort();
				uint num16 = this.ReadLEUint();
				uint num17 = this.ReadLEUint();
				num4 = this.ReadLEUlong();
				num5 = this.ReadLEUlong();
				num6 = this.ReadLEUlong();
				num7 = (long)this.ReadLEUlong();
			}
			this.entries_ = new ZipEntry[num4];
			bool flag8 = !flag4 && num7 < num - (long)(4UL + num6);
			if (flag8)
			{
				this.offsetOfFirstEntry = num - (long)(4UL + num6 + (ulong)num7);
				bool flag9 = this.offsetOfFirstEntry <= 0L;
				if (flag9)
				{
					throw new ZipException("Invalid embedded zip archive");
				}
			}
			this.baseStream_.Seek(this.offsetOfFirstEntry + num7, SeekOrigin.Begin);
			for (ulong num18 = 0UL; num18 < num4; num18 += 1UL)
			{
				bool flag10 = this.ReadLEUint() != 33639248U;
				if (flag10)
				{
					throw new ZipException("Wrong Central Directory signature");
				}
				int madeByInfo = (int)this.ReadLEUshort();
				int versionRequiredToExtract = (int)this.ReadLEUshort();
				int num19 = (int)this.ReadLEUshort();
				int method = (int)this.ReadLEUshort();
				uint num20 = this.ReadLEUint();
				uint num21 = this.ReadLEUint();
				long num22 = (long)((ulong)this.ReadLEUint());
				long num23 = (long)((ulong)this.ReadLEUint());
				int num24 = (int)this.ReadLEUshort();
				int num25 = (int)this.ReadLEUshort();
				int num26 = (int)this.ReadLEUshort();
				int num27 = (int)this.ReadLEUshort();
				int num28 = (int)this.ReadLEUshort();
				uint externalFileAttributes = this.ReadLEUint();
				long offset = (long)((ulong)this.ReadLEUint());
				byte[] array2 = new byte[Math.Max(num24, num26)];
				StreamUtils.ReadFully(this.baseStream_, array2, 0, num24);
				string name = ZipConstants.ConvertToStringExt(num19, array2, num24);
				ZipEntry zipEntry = new ZipEntry(name, versionRequiredToExtract, madeByInfo, (CompressionMethod)method);
				zipEntry.Crc = (long)((ulong)num21 & (ulong)-1);
				zipEntry.Size = (num23 & (long)((ulong)-1));
				zipEntry.CompressedSize = (num22 & (long)((ulong)-1));
				zipEntry.Flags = num19;
				zipEntry.DosTime = (long)((ulong)num20);
				zipEntry.ZipFileIndex = (long)num18;
				zipEntry.Offset = offset;
				zipEntry.ExternalFileAttributes = (int)externalFileAttributes;
				bool flag11 = (num19 & 8) == 0;
				if (flag11)
				{
					zipEntry.CryptoCheckValue = (byte)(num21 >> 24);
				}
				else
				{
					zipEntry.CryptoCheckValue = (byte)(num20 >> 8 & 255U);
				}
				bool flag12 = num25 > 0;
				if (flag12)
				{
					byte[] array3 = new byte[num25];
					StreamUtils.ReadFully(this.baseStream_, array3);
					zipEntry.ExtraData = array3;
				}
				zipEntry.ProcessExtraData(false);
				bool flag13 = num26 > 0;
				if (flag13)
				{
					StreamUtils.ReadFully(this.baseStream_, array2, 0, num26);
					zipEntry.Comment = ZipConstants.ConvertToStringExt(num19, array2, num26);
				}
				this.entries_[(int)(checked((IntPtr)num18))] = zipEntry;
			}
		}

		// Token: 0x060005F8 RID: 1528 RVA: 0x00030010 File Offset: 0x0002E210
		private long LocateEntry(ZipEntry entry)
		{
			return this.TestLocalHeader(entry, ZipFile.HeaderTest.Extract);
		}

		// Token: 0x060005F9 RID: 1529 RVA: 0x00030034 File Offset: 0x0002E234
		private Stream CreateAndInitDecryptionStream(Stream baseStream, ZipEntry entry)
		{
			bool flag = entry.Version < 50 || (entry.Flags & 64) == 0;
			CryptoStream cryptoStream;
			if (flag)
			{
				PkzipClassicManaged pkzipClassicManaged = new PkzipClassicManaged();
				this.OnKeysRequired(entry.Name);
				bool flag2 = !this.HaveKeys;
				if (flag2)
				{
					throw new ZipException("No password available for encrypted stream");
				}
				cryptoStream = new CryptoStream(baseStream, pkzipClassicManaged.CreateDecryptor(this.key, null), CryptoStreamMode.Read);
				ZipFile.CheckClassicPassword(cryptoStream, entry);
			}
			else
			{
				bool flag3 = entry.Version == 51;
				if (!flag3)
				{
					throw new ZipException("Decryption method not supported");
				}
				this.OnKeysRequired(entry.Name);
				bool flag4 = !this.HaveKeys;
				if (flag4)
				{
					throw new ZipException("No password available for AES encrypted stream");
				}
				int aessaltLen = entry.AESSaltLen;
				byte[] array = new byte[aessaltLen];
				int num = baseStream.Read(array, 0, aessaltLen);
				bool flag5 = num != aessaltLen;
				if (flag5)
				{
					throw new ZipException("AES Salt expected " + aessaltLen.ToString() + " got " + num.ToString());
				}
				byte[] array2 = new byte[2];
				baseStream.Read(array2, 0, 2);
				int blockSize = entry.AESKeySize / 8;
				ZipAESTransform zipAESTransform = new ZipAESTransform(this.rawPassword_, array, blockSize, false);
				byte[] pwdVerifier = zipAESTransform.PwdVerifier;
				bool flag6 = pwdVerifier[0] != array2[0] || pwdVerifier[1] != array2[1];
				if (flag6)
				{
					throw new Exception("Invalid password for AES");
				}
				cryptoStream = new ZipAESStream(baseStream, zipAESTransform, CryptoStreamMode.Read);
			}
			return cryptoStream;
		}

		// Token: 0x060005FA RID: 1530 RVA: 0x000301E8 File Offset: 0x0002E3E8
		private Stream CreateAndInitEncryptionStream(Stream baseStream, ZipEntry entry)
		{
			CryptoStream cryptoStream = null;
			bool flag = entry.Version < 50 || (entry.Flags & 64) == 0;
			if (flag)
			{
				PkzipClassicManaged pkzipClassicManaged = new PkzipClassicManaged();
				this.OnKeysRequired(entry.Name);
				bool flag2 = !this.HaveKeys;
				if (flag2)
				{
					throw new ZipException("No password available for encrypted stream");
				}
				cryptoStream = new CryptoStream(new ZipFile.UncompressedStream(baseStream), pkzipClassicManaged.CreateEncryptor(this.key, null), CryptoStreamMode.Write);
				bool flag3 = entry.Crc < 0L || (entry.Flags & 8) != 0;
				if (flag3)
				{
					ZipFile.WriteEncryptionHeader(cryptoStream, entry.DosTime << 16);
				}
				else
				{
					ZipFile.WriteEncryptionHeader(cryptoStream, entry.Crc);
				}
			}
			return cryptoStream;
		}

		// Token: 0x060005FB RID: 1531 RVA: 0x000302C4 File Offset: 0x0002E4C4
		private static void CheckClassicPassword(CryptoStream classicCryptoStream, ZipEntry entry)
		{
			byte[] array = new byte[12];
			StreamUtils.ReadFully(classicCryptoStream, array);
			bool flag = array[11] != entry.CryptoCheckValue;
			if (flag)
			{
				throw new ZipException("Invalid password");
			}
		}

		// Token: 0x060005FC RID: 1532 RVA: 0x00030308 File Offset: 0x0002E508
		private static void WriteEncryptionHeader(Stream stream, long crcValue)
		{
			byte[] array = new byte[12];
			Random random = new Random();
			random.NextBytes(array);
			array[11] = (byte)(crcValue >> 24);
			stream.Write(array, 0, array.Length);
		}

		// Token: 0x04000374 RID: 884
		public ZipFile.KeysRequiredEventHandler KeysRequired;

		// Token: 0x04000375 RID: 885
		private const int DefaultBufferSize = 4096;

		// Token: 0x04000376 RID: 886
		private bool isDisposed_;

		// Token: 0x04000377 RID: 887
		private string name_;

		// Token: 0x04000378 RID: 888
		private string comment_;

		// Token: 0x04000379 RID: 889
		private string rawPassword_;

		// Token: 0x0400037A RID: 890
		private Stream baseStream_;

		// Token: 0x0400037B RID: 891
		private bool isStreamOwner;

		// Token: 0x0400037C RID: 892
		private long offsetOfFirstEntry;

		// Token: 0x0400037D RID: 893
		private ZipEntry[] entries_;

		// Token: 0x0400037E RID: 894
		private byte[] key;

		// Token: 0x0400037F RID: 895
		private bool isNewArchive_;

		// Token: 0x04000380 RID: 896
		private UseZip64 useZip64_ = UseZip64.Dynamic;

		// Token: 0x04000381 RID: 897
		private ArrayList updates_;

		// Token: 0x04000382 RID: 898
		private long updateCount_;

		// Token: 0x04000383 RID: 899
		private Hashtable updateIndex_;

		// Token: 0x04000384 RID: 900
		private IArchiveStorage archiveStorage_;

		// Token: 0x04000385 RID: 901
		private IDynamicDataSource updateDataSource_;

		// Token: 0x04000386 RID: 902
		private bool contentsEdited_;

		// Token: 0x04000387 RID: 903
		private int bufferSize_ = 4096;

		// Token: 0x04000388 RID: 904
		private byte[] copyBuffer_;

		// Token: 0x04000389 RID: 905
		private ZipFile.ZipString newComment_;

		// Token: 0x0400038A RID: 906
		private bool commentEdited_;

		// Token: 0x0400038B RID: 907
		private IEntryFactory updateEntryFactory_ = new ZipEntryFactory();

		// Token: 0x0200014D RID: 333
		// (Invoke) Token: 0x06000ABB RID: 2747
		public delegate void KeysRequiredEventHandler(object sender, KeysRequiredEventArgs e);

		// Token: 0x0200014E RID: 334
		[Flags]
		private enum HeaderTest
		{
			// Token: 0x040007AC RID: 1964
			Extract = 1,
			// Token: 0x040007AD RID: 1965
			Header = 2
		}

		// Token: 0x0200014F RID: 335
		private enum UpdateCommand
		{
			// Token: 0x040007AF RID: 1967
			Copy,
			// Token: 0x040007B0 RID: 1968
			Modify,
			// Token: 0x040007B1 RID: 1969
			Add
		}

		// Token: 0x02000150 RID: 336
		private class UpdateComparer : IComparer
		{
			// Token: 0x06000ABE RID: 2750 RVA: 0x0004B0E8 File Offset: 0x000492E8
			public int Compare(object x, object y)
			{
				ZipFile.ZipUpdate zipUpdate = x as ZipFile.ZipUpdate;
				ZipFile.ZipUpdate zipUpdate2 = y as ZipFile.ZipUpdate;
				bool flag = zipUpdate == null;
				int num;
				if (flag)
				{
					bool flag2 = zipUpdate2 == null;
					if (flag2)
					{
						num = 0;
					}
					else
					{
						num = -1;
					}
				}
				else
				{
					bool flag3 = zipUpdate2 == null;
					if (flag3)
					{
						num = 1;
					}
					else
					{
						int num2 = (zipUpdate.Command == ZipFile.UpdateCommand.Copy || zipUpdate.Command == ZipFile.UpdateCommand.Modify) ? 0 : 1;
						int num3 = (zipUpdate2.Command == ZipFile.UpdateCommand.Copy || zipUpdate2.Command == ZipFile.UpdateCommand.Modify) ? 0 : 1;
						num = num2 - num3;
						bool flag4 = num == 0;
						if (flag4)
						{
							long num4 = zipUpdate.Entry.Offset - zipUpdate2.Entry.Offset;
							bool flag5 = num4 < 0L;
							if (flag5)
							{
								num = -1;
							}
							else
							{
								bool flag6 = num4 == 0L;
								if (flag6)
								{
									num = 0;
								}
								else
								{
									num = 1;
								}
							}
						}
					}
				}
				return num;
			}
		}

		// Token: 0x02000151 RID: 337
		private class ZipUpdate
		{
			// Token: 0x06000AC0 RID: 2752 RVA: 0x0004B20C File Offset: 0x0004940C
			public ZipUpdate(string fileName, ZipEntry entry)
			{
				this.command_ = ZipFile.UpdateCommand.Add;
				this.entry_ = entry;
				this.filename_ = fileName;
			}

			// Token: 0x06000AC1 RID: 2753 RVA: 0x0004B244 File Offset: 0x00049444
			[Obsolete]
			public ZipUpdate(string fileName, string entryName, CompressionMethod compressionMethod)
			{
				this.command_ = ZipFile.UpdateCommand.Add;
				this.entry_ = new ZipEntry(entryName);
				this.entry_.CompressionMethod = compressionMethod;
				this.filename_ = fileName;
			}

			// Token: 0x06000AC2 RID: 2754 RVA: 0x0004B29C File Offset: 0x0004949C
			[Obsolete]
			public ZipUpdate(string fileName, string entryName) : this(fileName, entryName, CompressionMethod.Deflated)
			{
			}

			// Token: 0x06000AC3 RID: 2755 RVA: 0x0004B2AC File Offset: 0x000494AC
			[Obsolete]
			public ZipUpdate(IStaticDataSource dataSource, string entryName, CompressionMethod compressionMethod)
			{
				this.command_ = ZipFile.UpdateCommand.Add;
				this.entry_ = new ZipEntry(entryName);
				this.entry_.CompressionMethod = compressionMethod;
				this.dataSource_ = dataSource;
			}

			// Token: 0x06000AC4 RID: 2756 RVA: 0x0004B304 File Offset: 0x00049504
			public ZipUpdate(IStaticDataSource dataSource, ZipEntry entry)
			{
				this.command_ = ZipFile.UpdateCommand.Add;
				this.entry_ = entry;
				this.dataSource_ = dataSource;
			}

			// Token: 0x06000AC5 RID: 2757 RVA: 0x0004B33C File Offset: 0x0004953C
			public ZipUpdate(ZipEntry original, ZipEntry updated)
			{
				throw new ZipException("Modify not currently supported");
			}

			// Token: 0x06000AC6 RID: 2758 RVA: 0x0004B368 File Offset: 0x00049568
			public ZipUpdate(ZipFile.UpdateCommand command, ZipEntry entry)
			{
				this.command_ = command;
				this.entry_ = (ZipEntry)entry.Clone();
			}

			// Token: 0x06000AC7 RID: 2759 RVA: 0x0004B3A4 File Offset: 0x000495A4
			public ZipUpdate(ZipEntry entry) : this(ZipFile.UpdateCommand.Copy, entry)
			{
			}

			// Token: 0x170001E5 RID: 485
			// (get) Token: 0x06000AC8 RID: 2760 RVA: 0x0004B3B0 File Offset: 0x000495B0
			public ZipEntry Entry
			{
				get
				{
					return this.entry_;
				}
			}

			// Token: 0x170001E6 RID: 486
			// (get) Token: 0x06000AC9 RID: 2761 RVA: 0x0004B3D0 File Offset: 0x000495D0
			public ZipEntry OutEntry
			{
				get
				{
					bool flag = this.outEntry_ == null;
					if (flag)
					{
						this.outEntry_ = (ZipEntry)this.entry_.Clone();
					}
					return this.outEntry_;
				}
			}

			// Token: 0x170001E7 RID: 487
			// (get) Token: 0x06000ACA RID: 2762 RVA: 0x0004B418 File Offset: 0x00049618
			public ZipFile.UpdateCommand Command
			{
				get
				{
					return this.command_;
				}
			}

			// Token: 0x170001E8 RID: 488
			// (get) Token: 0x06000ACB RID: 2763 RVA: 0x0004B438 File Offset: 0x00049638
			public string Filename
			{
				get
				{
					return this.filename_;
				}
			}

			// Token: 0x170001E9 RID: 489
			// (get) Token: 0x06000ACC RID: 2764 RVA: 0x0004B458 File Offset: 0x00049658
			// (set) Token: 0x06000ACD RID: 2765 RVA: 0x0004B478 File Offset: 0x00049678
			public long SizePatchOffset
			{
				get
				{
					return this.sizePatchOffset_;
				}
				set
				{
					this.sizePatchOffset_ = value;
				}
			}

			// Token: 0x170001EA RID: 490
			// (get) Token: 0x06000ACE RID: 2766 RVA: 0x0004B484 File Offset: 0x00049684
			// (set) Token: 0x06000ACF RID: 2767 RVA: 0x0004B4A4 File Offset: 0x000496A4
			public long CrcPatchOffset
			{
				get
				{
					return this.crcPatchOffset_;
				}
				set
				{
					this.crcPatchOffset_ = value;
				}
			}

			// Token: 0x170001EB RID: 491
			// (get) Token: 0x06000AD0 RID: 2768 RVA: 0x0004B4B0 File Offset: 0x000496B0
			// (set) Token: 0x06000AD1 RID: 2769 RVA: 0x0004B4D0 File Offset: 0x000496D0
			public long OffsetBasedSize
			{
				get
				{
					return this._offsetBasedSize;
				}
				set
				{
					this._offsetBasedSize = value;
				}
			}

			// Token: 0x06000AD2 RID: 2770 RVA: 0x0004B4DC File Offset: 0x000496DC
			public Stream GetSource()
			{
				Stream result = null;
				bool flag = this.dataSource_ != null;
				if (flag)
				{
					result = this.dataSource_.GetSource();
				}
				return result;
			}

			// Token: 0x040007B2 RID: 1970
			private ZipEntry entry_;

			// Token: 0x040007B3 RID: 1971
			private ZipEntry outEntry_;

			// Token: 0x040007B4 RID: 1972
			private ZipFile.UpdateCommand command_;

			// Token: 0x040007B5 RID: 1973
			private IStaticDataSource dataSource_;

			// Token: 0x040007B6 RID: 1974
			private string filename_;

			// Token: 0x040007B7 RID: 1975
			private long sizePatchOffset_ = -1L;

			// Token: 0x040007B8 RID: 1976
			private long crcPatchOffset_ = -1L;

			// Token: 0x040007B9 RID: 1977
			private long _offsetBasedSize = -1L;
		}

		// Token: 0x02000152 RID: 338
		private class ZipString
		{
			// Token: 0x06000AD3 RID: 2771 RVA: 0x0004B518 File Offset: 0x00049718
			public ZipString(string comment)
			{
				this.comment_ = comment;
				this.isSourceString_ = true;
			}

			// Token: 0x06000AD4 RID: 2772 RVA: 0x0004B530 File Offset: 0x00049730
			public ZipString(byte[] rawString)
			{
				this.rawComment_ = rawString;
			}

			// Token: 0x170001EC RID: 492
			// (get) Token: 0x06000AD5 RID: 2773 RVA: 0x0004B544 File Offset: 0x00049744
			public bool IsSourceString
			{
				get
				{
					return this.isSourceString_;
				}
			}

			// Token: 0x170001ED RID: 493
			// (get) Token: 0x06000AD6 RID: 2774 RVA: 0x0004B564 File Offset: 0x00049764
			public int RawLength
			{
				get
				{
					this.MakeBytesAvailable();
					return this.rawComment_.Length;
				}
			}

			// Token: 0x170001EE RID: 494
			// (get) Token: 0x06000AD7 RID: 2775 RVA: 0x0004B58C File Offset: 0x0004978C
			public byte[] RawComment
			{
				get
				{
					this.MakeBytesAvailable();
					return (byte[])this.rawComment_.Clone();
				}
			}

			// Token: 0x06000AD8 RID: 2776 RVA: 0x0004B5BC File Offset: 0x000497BC
			public void Reset()
			{
				bool flag = this.isSourceString_;
				if (flag)
				{
					this.rawComment_ = null;
				}
				else
				{
					this.comment_ = null;
				}
			}

			// Token: 0x06000AD9 RID: 2777 RVA: 0x0004B5F4 File Offset: 0x000497F4
			private void MakeTextAvailable()
			{
				bool flag = this.comment_ == null;
				if (flag)
				{
					this.comment_ = ZipConstants.ConvertToString(this.rawComment_);
				}
			}

			// Token: 0x06000ADA RID: 2778 RVA: 0x0004B62C File Offset: 0x0004982C
			private void MakeBytesAvailable()
			{
				bool flag = this.rawComment_ == null;
				if (flag)
				{
					this.rawComment_ = ZipConstants.ConvertToArray(this.comment_);
				}
			}

			// Token: 0x06000ADB RID: 2779 RVA: 0x0004B664 File Offset: 0x00049864
			public static implicit operator string(ZipFile.ZipString zipString)
			{
				zipString.MakeTextAvailable();
				return zipString.comment_;
			}

			// Token: 0x040007BA RID: 1978
			private string comment_;

			// Token: 0x040007BB RID: 1979
			private byte[] rawComment_;

			// Token: 0x040007BC RID: 1980
			private bool isSourceString_;
		}

		// Token: 0x02000153 RID: 339
		private class ZipEntryEnumerator : IEnumerator
		{
			// Token: 0x06000ADC RID: 2780 RVA: 0x0004B68C File Offset: 0x0004988C
			public ZipEntryEnumerator(ZipEntry[] entries)
			{
				this.array = entries;
			}

			// Token: 0x170001EF RID: 495
			// (get) Token: 0x06000ADD RID: 2781 RVA: 0x0004B6A4 File Offset: 0x000498A4
			public object Current
			{
				get
				{
					return this.array[this.index];
				}
			}

			// Token: 0x06000ADE RID: 2782 RVA: 0x0004B6D0 File Offset: 0x000498D0
			public void Reset()
			{
				this.index = -1;
			}

			// Token: 0x06000ADF RID: 2783 RVA: 0x0004B6DC File Offset: 0x000498DC
			public bool MoveNext()
			{
				int num = this.index + 1;
				this.index = num;
				return num < this.array.Length;
			}

			// Token: 0x040007BD RID: 1981
			private ZipEntry[] array;

			// Token: 0x040007BE RID: 1982
			private int index = -1;
		}

		// Token: 0x02000154 RID: 340
		private class UncompressedStream : Stream
		{
			// Token: 0x06000AE0 RID: 2784 RVA: 0x0004B710 File Offset: 0x00049910
			public UncompressedStream(Stream baseStream)
			{
				this.baseStream_ = baseStream;
			}

			// Token: 0x06000AE1 RID: 2785 RVA: 0x0004B724 File Offset: 0x00049924
			public override void Close()
			{
			}

			// Token: 0x170001F0 RID: 496
			// (get) Token: 0x06000AE2 RID: 2786 RVA: 0x0004B728 File Offset: 0x00049928
			public override bool CanRead
			{
				get
				{
					return false;
				}
			}

			// Token: 0x06000AE3 RID: 2787 RVA: 0x0004B744 File Offset: 0x00049944
			public override void Flush()
			{
				this.baseStream_.Flush();
			}

			// Token: 0x170001F1 RID: 497
			// (get) Token: 0x06000AE4 RID: 2788 RVA: 0x0004B754 File Offset: 0x00049954
			public override bool CanWrite
			{
				get
				{
					return this.baseStream_.CanWrite;
				}
			}

			// Token: 0x170001F2 RID: 498
			// (get) Token: 0x06000AE5 RID: 2789 RVA: 0x0004B778 File Offset: 0x00049978
			public override bool CanSeek
			{
				get
				{
					return false;
				}
			}

			// Token: 0x170001F3 RID: 499
			// (get) Token: 0x06000AE6 RID: 2790 RVA: 0x0004B794 File Offset: 0x00049994
			public override long Length
			{
				get
				{
					return 0L;
				}
			}

			// Token: 0x170001F4 RID: 500
			// (get) Token: 0x06000AE7 RID: 2791 RVA: 0x0004B7B0 File Offset: 0x000499B0
			// (set) Token: 0x06000AE8 RID: 2792 RVA: 0x0004B7D4 File Offset: 0x000499D4
			public override long Position
			{
				get
				{
					return this.baseStream_.Position;
				}
				set
				{
				}
			}

			// Token: 0x06000AE9 RID: 2793 RVA: 0x0004B7D8 File Offset: 0x000499D8
			public override int Read(byte[] buffer, int offset, int count)
			{
				return 0;
			}

			// Token: 0x06000AEA RID: 2794 RVA: 0x0004B7F4 File Offset: 0x000499F4
			public override long Seek(long offset, SeekOrigin origin)
			{
				return 0L;
			}

			// Token: 0x06000AEB RID: 2795 RVA: 0x0004B810 File Offset: 0x00049A10
			public override void SetLength(long value)
			{
			}

			// Token: 0x06000AEC RID: 2796 RVA: 0x0004B814 File Offset: 0x00049A14
			public override void Write(byte[] buffer, int offset, int count)
			{
				this.baseStream_.Write(buffer, offset, count);
			}

			// Token: 0x040007BF RID: 1983
			private Stream baseStream_;
		}

		// Token: 0x02000155 RID: 341
		private class PartialInputStream : Stream
		{
			// Token: 0x06000AED RID: 2797 RVA: 0x0004B828 File Offset: 0x00049A28
			public PartialInputStream(ZipFile zipFile, long start, long length)
			{
				this.start_ = start;
				this.length_ = length;
				this.zipFile_ = zipFile;
				this.baseStream_ = this.zipFile_.baseStream_;
				this.readPos_ = start;
				this.end_ = start + length;
			}

			// Token: 0x06000AEE RID: 2798 RVA: 0x0004B868 File Offset: 0x00049A68
			public override int ReadByte()
			{
				bool flag = this.readPos_ >= this.end_;
				int result;
				if (flag)
				{
					result = -1;
				}
				else
				{
					Stream obj = this.baseStream_;
					lock (obj)
					{
						Stream stream = this.baseStream_;
						long num = this.readPos_;
						this.readPos_ = num + 1L;
						stream.Seek(num, SeekOrigin.Begin);
						result = this.baseStream_.ReadByte();
					}
				}
				return result;
			}

			// Token: 0x06000AEF RID: 2799 RVA: 0x0004B8F0 File Offset: 0x00049AF0
			public override void Close()
			{
			}

			// Token: 0x06000AF0 RID: 2800 RVA: 0x0004B8F4 File Offset: 0x00049AF4
			public override int Read(byte[] buffer, int offset, int count)
			{
				Stream obj = this.baseStream_;
				int result;
				lock (obj)
				{
					bool flag = (long)count > this.end_ - this.readPos_;
					if (flag)
					{
						count = (int)(this.end_ - this.readPos_);
						bool flag2 = count == 0;
						if (flag2)
						{
							return 0;
						}
					}
					this.baseStream_.Seek(this.readPos_, SeekOrigin.Begin);
					int num = this.baseStream_.Read(buffer, offset, count);
					bool flag3 = num > 0;
					if (flag3)
					{
						this.readPos_ += (long)num;
					}
					result = num;
				}
				return result;
			}

			// Token: 0x06000AF1 RID: 2801 RVA: 0x0004B9B4 File Offset: 0x00049BB4
			public override void Write(byte[] buffer, int offset, int count)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06000AF2 RID: 2802 RVA: 0x0004B9BC File Offset: 0x00049BBC
			public override void SetLength(long value)
			{
				throw new NotSupportedException();
			}

			// Token: 0x06000AF3 RID: 2803 RVA: 0x0004B9C4 File Offset: 0x00049BC4
			public override long Seek(long offset, SeekOrigin origin)
			{
				long num = this.readPos_;
				switch (origin)
				{
				case SeekOrigin.Begin:
					num = this.start_ + offset;
					break;
				case SeekOrigin.Current:
					num = this.readPos_ + offset;
					break;
				case SeekOrigin.End:
					num = this.end_ + offset;
					break;
				}
				bool flag = num < this.start_;
				if (flag)
				{
					throw new ArgumentException("Negative position is invalid");
				}
				bool flag2 = num >= this.end_;
				if (flag2)
				{
					throw new IOException("Cannot seek past end");
				}
				this.readPos_ = num;
				return this.readPos_;
			}

			// Token: 0x06000AF4 RID: 2804 RVA: 0x0004BA78 File Offset: 0x00049C78
			public override void Flush()
			{
			}

			// Token: 0x170001F5 RID: 501
			// (get) Token: 0x06000AF5 RID: 2805 RVA: 0x0004BA7C File Offset: 0x00049C7C
			// (set) Token: 0x06000AF6 RID: 2806 RVA: 0x0004BAA4 File Offset: 0x00049CA4
			public override long Position
			{
				get
				{
					return this.readPos_ - this.start_;
				}
				set
				{
					long num = this.start_ + value;
					bool flag = num < this.start_;
					if (flag)
					{
						throw new ArgumentException("Negative position is invalid");
					}
					bool flag2 = num >= this.end_;
					if (flag2)
					{
						throw new InvalidOperationException("Cannot seek past end");
					}
					this.readPos_ = num;
				}
			}

			// Token: 0x170001F6 RID: 502
			// (get) Token: 0x06000AF7 RID: 2807 RVA: 0x0004BB04 File Offset: 0x00049D04
			public override long Length
			{
				get
				{
					return this.length_;
				}
			}

			// Token: 0x170001F7 RID: 503
			// (get) Token: 0x06000AF8 RID: 2808 RVA: 0x0004BB24 File Offset: 0x00049D24
			public override bool CanWrite
			{
				get
				{
					return false;
				}
			}

			// Token: 0x170001F8 RID: 504
			// (get) Token: 0x06000AF9 RID: 2809 RVA: 0x0004BB40 File Offset: 0x00049D40
			public override bool CanSeek
			{
				get
				{
					return true;
				}
			}

			// Token: 0x170001F9 RID: 505
			// (get) Token: 0x06000AFA RID: 2810 RVA: 0x0004BB5C File Offset: 0x00049D5C
			public override bool CanRead
			{
				get
				{
					return true;
				}
			}

			// Token: 0x170001FA RID: 506
			// (get) Token: 0x06000AFB RID: 2811 RVA: 0x0004BB78 File Offset: 0x00049D78
			public override bool CanTimeout
			{
				get
				{
					return this.baseStream_.CanTimeout;
				}
			}

			// Token: 0x040007C0 RID: 1984
			private ZipFile zipFile_;

			// Token: 0x040007C1 RID: 1985
			private Stream baseStream_;

			// Token: 0x040007C2 RID: 1986
			private long start_;

			// Token: 0x040007C3 RID: 1987
			private long length_;

			// Token: 0x040007C4 RID: 1988
			private long readPos_;

			// Token: 0x040007C5 RID: 1989
			private long end_;
		}
	}
}
