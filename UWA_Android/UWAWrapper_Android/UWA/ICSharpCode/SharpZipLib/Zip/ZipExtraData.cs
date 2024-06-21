using System;
using System.IO;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x02000069 RID: 105
	[ComVisible(false)]
	public sealed class ZipExtraData : IDisposable
	{
		// Token: 0x0600048B RID: 1163 RVA: 0x0001E7F8 File Offset: 0x0001C9F8
		public ZipExtraData()
		{
			this.Clear();
		}

		// Token: 0x0600048C RID: 1164 RVA: 0x0001E80C File Offset: 0x0001CA0C
		public ZipExtraData(byte[] data)
		{
			bool flag = data == null;
			if (flag)
			{
				this._data = new byte[0];
			}
			else
			{
				this._data = data;
			}
		}

		// Token: 0x0600048D RID: 1165 RVA: 0x0001E84C File Offset: 0x0001CA4C
		public byte[] GetEntryData()
		{
			bool flag = this.Length > 65535;
			if (flag)
			{
				throw new ZipException("Data exceeds maximum length");
			}
			return (byte[])this._data.Clone();
		}

		// Token: 0x0600048E RID: 1166 RVA: 0x0001E898 File Offset: 0x0001CA98
		public void Clear()
		{
			bool flag = this._data == null || this._data.Length != 0;
			if (flag)
			{
				this._data = new byte[0];
			}
		}

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x0600048F RID: 1167 RVA: 0x0001E8DC File Offset: 0x0001CADC
		public int Length
		{
			get
			{
				return this._data.Length;
			}
		}

		// Token: 0x06000490 RID: 1168 RVA: 0x0001E900 File Offset: 0x0001CB00
		public Stream GetStreamForTag(int tag)
		{
			Stream result = null;
			bool flag = this.Find(tag);
			if (flag)
			{
				result = new MemoryStream(this._data, this._index, this._readValueLength, false);
			}
			return result;
		}

		// Token: 0x06000491 RID: 1169 RVA: 0x0001E948 File Offset: 0x0001CB48
		private ITaggedData GetData(short tag)
		{
			ITaggedData result = null;
			bool flag = this.Find((int)tag);
			if (flag)
			{
				result = ZipExtraData.Create(tag, this._data, this._readValueStart, this._readValueLength);
			}
			return result;
		}

		// Token: 0x06000492 RID: 1170 RVA: 0x0001E990 File Offset: 0x0001CB90
		private static ITaggedData Create(short tag, byte[] data, int offset, int count)
		{
			ITaggedData taggedData;
			if (tag != 10)
			{
				if (tag != 21589)
				{
					taggedData = new RawTaggedData(tag);
				}
				else
				{
					taggedData = new ExtendedUnixData();
				}
			}
			else
			{
				taggedData = new NTTaggedData();
			}
			taggedData.SetData(data, offset, count);
			return taggedData;
		}

		// Token: 0x1700009E RID: 158
		// (get) Token: 0x06000493 RID: 1171 RVA: 0x0001E9FC File Offset: 0x0001CBFC
		public int ValueLength
		{
			get
			{
				return this._readValueLength;
			}
		}

		// Token: 0x1700009F RID: 159
		// (get) Token: 0x06000494 RID: 1172 RVA: 0x0001EA1C File Offset: 0x0001CC1C
		public int CurrentReadIndex
		{
			get
			{
				return this._index;
			}
		}

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x06000495 RID: 1173 RVA: 0x0001EA3C File Offset: 0x0001CC3C
		public int UnreadCount
		{
			get
			{
				bool flag = this._readValueStart > this._data.Length || this._readValueStart < 4;
				if (flag)
				{
					throw new ZipException("Find must be called before calling a Read method");
				}
				return this._readValueStart + this._readValueLength - this._index;
			}
		}

		// Token: 0x06000496 RID: 1174 RVA: 0x0001EAA0 File Offset: 0x0001CCA0
		public bool Find(int headerID)
		{
			this._readValueStart = this._data.Length;
			this._readValueLength = 0;
			this._index = 0;
			int num = this._readValueStart;
			int num2 = headerID - 1;
			while (num2 != headerID && this._index < this._data.Length - 3)
			{
				num2 = this.ReadShortInternal();
				num = this.ReadShortInternal();
				bool flag = num2 != headerID;
				if (flag)
				{
					this._index += num;
				}
			}
			bool flag2 = num2 == headerID && this._index + num <= this._data.Length;
			bool flag3 = flag2;
			if (flag3)
			{
				this._readValueStart = this._index;
				this._readValueLength = num;
			}
			return flag2;
		}

		// Token: 0x06000497 RID: 1175 RVA: 0x0001EB80 File Offset: 0x0001CD80
		public void AddEntry(ITaggedData taggedData)
		{
			bool flag = taggedData == null;
			if (flag)
			{
				throw new ArgumentNullException("taggedData");
			}
			this.AddEntry((int)taggedData.TagID, taggedData.GetData());
		}

		// Token: 0x06000498 RID: 1176 RVA: 0x0001EBBC File Offset: 0x0001CDBC
		public void AddEntry(int headerID, byte[] fieldData)
		{
			bool flag = headerID > 65535 || headerID < 0;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("headerID");
			}
			int num = (fieldData == null) ? 0 : fieldData.Length;
			bool flag2 = num > 65535;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("fieldData", "exceeds maximum length");
			}
			int num2 = this._data.Length + num + 4;
			bool flag3 = this.Find(headerID);
			if (flag3)
			{
				num2 -= this.ValueLength + 4;
			}
			bool flag4 = num2 > 65535;
			if (flag4)
			{
				throw new ZipException("Data exceeds maximum length");
			}
			this.Delete(headerID);
			byte[] array = new byte[num2];
			this._data.CopyTo(array, 0);
			int index = this._data.Length;
			this._data = array;
			this.SetShort(ref index, headerID);
			this.SetShort(ref index, num);
			bool flag5 = fieldData != null;
			if (flag5)
			{
				fieldData.CopyTo(array, index);
			}
		}

		// Token: 0x06000499 RID: 1177 RVA: 0x0001ECCC File Offset: 0x0001CECC
		public void StartNewEntry()
		{
			this._newEntry = new MemoryStream();
		}

		// Token: 0x0600049A RID: 1178 RVA: 0x0001ECDC File Offset: 0x0001CEDC
		public void AddNewEntry(int headerID)
		{
			byte[] fieldData = this._newEntry.ToArray();
			this._newEntry = null;
			this.AddEntry(headerID, fieldData);
		}

		// Token: 0x0600049B RID: 1179 RVA: 0x0001ED0C File Offset: 0x0001CF0C
		public void AddData(byte data)
		{
			this._newEntry.WriteByte(data);
		}

		// Token: 0x0600049C RID: 1180 RVA: 0x0001ED1C File Offset: 0x0001CF1C
		public void AddData(byte[] data)
		{
			bool flag = data == null;
			if (flag)
			{
				throw new ArgumentNullException("data");
			}
			this._newEntry.Write(data, 0, data.Length);
		}

		// Token: 0x0600049D RID: 1181 RVA: 0x0001ED58 File Offset: 0x0001CF58
		public void AddLeShort(int toAdd)
		{
			this._newEntry.WriteByte((byte)toAdd);
			this._newEntry.WriteByte((byte)(toAdd >> 8));
		}

		// Token: 0x0600049E RID: 1182 RVA: 0x0001ED7C File Offset: 0x0001CF7C
		public void AddLeInt(int toAdd)
		{
			this.AddLeShort((int)((short)toAdd));
			this.AddLeShort((int)((short)(toAdd >> 16)));
		}

		// Token: 0x0600049F RID: 1183 RVA: 0x0001ED98 File Offset: 0x0001CF98
		public void AddLeLong(long toAdd)
		{
			this.AddLeInt((int)(toAdd & (long)((ulong)-1)));
			this.AddLeInt((int)(toAdd >> 32));
		}

		// Token: 0x060004A0 RID: 1184 RVA: 0x0001EDB8 File Offset: 0x0001CFB8
		public bool Delete(int headerID)
		{
			bool result = false;
			bool flag = this.Find(headerID);
			if (flag)
			{
				result = true;
				int num = this._readValueStart - 4;
				byte[] array = new byte[this._data.Length - (this.ValueLength + 4)];
				Array.Copy(this._data, 0, array, 0, num);
				int num2 = num + this.ValueLength + 4;
				Array.Copy(this._data, num2, array, num, this._data.Length - num2);
				this._data = array;
			}
			return result;
		}

		// Token: 0x060004A1 RID: 1185 RVA: 0x0001EE48 File Offset: 0x0001D048
		public long ReadLong()
		{
			this.ReadCheck(8);
			return ((long)this.ReadInt() & (long)((ulong)-1)) | (long)this.ReadInt() << 32;
		}

		// Token: 0x060004A2 RID: 1186 RVA: 0x0001EE80 File Offset: 0x0001D080
		public int ReadInt()
		{
			this.ReadCheck(4);
			int result = (int)this._data[this._index] + ((int)this._data[this._index + 1] << 8) + ((int)this._data[this._index + 2] << 16) + ((int)this._data[this._index + 3] << 24);
			this._index += 4;
			return result;
		}

		// Token: 0x060004A3 RID: 1187 RVA: 0x0001EEF8 File Offset: 0x0001D0F8
		public int ReadShort()
		{
			this.ReadCheck(2);
			int result = (int)this._data[this._index] + ((int)this._data[this._index + 1] << 8);
			this._index += 2;
			return result;
		}

		// Token: 0x060004A4 RID: 1188 RVA: 0x0001EF48 File Offset: 0x0001D148
		public int ReadByte()
		{
			int result = -1;
			bool flag = this._index < this._data.Length && this._readValueStart + this._readValueLength > this._index;
			if (flag)
			{
				result = (int)this._data[this._index];
				this._index++;
			}
			return result;
		}

		// Token: 0x060004A5 RID: 1189 RVA: 0x0001EFB8 File Offset: 0x0001D1B8
		public void Skip(int amount)
		{
			this.ReadCheck(amount);
			this._index += amount;
		}

		// Token: 0x060004A6 RID: 1190 RVA: 0x0001EFD4 File Offset: 0x0001D1D4
		private void ReadCheck(int length)
		{
			bool flag = this._readValueStart > this._data.Length || this._readValueStart < 4;
			if (flag)
			{
				throw new ZipException("Find must be called before calling a Read method");
			}
			bool flag2 = this._index > this._readValueStart + this._readValueLength - length;
			if (flag2)
			{
				throw new ZipException("End of extra data");
			}
			bool flag3 = this._index + length < 4;
			if (flag3)
			{
				throw new ZipException("Cannot read before start of tag");
			}
		}

		// Token: 0x060004A7 RID: 1191 RVA: 0x0001F064 File Offset: 0x0001D264
		private int ReadShortInternal()
		{
			bool flag = this._index > this._data.Length - 2;
			if (flag)
			{
				throw new ZipException("End of extra data");
			}
			int result = (int)this._data[this._index] + ((int)this._data[this._index + 1] << 8);
			this._index += 2;
			return result;
		}

		// Token: 0x060004A8 RID: 1192 RVA: 0x0001F0D4 File Offset: 0x0001D2D4
		private void SetShort(ref int index, int source)
		{
			this._data[index] = (byte)source;
			this._data[index + 1] = (byte)(source >> 8);
			index += 2;
		}

		// Token: 0x060004A9 RID: 1193 RVA: 0x0001F0F8 File Offset: 0x0001D2F8
		public void Dispose()
		{
			bool flag = this._newEntry != null;
			if (flag)
			{
				this._newEntry.Close();
			}
		}

		// Token: 0x040002E7 RID: 743
		private int _index;

		// Token: 0x040002E8 RID: 744
		private int _readValueStart;

		// Token: 0x040002E9 RID: 745
		private int _readValueLength;

		// Token: 0x040002EA RID: 746
		private MemoryStream _newEntry;

		// Token: 0x040002EB RID: 747
		private byte[] _data;
	}
}
