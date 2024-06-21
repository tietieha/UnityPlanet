using System;
using System.IO;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Core
{
	// Token: 0x020000C4 RID: 196
	[Obsolete("Use ExtendedPathFilter instead")]
	[ComVisible(false)]
	public class NameAndSizeFilter : PathFilter
	{
		// Token: 0x060008BF RID: 2239 RVA: 0x0004032C File Offset: 0x0003E52C
		public NameAndSizeFilter(string filter, long minSize, long maxSize) : base(filter)
		{
			this.MinSize = minSize;
			this.MaxSize = maxSize;
		}

		// Token: 0x060008C0 RID: 2240 RVA: 0x00040358 File Offset: 0x0003E558
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

		// Token: 0x170001AC RID: 428
		// (get) Token: 0x060008C1 RID: 2241 RVA: 0x000403B4 File Offset: 0x0003E5B4
		// (set) Token: 0x060008C2 RID: 2242 RVA: 0x000403D4 File Offset: 0x0003E5D4
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

		// Token: 0x170001AD RID: 429
		// (get) Token: 0x060008C3 RID: 2243 RVA: 0x00040418 File Offset: 0x0003E618
		// (set) Token: 0x060008C4 RID: 2244 RVA: 0x00040438 File Offset: 0x0003E638
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

		// Token: 0x04000536 RID: 1334
		private long minSize_;

		// Token: 0x04000537 RID: 1335
		private long maxSize_ = long.MaxValue;
	}
}
