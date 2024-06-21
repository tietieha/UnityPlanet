using System;
using System.IO;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x02000061 RID: 97
	[ComVisible(false)]
	public class ZipEntry : ICloneable
	{
		// Token: 0x06000413 RID: 1043 RVA: 0x0001C660 File Offset: 0x0001A860
		public ZipEntry(string name) : this(name, 0, 51, CompressionMethod.Deflated)
		{
		}

		// Token: 0x06000414 RID: 1044 RVA: 0x0001C670 File Offset: 0x0001A870
		internal ZipEntry(string name, int versionRequiredToExtract) : this(name, versionRequiredToExtract, 51, CompressionMethod.Deflated)
		{
		}

		// Token: 0x06000415 RID: 1045 RVA: 0x0001C680 File Offset: 0x0001A880
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

		// Token: 0x06000416 RID: 1046 RVA: 0x0001C73C File Offset: 0x0001A93C
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

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x06000417 RID: 1047 RVA: 0x0001C878 File Offset: 0x0001AA78
		public bool HasCrc
		{
			get
			{
				return (this.known & ZipEntry.Known.Crc) > ZipEntry.Known.None;
			}
		}

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x06000418 RID: 1048 RVA: 0x0001C89C File Offset: 0x0001AA9C
		// (set) Token: 0x06000419 RID: 1049 RVA: 0x0001C8C0 File Offset: 0x0001AAC0
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

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x0600041A RID: 1050 RVA: 0x0001C900 File Offset: 0x0001AB00
		// (set) Token: 0x0600041B RID: 1051 RVA: 0x0001C928 File Offset: 0x0001AB28
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

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x0600041C RID: 1052 RVA: 0x0001C970 File Offset: 0x0001AB70
		// (set) Token: 0x0600041D RID: 1053 RVA: 0x0001C990 File Offset: 0x0001AB90
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

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x0600041E RID: 1054 RVA: 0x0001C99C File Offset: 0x0001AB9C
		// (set) Token: 0x0600041F RID: 1055 RVA: 0x0001C9BC File Offset: 0x0001ABBC
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

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x06000420 RID: 1056 RVA: 0x0001C9C8 File Offset: 0x0001ABC8
		// (set) Token: 0x06000421 RID: 1057 RVA: 0x0001C9E8 File Offset: 0x0001ABE8
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

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x06000422 RID: 1058 RVA: 0x0001C9F4 File Offset: 0x0001ABF4
		// (set) Token: 0x06000423 RID: 1059 RVA: 0x0001CA14 File Offset: 0x0001AC14
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

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x06000424 RID: 1060 RVA: 0x0001CA20 File Offset: 0x0001AC20
		// (set) Token: 0x06000425 RID: 1061 RVA: 0x0001CA5C File Offset: 0x0001AC5C
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

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x06000426 RID: 1062 RVA: 0x0001CA78 File Offset: 0x0001AC78
		public int VersionMadeBy
		{
			get
			{
				return (int)(this.versionMadeBy & 255);
			}
		}

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x06000427 RID: 1063 RVA: 0x0001CAA0 File Offset: 0x0001ACA0
		public bool IsDOSEntry
		{
			get
			{
				return this.HostSystem == 0 || this.HostSystem == 10;
			}
		}

		// Token: 0x06000428 RID: 1064 RVA: 0x0001CAD4 File Offset: 0x0001ACD4
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

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x06000429 RID: 1065 RVA: 0x0001CB3C File Offset: 0x0001AD3C
		// (set) Token: 0x0600042A RID: 1066 RVA: 0x0001CB64 File Offset: 0x0001AD64
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

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x0600042B RID: 1067 RVA: 0x0001CB94 File Offset: 0x0001AD94
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

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x0600042C RID: 1068 RVA: 0x0001CC68 File Offset: 0x0001AE68
		public bool CanDecompress
		{
			get
			{
				return this.Version <= 51 && (this.Version == 10 || this.Version == 11 || this.Version == 20 || this.Version == 45 || this.Version == 51) && this.IsCompressionMethodSupported();
			}
		}

		// Token: 0x0600042D RID: 1069 RVA: 0x0001CCDC File Offset: 0x0001AEDC
		public void ForceZip64()
		{
			this.forceZip64_ = true;
		}

		// Token: 0x0600042E RID: 1070 RVA: 0x0001CCE8 File Offset: 0x0001AEE8
		public bool IsZip64Forced()
		{
			return this.forceZip64_;
		}

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x0600042F RID: 1071 RVA: 0x0001CD08 File Offset: 0x0001AF08
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

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x06000430 RID: 1072 RVA: 0x0001CDA0 File Offset: 0x0001AFA0
		public bool CentralHeaderRequiresZip64
		{
			get
			{
				return this.LocalHeaderRequiresZip64 || this.offset >= (long)((ulong)-1);
			}
		}

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x06000431 RID: 1073 RVA: 0x0001CDD8 File Offset: 0x0001AFD8
		// (set) Token: 0x06000432 RID: 1074 RVA: 0x0001CE14 File Offset: 0x0001B014
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

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x06000433 RID: 1075 RVA: 0x0001CE30 File Offset: 0x0001B030
		// (set) Token: 0x06000434 RID: 1076 RVA: 0x0001CEE4 File Offset: 0x0001B0E4
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

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x06000435 RID: 1077 RVA: 0x0001CFAC File Offset: 0x0001B1AC
		public string Name
		{
			get
			{
				return this.name;
			}
		}

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x06000436 RID: 1078 RVA: 0x0001CFCC File Offset: 0x0001B1CC
		// (set) Token: 0x06000437 RID: 1079 RVA: 0x0001D000 File Offset: 0x0001B200
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

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x06000438 RID: 1080 RVA: 0x0001D018 File Offset: 0x0001B218
		// (set) Token: 0x06000439 RID: 1081 RVA: 0x0001D04C File Offset: 0x0001B24C
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

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x0600043A RID: 1082 RVA: 0x0001D064 File Offset: 0x0001B264
		// (set) Token: 0x0600043B RID: 1083 RVA: 0x0001D09C File Offset: 0x0001B29C
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

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x0600043C RID: 1084 RVA: 0x0001D0EC File Offset: 0x0001B2EC
		// (set) Token: 0x0600043D RID: 1085 RVA: 0x0001D10C File Offset: 0x0001B30C
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

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x0600043E RID: 1086 RVA: 0x0001D144 File Offset: 0x0001B344
		internal CompressionMethod CompressionMethodForHeader
		{
			get
			{
				return (this.AESKeySize > 0) ? CompressionMethod.WinZipAES : this.method;
			}
		}

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x0600043F RID: 1087 RVA: 0x0001D178 File Offset: 0x0001B378
		// (set) Token: 0x06000440 RID: 1088 RVA: 0x0001D198 File Offset: 0x0001B398
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

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x06000441 RID: 1089 RVA: 0x0001D204 File Offset: 0x0001B404
		// (set) Token: 0x06000442 RID: 1090 RVA: 0x0001D280 File Offset: 0x0001B480
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

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x06000443 RID: 1091 RVA: 0x0001D2FC File Offset: 0x0001B4FC
		internal byte AESEncryptionStrength
		{
			get
			{
				return (byte)this._aesEncryptionStrength;
			}
		}

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x06000444 RID: 1092 RVA: 0x0001D31C File Offset: 0x0001B51C
		internal int AESSaltLen
		{
			get
			{
				return this.AESKeySize / 16;
			}
		}

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x06000445 RID: 1093 RVA: 0x0001D340 File Offset: 0x0001B540
		internal int AESOverheadSize
		{
			get
			{
				return 12 + this.AESSaltLen;
			}
		}

		// Token: 0x06000446 RID: 1094 RVA: 0x0001D364 File Offset: 0x0001B564
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

		// Token: 0x06000447 RID: 1095 RVA: 0x0001D5F0 File Offset: 0x0001B7F0
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

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x06000448 RID: 1096 RVA: 0x0001D6A4 File Offset: 0x0001B8A4
		// (set) Token: 0x06000449 RID: 1097 RVA: 0x0001D6C4 File Offset: 0x0001B8C4
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

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x0600044A RID: 1098 RVA: 0x0001D710 File Offset: 0x0001B910
		public bool IsDirectory
		{
			get
			{
				int length = this.name.Length;
				return (length > 0 && (this.name[length - 1] == '/' || this.name[length - 1] == '\\')) || this.HasDosAttributes(16);
			}
		}

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x0600044B RID: 1099 RVA: 0x0001D778 File Offset: 0x0001B978
		public bool IsFile
		{
			get
			{
				return !this.IsDirectory && !this.HasDosAttributes(8);
			}
		}

		// Token: 0x0600044C RID: 1100 RVA: 0x0001D7AC File Offset: 0x0001B9AC
		public bool IsCompressionMethodSupported()
		{
			return ZipEntry.IsCompressionMethodSupported(this.CompressionMethod);
		}

		// Token: 0x0600044D RID: 1101 RVA: 0x0001D7D0 File Offset: 0x0001B9D0
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

		// Token: 0x0600044E RID: 1102 RVA: 0x0001D838 File Offset: 0x0001BA38
		public override string ToString()
		{
			return this.name;
		}

		// Token: 0x0600044F RID: 1103 RVA: 0x0001D858 File Offset: 0x0001BA58
		public static bool IsCompressionMethodSupported(CompressionMethod method)
		{
			return method == CompressionMethod.Deflated || method == CompressionMethod.Stored;
		}

		// Token: 0x06000450 RID: 1104 RVA: 0x0001D884 File Offset: 0x0001BA84
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

		// Token: 0x040002C5 RID: 709
		private ZipEntry.Known known;

		// Token: 0x040002C6 RID: 710
		private int externalFileAttributes;

		// Token: 0x040002C7 RID: 711
		private ushort versionMadeBy;

		// Token: 0x040002C8 RID: 712
		private string name;

		// Token: 0x040002C9 RID: 713
		private ulong size;

		// Token: 0x040002CA RID: 714
		private ulong compressedSize;

		// Token: 0x040002CB RID: 715
		private ushort versionToExtract;

		// Token: 0x040002CC RID: 716
		private uint crc;

		// Token: 0x040002CD RID: 717
		private uint dosTime;

		// Token: 0x040002CE RID: 718
		private CompressionMethod method;

		// Token: 0x040002CF RID: 719
		private byte[] extra;

		// Token: 0x040002D0 RID: 720
		private string comment;

		// Token: 0x040002D1 RID: 721
		private int flags;

		// Token: 0x040002D2 RID: 722
		private long zipFileIndex;

		// Token: 0x040002D3 RID: 723
		private long offset;

		// Token: 0x040002D4 RID: 724
		private bool forceZip64_;

		// Token: 0x040002D5 RID: 725
		private byte cryptoCheckValue_;

		// Token: 0x040002D6 RID: 726
		private int _aesVer;

		// Token: 0x040002D7 RID: 727
		private int _aesEncryptionStrength;

		// Token: 0x02000114 RID: 276
		[Flags]
		private enum Known : byte
		{
			// Token: 0x040006DC RID: 1756
			None = 0,
			// Token: 0x040006DD RID: 1757
			Size = 1,
			// Token: 0x040006DE RID: 1758
			CompressedSize = 2,
			// Token: 0x040006DF RID: 1759
			Crc = 4,
			// Token: 0x040006E0 RID: 1760
			Time = 8,
			// Token: 0x040006E1 RID: 1761
			ExternalAttributes = 16
		}
	}
}
