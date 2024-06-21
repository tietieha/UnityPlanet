using System;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x0200007C RID: 124
	[ComVisible(false)]
	public class TestStatus
	{
		// Token: 0x0600058B RID: 1419 RVA: 0x0002BF58 File Offset: 0x0002A158
		public TestStatus(ZipFile file)
		{
			this.file_ = file;
		}

		// Token: 0x170000F7 RID: 247
		// (get) Token: 0x0600058C RID: 1420 RVA: 0x0002BF6C File Offset: 0x0002A16C
		public TestOperation Operation
		{
			get
			{
				return this.operation_;
			}
		}

		// Token: 0x170000F8 RID: 248
		// (get) Token: 0x0600058D RID: 1421 RVA: 0x0002BF8C File Offset: 0x0002A18C
		public ZipFile File
		{
			get
			{
				return this.file_;
			}
		}

		// Token: 0x170000F9 RID: 249
		// (get) Token: 0x0600058E RID: 1422 RVA: 0x0002BFAC File Offset: 0x0002A1AC
		public ZipEntry Entry
		{
			get
			{
				return this.entry_;
			}
		}

		// Token: 0x170000FA RID: 250
		// (get) Token: 0x0600058F RID: 1423 RVA: 0x0002BFCC File Offset: 0x0002A1CC
		public int ErrorCount
		{
			get
			{
				return this.errorCount_;
			}
		}

		// Token: 0x170000FB RID: 251
		// (get) Token: 0x06000590 RID: 1424 RVA: 0x0002BFEC File Offset: 0x0002A1EC
		public long BytesTested
		{
			get
			{
				return this.bytesTested_;
			}
		}

		// Token: 0x170000FC RID: 252
		// (get) Token: 0x06000591 RID: 1425 RVA: 0x0002C00C File Offset: 0x0002A20C
		public bool EntryValid
		{
			get
			{
				return this.entryValid_;
			}
		}

		// Token: 0x06000592 RID: 1426 RVA: 0x0002C02C File Offset: 0x0002A22C
		internal void AddError()
		{
			this.errorCount_++;
			this.entryValid_ = false;
		}

		// Token: 0x06000593 RID: 1427 RVA: 0x0002C044 File Offset: 0x0002A244
		internal void SetOperation(TestOperation operation)
		{
			this.operation_ = operation;
		}

		// Token: 0x06000594 RID: 1428 RVA: 0x0002C050 File Offset: 0x0002A250
		internal void SetEntry(ZipEntry entry)
		{
			this.entry_ = entry;
			this.entryValid_ = true;
			this.bytesTested_ = 0L;
		}

		// Token: 0x06000595 RID: 1429 RVA: 0x0002C06C File Offset: 0x0002A26C
		internal void SetBytesTested(long value)
		{
			this.bytesTested_ = value;
		}

		// Token: 0x0400036B RID: 875
		private ZipFile file_;

		// Token: 0x0400036C RID: 876
		private ZipEntry entry_;

		// Token: 0x0400036D RID: 877
		private bool entryValid_;

		// Token: 0x0400036E RID: 878
		private int errorCount_;

		// Token: 0x0400036F RID: 879
		private long bytesTested_;

		// Token: 0x04000370 RID: 880
		private TestOperation operation_;
	}
}
