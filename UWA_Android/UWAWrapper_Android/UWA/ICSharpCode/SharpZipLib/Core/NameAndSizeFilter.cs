using System;
using System.IO;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Core
{
	// Token: 0x020000B5 RID: 181
	[Obsolete("Use ExtendedPathFilter instead")]
	[ComVisible(false)]
	public class NameAndSizeFilter : PathFilter
	{
		// Token: 0x060007E3 RID: 2019 RVA: 0x00033574 File Offset: 0x00031774
		public NameAndSizeFilter(string filter, long minSize, long maxSize) : base(filter)
		{
			this.MinSize = minSize;
			this.MaxSize = maxSize;
		}

		// Token: 0x060007E4 RID: 2020 RVA: 0x000335A0 File Offset: 0x000317A0
		public override bool IsMatch(string name)
		{
			bool flag = base.IsMatch(name);
			bool flag2 = flag;
			if (flag2)
			{
				FileInfo fileInfo = new FileInfo(name);
				long length = fileInfo.Length;
				flag = (this.MinSize <= length && this.MaxSize >= length);
			}
			return flag;
		}

		// Token: 0x17000158 RID: 344
		// (get) Token: 0x060007E5 RID: 2021 RVA: 0x000335FC File Offset: 0x000317FC
		// (set) Token: 0x060007E6 RID: 2022 RVA: 0x0003361C File Offset: 0x0003181C
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

		// Token: 0x17000159 RID: 345
		// (get) Token: 0x060007E7 RID: 2023 RVA: 0x00033660 File Offset: 0x00031860
		// (set) Token: 0x060007E8 RID: 2024 RVA: 0x00033680 File Offset: 0x00031880
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

		// Token: 0x040004C3 RID: 1219
		private long minSize_;

		// Token: 0x040004C4 RID: 1220
		private long maxSize_ = long.MaxValue;
	}
}
