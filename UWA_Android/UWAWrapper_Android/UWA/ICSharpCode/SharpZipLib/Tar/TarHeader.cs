using System;
using System.Runtime.InteropServices;
using System.Text;

namespace UWA.ICSharpCode.SharpZipLib.Tar
{
	// Token: 0x02000094 RID: 148
	[ComVisible(false)]
	public class TarHeader : ICloneable
	{
		// Token: 0x060006CE RID: 1742 RVA: 0x0002E5D4 File Offset: 0x0002C7D4
		public TarHeader()
		{
			this.Magic = "ustar ";
			this.Version = " ";
			this.Name = "";
			this.LinkName = "";
			this.UserId = TarHeader.defaultUserId;
			this.GroupId = TarHeader.defaultGroupId;
			this.UserName = TarHeader.defaultUser;
			this.GroupName = TarHeader.defaultGroupName;
			this.Size = 0L;
		}

		// Token: 0x17000111 RID: 273
		// (get) Token: 0x060006CF RID: 1743 RVA: 0x0002E658 File Offset: 0x0002C858
		// (set) Token: 0x060006D0 RID: 1744 RVA: 0x0002E678 File Offset: 0x0002C878
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("value");
				}
				this.name = value;
			}
		}

		// Token: 0x060006D1 RID: 1745 RVA: 0x0002E6A8 File Offset: 0x0002C8A8
		[Obsolete("Use the Name property instead", true)]
		public string GetName()
		{
			return this.name;
		}

		// Token: 0x17000112 RID: 274
		// (get) Token: 0x060006D2 RID: 1746 RVA: 0x0002E6C8 File Offset: 0x0002C8C8
		// (set) Token: 0x060006D3 RID: 1747 RVA: 0x0002E6E8 File Offset: 0x0002C8E8
		public int Mode
		{
			get
			{
				return this.mode;
			}
			set
			{
				this.mode = value;
			}
		}

		// Token: 0x17000113 RID: 275
		// (get) Token: 0x060006D4 RID: 1748 RVA: 0x0002E6F4 File Offset: 0x0002C8F4
		// (set) Token: 0x060006D5 RID: 1749 RVA: 0x0002E714 File Offset: 0x0002C914
		public int UserId
		{
			get
			{
				return this.userId;
			}
			set
			{
				this.userId = value;
			}
		}

		// Token: 0x17000114 RID: 276
		// (get) Token: 0x060006D6 RID: 1750 RVA: 0x0002E720 File Offset: 0x0002C920
		// (set) Token: 0x060006D7 RID: 1751 RVA: 0x0002E740 File Offset: 0x0002C940
		public int GroupId
		{
			get
			{
				return this.groupId;
			}
			set
			{
				this.groupId = value;
			}
		}

		// Token: 0x17000115 RID: 277
		// (get) Token: 0x060006D8 RID: 1752 RVA: 0x0002E74C File Offset: 0x0002C94C
		// (set) Token: 0x060006D9 RID: 1753 RVA: 0x0002E76C File Offset: 0x0002C96C
		public long Size
		{
			get
			{
				return this.size;
			}
			set
			{
				bool flag = value < 0L;
				if (flag)
				{
					throw new ArgumentOutOfRangeException("value", "Cannot be less than zero");
				}
				this.size = value;
			}
		}

		// Token: 0x17000116 RID: 278
		// (get) Token: 0x060006DA RID: 1754 RVA: 0x0002E7A4 File Offset: 0x0002C9A4
		// (set) Token: 0x060006DB RID: 1755 RVA: 0x0002E7C4 File Offset: 0x0002C9C4
		public DateTime ModTime
		{
			get
			{
				return this.modTime;
			}
			set
			{
				bool flag = value < TarHeader.dateTime1970;
				if (flag)
				{
					throw new ArgumentOutOfRangeException("value", "ModTime cannot be before Jan 1st 1970");
				}
				this.modTime = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
			}
		}

		// Token: 0x17000117 RID: 279
		// (get) Token: 0x060006DC RID: 1756 RVA: 0x0002E830 File Offset: 0x0002CA30
		public int Checksum
		{
			get
			{
				return this.checksum;
			}
		}

		// Token: 0x17000118 RID: 280
		// (get) Token: 0x060006DD RID: 1757 RVA: 0x0002E850 File Offset: 0x0002CA50
		public bool IsChecksumValid
		{
			get
			{
				return this.isChecksumValid;
			}
		}

		// Token: 0x17000119 RID: 281
		// (get) Token: 0x060006DE RID: 1758 RVA: 0x0002E870 File Offset: 0x0002CA70
		// (set) Token: 0x060006DF RID: 1759 RVA: 0x0002E890 File Offset: 0x0002CA90
		public byte TypeFlag
		{
			get
			{
				return this.typeFlag;
			}
			set
			{
				this.typeFlag = value;
			}
		}

		// Token: 0x1700011A RID: 282
		// (get) Token: 0x060006E0 RID: 1760 RVA: 0x0002E89C File Offset: 0x0002CA9C
		// (set) Token: 0x060006E1 RID: 1761 RVA: 0x0002E8BC File Offset: 0x0002CABC
		public string LinkName
		{
			get
			{
				return this.linkName;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("value");
				}
				this.linkName = value;
			}
		}

		// Token: 0x1700011B RID: 283
		// (get) Token: 0x060006E2 RID: 1762 RVA: 0x0002E8EC File Offset: 0x0002CAEC
		// (set) Token: 0x060006E3 RID: 1763 RVA: 0x0002E90C File Offset: 0x0002CB0C
		public string Magic
		{
			get
			{
				return this.magic;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("value");
				}
				this.magic = value;
			}
		}

		// Token: 0x1700011C RID: 284
		// (get) Token: 0x060006E4 RID: 1764 RVA: 0x0002E93C File Offset: 0x0002CB3C
		// (set) Token: 0x060006E5 RID: 1765 RVA: 0x0002E95C File Offset: 0x0002CB5C
		public string Version
		{
			get
			{
				return this.version;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("value");
				}
				this.version = value;
			}
		}

		// Token: 0x1700011D RID: 285
		// (get) Token: 0x060006E6 RID: 1766 RVA: 0x0002E98C File Offset: 0x0002CB8C
		// (set) Token: 0x060006E7 RID: 1767 RVA: 0x0002E9AC File Offset: 0x0002CBAC
		public string UserName
		{
			get
			{
				return this.userName;
			}
			set
			{
				bool flag = value != null;
				if (flag)
				{
					this.userName = value.Substring(0, Math.Min(32, value.Length));
				}
				else
				{
					string text = Environment.UserName;
					bool flag2 = text.Length > 32;
					if (flag2)
					{
						text = text.Substring(0, 32);
					}
					this.userName = text;
				}
			}
		}

		// Token: 0x1700011E RID: 286
		// (get) Token: 0x060006E8 RID: 1768 RVA: 0x0002EA18 File Offset: 0x0002CC18
		// (set) Token: 0x060006E9 RID: 1769 RVA: 0x0002EA38 File Offset: 0x0002CC38
		public string GroupName
		{
			get
			{
				return this.groupName;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					this.groupName = "None";
				}
				else
				{
					this.groupName = value;
				}
			}
		}

		// Token: 0x1700011F RID: 287
		// (get) Token: 0x060006EA RID: 1770 RVA: 0x0002EA70 File Offset: 0x0002CC70
		// (set) Token: 0x060006EB RID: 1771 RVA: 0x0002EA90 File Offset: 0x0002CC90
		public int DevMajor
		{
			get
			{
				return this.devMajor;
			}
			set
			{
				this.devMajor = value;
			}
		}

		// Token: 0x17000120 RID: 288
		// (get) Token: 0x060006EC RID: 1772 RVA: 0x0002EA9C File Offset: 0x0002CC9C
		// (set) Token: 0x060006ED RID: 1773 RVA: 0x0002EABC File Offset: 0x0002CCBC
		public int DevMinor
		{
			get
			{
				return this.devMinor;
			}
			set
			{
				this.devMinor = value;
			}
		}

		// Token: 0x060006EE RID: 1774 RVA: 0x0002EAC8 File Offset: 0x0002CCC8
		public object Clone()
		{
			return base.MemberwiseClone();
		}

		// Token: 0x060006EF RID: 1775 RVA: 0x0002EAE8 File Offset: 0x0002CCE8
		public void ParseBuffer(byte[] header)
		{
			bool flag = header == null;
			if (flag)
			{
				throw new ArgumentNullException("header");
			}
			int num = 0;
			this.name = TarHeader.ParseName(header, num, 100).ToString();
			num += 100;
			this.mode = (int)TarHeader.ParseOctal(header, num, 8);
			num += 8;
			this.UserId = (int)TarHeader.ParseOctal(header, num, 8);
			num += 8;
			this.GroupId = (int)TarHeader.ParseOctal(header, num, 8);
			num += 8;
			this.Size = TarHeader.ParseOctal(header, num, 12);
			num += 12;
			this.ModTime = TarHeader.GetDateTimeFromCTime(TarHeader.ParseOctal(header, num, 12));
			num += 12;
			this.checksum = (int)TarHeader.ParseOctal(header, num, 8);
			num += 8;
			this.TypeFlag = header[num++];
			this.LinkName = TarHeader.ParseName(header, num, 100).ToString();
			num += 100;
			this.Magic = TarHeader.ParseName(header, num, 6).ToString();
			num += 6;
			this.Version = TarHeader.ParseName(header, num, 2).ToString();
			num += 2;
			this.UserName = TarHeader.ParseName(header, num, 32).ToString();
			num += 32;
			this.GroupName = TarHeader.ParseName(header, num, 32).ToString();
			num += 32;
			this.DevMajor = (int)TarHeader.ParseOctal(header, num, 8);
			num += 8;
			this.DevMinor = (int)TarHeader.ParseOctal(header, num, 8);
			this.isChecksumValid = (this.Checksum == TarHeader.MakeCheckSum(header));
		}

		// Token: 0x060006F0 RID: 1776 RVA: 0x0002EC70 File Offset: 0x0002CE70
		public void WriteHeader(byte[] outBuffer)
		{
			bool flag = outBuffer == null;
			if (flag)
			{
				throw new ArgumentNullException("outBuffer");
			}
			int i = 0;
			i = TarHeader.GetNameBytes(this.Name, outBuffer, i, 100);
			i = TarHeader.GetOctalBytes((long)this.mode, outBuffer, i, 8);
			i = TarHeader.GetOctalBytes((long)this.UserId, outBuffer, i, 8);
			i = TarHeader.GetOctalBytes((long)this.GroupId, outBuffer, i, 8);
			i = TarHeader.GetLongOctalBytes(this.Size, outBuffer, i, 12);
			i = TarHeader.GetLongOctalBytes((long)TarHeader.GetCTime(this.ModTime), outBuffer, i, 12);
			int offset = i;
			for (int j = 0; j < 8; j++)
			{
				outBuffer[i++] = 32;
			}
			outBuffer[i++] = this.TypeFlag;
			i = TarHeader.GetNameBytes(this.LinkName, outBuffer, i, 100);
			i = TarHeader.GetAsciiBytes(this.Magic, 0, outBuffer, i, 6);
			i = TarHeader.GetNameBytes(this.Version, outBuffer, i, 2);
			i = TarHeader.GetNameBytes(this.UserName, outBuffer, i, 32);
			i = TarHeader.GetNameBytes(this.GroupName, outBuffer, i, 32);
			bool flag2 = this.TypeFlag == 51 || this.TypeFlag == 52;
			if (flag2)
			{
				i = TarHeader.GetOctalBytes((long)this.DevMajor, outBuffer, i, 8);
				i = TarHeader.GetOctalBytes((long)this.DevMinor, outBuffer, i, 8);
			}
			while (i < outBuffer.Length)
			{
				outBuffer[i++] = 0;
			}
			this.checksum = TarHeader.ComputeCheckSum(outBuffer);
			TarHeader.GetCheckSumOctalBytes((long)this.checksum, outBuffer, offset, 8);
			this.isChecksumValid = true;
		}

		// Token: 0x060006F1 RID: 1777 RVA: 0x0002EE08 File Offset: 0x0002D008
		public override int GetHashCode()
		{
			return this.Name.GetHashCode();
		}

		// Token: 0x060006F2 RID: 1778 RVA: 0x0002EE2C File Offset: 0x0002D02C
		public override bool Equals(object obj)
		{
			TarHeader tarHeader = obj as TarHeader;
			bool flag = tarHeader != null;
			return flag && (this.name == tarHeader.name && this.mode == tarHeader.mode && this.UserId == tarHeader.UserId && this.GroupId == tarHeader.GroupId && this.Size == tarHeader.Size && this.ModTime == tarHeader.ModTime && this.Checksum == tarHeader.Checksum && this.TypeFlag == tarHeader.TypeFlag && this.LinkName == tarHeader.LinkName && this.Magic == tarHeader.Magic && this.Version == tarHeader.Version && this.UserName == tarHeader.UserName && this.GroupName == tarHeader.GroupName && this.DevMajor == tarHeader.DevMajor) && this.DevMinor == tarHeader.DevMinor;
		}

		// Token: 0x060006F3 RID: 1779 RVA: 0x0002EF8C File Offset: 0x0002D18C
		internal static void SetValueDefaults(int userId, string userName, int groupId, string groupName)
		{
			TarHeader.userIdAsSet = userId;
			TarHeader.defaultUserId = userId;
			TarHeader.userNameAsSet = userName;
			TarHeader.defaultUser = userName;
			TarHeader.groupIdAsSet = groupId;
			TarHeader.defaultGroupId = groupId;
			TarHeader.groupNameAsSet = groupName;
			TarHeader.defaultGroupName = groupName;
		}

		// Token: 0x060006F4 RID: 1780 RVA: 0x0002EFC0 File Offset: 0x0002D1C0
		internal static void RestoreSetValues()
		{
			TarHeader.defaultUserId = TarHeader.userIdAsSet;
			TarHeader.defaultUser = TarHeader.userNameAsSet;
			TarHeader.defaultGroupId = TarHeader.groupIdAsSet;
			TarHeader.defaultGroupName = TarHeader.groupNameAsSet;
		}

		// Token: 0x060006F5 RID: 1781 RVA: 0x0002EFEC File Offset: 0x0002D1EC
		public static long ParseOctal(byte[] header, int offset, int length)
		{
			bool flag = header == null;
			if (flag)
			{
				throw new ArgumentNullException("header");
			}
			long num = 0L;
			bool flag2 = true;
			int num2 = offset + length;
			int i = offset;
			while (i < num2)
			{
				bool flag3 = header[i] == 0;
				if (flag3)
				{
					break;
				}
				bool flag4 = header[i] == 32 || header[i] == 48;
				if (!flag4)
				{
					goto IL_8B;
				}
				bool flag5 = flag2;
				if (!flag5)
				{
					bool flag6 = header[i] == 32;
					if (flag6)
					{
						break;
					}
					goto IL_8B;
				}
				IL_9B:
				i++;
				continue;
				IL_8B:
				flag2 = false;
				num = (num << 3) + (long)(header[i] - 48);
				goto IL_9B;
			}
			return num;
		}

		// Token: 0x060006F6 RID: 1782 RVA: 0x0002F0B8 File Offset: 0x0002D2B8
		public static StringBuilder ParseName(byte[] header, int offset, int length)
		{
			bool flag = header == null;
			if (flag)
			{
				throw new ArgumentNullException("header");
			}
			bool flag2 = offset < 0;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("offset", "Cannot be less than zero");
			}
			bool flag3 = length < 0;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException("length", "Cannot be less than zero");
			}
			bool flag4 = offset + length > header.Length;
			if (flag4)
			{
				throw new ArgumentException("Exceeds header size", "length");
			}
			StringBuilder stringBuilder = new StringBuilder(length);
			for (int i = offset; i < offset + length; i++)
			{
				bool flag5 = header[i] == 0;
				if (flag5)
				{
					break;
				}
				stringBuilder.Append((char)header[i]);
			}
			return stringBuilder;
		}

		// Token: 0x060006F7 RID: 1783 RVA: 0x0002F18C File Offset: 0x0002D38C
		public static int GetNameBytes(StringBuilder name, int nameOffset, byte[] buffer, int bufferOffset, int length)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			bool flag2 = buffer == null;
			if (flag2)
			{
				throw new ArgumentNullException("buffer");
			}
			return TarHeader.GetNameBytes(name.ToString(), nameOffset, buffer, bufferOffset, length);
		}

		// Token: 0x060006F8 RID: 1784 RVA: 0x0002F1E4 File Offset: 0x0002D3E4
		public static int GetNameBytes(string name, int nameOffset, byte[] buffer, int bufferOffset, int length)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			bool flag2 = buffer == null;
			if (flag2)
			{
				throw new ArgumentNullException("buffer");
			}
			int i = 0;
			while (i < length - 1 && nameOffset + i < name.Length)
			{
				buffer[bufferOffset + i] = (byte)name[nameOffset + i];
				i++;
			}
			while (i < length)
			{
				buffer[bufferOffset + i] = 0;
				i++;
			}
			return bufferOffset + length;
		}

		// Token: 0x060006F9 RID: 1785 RVA: 0x0002F288 File Offset: 0x0002D488
		public static int GetNameBytes(StringBuilder name, byte[] buffer, int offset, int length)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			bool flag2 = buffer == null;
			if (flag2)
			{
				throw new ArgumentNullException("buffer");
			}
			return TarHeader.GetNameBytes(name.ToString(), 0, buffer, offset, length);
		}

		// Token: 0x060006FA RID: 1786 RVA: 0x0002F2E0 File Offset: 0x0002D4E0
		public static int GetNameBytes(string name, byte[] buffer, int offset, int length)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			bool flag2 = buffer == null;
			if (flag2)
			{
				throw new ArgumentNullException("buffer");
			}
			return TarHeader.GetNameBytes(name, 0, buffer, offset, length);
		}

		// Token: 0x060006FB RID: 1787 RVA: 0x0002F334 File Offset: 0x0002D534
		public static int GetAsciiBytes(string toAdd, int nameOffset, byte[] buffer, int bufferOffset, int length)
		{
			bool flag = toAdd == null;
			if (flag)
			{
				throw new ArgumentNullException("toAdd");
			}
			bool flag2 = buffer == null;
			if (flag2)
			{
				throw new ArgumentNullException("buffer");
			}
			int num = 0;
			while (num < length && nameOffset + num < toAdd.Length)
			{
				buffer[bufferOffset + num] = (byte)toAdd[nameOffset + num];
				num++;
			}
			return bufferOffset + length;
		}

		// Token: 0x060006FC RID: 1788 RVA: 0x0002F3BC File Offset: 0x0002D5BC
		public static int GetOctalBytes(long value, byte[] buffer, int offset, int length)
		{
			bool flag = buffer == null;
			if (flag)
			{
				throw new ArgumentNullException("buffer");
			}
			int i = length - 1;
			buffer[offset + i] = 0;
			i--;
			bool flag2 = value > 0L;
			if (flag2)
			{
				long num = value;
				while (i >= 0 && num > 0L)
				{
					buffer[offset + i] = 48 + (byte)(num & 7L);
					num >>= 3;
					i--;
				}
			}
			while (i >= 0)
			{
				buffer[offset + i] = 48;
				i--;
			}
			return offset + length;
		}

		// Token: 0x060006FD RID: 1789 RVA: 0x0002F464 File Offset: 0x0002D664
		public static int GetLongOctalBytes(long value, byte[] buffer, int offset, int length)
		{
			return TarHeader.GetOctalBytes(value, buffer, offset, length);
		}

		// Token: 0x060006FE RID: 1790 RVA: 0x0002F488 File Offset: 0x0002D688
		private static int GetCheckSumOctalBytes(long value, byte[] buffer, int offset, int length)
		{
			TarHeader.GetOctalBytes(value, buffer, offset, length - 1);
			return offset + length;
		}

		// Token: 0x060006FF RID: 1791 RVA: 0x0002F4B0 File Offset: 0x0002D6B0
		private static int ComputeCheckSum(byte[] buffer)
		{
			int num = 0;
			for (int i = 0; i < buffer.Length; i++)
			{
				num += (int)buffer[i];
			}
			return num;
		}

		// Token: 0x06000700 RID: 1792 RVA: 0x0002F4EC File Offset: 0x0002D6EC
		private static int MakeCheckSum(byte[] buffer)
		{
			int num = 0;
			for (int i = 0; i < 148; i++)
			{
				num += (int)buffer[i];
			}
			for (int j = 0; j < 8; j++)
			{
				num += 32;
			}
			for (int k = 156; k < buffer.Length; k++)
			{
				num += (int)buffer[k];
			}
			return num;
		}

		// Token: 0x06000701 RID: 1793 RVA: 0x0002F570 File Offset: 0x0002D770
		private static int GetCTime(DateTime dateTime)
		{
			return (int)((dateTime.Ticks - TarHeader.dateTime1970.Ticks) / 10000000L);
		}

		// Token: 0x06000702 RID: 1794 RVA: 0x0002F5A8 File Offset: 0x0002D7A8
		private static DateTime GetDateTimeFromCTime(long ticks)
		{
			DateTime result;
			try
			{
				result = new DateTime(TarHeader.dateTime1970.Ticks + ticks * 10000000L);
			}
			catch (ArgumentOutOfRangeException)
			{
				result = TarHeader.dateTime1970;
			}
			return result;
		}

		// Token: 0x04000418 RID: 1048
		public const int NAMELEN = 100;

		// Token: 0x04000419 RID: 1049
		public const int MODELEN = 8;

		// Token: 0x0400041A RID: 1050
		public const int UIDLEN = 8;

		// Token: 0x0400041B RID: 1051
		public const int GIDLEN = 8;

		// Token: 0x0400041C RID: 1052
		public const int CHKSUMLEN = 8;

		// Token: 0x0400041D RID: 1053
		public const int CHKSUMOFS = 148;

		// Token: 0x0400041E RID: 1054
		public const int SIZELEN = 12;

		// Token: 0x0400041F RID: 1055
		public const int MAGICLEN = 6;

		// Token: 0x04000420 RID: 1056
		public const int VERSIONLEN = 2;

		// Token: 0x04000421 RID: 1057
		public const int MODTIMELEN = 12;

		// Token: 0x04000422 RID: 1058
		public const int UNAMELEN = 32;

		// Token: 0x04000423 RID: 1059
		public const int GNAMELEN = 32;

		// Token: 0x04000424 RID: 1060
		public const int DEVLEN = 8;

		// Token: 0x04000425 RID: 1061
		public const byte LF_OLDNORM = 0;

		// Token: 0x04000426 RID: 1062
		public const byte LF_NORMAL = 48;

		// Token: 0x04000427 RID: 1063
		public const byte LF_LINK = 49;

		// Token: 0x04000428 RID: 1064
		public const byte LF_SYMLINK = 50;

		// Token: 0x04000429 RID: 1065
		public const byte LF_CHR = 51;

		// Token: 0x0400042A RID: 1066
		public const byte LF_BLK = 52;

		// Token: 0x0400042B RID: 1067
		public const byte LF_DIR = 53;

		// Token: 0x0400042C RID: 1068
		public const byte LF_FIFO = 54;

		// Token: 0x0400042D RID: 1069
		public const byte LF_CONTIG = 55;

		// Token: 0x0400042E RID: 1070
		public const byte LF_GHDR = 103;

		// Token: 0x0400042F RID: 1071
		public const byte LF_XHDR = 120;

		// Token: 0x04000430 RID: 1072
		public const byte LF_ACL = 65;

		// Token: 0x04000431 RID: 1073
		public const byte LF_GNU_DUMPDIR = 68;

		// Token: 0x04000432 RID: 1074
		public const byte LF_EXTATTR = 69;

		// Token: 0x04000433 RID: 1075
		public const byte LF_META = 73;

		// Token: 0x04000434 RID: 1076
		public const byte LF_GNU_LONGLINK = 75;

		// Token: 0x04000435 RID: 1077
		public const byte LF_GNU_LONGNAME = 76;

		// Token: 0x04000436 RID: 1078
		public const byte LF_GNU_MULTIVOL = 77;

		// Token: 0x04000437 RID: 1079
		public const byte LF_GNU_NAMES = 78;

		// Token: 0x04000438 RID: 1080
		public const byte LF_GNU_SPARSE = 83;

		// Token: 0x04000439 RID: 1081
		public const byte LF_GNU_VOLHDR = 86;

		// Token: 0x0400043A RID: 1082
		public const string TMAGIC = "ustar ";

		// Token: 0x0400043B RID: 1083
		public const string GNU_TMAGIC = "ustar  ";

		// Token: 0x0400043C RID: 1084
		private const long timeConversionFactor = 10000000L;

		// Token: 0x0400043D RID: 1085
		private static readonly DateTime dateTime1970 = new DateTime(1970, 1, 1, 0, 0, 0, 0);

		// Token: 0x0400043E RID: 1086
		private string name;

		// Token: 0x0400043F RID: 1087
		private int mode;

		// Token: 0x04000440 RID: 1088
		private int userId;

		// Token: 0x04000441 RID: 1089
		private int groupId;

		// Token: 0x04000442 RID: 1090
		private long size;

		// Token: 0x04000443 RID: 1091
		private DateTime modTime;

		// Token: 0x04000444 RID: 1092
		private int checksum;

		// Token: 0x04000445 RID: 1093
		private bool isChecksumValid;

		// Token: 0x04000446 RID: 1094
		private byte typeFlag;

		// Token: 0x04000447 RID: 1095
		private string linkName;

		// Token: 0x04000448 RID: 1096
		private string magic;

		// Token: 0x04000449 RID: 1097
		private string version;

		// Token: 0x0400044A RID: 1098
		private string userName;

		// Token: 0x0400044B RID: 1099
		private string groupName;

		// Token: 0x0400044C RID: 1100
		private int devMajor;

		// Token: 0x0400044D RID: 1101
		private int devMinor;

		// Token: 0x0400044E RID: 1102
		internal static int userIdAsSet;

		// Token: 0x0400044F RID: 1103
		internal static int groupIdAsSet;

		// Token: 0x04000450 RID: 1104
		internal static string userNameAsSet;

		// Token: 0x04000451 RID: 1105
		internal static string groupNameAsSet = "None";

		// Token: 0x04000452 RID: 1106
		internal static int defaultUserId;

		// Token: 0x04000453 RID: 1107
		internal static int defaultGroupId;

		// Token: 0x04000454 RID: 1108
		internal static string defaultGroupName = "None";

		// Token: 0x04000455 RID: 1109
		internal static string defaultUser;
	}
}
