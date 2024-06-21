using System;
using System.Runtime.InteropServices;
using System.Text;

namespace UWA.ICSharpCode.SharpZipLib.Tar
{
	// Token: 0x020000A3 RID: 163
	[ComVisible(false)]
	public class TarHeader : ICloneable
	{
		// Token: 0x060007AA RID: 1962 RVA: 0x0003B38C File Offset: 0x0003958C
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

		// Token: 0x17000165 RID: 357
		// (get) Token: 0x060007AB RID: 1963 RVA: 0x0003B410 File Offset: 0x00039610
		// (set) Token: 0x060007AC RID: 1964 RVA: 0x0003B430 File Offset: 0x00039630
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

		// Token: 0x060007AD RID: 1965 RVA: 0x0003B460 File Offset: 0x00039660
		[Obsolete("Use the Name property instead", true)]
		public string GetName()
		{
			return this.name;
		}

		// Token: 0x17000166 RID: 358
		// (get) Token: 0x060007AE RID: 1966 RVA: 0x0003B480 File Offset: 0x00039680
		// (set) Token: 0x060007AF RID: 1967 RVA: 0x0003B4A0 File Offset: 0x000396A0
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

		// Token: 0x17000167 RID: 359
		// (get) Token: 0x060007B0 RID: 1968 RVA: 0x0003B4AC File Offset: 0x000396AC
		// (set) Token: 0x060007B1 RID: 1969 RVA: 0x0003B4CC File Offset: 0x000396CC
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

		// Token: 0x17000168 RID: 360
		// (get) Token: 0x060007B2 RID: 1970 RVA: 0x0003B4D8 File Offset: 0x000396D8
		// (set) Token: 0x060007B3 RID: 1971 RVA: 0x0003B4F8 File Offset: 0x000396F8
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

		// Token: 0x17000169 RID: 361
		// (get) Token: 0x060007B4 RID: 1972 RVA: 0x0003B504 File Offset: 0x00039704
		// (set) Token: 0x060007B5 RID: 1973 RVA: 0x0003B524 File Offset: 0x00039724
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

		// Token: 0x1700016A RID: 362
		// (get) Token: 0x060007B6 RID: 1974 RVA: 0x0003B55C File Offset: 0x0003975C
		// (set) Token: 0x060007B7 RID: 1975 RVA: 0x0003B57C File Offset: 0x0003977C
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

		// Token: 0x1700016B RID: 363
		// (get) Token: 0x060007B8 RID: 1976 RVA: 0x0003B5E8 File Offset: 0x000397E8
		public int Checksum
		{
			get
			{
				return this.checksum;
			}
		}

		// Token: 0x1700016C RID: 364
		// (get) Token: 0x060007B9 RID: 1977 RVA: 0x0003B608 File Offset: 0x00039808
		public bool IsChecksumValid
		{
			get
			{
				return this.isChecksumValid;
			}
		}

		// Token: 0x1700016D RID: 365
		// (get) Token: 0x060007BA RID: 1978 RVA: 0x0003B628 File Offset: 0x00039828
		// (set) Token: 0x060007BB RID: 1979 RVA: 0x0003B648 File Offset: 0x00039848
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

		// Token: 0x1700016E RID: 366
		// (get) Token: 0x060007BC RID: 1980 RVA: 0x0003B654 File Offset: 0x00039854
		// (set) Token: 0x060007BD RID: 1981 RVA: 0x0003B674 File Offset: 0x00039874
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

		// Token: 0x1700016F RID: 367
		// (get) Token: 0x060007BE RID: 1982 RVA: 0x0003B6A4 File Offset: 0x000398A4
		// (set) Token: 0x060007BF RID: 1983 RVA: 0x0003B6C4 File Offset: 0x000398C4
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

		// Token: 0x17000170 RID: 368
		// (get) Token: 0x060007C0 RID: 1984 RVA: 0x0003B6F4 File Offset: 0x000398F4
		// (set) Token: 0x060007C1 RID: 1985 RVA: 0x0003B714 File Offset: 0x00039914
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

		// Token: 0x17000171 RID: 369
		// (get) Token: 0x060007C2 RID: 1986 RVA: 0x0003B744 File Offset: 0x00039944
		// (set) Token: 0x060007C3 RID: 1987 RVA: 0x0003B764 File Offset: 0x00039964
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

		// Token: 0x17000172 RID: 370
		// (get) Token: 0x060007C4 RID: 1988 RVA: 0x0003B7D0 File Offset: 0x000399D0
		// (set) Token: 0x060007C5 RID: 1989 RVA: 0x0003B7F0 File Offset: 0x000399F0
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

		// Token: 0x17000173 RID: 371
		// (get) Token: 0x060007C6 RID: 1990 RVA: 0x0003B828 File Offset: 0x00039A28
		// (set) Token: 0x060007C7 RID: 1991 RVA: 0x0003B848 File Offset: 0x00039A48
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

		// Token: 0x17000174 RID: 372
		// (get) Token: 0x060007C8 RID: 1992 RVA: 0x0003B854 File Offset: 0x00039A54
		// (set) Token: 0x060007C9 RID: 1993 RVA: 0x0003B874 File Offset: 0x00039A74
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

		// Token: 0x060007CA RID: 1994 RVA: 0x0003B880 File Offset: 0x00039A80
		public object Clone()
		{
			return base.MemberwiseClone();
		}

		// Token: 0x060007CB RID: 1995 RVA: 0x0003B8A0 File Offset: 0x00039AA0
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

		// Token: 0x060007CC RID: 1996 RVA: 0x0003BA28 File Offset: 0x00039C28
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

		// Token: 0x060007CD RID: 1997 RVA: 0x0003BBC0 File Offset: 0x00039DC0
		public override int GetHashCode()
		{
			return this.Name.GetHashCode();
		}

		// Token: 0x060007CE RID: 1998 RVA: 0x0003BBE4 File Offset: 0x00039DE4
		public override bool Equals(object obj)
		{
			TarHeader tarHeader = obj as TarHeader;
			bool flag = tarHeader != null;
			return flag && (this.name == tarHeader.name && this.mode == tarHeader.mode && this.UserId == tarHeader.UserId && this.GroupId == tarHeader.GroupId && this.Size == tarHeader.Size && this.ModTime == tarHeader.ModTime && this.Checksum == tarHeader.Checksum && this.TypeFlag == tarHeader.TypeFlag && this.LinkName == tarHeader.LinkName && this.Magic == tarHeader.Magic && this.Version == tarHeader.Version && this.UserName == tarHeader.UserName && this.GroupName == tarHeader.GroupName && this.DevMajor == tarHeader.DevMajor) && this.DevMinor == tarHeader.DevMinor;
		}

		// Token: 0x060007CF RID: 1999 RVA: 0x0003BD44 File Offset: 0x00039F44
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

		// Token: 0x060007D0 RID: 2000 RVA: 0x0003BD78 File Offset: 0x00039F78
		internal static void RestoreSetValues()
		{
			TarHeader.defaultUserId = TarHeader.userIdAsSet;
			TarHeader.defaultUser = TarHeader.userNameAsSet;
			TarHeader.defaultGroupId = TarHeader.groupIdAsSet;
			TarHeader.defaultGroupName = TarHeader.groupNameAsSet;
		}

		// Token: 0x060007D1 RID: 2001 RVA: 0x0003BDA4 File Offset: 0x00039FA4
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

		// Token: 0x060007D2 RID: 2002 RVA: 0x0003BE70 File Offset: 0x0003A070
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

		// Token: 0x060007D3 RID: 2003 RVA: 0x0003BF44 File Offset: 0x0003A144
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

		// Token: 0x060007D4 RID: 2004 RVA: 0x0003BF9C File Offset: 0x0003A19C
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

		// Token: 0x060007D5 RID: 2005 RVA: 0x0003C040 File Offset: 0x0003A240
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

		// Token: 0x060007D6 RID: 2006 RVA: 0x0003C098 File Offset: 0x0003A298
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

		// Token: 0x060007D7 RID: 2007 RVA: 0x0003C0EC File Offset: 0x0003A2EC
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

		// Token: 0x060007D8 RID: 2008 RVA: 0x0003C174 File Offset: 0x0003A374
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

		// Token: 0x060007D9 RID: 2009 RVA: 0x0003C21C File Offset: 0x0003A41C
		public static int GetLongOctalBytes(long value, byte[] buffer, int offset, int length)
		{
			return TarHeader.GetOctalBytes(value, buffer, offset, length);
		}

		// Token: 0x060007DA RID: 2010 RVA: 0x0003C240 File Offset: 0x0003A440
		private static int GetCheckSumOctalBytes(long value, byte[] buffer, int offset, int length)
		{
			TarHeader.GetOctalBytes(value, buffer, offset, length - 1);
			return offset + length;
		}

		// Token: 0x060007DB RID: 2011 RVA: 0x0003C268 File Offset: 0x0003A468
		private static int ComputeCheckSum(byte[] buffer)
		{
			int num = 0;
			for (int i = 0; i < buffer.Length; i++)
			{
				num += (int)buffer[i];
			}
			return num;
		}

		// Token: 0x060007DC RID: 2012 RVA: 0x0003C2A4 File Offset: 0x0003A4A4
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

		// Token: 0x060007DD RID: 2013 RVA: 0x0003C328 File Offset: 0x0003A528
		private static int GetCTime(DateTime dateTime)
		{
			return (int)((dateTime.Ticks - TarHeader.dateTime1970.Ticks) / 10000000L);
		}

		// Token: 0x060007DE RID: 2014 RVA: 0x0003C360 File Offset: 0x0003A560
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

		// Token: 0x0400048B RID: 1163
		public const int NAMELEN = 100;

		// Token: 0x0400048C RID: 1164
		public const int MODELEN = 8;

		// Token: 0x0400048D RID: 1165
		public const int UIDLEN = 8;

		// Token: 0x0400048E RID: 1166
		public const int GIDLEN = 8;

		// Token: 0x0400048F RID: 1167
		public const int CHKSUMLEN = 8;

		// Token: 0x04000490 RID: 1168
		public const int CHKSUMOFS = 148;

		// Token: 0x04000491 RID: 1169
		public const int SIZELEN = 12;

		// Token: 0x04000492 RID: 1170
		public const int MAGICLEN = 6;

		// Token: 0x04000493 RID: 1171
		public const int VERSIONLEN = 2;

		// Token: 0x04000494 RID: 1172
		public const int MODTIMELEN = 12;

		// Token: 0x04000495 RID: 1173
		public const int UNAMELEN = 32;

		// Token: 0x04000496 RID: 1174
		public const int GNAMELEN = 32;

		// Token: 0x04000497 RID: 1175
		public const int DEVLEN = 8;

		// Token: 0x04000498 RID: 1176
		public const byte LF_OLDNORM = 0;

		// Token: 0x04000499 RID: 1177
		public const byte LF_NORMAL = 48;

		// Token: 0x0400049A RID: 1178
		public const byte LF_LINK = 49;

		// Token: 0x0400049B RID: 1179
		public const byte LF_SYMLINK = 50;

		// Token: 0x0400049C RID: 1180
		public const byte LF_CHR = 51;

		// Token: 0x0400049D RID: 1181
		public const byte LF_BLK = 52;

		// Token: 0x0400049E RID: 1182
		public const byte LF_DIR = 53;

		// Token: 0x0400049F RID: 1183
		public const byte LF_FIFO = 54;

		// Token: 0x040004A0 RID: 1184
		public const byte LF_CONTIG = 55;

		// Token: 0x040004A1 RID: 1185
		public const byte LF_GHDR = 103;

		// Token: 0x040004A2 RID: 1186
		public const byte LF_XHDR = 120;

		// Token: 0x040004A3 RID: 1187
		public const byte LF_ACL = 65;

		// Token: 0x040004A4 RID: 1188
		public const byte LF_GNU_DUMPDIR = 68;

		// Token: 0x040004A5 RID: 1189
		public const byte LF_EXTATTR = 69;

		// Token: 0x040004A6 RID: 1190
		public const byte LF_META = 73;

		// Token: 0x040004A7 RID: 1191
		public const byte LF_GNU_LONGLINK = 75;

		// Token: 0x040004A8 RID: 1192
		public const byte LF_GNU_LONGNAME = 76;

		// Token: 0x040004A9 RID: 1193
		public const byte LF_GNU_MULTIVOL = 77;

		// Token: 0x040004AA RID: 1194
		public const byte LF_GNU_NAMES = 78;

		// Token: 0x040004AB RID: 1195
		public const byte LF_GNU_SPARSE = 83;

		// Token: 0x040004AC RID: 1196
		public const byte LF_GNU_VOLHDR = 86;

		// Token: 0x040004AD RID: 1197
		public const string TMAGIC = "ustar ";

		// Token: 0x040004AE RID: 1198
		public const string GNU_TMAGIC = "ustar  ";

		// Token: 0x040004AF RID: 1199
		private const long timeConversionFactor = 10000000L;

		// Token: 0x040004B0 RID: 1200
		private static readonly DateTime dateTime1970 = new DateTime(1970, 1, 1, 0, 0, 0, 0);

		// Token: 0x040004B1 RID: 1201
		private string name;

		// Token: 0x040004B2 RID: 1202
		private int mode;

		// Token: 0x040004B3 RID: 1203
		private int userId;

		// Token: 0x040004B4 RID: 1204
		private int groupId;

		// Token: 0x040004B5 RID: 1205
		private long size;

		// Token: 0x040004B6 RID: 1206
		private DateTime modTime;

		// Token: 0x040004B7 RID: 1207
		private int checksum;

		// Token: 0x040004B8 RID: 1208
		private bool isChecksumValid;

		// Token: 0x040004B9 RID: 1209
		private byte typeFlag;

		// Token: 0x040004BA RID: 1210
		private string linkName;

		// Token: 0x040004BB RID: 1211
		private string magic;

		// Token: 0x040004BC RID: 1212
		private string version;

		// Token: 0x040004BD RID: 1213
		private string userName;

		// Token: 0x040004BE RID: 1214
		private string groupName;

		// Token: 0x040004BF RID: 1215
		private int devMajor;

		// Token: 0x040004C0 RID: 1216
		private int devMinor;

		// Token: 0x040004C1 RID: 1217
		internal static int userIdAsSet;

		// Token: 0x040004C2 RID: 1218
		internal static int groupIdAsSet;

		// Token: 0x040004C3 RID: 1219
		internal static string userNameAsSet;

		// Token: 0x040004C4 RID: 1220
		internal static string groupNameAsSet = "None";

		// Token: 0x040004C5 RID: 1221
		internal static int defaultUserId;

		// Token: 0x040004C6 RID: 1222
		internal static int defaultGroupId;

		// Token: 0x040004C7 RID: 1223
		internal static string defaultGroupName = "None";

		// Token: 0x040004C8 RID: 1224
		internal static string defaultUser;
	}
}
