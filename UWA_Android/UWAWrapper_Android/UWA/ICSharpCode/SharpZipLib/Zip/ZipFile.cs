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
	// Token: 0x02000070 RID: 112
	[ComVisible(false)]
	public class ZipFile : IEnumerable, IDisposable
	{
		// Token: 0x060004BE RID: 1214 RVA: 0x0001F2C0 File Offset: 0x0001D4C0
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

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x060004BF RID: 1215 RVA: 0x0001F30C File Offset: 0x0001D50C
		// (set) Token: 0x060004C0 RID: 1216 RVA: 0x0001F32C File Offset: 0x0001D52C
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

		// Token: 0x170000AA RID: 170
		// (set) Token: 0x060004C1 RID: 1217 RVA: 0x0001F338 File Offset: 0x0001D538
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
					this.key = UWA.ICSharpCode.SharpZipLib.Encryption.PkzipClassic.GenerateKeys(ZipConstants.ConvertToArray(value));
				}
			}
		}

		// Token: 0x170000AB RID: 171
		// (get) Token: 0x060004C2 RID: 1218 RVA: 0x0001F390 File Offset: 0x0001D590
		private bool HaveKeys
		{
			get
			{
				return this.key != null;
			}
		}

		// Token: 0x060004C3 RID: 1219 RVA: 0x0001F3B4 File Offset: 0x0001D5B4
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

		// Token: 0x060004C4 RID: 1220 RVA: 0x0001F444 File Offset: 0x0001D644
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

		// Token: 0x060004C5 RID: 1221 RVA: 0x0001F4F4 File Offset: 0x0001D6F4
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

		// Token: 0x060004C6 RID: 1222 RVA: 0x0001F5C8 File Offset: 0x0001D7C8
		internal ZipFile()
		{
			this.entries_ = new ZipEntry[0];
			this.isNewArchive_ = true;
		}

		// Token: 0x060004C7 RID: 1223 RVA: 0x0001F604 File Offset: 0x0001D804
		~ZipFile()
		{
			this.Dispose(false);
		}

		// Token: 0x060004C8 RID: 1224 RVA: 0x0001F638 File Offset: 0x0001D838
		public void Close()
		{
			this.DisposeInternal(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x060004C9 RID: 1225 RVA: 0x0001F64C File Offset: 0x0001D84C
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

		// Token: 0x060004CA RID: 1226 RVA: 0x0001F6A0 File Offset: 0x0001D8A0
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

		// Token: 0x170000AC RID: 172
		// (get) Token: 0x060004CB RID: 1227 RVA: 0x0001F724 File Offset: 0x0001D924
		// (set) Token: 0x060004CC RID: 1228 RVA: 0x0001F744 File Offset: 0x0001D944
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

		// Token: 0x170000AD RID: 173
		// (get) Token: 0x060004CD RID: 1229 RVA: 0x0001F750 File Offset: 0x0001D950
		public bool IsEmbeddedArchive
		{
			get
			{
				return this.offsetOfFirstEntry > 0L;
			}
		}

		// Token: 0x170000AE RID: 174
		// (get) Token: 0x060004CE RID: 1230 RVA: 0x0001F774 File Offset: 0x0001D974
		public bool IsNewArchive
		{
			get
			{
				return this.isNewArchive_;
			}
		}

		// Token: 0x170000AF RID: 175
		// (get) Token: 0x060004CF RID: 1231 RVA: 0x0001F794 File Offset: 0x0001D994
		public string ZipFileComment
		{
			get
			{
				return this.comment_;
			}
		}

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x060004D0 RID: 1232 RVA: 0x0001F7B4 File Offset: 0x0001D9B4
		public string Name
		{
			get
			{
				return this.name_;
			}
		}

		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x060004D1 RID: 1233 RVA: 0x0001F7D4 File Offset: 0x0001D9D4
		[Obsolete("Use the Count property instead")]
		public int Size
		{
			get
			{
				return this.entries_.Length;
			}
		}

		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x060004D2 RID: 1234 RVA: 0x0001F7F8 File Offset: 0x0001D9F8
		public long Count
		{
			get
			{
				return (long)this.entries_.Length;
			}
		}

		// Token: 0x170000B3 RID: 179
		[IndexerName("EntryByIndex")]
		public ZipEntry this[int index]
		{
			get
			{
				return (ZipEntry)this.entries_[index].Clone();
			}
		}

		// Token: 0x060004D4 RID: 1236 RVA: 0x0001F84C File Offset: 0x0001DA4C
		public IEnumerator GetEnumerator()
		{
			bool flag = this.isDisposed_;
			if (flag)
			{
				throw new ObjectDisposedException("ZipFile");
			}
			return new ZipFile.ZipEntryEnumerator(this.entries_);
		}

		// Token: 0x060004D5 RID: 1237 RVA: 0x0001F88C File Offset: 0x0001DA8C
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

		// Token: 0x060004D6 RID: 1238 RVA: 0x0001F90C File Offset: 0x0001DB0C
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

		// Token: 0x060004D7 RID: 1239 RVA: 0x0001F96C File Offset: 0x0001DB6C
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

		// Token: 0x060004D8 RID: 1240 RVA: 0x0001FA34 File Offset: 0x0001DC34
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
					stream = new UWA.ICSharpCode.SharpZipLib.Zip.Compression.Streams.InflaterInputStream(stream, new UWA.ICSharpCode.SharpZipLib.Zip.Compression.Inflater(true));
				}
				return stream;
			}
		}

		// Token: 0x060004D9 RID: 1241 RVA: 0x0001FB4C File Offset: 0x0001DD4C
		public bool TestArchive(bool testData)
		{
			return this.TestArchive(testData, TestStrategy.FindFirstError, null);
		}

		// Token: 0x060004DA RID: 1242 RVA: 0x0001FB70 File Offset: 0x0001DD70
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
						UWA.ICSharpCode.SharpZipLib.Checksums.Crc32 crc = new UWA.ICSharpCode.SharpZipLib.Checksums.Crc32();
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

		// Token: 0x060004DB RID: 1243 RVA: 0x0001FF8C File Offset: 0x0001E18C
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
				UWA.ICSharpCode.SharpZipLib.Core.StreamUtils.ReadFully(this.baseStream_, array);
				byte[] array2 = new byte[num10];
				UWA.ICSharpCode.SharpZipLib.Core.StreamUtils.ReadFully(this.baseStream_, array2);
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

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x060004DC RID: 1244 RVA: 0x000206DC File Offset: 0x0001E8DC
		// (set) Token: 0x060004DD RID: 1245 RVA: 0x00020700 File Offset: 0x0001E900
		public UWA.ICSharpCode.SharpZipLib.Core.INameTransform NameTransform
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

		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x060004DE RID: 1246 RVA: 0x00020710 File Offset: 0x0001E910
		// (set) Token: 0x060004DF RID: 1247 RVA: 0x00020730 File Offset: 0x0001E930
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

		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x060004E0 RID: 1248 RVA: 0x00020768 File Offset: 0x0001E968
		// (set) Token: 0x060004E1 RID: 1249 RVA: 0x00020788 File Offset: 0x0001E988
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

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x060004E2 RID: 1250 RVA: 0x000207E0 File Offset: 0x0001E9E0
		public bool IsUpdating
		{
			get
			{
				return this.updates_ != null;
			}
		}

		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x060004E3 RID: 1251 RVA: 0x00020804 File Offset: 0x0001EA04
		// (set) Token: 0x060004E4 RID: 1252 RVA: 0x00020824 File Offset: 0x0001EA24
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

		// Token: 0x060004E5 RID: 1253 RVA: 0x00020830 File Offset: 0x0001EA30
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

		// Token: 0x060004E6 RID: 1254 RVA: 0x00020A14 File Offset: 0x0001EC14
		public void BeginUpdate(IArchiveStorage archiveStorage)
		{
			this.BeginUpdate(archiveStorage, new DynamicDiskDataSource());
		}

		// Token: 0x060004E7 RID: 1255 RVA: 0x00020A24 File Offset: 0x0001EC24
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

		// Token: 0x060004E8 RID: 1256 RVA: 0x00020A74 File Offset: 0x0001EC74
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

		// Token: 0x060004E9 RID: 1257 RVA: 0x00020B8C File Offset: 0x0001ED8C
		public void AbortUpdate()
		{
			this.PostUpdateCleanup();
		}

		// Token: 0x060004EA RID: 1258 RVA: 0x00020B98 File Offset: 0x0001ED98
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

		// Token: 0x060004EB RID: 1259 RVA: 0x00020C0C File Offset: 0x0001EE0C
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

		// Token: 0x060004EC RID: 1260 RVA: 0x00020CBC File Offset: 0x0001EEBC
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

		// Token: 0x060004ED RID: 1261 RVA: 0x00020D54 File Offset: 0x0001EF54
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

		// Token: 0x060004EE RID: 1262 RVA: 0x00020DCC File Offset: 0x0001EFCC
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

		// Token: 0x060004EF RID: 1263 RVA: 0x00020E18 File Offset: 0x0001F018
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

		// Token: 0x060004F0 RID: 1264 RVA: 0x00020E78 File Offset: 0x0001F078
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

		// Token: 0x060004F1 RID: 1265 RVA: 0x00020EDC File Offset: 0x0001F0DC
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

		// Token: 0x060004F2 RID: 1266 RVA: 0x00020F48 File Offset: 0x0001F148
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

		// Token: 0x060004F3 RID: 1267 RVA: 0x00020FBC File Offset: 0x0001F1BC
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

		// Token: 0x060004F4 RID: 1268 RVA: 0x00021028 File Offset: 0x0001F228
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

		// Token: 0x060004F5 RID: 1269 RVA: 0x00021074 File Offset: 0x0001F274
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

		// Token: 0x060004F6 RID: 1270 RVA: 0x00021114 File Offset: 0x0001F314
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

		// Token: 0x060004F7 RID: 1271 RVA: 0x00021194 File Offset: 0x0001F394
		private void WriteLEShort(int value)
		{
			this.baseStream_.WriteByte((byte)(value & 255));
			this.baseStream_.WriteByte((byte)(value >> 8 & 255));
		}

		// Token: 0x060004F8 RID: 1272 RVA: 0x000211C4 File Offset: 0x0001F3C4
		private void WriteLEUshort(ushort value)
		{
			this.baseStream_.WriteByte((byte)(value & 255));
			this.baseStream_.WriteByte((byte)(value >> 8));
		}

		// Token: 0x060004F9 RID: 1273 RVA: 0x000211EC File Offset: 0x0001F3EC
		private void WriteLEInt(int value)
		{
			this.WriteLEShort(value & 65535);
			this.WriteLEShort(value >> 16);
		}

		// Token: 0x060004FA RID: 1274 RVA: 0x00021208 File Offset: 0x0001F408
		private void WriteLEUint(uint value)
		{
			this.WriteLEUshort((ushort)(value & 65535U));
			this.WriteLEUshort((ushort)(value >> 16));
		}

		// Token: 0x060004FB RID: 1275 RVA: 0x00021228 File Offset: 0x0001F428
		private void WriteLeLong(long value)
		{
			this.WriteLEInt((int)(value & (long)((ulong)-1)));
			this.WriteLEInt((int)(value >> 32));
		}

		// Token: 0x060004FC RID: 1276 RVA: 0x00021244 File Offset: 0x0001F444
		private void WriteLEUlong(ulong value)
		{
			this.WriteLEUint((uint)(value & (ulong)-1));
			this.WriteLEUint((uint)(value >> 32));
		}

		// Token: 0x060004FD RID: 1277 RVA: 0x00021260 File Offset: 0x0001F460
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

		// Token: 0x060004FE RID: 1278 RVA: 0x00021604 File Offset: 0x0001F804
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

		// Token: 0x060004FF RID: 1279 RVA: 0x000219E0 File Offset: 0x0001FBE0
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

		// Token: 0x06000500 RID: 1280 RVA: 0x00021A2C File Offset: 0x0001FC2C
		private string GetTransformedFileName(string name)
		{
			UWA.ICSharpCode.SharpZipLib.Core.INameTransform nameTransform = this.NameTransform;
			return (nameTransform != null) ? nameTransform.TransformFile(name) : name;
		}

		// Token: 0x06000501 RID: 1281 RVA: 0x00021A60 File Offset: 0x0001FC60
		private string GetTransformedDirectoryName(string name)
		{
			UWA.ICSharpCode.SharpZipLib.Core.INameTransform nameTransform = this.NameTransform;
			return (nameTransform != null) ? nameTransform.TransformDirectory(name) : name;
		}

		// Token: 0x06000502 RID: 1282 RVA: 0x00021A94 File Offset: 0x0001FC94
		private byte[] GetBuffer()
		{
			bool flag = this.copyBuffer_ == null;
			if (flag)
			{
				this.copyBuffer_ = new byte[this.bufferSize_];
			}
			return this.copyBuffer_;
		}

		// Token: 0x06000503 RID: 1283 RVA: 0x00021AD8 File Offset: 0x0001FCD8
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

		// Token: 0x06000504 RID: 1284 RVA: 0x00021B60 File Offset: 0x0001FD60
		private void CopyBytes(ZipFile.ZipUpdate update, Stream destination, Stream source, long bytesToCopy, bool updateCrc)
		{
			bool flag = destination == source;
			if (flag)
			{
				throw new InvalidOperationException("Destination and source are the same");
			}
			UWA.ICSharpCode.SharpZipLib.Checksums.Crc32 crc = new UWA.ICSharpCode.SharpZipLib.Checksums.Crc32();
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

		// Token: 0x06000505 RID: 1285 RVA: 0x00021C78 File Offset: 0x0001FE78
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

		// Token: 0x06000506 RID: 1286 RVA: 0x00021CC8 File Offset: 0x0001FEC8
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

		// Token: 0x06000507 RID: 1287 RVA: 0x00021D54 File Offset: 0x0001FF54
		private void CopyEntryDataDirect(ZipFile.ZipUpdate update, Stream stream, bool updateCrc, ref long destinationPosition, ref long sourcePosition)
		{
			long num = update.Entry.CompressedSize;
			UWA.ICSharpCode.SharpZipLib.Checksums.Crc32 crc = new UWA.ICSharpCode.SharpZipLib.Checksums.Crc32();
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

		// Token: 0x06000508 RID: 1288 RVA: 0x00021E80 File Offset: 0x00020080
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

		// Token: 0x06000509 RID: 1289 RVA: 0x00021ED0 File Offset: 0x000200D0
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

		// Token: 0x0600050A RID: 1290 RVA: 0x00021F1C File Offset: 0x0002011C
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
				stream = new UWA.ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream(stream, new UWA.ICSharpCode.SharpZipLib.Zip.Compression.Deflater(9, true))
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

		// Token: 0x0600050B RID: 1291 RVA: 0x00021FCC File Offset: 0x000201CC
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

		// Token: 0x0600050C RID: 1292 RVA: 0x00022190 File Offset: 0x00020390
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

		// Token: 0x0600050D RID: 1293 RVA: 0x00022268 File Offset: 0x00020468
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

		// Token: 0x0600050E RID: 1294 RVA: 0x000223A4 File Offset: 0x000205A4
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

		// Token: 0x0600050F RID: 1295 RVA: 0x00022448 File Offset: 0x00020648
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

		// Token: 0x06000510 RID: 1296 RVA: 0x00022488 File Offset: 0x00020688
		private void Reopen()
		{
			bool flag = this.Name == null;
			if (flag)
			{
				throw new InvalidOperationException("Name is not known cannot Reopen");
			}
			this.Reopen(File.Open(this.Name, FileMode.Open, FileAccess.Read, FileShare.Read));
		}

		// Token: 0x06000511 RID: 1297 RVA: 0x000224CC File Offset: 0x000206CC
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

		// Token: 0x06000512 RID: 1298 RVA: 0x00022674 File Offset: 0x00020874
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

		// Token: 0x06000513 RID: 1299 RVA: 0x00022BC8 File Offset: 0x00020DC8
		private void CheckUpdating()
		{
			bool flag = this.updates_ == null;
			if (flag)
			{
				throw new InvalidOperationException("BeginUpdate has not been called");
			}
		}

		// Token: 0x06000514 RID: 1300 RVA: 0x00022BF8 File Offset: 0x00020DF8
		void IDisposable.Dispose()
		{
			this.Close();
		}

		// Token: 0x06000515 RID: 1301 RVA: 0x00022C04 File Offset: 0x00020E04
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

		// Token: 0x06000516 RID: 1302 RVA: 0x00022C9C File Offset: 0x00020E9C
		protected virtual void Dispose(bool disposing)
		{
			this.DisposeInternal(disposing);
		}

		// Token: 0x06000517 RID: 1303 RVA: 0x00022CA8 File Offset: 0x00020EA8
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

		// Token: 0x06000518 RID: 1304 RVA: 0x00022D14 File Offset: 0x00020F14
		private uint ReadLEUint()
		{
			return (uint)((int)this.ReadLEUshort() | (int)this.ReadLEUshort() << 16);
		}

		// Token: 0x06000519 RID: 1305 RVA: 0x00022D40 File Offset: 0x00020F40
		private ulong ReadLEUlong()
		{
			return (ulong)this.ReadLEUint() | (ulong)this.ReadLEUint() << 32;
		}

		// Token: 0x0600051A RID: 1306 RVA: 0x00022D6C File Offset: 0x00020F6C
		private long LocateBlockWithSignature(int signature, long endLocation, int minimumBlockSize, int maximumVariableData)
		{
			long result;
			using (ZipHelperStream zipHelperStream = new ZipHelperStream(this.baseStream_))
			{
				result = zipHelperStream.LocateBlockWithSignature(signature, endLocation, minimumBlockSize, maximumVariableData);
			}
			return result;
		}

		// Token: 0x0600051B RID: 1307 RVA: 0x00022DB8 File Offset: 0x00020FB8
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
				UWA.ICSharpCode.SharpZipLib.Core.StreamUtils.ReadFully(this.baseStream_, array);
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
				UWA.ICSharpCode.SharpZipLib.Core.StreamUtils.ReadFully(this.baseStream_, array2, 0, num24);
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
					UWA.ICSharpCode.SharpZipLib.Core.StreamUtils.ReadFully(this.baseStream_, array3);
					zipEntry.ExtraData = array3;
				}
				zipEntry.ProcessExtraData(false);
				bool flag13 = num26 > 0;
				if (flag13)
				{
					UWA.ICSharpCode.SharpZipLib.Core.StreamUtils.ReadFully(this.baseStream_, array2, 0, num26);
					zipEntry.Comment = ZipConstants.ConvertToStringExt(num19, array2, num26);
				}
				this.entries_[(int)(checked((IntPtr)num18))] = zipEntry;
			}
		}

		// Token: 0x0600051C RID: 1308 RVA: 0x00023258 File Offset: 0x00021458
		private long LocateEntry(ZipEntry entry)
		{
			return this.TestLocalHeader(entry, ZipFile.HeaderTest.Extract);
		}

		// Token: 0x0600051D RID: 1309 RVA: 0x0002327C File Offset: 0x0002147C
		private Stream CreateAndInitDecryptionStream(Stream baseStream, ZipEntry entry)
		{
			bool flag = entry.Version < 50 || (entry.Flags & 64) == 0;
			CryptoStream cryptoStream;
			if (flag)
			{
				UWA.ICSharpCode.SharpZipLib.Encryption.PkzipClassicManaged pkzipClassicManaged = new UWA.ICSharpCode.SharpZipLib.Encryption.PkzipClassicManaged();
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

		// Token: 0x0600051E RID: 1310 RVA: 0x00023430 File Offset: 0x00021630
		private Stream CreateAndInitEncryptionStream(Stream baseStream, ZipEntry entry)
		{
			CryptoStream cryptoStream = null;
			bool flag = entry.Version < 50 || (entry.Flags & 64) == 0;
			if (flag)
			{
				UWA.ICSharpCode.SharpZipLib.Encryption.PkzipClassicManaged pkzipClassicManaged = new UWA.ICSharpCode.SharpZipLib.Encryption.PkzipClassicManaged();
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

		// Token: 0x0600051F RID: 1311 RVA: 0x0002350C File Offset: 0x0002170C
		private static void CheckClassicPassword(CryptoStream classicCryptoStream, ZipEntry entry)
		{
			byte[] array = new byte[12];
			UWA.ICSharpCode.SharpZipLib.Core.StreamUtils.ReadFully(classicCryptoStream, array);
			bool flag = array[11] != entry.CryptoCheckValue;
			if (flag)
			{
				throw new ZipException("Invalid password");
			}
		}

		// Token: 0x06000520 RID: 1312 RVA: 0x00023550 File Offset: 0x00021750
		private static void WriteEncryptionHeader(Stream stream, long crcValue)
		{
			byte[] array = new byte[12];
			Random random = new Random();
			random.NextBytes(array);
			array[11] = (byte)(crcValue >> 24);
			stream.Write(array, 0, array.Length);
		}

		// Token: 0x04000301 RID: 769
		public ZipFile.KeysRequiredEventHandler KeysRequired;

		// Token: 0x04000302 RID: 770
		private const int DefaultBufferSize = 4096;

		// Token: 0x04000303 RID: 771
		private bool isDisposed_;

		// Token: 0x04000304 RID: 772
		private string name_;

		// Token: 0x04000305 RID: 773
		private string comment_;

		// Token: 0x04000306 RID: 774
		private string rawPassword_;

		// Token: 0x04000307 RID: 775
		private Stream baseStream_;

		// Token: 0x04000308 RID: 776
		private bool isStreamOwner;

		// Token: 0x04000309 RID: 777
		private long offsetOfFirstEntry;

		// Token: 0x0400030A RID: 778
		private ZipEntry[] entries_;

		// Token: 0x0400030B RID: 779
		private byte[] key;

		// Token: 0x0400030C RID: 780
		private bool isNewArchive_;

		// Token: 0x0400030D RID: 781
		private UseZip64 useZip64_ = UseZip64.Dynamic;

		// Token: 0x0400030E RID: 782
		private ArrayList updates_;

		// Token: 0x0400030F RID: 783
		private long updateCount_;

		// Token: 0x04000310 RID: 784
		private Hashtable updateIndex_;

		// Token: 0x04000311 RID: 785
		private IArchiveStorage archiveStorage_;

		// Token: 0x04000312 RID: 786
		private IDynamicDataSource updateDataSource_;

		// Token: 0x04000313 RID: 787
		private bool contentsEdited_;

		// Token: 0x04000314 RID: 788
		private int bufferSize_ = 4096;

		// Token: 0x04000315 RID: 789
		private byte[] copyBuffer_;

		// Token: 0x04000316 RID: 790
		private ZipFile.ZipString newComment_;

		// Token: 0x04000317 RID: 791
		private bool commentEdited_;

		// Token: 0x04000318 RID: 792
		private IEntryFactory updateEntryFactory_ = new ZipEntryFactory();

		// Token: 0x02000117 RID: 279
		// (Invoke) Token: 0x06000965 RID: 2405
		public delegate void KeysRequiredEventHandler(object sender, KeysRequiredEventArgs e);

		// Token: 0x02000118 RID: 280
		[Flags]
		private enum HeaderTest
		{
			// Token: 0x040006EF RID: 1775
			Extract = 1,
			// Token: 0x040006F0 RID: 1776
			Header = 2
		}

		// Token: 0x02000119 RID: 281
		private enum UpdateCommand
		{
			// Token: 0x040006F2 RID: 1778
			Copy,
			// Token: 0x040006F3 RID: 1779
			Modify,
			// Token: 0x040006F4 RID: 1780
			Add
		}

		// Token: 0x0200011A RID: 282
		private class UpdateComparer : IComparer
		{
			// Token: 0x06000968 RID: 2408 RVA: 0x0003D984 File Offset: 0x0003BB84
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

		// Token: 0x0200011B RID: 283
		private class ZipUpdate
		{
			// Token: 0x0600096A RID: 2410 RVA: 0x0003DAA8 File Offset: 0x0003BCA8
			public ZipUpdate(string fileName, ZipEntry entry)
			{
				this.command_ = ZipFile.UpdateCommand.Add;
				this.entry_ = entry;
				this.filename_ = fileName;
			}

			// Token: 0x0600096B RID: 2411 RVA: 0x0003DAE0 File Offset: 0x0003BCE0
			[Obsolete]
			public ZipUpdate(string fileName, string entryName, CompressionMethod compressionMethod)
			{
				this.command_ = ZipFile.UpdateCommand.Add;
				this.entry_ = new ZipEntry(entryName);
				this.entry_.CompressionMethod = compressionMethod;
				this.filename_ = fileName;
			}

			// Token: 0x0600096C RID: 2412 RVA: 0x0003DB38 File Offset: 0x0003BD38
			[Obsolete]
			public ZipUpdate(string fileName, string entryName) : this(fileName, entryName, CompressionMethod.Deflated)
			{
			}

			// Token: 0x0600096D RID: 2413 RVA: 0x0003DB48 File Offset: 0x0003BD48
			[Obsolete]
			public ZipUpdate(IStaticDataSource dataSource, string entryName, CompressionMethod compressionMethod)
			{
				this.command_ = ZipFile.UpdateCommand.Add;
				this.entry_ = new ZipEntry(entryName);
				this.entry_.CompressionMethod = compressionMethod;
				this.dataSource_ = dataSource;
			}

			// Token: 0x0600096E RID: 2414 RVA: 0x0003DBA0 File Offset: 0x0003BDA0
			public ZipUpdate(IStaticDataSource dataSource, ZipEntry entry)
			{
				this.command_ = ZipFile.UpdateCommand.Add;
				this.entry_ = entry;
				this.dataSource_ = dataSource;
			}

			// Token: 0x0600096F RID: 2415 RVA: 0x0003DBD8 File Offset: 0x0003BDD8
			public ZipUpdate(ZipEntry original, ZipEntry updated)
			{
				throw new ZipException("Modify not currently supported");
			}

			// Token: 0x06000970 RID: 2416 RVA: 0x0003DC04 File Offset: 0x0003BE04
			public ZipUpdate(ZipFile.UpdateCommand command, ZipEntry entry)
			{
				this.command_ = command;
				this.entry_ = (ZipEntry)entry.Clone();
			}

			// Token: 0x06000971 RID: 2417 RVA: 0x0003DC40 File Offset: 0x0003BE40
			public ZipUpdate(ZipEntry entry) : this(ZipFile.UpdateCommand.Copy, entry)
			{
			}

			// Token: 0x1700019D RID: 413
			// (get) Token: 0x06000972 RID: 2418 RVA: 0x0003DC4C File Offset: 0x0003BE4C
			public ZipEntry Entry
			{
				get
				{
					return this.entry_;
				}
			}

			// Token: 0x1700019E RID: 414
			// (get) Token: 0x06000973 RID: 2419 RVA: 0x0003DC6C File Offset: 0x0003BE6C
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

			// Token: 0x1700019F RID: 415
			// (get) Token: 0x06000974 RID: 2420 RVA: 0x0003DCB4 File Offset: 0x0003BEB4
			public ZipFile.UpdateCommand Command
			{
				get
				{
					return this.command_;
				}
			}

			// Token: 0x170001A0 RID: 416
			// (get) Token: 0x06000975 RID: 2421 RVA: 0x0003DCD4 File Offset: 0x0003BED4
			public string Filename
			{
				get
				{
					return this.filename_;
				}
			}

			// Token: 0x170001A1 RID: 417
			// (get) Token: 0x06000976 RID: 2422 RVA: 0x0003DCF4 File Offset: 0x0003BEF4
			// (set) Token: 0x06000977 RID: 2423 RVA: 0x0003DD14 File Offset: 0x0003BF14
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

			// Token: 0x170001A2 RID: 418
			// (get) Token: 0x06000978 RID: 2424 RVA: 0x0003DD20 File Offset: 0x0003BF20
			// (set) Token: 0x06000979 RID: 2425 RVA: 0x0003DD40 File Offset: 0x0003BF40
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

			// Token: 0x170001A3 RID: 419
			// (get) Token: 0x0600097A RID: 2426 RVA: 0x0003DD4C File Offset: 0x0003BF4C
			// (set) Token: 0x0600097B RID: 2427 RVA: 0x0003DD6C File Offset: 0x0003BF6C
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

			// Token: 0x0600097C RID: 2428 RVA: 0x0003DD78 File Offset: 0x0003BF78
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

			// Token: 0x040006F5 RID: 1781
			private ZipEntry entry_;

			// Token: 0x040006F6 RID: 1782
			private ZipEntry outEntry_;

			// Token: 0x040006F7 RID: 1783
			private ZipFile.UpdateCommand command_;

			// Token: 0x040006F8 RID: 1784
			private IStaticDataSource dataSource_;

			// Token: 0x040006F9 RID: 1785
			private string filename_;

			// Token: 0x040006FA RID: 1786
			private long sizePatchOffset_ = -1L;

			// Token: 0x040006FB RID: 1787
			private long crcPatchOffset_ = -1L;

			// Token: 0x040006FC RID: 1788
			private long _offsetBasedSize = -1L;
		}

		// Token: 0x0200011C RID: 284
		private class ZipString
		{
			// Token: 0x0600097D RID: 2429 RVA: 0x0003DDB4 File Offset: 0x0003BFB4
			public ZipString(string comment)
			{
				this.comment_ = comment;
				this.isSourceString_ = true;
			}

			// Token: 0x0600097E RID: 2430 RVA: 0x0003DDCC File Offset: 0x0003BFCC
			public ZipString(byte[] rawString)
			{
				this.rawComment_ = rawString;
			}

			// Token: 0x170001A4 RID: 420
			// (get) Token: 0x0600097F RID: 2431 RVA: 0x0003DDE0 File Offset: 0x0003BFE0
			public bool IsSourceString
			{
				get
				{
					return this.isSourceString_;
				}
			}

			// Token: 0x170001A5 RID: 421
			// (get) Token: 0x06000980 RID: 2432 RVA: 0x0003DE00 File Offset: 0x0003C000
			public int RawLength
			{
				get
				{
					this.MakeBytesAvailable();
					return this.rawComment_.Length;
				}
			}

			// Token: 0x170001A6 RID: 422
			// (get) Token: 0x06000981 RID: 2433 RVA: 0x0003DE28 File Offset: 0x0003C028
			public byte[] RawComment
			{
				get
				{
					this.MakeBytesAvailable();
					return (byte[])this.rawComment_.Clone();
				}
			}

			// Token: 0x06000982 RID: 2434 RVA: 0x0003DE58 File Offset: 0x0003C058
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

			// Token: 0x06000983 RID: 2435 RVA: 0x0003DE90 File Offset: 0x0003C090
			private void MakeTextAvailable()
			{
				bool flag = this.comment_ == null;
				if (flag)
				{
					this.comment_ = ZipConstants.ConvertToString(this.rawComment_);
				}
			}

			// Token: 0x06000984 RID: 2436 RVA: 0x0003DEC8 File Offset: 0x0003C0C8
			private void MakeBytesAvailable()
			{
				bool flag = this.rawComment_ == null;
				if (flag)
				{
					this.rawComment_ = ZipConstants.ConvertToArray(this.comment_);
				}
			}

			// Token: 0x06000985 RID: 2437 RVA: 0x0003DF00 File Offset: 0x0003C100
			public static implicit operator string(ZipFile.ZipString zipString)
			{
				zipString.MakeTextAvailable();
				return zipString.comment_;
			}

			// Token: 0x040006FD RID: 1789
			private string comment_;

			// Token: 0x040006FE RID: 1790
			private byte[] rawComment_;

			// Token: 0x040006FF RID: 1791
			private bool isSourceString_;
		}

		// Token: 0x0200011D RID: 285
		private class ZipEntryEnumerator : IEnumerator
		{
			// Token: 0x06000986 RID: 2438 RVA: 0x0003DF28 File Offset: 0x0003C128
			public ZipEntryEnumerator(ZipEntry[] entries)
			{
				this.array = entries;
			}

			// Token: 0x170001A7 RID: 423
			// (get) Token: 0x06000987 RID: 2439 RVA: 0x0003DF40 File Offset: 0x0003C140
			public object Current
			{
				get
				{
					return this.array[this.index];
				}
			}

			// Token: 0x06000988 RID: 2440 RVA: 0x0003DF6C File Offset: 0x0003C16C
			public void Reset()
			{
				this.index = -1;
			}

			// Token: 0x06000989 RID: 2441 RVA: 0x0003DF78 File Offset: 0x0003C178
			public bool MoveNext()
			{
				int num = this.index + 1;
				this.index = num;
				return num < this.array.Length;
			}

			// Token: 0x04000700 RID: 1792
			private ZipEntry[] array;

			// Token: 0x04000701 RID: 1793
			private int index = -1;
		}

		// Token: 0x0200011E RID: 286
		private class UncompressedStream : Stream
		{
			// Token: 0x0600098A RID: 2442 RVA: 0x0003DFAC File Offset: 0x0003C1AC
			public UncompressedStream(Stream baseStream)
			{
				this.baseStream_ = baseStream;
			}

			// Token: 0x0600098B RID: 2443 RVA: 0x0003DFC0 File Offset: 0x0003C1C0
			public override void Close()
			{
			}

			// Token: 0x170001A8 RID: 424
			// (get) Token: 0x0600098C RID: 2444 RVA: 0x0003DFC4 File Offset: 0x0003C1C4
			public override bool CanRead
			{
				get
				{
					return false;
				}
			}

			// Token: 0x0600098D RID: 2445 RVA: 0x0003DFE0 File Offset: 0x0003C1E0
			public override void Flush()
			{
				this.baseStream_.Flush();
			}

			// Token: 0x170001A9 RID: 425
			// (get) Token: 0x0600098E RID: 2446 RVA: 0x0003DFF0 File Offset: 0x0003C1F0
			public override bool CanWrite
			{
				get
				{
					return this.baseStream_.CanWrite;
				}
			}

			// Token: 0x170001AA RID: 426
			// (get) Token: 0x0600098F RID: 2447 RVA: 0x0003E014 File Offset: 0x0003C214
			public override bool CanSeek
			{
				get
				{
					return false;
				}
			}

			// Token: 0x170001AB RID: 427
			// (get) Token: 0x06000990 RID: 2448 RVA: 0x0003E030 File Offset: 0x0003C230
			public override long Length
			{
				get
				{
					return 0L;
				}
			}

			// Token: 0x170001AC RID: 428
			// (get) Token: 0x06000991 RID: 2449 RVA: 0x0003E04C File Offset: 0x0003C24C
			// (set) Token: 0x06000992 RID: 2450 RVA: 0x0003E070 File Offset: 0x0003C270
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

			// Token: 0x06000993 RID: 2451 RVA: 0x0003E074 File Offset: 0x0003C274
			public override int Read(byte[] buffer, int offset, int count)
			{
				return 0;
			}

			// Token: 0x06000994 RID: 2452 RVA: 0x0003E090 File Offset: 0x0003C290
			public override long Seek(long offset, SeekOrigin origin)
			{
				return 0L;
			}

			// Token: 0x06000995 RID: 2453 RVA: 0x0003E0AC File Offset: 0x0003C2AC
			public override void SetLength(long value)
			{
			}

			// Token: 0x06000996 RID: 2454 RVA: 0x0003E0B0 File Offset: 0x0003C2B0
			public override void Write(byte[] buffer, int offset, int count)
			{
				this.baseStream_.Write(buffer, offset, count);
			}

			// Token: 0x04000702 RID: 1794
			private Stream baseStream_;
		}

		// Token: 0x0200011F RID: 287
		private class PartialInputStream : Stream
		{
			// Token: 0x06000997 RID: 2455 RVA: 0x0003E0C4 File Offset: 0x0003C2C4
			public PartialInputStream(ZipFile zipFile, long start, long length)
			{
				this.start_ = start;
				this.length_ = length;
				this.zipFile_ = zipFile;
				this.baseStream_ = this.zipFile_.baseStream_;
				this.readPos_ = start;
				this.end_ = start + length;
			}

			// Token: 0x06000998 RID: 2456 RVA: 0x0003E104 File Offset: 0x0003C304
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

			// Token: 0x06000999 RID: 2457 RVA: 0x0003E18C File Offset: 0x0003C38C
			public override void Close()
			{
			}

			// Token: 0x0600099A RID: 2458 RVA: 0x0003E190 File Offset: 0x0003C390
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

			// Token: 0x0600099B RID: 2459 RVA: 0x0003E250 File Offset: 0x0003C450
			public override void Write(byte[] buffer, int offset, int count)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0600099C RID: 2460 RVA: 0x0003E258 File Offset: 0x0003C458
			public override void SetLength(long value)
			{
				throw new NotSupportedException();
			}

			// Token: 0x0600099D RID: 2461 RVA: 0x0003E260 File Offset: 0x0003C460
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

			// Token: 0x0600099E RID: 2462 RVA: 0x0003E314 File Offset: 0x0003C514
			public override void Flush()
			{
			}

			// Token: 0x170001AD RID: 429
			// (get) Token: 0x0600099F RID: 2463 RVA: 0x0003E318 File Offset: 0x0003C518
			// (set) Token: 0x060009A0 RID: 2464 RVA: 0x0003E340 File Offset: 0x0003C540
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

			// Token: 0x170001AE RID: 430
			// (get) Token: 0x060009A1 RID: 2465 RVA: 0x0003E3A0 File Offset: 0x0003C5A0
			public override long Length
			{
				get
				{
					return this.length_;
				}
			}

			// Token: 0x170001AF RID: 431
			// (get) Token: 0x060009A2 RID: 2466 RVA: 0x0003E3C0 File Offset: 0x0003C5C0
			public override bool CanWrite
			{
				get
				{
					return false;
				}
			}

			// Token: 0x170001B0 RID: 432
			// (get) Token: 0x060009A3 RID: 2467 RVA: 0x0003E3DC File Offset: 0x0003C5DC
			public override bool CanSeek
			{
				get
				{
					return true;
				}
			}

			// Token: 0x170001B1 RID: 433
			// (get) Token: 0x060009A4 RID: 2468 RVA: 0x0003E3F8 File Offset: 0x0003C5F8
			public override bool CanRead
			{
				get
				{
					return true;
				}
			}

			// Token: 0x170001B2 RID: 434
			// (get) Token: 0x060009A5 RID: 2469 RVA: 0x0003E414 File Offset: 0x0003C614
			public override bool CanTimeout
			{
				get
				{
					return this.baseStream_.CanTimeout;
				}
			}

			// Token: 0x04000703 RID: 1795
			private ZipFile zipFile_;

			// Token: 0x04000704 RID: 1796
			private Stream baseStream_;

			// Token: 0x04000705 RID: 1797
			private long start_;

			// Token: 0x04000706 RID: 1798
			private long length_;

			// Token: 0x04000707 RID: 1799
			private long readPos_;

			// Token: 0x04000708 RID: 1800
			private long end_;
		}
	}
}
