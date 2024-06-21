using System;
using UnityEngine.Profiling;

namespace UWALocal
{
	// Token: 0x02000023 RID: 35
	internal class UnityRecord : BaseRecord
	{
		// Token: 0x060001D7 RID: 471 RVA: 0x0000B928 File Offset: 0x00009B28
		public UnityRecord(Recorder r)
		{
			this._r = r;
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x060001D8 RID: 472 RVA: 0x0000B938 File Offset: 0x00009B38
		public bool isValid
		{
			get
			{
				return this._r.isValid;
			}
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x060001D9 RID: 473 RVA: 0x0000B948 File Offset: 0x00009B48
		public int sampleBlockCountX
		{
			get
			{
				return this._r.sampleBlockCountX;
			}
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x060001DA RID: 474 RVA: 0x0000B958 File Offset: 0x00009B58
		public long elapsedNanosecondsX
		{
			get
			{
				return this._r.elapsedNanosecondsX;
			}
		}

		// Token: 0x040000D8 RID: 216
		private Recorder _r;
	}
}
