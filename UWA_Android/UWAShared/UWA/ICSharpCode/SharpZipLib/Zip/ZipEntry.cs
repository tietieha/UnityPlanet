using System;
using System.IO;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x02000070 RID: 112
	[ComVisible(false)]
	public class ZipEntry : ICloneable
	{
		// Token: 0x060004EF RID: 1263 RVA: 0x00029418 File Offset: 0x00027618
		public ZipEntry(string name) : this(name, 0, 51, CompressionMethod.Deflated)
		{
		}

		// Token: 0x060004F0 RID: 1264 RVA: 0x00029428 File Offset: 0x00027628
		internal ZipEntry(string name, int versionRequiredToExtract) : this(name, versionRequiredToExtract, 51, CompressionMethod.Deflated)
		{
		}

		// Token: 0x060004F1 RID: 1265 RVA: 0x00029438 File Offset: 0x00027638
		internal ZipEntry(string name, int versionRequiredToExtract, int madeByInfo, CompressionMethod method)
		{
			this.externalFileAttributes = -1;
			this.method = CompressionMethod.Deflated;
			this.zipFileIndex = -1L;
			base..ctor();
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			bool flag2 = name.Length > 65535;
			if (flag2)
			{
				throw new ArgumentException("Name is too long", "name");
			}
			bool flag3 = versionRequiredToExtract != 0 && versionRequiredToExtract < 10;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException("versionRequiredToExtract");
			}
			this.DateTime = DateTime.Now;
			this.name = name;
			this.versionMadeBy = (ushort)madeByInfo;
			this.versionToExtract = (ushort)versionRequiredToExtract;
			this.method = method;
		}

		// Token: 0x060004F2 RID: 1266 RVA: 0x000294F4 File Offset: 0x000276F4
		[Obsolete("Use Clone instead")]
		public ZipEntry(ZipEntry entry)
		{
			this.externalFileAttributes = -1;
			this.method = CompressionMethod.Deflated;
			this.zipFileIndex = -1L;
			base..ctor();
			bool flag = entry == null;
			if (flag)
			{
				throw new ArgumentNullException("entry");
			}
			this.known = entry.known;
			this.name = entry.name;
			this.size = entry.size;
			this.compressedSize = entry.compressedSize;
			this.crc = entry.crc;
			this.dosTime = entry.dosTime;
			this.method = entry.method;
			this.comment = entry.comment;
			this.versionToExtract = entry.versionToExtract;
			this.versionMadeBy = entry.versionMadeBy;
			this.externalFileAttributes = entry.externalFileAttributes;
			this.flags = entry.flags;
			this.zipFileIndex = entry.zipFileIndex;
			this.offset = entry.offset;
			this.forceZip64_ = entry.forceZip64_;
			bool flag2 = entry.extra != null;
			if (flag2)
			{
				this.extra = new byte[entry.extra.Length];
				Array.Copy(entry.extra, 0, this.extra, 0, entry.extra.Length);
			}
		}

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x060004F3 RID: 1267 RVA: 0x00029630 File Offset: 0x00027830
		public bool HasCrc
		{
			get
			{
				return (this.known & ZipEntry.Known.Crc) > ZipEntry.Known.None;
			}
		}

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x060004F4 RID: 1268 RVA: 0x00029654 File Offset: 0x00027854
		// (set) Token: 0x060004F5 RID: 1269 RVA: 0x00029678 File Offset: 0x00027878
		public bool IsCrypted
		{
			get
			{
				return (this.flags & 1) != 0;
			}
			set
			{
				if (value)
				{
					this.flags |= 1;
				}
				else
				{
					this.flags &= -2;
				}
			}
		}

		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x060004F6 RID: 1270 RVA: 0x000296B8 File Offset: 0x000278B8
		// (set) Token: 0x060004F7 RID: 1271 RVA: 0x000296E0 File Offset: 0x000278E0
		public bool IsUnicodeText
		{
			get
			{
				return (this.flags & 2048) != 0;
			}
			set
			{
				if (value)
				{
					this.flags |= 2048;
				}
				else
				{
					this.flags &= -2049;
				}
			}
		}

		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x060004F8 RID: 1272 RVA: 0x00029728 File Offset: 0x00027928
		// (set) Token: 0x060004F9 RID: 1273 RVA: 0x00029748 File Offset: 0x00027948
		internal byte CryptoCheckValue
		{
			get
			{
				return this.cryptoCheckValue_;
			}
			set
			{
				this.cryptoCheckValue_ = value;
			}
		}

		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x060004FA RID: 1274 RVA: 0x00029754 File Offset: 0x00027954
		// (set) Token: 0x060004FB RID: 1275 RVA: 0x00029774 File Offset: 0x00027974
		public int Flags
		{
			get
			{
				return this.flags;
			}
			set
			{
				this.flags = value;
			}
		}

		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x060004FC RID: 1276 RVA: 0x00029780 File Offset: 0x00027980
		// (set) Token: 0x060004FD RID: 1277 RVA: 0x000297A0 File Offset: 0x000279A0
		public long ZipFileIndex
		{
			get
			{
				return this.zipFileIndex;
			}
			set
			{
				this.zipFileIndex = value;
			}
		}

		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x060004FE RID: 1278 RVA: 0x000297AC File Offset: 0x000279AC
		// (set) Token: 0x060004FF RID: 1279 RVA: 0x000297CC File Offset: 0x000279CC
		public long Offset
		{
			get
			{
				return this.offset;
			}
			set
			{
				this.offset = value;
			}
		}

		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x06000500 RID: 1280 RVA: 0x000297D8 File Offset: 0x000279D8
		// (set) Token: 0x06000501 RID: 1281 RVA: 0x00029814 File Offset: 0x00027A14
		public int ExternalFileAttributes
		{
			get
			{
				bool flag = (this.known & ZipEntry.Known.ExternalAttributes) == ZipEntry.Known.None;
				int result;
				if (flag)
				{
					result = -1;
				}
				else
				{
					result = this.externalFileAttributes;
				}
				return result;
			}
			set
			{
				this.externalFileAttributes = value;
				this.known |= ZipEntry.Known.ExternalAttributes;
			}
		}

		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x06000502 RID: 1282 RVA: 0x00029830 File Offset: 0x00027A30
		public int VersionMadeBy
		{
			get
			{
				return (int)(this.versionMadeBy & 255);
			}
		}

		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x06000503 RID: 1283 RVA: 0x00029858 File Offset: 0x00027A58
		public bool IsDOSEntry
		{
			get
			{
				return this.HostSystem == 0 || this.HostSystem == 10;
			}
		}

		// Token: 0x06000504 RID: 1284 RVA: 0x0002988C File Offset: 0x00027A8C
		private bool HasDosAttributes(int attributes)
		{
			bool result = false;
			bool flag = (this.known & ZipEntry.Known.ExternalAttributes) > ZipEntry.Known.None;
			if (flag)
			{
				bool flag2 = (this.HostSystem == 0 || this.HostSystem == 10) && (this.ExternalFileAttributes & attributes) == attributes;
				if (flag2)
				{
					result = true;
				}
			}
			return result;
		}

		// Token: 0x170000CA RID: 202
		// (get) Token: 0x06000505 RID: 1285 RVA: 0x000298F4 File Offset: 0x00027AF4
		// (set) Token: 0x06000506 RID: 1286 RVA: 0x0002991C File Offset: 0x00027B1C
		public int HostSystem
		{
			get
			{
				return this.versionMadeBy >> 8 & 255;
			}
			set
			{
				this.versionMadeBy &= 255;
				this.versionMadeBy |= (ushort)((value & 255) << 8);
			}
		}

		// Token: 0x170000CB RID: 203
		// (get) Token: 0x06000507 RID: 1287 RVA: 0x0002994C File Offset: 0x00027B4C
		public int Version
		{
			get
			{
				bool flag = this.versionToExtract > 0;
				int result;
				if (flag)
				{
					result = (int)this.versionToExtract;
				}
				else
				{
					int num = 10;
					bool flag2 = this.AESKeySize > 0;
					if (flag2)
					{
						num = 51;
					}
					else
					{
						bool centralHeaderRequiresZip = this.CentralHeaderRequiresZip64;
						if (centralHeaderRequiresZip)
						{
							num = 45;
						}
						else
						{
							bool flag3 = CompressionMethod.Deflated == this.method;
							if (flag3)
							{
								num = 20;
							}
							else
							{
								bool isDirectory = this.IsDirectory;
								if (isDirectory)
								{
									num = 20;
								}
								else
								{
									bool isCrypted = this.IsCrypted;
									if (isCrypted)
									{
										num = 20;
									}
									else
									{
										bool flag4 = this.HasDosAttributes(8);
										if (flag4)
										{
											num = 11;
										}
									}
								}
							}
						}
					}
					result = num;
				}
				return result;
			}
		}

		// Token: 0x170000CC RID: 204
		// (get) Token: 0x06000508 RID: 1288 RVA: 0x00029A20 File Offset: 0x00027C20
		public bool CanDecompress
		{
			get
			{
				return this.Version <= 51 && (this.Version == 10 || this.Version == 11 || this.Version == 20 || this.Version == 45 || this.Version == 51) && this.IsCompressionMethodSupported();
			}
		}

		// Token: 0x06000509 RID: 1289 RVA: 0x00029A94 File Offset: 0x00027C94
		public void ForceZip64()
		{
			this.forceZip64_ = true;
		}

		// Token: 0x0600050A RID: 1290 RVA: 0x00029AA0 File Offset: 0x00027CA0
		public bool IsZip64Forced()
		{
			return this.forceZip64_;
		}

		// Token: 0x170000CD RID: 205
		// (get) Token: 0x0600050B RID: 1291 RVA: 0x00029AC0 File Offset: 0x00027CC0
		public bool LocalHeaderRequiresZip64
		{
			get
			{
				bool flag = this.forceZip64_;
				bool flag2 = !flag;
				if (flag2)
				{
					ulong num = this.compressedSize;
					bool flag3 = this.versionToExtract == 0 && this.IsCrypted;
					if (flag3)
					{
						num += 12UL;
					}
					flag = ((this.size >= (ulong)-1 || num >= (ulong)-1) && (this.versionToExtract == 0 || this.versionToExtract >= 45));
				}
				return flag;
			}
		}

		// Token: 0x170000CE RID: 206
		// (get) Token: 0x0600050C RID: 1292 RVA: 0x00029B58 File Offset: 0x00027D58
		public bool CentralHeaderRequiresZip64
		{
			get
			{
				return this.LocalHeaderRequiresZip64 || this.offset >= (long)((ulong)-1);
			}
		}

		// Token: 0x170000CF RID: 207
		// (get) Token: 0x0600050D RID: 1293 RVA: 0x00029B90 File Offset: 0x00027D90
		// (set) Token: 0x0600050E RID: 1294 RVA: 0x00029BCC File Offset: 0x00027DCC
		public long DosTime
		{
			get
			{
				bool flag = (this.known & ZipEntry.Known.Time) == ZipEntry.Known.None;
				long result;
				if (flag)
				{
					result = 0L;
				}
				else
				{
					result = (long)((ulong)this.dosTime);
				}
				return result;
			}
			set
			{
				this.dosTime = (uint)value;
				this.known |= ZipEntry.Known.Time;
			}
		}

		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x0600050F RID: 1295 RVA: 0x00029BE8 File Offset: 0x00027DE8
		// (set) Token: 0x06000510 RID: 1296 RVA: 0x00029C9C File Offset: 0x00027E9C
		public DateTime DateTime
		{
			get
			{
				uint second = Math.Min(59U, 2U * (this.dosTime & 31U));
				uint minute = Math.Min(59U, this.dosTime >> 5 & 63U);
				uint hour = Math.Min(23U, this.dosTime >> 11 & 31U);
				uint month = Math.Max(1U, Math.Min(12U, this.dosTime >> 21 & 15U));
				uint year = (this.dosTime >> 25 & 127U) + 1980U;
				int day = Math.Max(1, Math.Min(DateTime.DaysInMonth((int)year, (int)month), (int)(this.dosTime >> 16 & 31U)));
				return new DateTime((int)year, (int)month, day, (int)hour, (int)minute, (int)second);
			}
			set
			{
				uint num = (uint)value.Year;
				uint num2 = (uint)value.Month;
				uint num3 = (uint)value.Day;
				uint num4 = (uint)value.Hour;
				uint num5 = (uint)value.Minute;
				uint num6 = (uint)value.Second;
				bool flag = num < 1980U;
				if (flag)
				{
					num = 1980U;
					num2 = 1U;
					num3 = 1U;
					num4 = 0U;
					num5 = 0U;
					num6 = 0U;
				}
				else
				{
					bool flag2 = num > 2107U;
					if (flag2)
					{
						num = 2107U;
						num2 = 12U;
						num3 = 31U;
						num4 = 23U;
						num5 = 59U;
						num6 = 59U;
					}
				}
				this.DosTime = (long)((ulong)((num - 1980U & 127U) << 25 | num2 << 21 | num3 << 16 | num4 << 11 | num5 << 5 | num6 >> 1));
			}
		}

		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x06000511 RID: 1297 RVA: 0x00029D64 File Offset: 0x00027F64
		public string Name
		{
			get
			{
				return this.name;
			}
		}

		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x06000512 RID: 1298 RVA: 0x00029D84 File Offset: 0x00027F84
		// (set) Token: 0x06000513 RID: 1299 RVA: 0x00029DB8 File Offset: 0x00027FB8
		public long Size
		{
			get
			{
				return (long)(((this.known & ZipEntry.Known.Size) != ZipEntry.Known.None) ? this.size : ulong.MaxValue);
			}
			set
			{
				this.size = (ulong)value;
				this.known |= ZipEntry.Known.Size;
			}
		}

		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x06000514 RID: 1300 RVA: 0x00029DD0 File Offset: 0x00027FD0
		// (set) Token: 0x06000515 RID: 1301 RVA: 0x00029E04 File Offset: 0x00028004
		public long CompressedSize
		{
			get
			{
				return (long)(((this.known & ZipEntry.Known.CompressedSize) != ZipEntry.Known.None) ? this.compressedSize : ulong.MaxValue);
			}
			set
			{
				this.compressedSize = (ulong)value;
				this.known |= ZipEntry.Known.CompressedSize;
			}
		}

		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x06000516 RID: 1302 RVA: 0x00029E1C File Offset: 0x0002801C
		// (set) Token: 0x06000517 RID: 1303 RVA: 0x00029E54 File Offset: 0x00028054
		public long Crc
		{
			get
			{
				return (long)(((this.known & ZipEntry.Known.Crc) != ZipEntry.Known.None) ? ((ulong)this.crc & (ulong)-1) : ulong.MaxValue);
			}
			set
			{
				bool flag = ((ulong)this.crc & 18446744069414584320UL) > 0UL;
				if (flag)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.crc = (uint)value;
				this.known |= ZipEntry.Known.Crc;
			}
		}

		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x06000518 RID: 1304 RVA: 0x00029EA4 File Offset: 0x000280A4
		// (set) Token: 0x06000519 RID: 1305 RVA: 0x00029EC4 File Offset: 0x000280C4
		public CompressionMethod CompressionMethod
		{
			get
			{
				return this.method;
			}
			set
			{
				bool flag = !ZipEntry.IsCompressionMethodSupported(value);
				if (flag)
				{
					throw new NotSupportedException("Compression method not supported");
				}
				this.method = value;
			}
		}

		// Token: 0x170000D6 RID: 214
		// (get) Token: 0x0600051A RID: 1306 RVA: 0x00029EFC File Offset: 0x000280FC
		internal CompressionMethod CompressionMethodForHeader
		{
			get
			{
				return (this.AESKeySize > 0) ? CompressionMethod.WinZipAES : this.method;
			}
		}

		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x0600051B RID: 1307 RVA: 0x00029F30 File Offset: 0x00028130
		// (set) Token: 0x0600051C RID: 1308 RVA: 0x00029F50 File Offset: 0x00028150
		public byte[] ExtraData
		{
			get
			{
				return this.extra;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					this.extra = null;
				}
				else
				{
					bool flag2 = value.Length > 65535;
					if (flag2)
					{
						throw new ArgumentOutOfRangeException("value");
					}
					this.extra = new byte[value.Length];
					Array.Copy(value, 0, this.extra, 0, value.Length);
				}
			}
		}

		// Token: 0x170000D8 RID: 216
		// (get) Token: 0x0600051D RID: 1309 RVA: 0x00029FBC File Offset: 0x000281BC
		// (set) Token: 0x0600051E RID: 1310 RVA: 0x0002A038 File Offset: 0x00028238
		public int AESKeySize
		{
			get
			{
				int result;
				switch (this._aesEncryptionStrength)
				{
				case 0:
					result = 0;
					break;
				case 1:
					result = 128;
					break;
				case 2:
					result = 192;
					break;
				case 3:
					result = 256;
					break;
				default:
					throw new ZipException("Invalid AESEncryptionStrength " + this._aesEncryptionStrength.ToString());
				}
				return result;
			}
			set
			{
				int num = value;
				int num2 = num;
				if (num2 != 0)
				{
					if (num2 != 128)
					{
						if (num2 != 256)
						{
							throw new ZipException("AESKeySize must be 0, 128 or 256: " + value.ToString());
						}
						this._aesEncryptionStrength = 3;
					}
					else
					{
						this._aesEncryptionStrength = 1;
					}
				}
				else
				{
					this._aesEncryptionStrength = 0;
				}
			}
		}

		// Token: 0x170000D9 RID: 217
		// (get) Token: 0x0600051F RID: 1311 RVA: 0x0002A0B4 File Offset: 0x000282B4
		internal byte AESEncryptionStrength
		{
			get
			{
				return (byte)this._aesEncryptionStrength;
			}
		}

		// Token: 0x170000DA RID: 218
		// (get) Token: 0x06000520 RID: 1312 RVA: 0x0002A0D4 File Offset: 0x000282D4
		internal int AESSaltLen
		{
			get
			{
				return this.AESKeySize / 16;
			}
		}

		// Token: 0x170000DB RID: 219
		// (get) Token: 0x06000521 RID: 1313 RVA: 0x0002A0F8 File Offset: 0x000282F8
		internal int AESOverheadSize
		{
			get
			{
				return 12 + this.AESSaltLen;
			}
		}

		// Token: 0x06000522 RID: 1314 RVA: 0x0002A11C File Offset: 0x0002831C
		internal void ProcessExtraData(bool localHeader)
		{
			ZipExtraData zipExtraData = new ZipExtraData(this.extra);
			bool flag = zipExtraData.Find(1);
			if (flag)
			{
				this.forceZip64_ = true;
				bool flag2 = zipExtraData.ValueLength < 4;
				if (flag2)
				{
					throw new ZipException("Extra data extended Zip64 information length is invalid");
				}
				bool flag3 = localHeader || this.size == (ulong)-1;
				if (flag3)
				{
					this.size = (ulong)zipExtraData.ReadLong();
				}
				bool flag4 = localHeader || this.compressedSize == (ulong)-1;
				if (flag4)
				{
					this.compressedSize = (ulong)zipExtraData.ReadLong();
				}
				bool flag5 = !localHeader && this.offset == (long)((ulong)-1);
				if (flag5)
				{
					this.offset = zipExtraData.ReadLong();
				}
			}
			else
			{
				bool flag6 = (this.versionToExtract & 255) >= 45 && (this.size == (ulong)-1 || this.compressedSize == (ulong)-1);
				if (flag6)
				{
					throw new ZipException("Zip64 Extended information required but is missing.");
				}
			}
			bool flag7 = zipExtraData.Find(10);
			if (flag7)
			{
				bool flag8 = zipExtraData.ValueLength < 4;
				if (flag8)
				{
					throw new ZipException("NTFS Extra data invalid");
				}
				zipExtraData.ReadInt();
				while (zipExtraData.UnreadCount >= 4)
				{
					int num = zipExtraData.ReadShort();
					int num2 = zipExtraData.ReadShort();
					bool flag9 = num == 1;
					if (flag9)
					{
						bool flag10 = num2 >= 24;
						if (flag10)
						{
							long fileTime = zipExtraData.ReadLong();
							long num3 = zipExtraData.ReadLong();
							long num4 = zipExtraData.ReadLong();
							this.DateTime = DateTime.FromFileTime(fileTime);
						}
						break;
					}
					zipExtraData.Skip(num2);
				}
			}
			else
			{
				bool flag11 = zipExtraData.Find(21589);
				if (flag11)
				{
					int valueLength = zipExtraData.ValueLength;
					int num5 = zipExtraData.ReadByte();
					bool flag12 = (num5 & 1) != 0 && valueLength >= 5;
					if (flag12)
					{
						int seconds = zipExtraData.ReadInt();
						this.DateTime = (new DateTime(1970, 1, 1, 0, 0, 0).ToUniversalTime() + new TimeSpan(0, 0, 0, seconds, 0)).ToLocalTime();
					}
				}
			}
			bool flag13 = this.method == CompressionMethod.WinZipAES;
			if (flag13)
			{
				this.ProcessAESExtraData(zipExtraData);
			}
		}

		// Token: 0x06000523 RID: 1315 RVA: 0x0002A3A8 File Offset: 0x000285A8
		private void ProcessAESExtraData(ZipExtraData extraData)
		{
			bool flag = extraData.Find(39169);
			if (!flag)
			{
				throw new ZipException("AES Extra Data missing");
			}
			this.versionToExtract = 51;
			this.Flags |= 64;
			int valueLength = extraData.ValueLength;
			bool flag2 = valueLength < 7;
			if (flag2)
			{
				throw new ZipException("AES Extra Data Length " + valueLength.ToString() + " invalid.");
			}
			int aesVer = extraData.ReadShort();
			int num = extraData.ReadShort();
			int aesEncryptionStrength = extraData.ReadByte();
			int num2 = extraData.ReadShort();
			this._aesVer = aesVer;
			this._aesEncryptionStrength = aesEncryptionStrength;
			this.method = (CompressionMethod)num2;
		}

		// Token: 0x170000DC RID: 220
		// (get) Token: 0x06000524 RID: 1316 RVA: 0x0002A45C File Offset: 0x0002865C
		// (set) Token: 0x06000525 RID: 1317 RVA: 0x0002A47C File Offset: 0x0002867C
		public string Comment
		{
			get
			{
				return this.comment;
			}
			set
			{
				bool flag = value != null && value.Length > 65535;
				if (flag)
				{
					throw new ArgumentOutOfRangeException("value", "cannot exceed 65535");
				}
				this.comment = value;
			}
		}

		// Token: 0x170000DD RID: 221
		// (get) Token: 0x06000526 RID: 1318 RVA: 0x0002A4C8 File Offset: 0x000286C8
		public bool IsDirectory
		{
			get
			{
				int length = this.name.Length;
				return (length > 0 && (this.name[length - 1] == '/' || this.name[length - 1] == '\\')) || this.HasDosAttributes(16);
			}
		}

		// Token: 0x170000DE RID: 222
		// (get) Token: 0x06000527 RID: 1319 RVA: 0x0002A530 File Offset: 0x00028730
		public bool IsFile
		{
			get
			{
				return !this.IsDirectory && !this.HasDosAttributes(8);
			}
		}

		// Token: 0x06000528 RID: 1320 RVA: 0x0002A564 File Offset: 0x00028764
		public bool IsCompressionMethodSupported()
		{
			return ZipEntry.IsCompressionMethodSupported(this.CompressionMethod);
		}

		// Token: 0x06000529 RID: 1321 RVA: 0x0002A588 File Offset: 0x00028788
		public object Clone()
		{
			ZipEntry zipEntry = (ZipEntry)base.MemberwiseClone();
			bool flag = this.extra != null;
			if (flag)
			{
				zipEntry.extra = new byte[this.extra.Length];
				Array.Copy(this.extra, 0, zipEntry.extra, 0, this.extra.Length);
			}
			return zipEntry;
		}

		// Token: 0x0600052A RID: 1322 RVA: 0x0002A5F0 File Offset: 0x000287F0
		public override string ToString()
		{
			return this.name;
		}

		// Token: 0x0600052B RID: 1323 RVA: 0x0002A610 File Offset: 0x00028810
		public static bool IsCompressionMethodSupported(CompressionMethod method)
		{
			return method == CompressionMethod.Deflated || method == CompressionMethod.Stored;
		}

		// Token: 0x0600052C RID: 1324 RVA: 0x0002A63C File Offset: 0x0002883C
		public static string CleanName(string name)
		{
			bool flag = name == null;
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				bool flag2 = Path.IsPathRooted(name);
				if (flag2)
				{
					name = name.Substring(Path.GetPathRoot(name).Length);
				}
				name = name.Replace("\\", "/");
				while (name.Length > 0 && name[0] == '/')
				{
					name = name.Remove(0, 1);
				}
				result = name;
			}
			return result;
		}

		// Token: 0x04000338 RID: 824
		private ZipEntry.Known known;

		// Token: 0x04000339 RID: 825
		private int externalFileAttributes;

		// Token: 0x0400033A RID: 826
		private ushort versionMadeBy;

		// Token: 0x0400033B RID: 827
		private string name;

		// Token: 0x0400033C RID: 828
		private ulong size;

		// Token: 0x0400033D RID: 829
		private ulong compressedSize;

		// Token: 0x0400033E RID: 830
		private ushort versionToExtract;

		// Token: 0x0400033F RID: 831
		private uint crc;

		// Token: 0x04000340 RID: 832
		private uint dosTime;

		// Token: 0x04000341 RID: 833
		private CompressionMethod method;

		// Token: 0x04000342 RID: 834
		private byte[] extra;

		// Token: 0x04000343 RID: 835
		private string comment;

		// Token: 0x04000344 RID: 836
		private int flags;

		// Token: 0x04000345 RID: 837
		private long zipFileIndex;

		// Token: 0x04000346 RID: 838
		private long offset;

		// Token: 0x04000347 RID: 839
		private bool forceZip64_;

		// Token: 0x04000348 RID: 840
		private byte cryptoCheckValue_;

		// Token: 0x04000349 RID: 841
		private int _aesVer;

		// Token: 0x0400034A RID: 842
		private int _aesEncryptionStrength;

		// Token: 0x0200014A RID: 330
		[Flags]
		private enum Known : byte
		{
			// Token: 0x04000799 RID: 1945
			None = 0,
			// Token: 0x0400079A RID: 1946
			Size = 1,
			// Token: 0x0400079B RID: 1947
			CompressedSize = 2,
			// Token: 0x0400079C RID: 1948
			Crc = 4,
			// Token: 0x0400079D RID: 1949
			Time = 8,
			// Token: 0x0400079E RID: 1950
			ExternalAttributes = 16
		}
	}
}
