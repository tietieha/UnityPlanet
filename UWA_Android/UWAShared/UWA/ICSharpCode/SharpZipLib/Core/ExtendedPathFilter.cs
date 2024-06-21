using System;
using System.IO;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Core
{
	// Token: 0x020000C3 RID: 195
	[ComVisible(false)]
	public class ExtendedPathFilter : PathFilter
	{
		// Token: 0x060008B3 RID: 2227 RVA: 0x0004003C File Offset: 0x0003E23C
		public ExtendedPathFilter(string filter, long minSize, long maxSize) : base(filter)
		{
			this.MinSize = minSize;
			this.MaxSize = maxSize;
		}

		// Token: 0x060008B4 RID: 2228 RVA: 0x0004007C File Offset: 0x0003E27C
		public ExtendedPathFilter(string filter, DateTime minDate, DateTime maxDate) : base(filter)
		{
			this.MinDate = minDate;
			this.MaxDate = maxDate;
		}

		// Token: 0x060008B5 RID: 2229 RVA: 0x000400BC File Offset: 0x0003E2BC
		public ExtendedPathFilter(string filter, long minSize, long maxSize, DateTime minDate, DateTime maxDate) : base(filter)
		{
			this.MinSize = minSize;
			this.MaxSize = maxSize;
			this.MinDate = minDate;
			this.MaxDate = maxDate;
		}

		// Token: 0x060008B6 RID: 2230 RVA: 0x00040120 File Offset: 0x0003E320
		public override bool IsMatch(string name)
		{
			bool flag = base.IsMatch(name);
			bool flag2 = flag;
			if (flag2)
			{
				FileInfo fileInfo = new FileInfo(name);
				flag = (this.MinSize <= fileInfo.Length && this.MaxSize >= fileInfo.Length && this.MinDate <= fileInfo.LastWriteTime && this.MaxDate >= fileInfo.LastWriteTime);
			}
			return flag;
		}

		// Token: 0x170001A8 RID: 424
		// (get) Token: 0x060008B7 RID: 2231 RVA: 0x000401A4 File Offset: 0x0003E3A4
		// (set) Token: 0x060008B8 RID: 2232 RVA: 0x000401C4 File Offset: 0x0003E3C4
		public long MinSize
		{
			get
			{
				return this.minSize_;
			}
			set
			{
				bool flag = value < 0L || this.maxSize_ < value;
				if (flag)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.minSize_ = value;
			}
		}

		// Token: 0x170001A9 RID: 425
		// (get) Token: 0x060008B9 RID: 2233 RVA: 0x00040208 File Offset: 0x0003E408
		// (set) Token: 0x060008BA RID: 2234 RVA: 0x00040228 File Offset: 0x0003E428
		public long MaxSize
		{
			get
			{
				return this.maxSize_;
			}
			set
			{
				bool flag = value < 0L || this.minSize_ > value;
				if (flag)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.maxSize_ = value;
			}
		}

		// Token: 0x170001AA RID: 426
		// (get) Token: 0x060008BB RID: 2235 RVA: 0x0004026C File Offset: 0x0003E46C
		// (set) Token: 0x060008BC RID: 2236 RVA: 0x0004028C File Offset: 0x0003E48C
		public DateTime MinDate
		{
			get
			{
				return this.minDate_;
			}
			set
			{
				bool flag = value > this.maxDate_;
				if (flag)
				{
					throw new ArgumentOutOfRangeException("value", "Exceeds MaxDate");
				}
				this.minDate_ = value;
			}
		}

		// Token: 0x170001AB RID: 427
		// (get) Token: 0x060008BD RID: 2237 RVA: 0x000402CC File Offset: 0x0003E4CC
		// (set) Token: 0x060008BE RID: 2238 RVA: 0x000402EC File Offset: 0x0003E4EC
		public DateTime MaxDate
		{
			get
			{
				return this.maxDate_;
			}
			set
			{
				bool flag = this.minDate_ > value;
				if (flag)
				{
					throw new ArgumentOutOfRangeException("value", "Exceeds MinDate");
				}
				this.maxDate_ = value;
			}
		}

		// Token: 0x04000532 RID: 1330
		private long minSize_;

		// Token: 0x04000533 RID: 1331
		private long maxSize_ = long.MaxValue;

		// Token: 0x04000534 RID: 1332
		private DateTime minDate_ = DateTime.MinValue;

		// Token: 0x04000535 RID: 1333
		private DateTime maxDate_ = DateTime.MaxValue;
	}
}
