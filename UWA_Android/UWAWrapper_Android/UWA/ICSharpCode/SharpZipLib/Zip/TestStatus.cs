using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x0200006D RID: 109
	[ComVisible(false)]
	public class TestStatus
	{
		// Token: 0x060004AF RID: 1199 RVA: 0x0001F1A0 File Offset: 0x0001D3A0
		public TestStatus(ZipFile file)
		{
			this.file_ = file;
		}

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x060004B0 RID: 1200 RVA: 0x0001F1B4 File Offset: 0x0001D3B4
		public TestOperation Operation
		{
			get
			{
				return this.operation_;
			}
		}

		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x060004B1 RID: 1201 RVA: 0x0001F1D4 File Offset: 0x0001D3D4
		public ZipFile File
		{
			get
			{
				return this.file_;
			}
		}

		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x060004B2 RID: 1202 RVA: 0x0001F1F4 File Offset: 0x0001D3F4
		public ZipEntry Entry
		{
			get
			{
				return this.entry_;
			}
		}

		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x060004B3 RID: 1203 RVA: 0x0001F214 File Offset: 0x0001D414
		public int ErrorCount
		{
			get
			{
				return this.errorCount_;
			}
		}

		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x060004B4 RID: 1204 RVA: 0x0001F234 File Offset: 0x0001D434
		public long BytesTested
		{
			get
			{
				return this.bytesTested_;
			}
		}

		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x060004B5 RID: 1205 RVA: 0x0001F254 File Offset: 0x0001D454
		public bool EntryValid
		{
			get
			{
				return this.entryValid_;
			}
		}

		// Token: 0x060004B6 RID: 1206 RVA: 0x0001F274 File Offset: 0x0001D474
		internal void AddError()
		{
			this.errorCount_++;
			this.entryValid_ = false;
		}

		// Token: 0x060004B7 RID: 1207 RVA: 0x0001F28C File Offset: 0x0001D48C
		internal void SetOperation(TestOperation operation)
		{
			this.operation_ = operation;
		}

		// Token: 0x060004B8 RID: 1208 RVA: 0x0001F298 File Offset: 0x0001D498
		internal void SetEntry(ZipEntry entry)
		{
			this.entry_ = entry;
			this.entryValid_ = true;
			this.bytesTested_ = 0L;
		}

		// Token: 0x060004B9 RID: 1209 RVA: 0x0001F2B4 File Offset: 0x0001D4B4
		internal void SetBytesTested(long value)
		{
			this.bytesTested_ = value;
		}

		// Token: 0x040002F8 RID: 760
		private ZipFile file_;

		// Token: 0x040002F9 RID: 761
		private ZipEntry entry_;

		// Token: 0x040002FA RID: 762
		private bool entryValid_;

		// Token: 0x040002FB RID: 763
		private int errorCount_;

		// Token: 0x040002FC RID: 764
		private long bytesTested_;

		// Token: 0x040002FD RID: 765
		private TestOperation operation_;
	}
}
