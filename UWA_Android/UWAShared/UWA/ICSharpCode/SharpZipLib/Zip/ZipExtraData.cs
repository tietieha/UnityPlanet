using System;
using System.IO;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x02000078 RID: 120
	[ComVisible(false)]
	public sealed class ZipExtraData : IDisposable
	{
		// Token: 0x06000567 RID: 1383 RVA: 0x0002B5B0 File Offset: 0x000297B0
		public ZipExtraData()
		{
			this.Clear();
		}

		// Token: 0x06000568 RID: 1384 RVA: 0x0002B5C4 File Offset: 0x000297C4
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

		// Token: 0x06000569 RID: 1385 RVA: 0x0002B604 File Offset: 0x00029804
		public byte[] GetEntryData()
		{
			bool flag = this.Length > 65535;
			if (flag)
			{
				throw new ZipException("Data exceeds maximum length");
			}
			return (byte[])this._data.Clone();
		}

		// Token: 0x0600056A RID: 1386 RVA: 0x0002B650 File Offset: 0x00029850
		public void Clear()
		{
			bool flag = this._data == null || this._data.Length != 0;
			if (flag)
			{
				this._data = new byte[0];
			}
		}

		// Token: 0x170000F1 RID: 241
		// (get) Token: 0x0600056B RID: 1387 RVA: 0x0002B694 File Offset: 0x00029894
		public int Length
		{
			get
			{
				return this._data.Length;
			}
		}

		// Token: 0x0600056C RID: 1388 RVA: 0x0002B6B8 File Offset: 0x000298B8
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

		// Token: 0x0600056D RID: 1389 RVA: 0x0002B700 File Offset: 0x00029900
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

		// Token: 0x0600056E RID: 1390 RVA: 0x0002B748 File Offset: 0x00029948
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

		// Token: 0x170000F2 RID: 242
		// (get) Token: 0x0600056F RID: 1391 RVA: 0x0002B7B4 File Offset: 0x000299B4
		public int ValueLength
		{
			get
			{
				return this._readValueLength;
			}
		}

		// Token: 0x170000F3 RID: 243
		// (get) Token: 0x06000570 RID: 1392 RVA: 0x0002B7D4 File Offset: 0x000299D4
		public int CurrentReadIndex
		{
			get
			{
				return this._index;
			}
		}

		// Token: 0x170000F4 RID: 244
		// (get) Token: 0x06000571 RID: 1393 RVA: 0x0002B7F4 File Offset: 0x000299F4
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

		// Token: 0x06000572 RID: 1394 RVA: 0x0002B858 File Offset: 0x00029A58
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

		// Token: 0x06000573 RID: 1395 RVA: 0x0002B938 File Offset: 0x00029B38
		public void AddEntry(ITaggedData taggedData)
		{
			bool flag = taggedData == null;
			if (flag)
			{
				throw new ArgumentNullException("taggedData");
			}
			this.AddEntry((int)taggedData.TagID, taggedData.GetData());
		}

		// Token: 0x06000574 RID: 1396 RVA: 0x0002B974 File Offset: 0x00029B74
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

		// Token: 0x06000575 RID: 1397 RVA: 0x0002BA84 File Offset: 0x00029C84
		public void StartNewEntry()
		{
			this._newEntry = new MemoryStream();
		}

		// Token: 0x06000576 RID: 1398 RVA: 0x0002BA94 File Offset: 0x00029C94
		public void AddNewEntry(int headerID)
		{
			byte[] fieldData = this._newEntry.ToArray();
			this._newEntry = null;
			this.AddEntry(headerID, fieldData);
		}

		// Token: 0x06000577 RID: 1399 RVA: 0x0002BAC4 File Offset: 0x00029CC4
		public void AddData(byte data)
		{
			this._newEntry.WriteByte(data);
		}

		// Token: 0x06000578 RID: 1400 RVA: 0x0002BAD4 File Offset: 0x00029CD4
		public void AddData(byte[] data)
		{
			bool flag = data == null;
			if (flag)
			{
				throw new ArgumentNullException("data");
			}
			this._newEntry.Write(data, 0, data.Length);
		}

		// Token: 0x06000579 RID: 1401 RVA: 0x0002BB10 File Offset: 0x00029D10
		public void AddLeShort(int toAdd)
		{
			this._newEntry.WriteByte((byte)toAdd);
			this._newEntry.WriteByte((byte)(toAdd >> 8));
		}

		// Token: 0x0600057A RID: 1402 RVA: 0x0002BB34 File Offset: 0x00029D34
		public void AddLeInt(int toAdd)
		{
			this.AddLeShort((int)((short)toAdd));
			this.AddLeShort((int)((short)(toAdd >> 16)));
		}

		// Token: 0x0600057B RID: 1403 RVA: 0x0002BB50 File Offset: 0x00029D50
		public void AddLeLong(long toAdd)
		{
			this.AddLeInt((int)(toAdd & (long)((ulong)-1)));
			this.AddLeInt((int)(toAdd >> 32));
		}

		// Token: 0x0600057C RID: 1404 RVA: 0x0002BB70 File Offset: 0x00029D70
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

		// Token: 0x0600057D RID: 1405 RVA: 0x0002BC00 File Offset: 0x00029E00
		public long ReadLong()
		{
			this.ReadCheck(8);
			return ((long)this.ReadInt() & (long)((ulong)-1)) | (long)this.ReadInt() << 32;
		}

		// Token: 0x0600057E RID: 1406 RVA: 0x0002BC38 File Offset: 0x00029E38
		public int ReadInt()
		{
			this.ReadCheck(4);
			int result = (int)this._data[this._index] + ((int)this._data[this._index + 1] << 8) + ((int)this._data[this._index + 2] << 16) + ((int)this._data[this._index + 3] << 24);
			this._index += 4;
			return result;
		}

		// Token: 0x0600057F RID: 1407 RVA: 0x0002BCB0 File Offset: 0x00029EB0
		public int ReadShort()
		{
			this.ReadCheck(2);
			int result = (int)this._data[this._index] + ((int)this._data[this._index + 1] << 8);
			this._index += 2;
			return result;
		}

		// Token: 0x06000580 RID: 1408 RVA: 0x0002BD00 File Offset: 0x00029F00
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

		// Token: 0x06000581 RID: 1409 RVA: 0x0002BD70 File Offset: 0x00029F70
		public void Skip(int amount)
		{
			this.ReadCheck(amount);
			this._index += amount;
		}

		// Token: 0x06000582 RID: 1410 RVA: 0x0002BD8C File Offset: 0x00029F8C
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

		// Token: 0x06000583 RID: 1411 RVA: 0x0002BE1C File Offset: 0x0002A01C
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

		// Token: 0x06000584 RID: 1412 RVA: 0x0002BE8C File Offset: 0x0002A08C
		private void SetShort(ref int index, int source)
		{
			this._data[index] = (byte)source;
			this._data[index + 1] = (byte)(source >> 8);
			index += 2;
		}

		// Token: 0x06000585 RID: 1413 RVA: 0x0002BEB0 File Offset: 0x0002A0B0
		public void Dispose()
		{
			bool flag = this._newEntry != null;
			if (flag)
			{
				this._newEntry.Close();
			}
		}

		// Token: 0x0400035A RID: 858
		private int _index;

		// Token: 0x0400035B RID: 859
		private int _readValueStart;

		// Token: 0x0400035C RID: 860
		private int _readValueLength;

		// Token: 0x0400035D RID: 861
		private MemoryStream _newEntry;

		// Token: 0x0400035E RID: 862
		private byte[] _data;
	}
}
