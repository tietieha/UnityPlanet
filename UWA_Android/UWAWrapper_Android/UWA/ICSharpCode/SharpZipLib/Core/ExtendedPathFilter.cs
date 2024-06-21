using System;
using System.IO;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Core
{
	// Token: 0x020000B4 RID: 180
	[ComVisible(false)]
	public class ExtendedPathFilter : PathFilter
	{
		// Token: 0x060007D7 RID: 2007 RVA: 0x00033284 File Offset: 0x00031484
		public ExtendedPathFilter(string filter, long minSize, long maxSize) : base(filter)
		{
			this.MinSize = minSize;
			this.MaxSize = maxSize;
		}

		// Token: 0x060007D8 RID: 2008 RVA: 0x000332C4 File Offset: 0x000314C4
		public ExtendedPathFilter(string filter, DateTime minDate, DateTime maxDate) : base(filter)
		{
			this.MinDate = minDate;
			this.MaxDate = maxDate;
		}

		// Token: 0x060007D9 RID: 2009 RVA: 0x00033304 File Offset: 0x00031504
		public ExtendedPathFilter(string filter, long minSize, long maxSize, DateTime minDate, DateTime maxDate) : base(filter)
		{
			this.MinSize = minSize;
			this.MaxSize = maxSize;
			this.MinDate = minDate;
			this.MaxDate = maxDate;
		}

		// Token: 0x060007DA RID: 2010 RVA: 0x00033368 File Offset: 0x00031568
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

		// Token: 0x17000154 RID: 340
		// (get) Token: 0x060007DB RID: 2011 RVA: 0x000333EC File Offset: 0x000315EC
		// (set) Token: 0x060007DC RID: 2012 RVA: 0x0003340C File Offset: 0x0003160C
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

		// Token: 0x17000155 RID: 341
		// (get) Token: 0x060007DD RID: 2013 RVA: 0x00033450 File Offset: 0x00031650
		// (set) Token: 0x060007DE RID: 2014 RVA: 0x00033470 File Offset: 0x00031670
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

		// Token: 0x17000156 RID: 342
		// (get) Token: 0x060007DF RID: 2015 RVA: 0x000334B4 File Offset: 0x000316B4
		// (set) Token: 0x060007E0 RID: 2016 RVA: 0x000334D4 File Offset: 0x000316D4
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

		// Token: 0x17000157 RID: 343
		// (get) Token: 0x060007E1 RID: 2017 RVA: 0x00033514 File Offset: 0x00031714
		// (set) Token: 0x060007E2 RID: 2018 RVA: 0x00033534 File Offset: 0x00031734
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

		// Token: 0x040004BF RID: 1215
		private long minSize_;

		// Token: 0x040004C0 RID: 1216
		private long maxSize_ = long.MaxValue;

		// Token: 0x040004C1 RID: 1217
		private DateTime minDate_ = DateTime.MinValue;

		// Token: 0x040004C2 RID: 1218
		private DateTime maxDate_ = DateTime.MaxValue;
	}
}
