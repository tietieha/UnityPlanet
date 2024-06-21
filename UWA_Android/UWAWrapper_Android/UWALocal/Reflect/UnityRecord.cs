using System;

namespace UWALocal.Reflect
{
	// Token: 0x02000029 RID: 41
	internal class UnityRecord : BaseRecord
	{
		// Token: 0x060001F7 RID: 503 RVA: 0x0000C810 File Offset: 0x0000AA10
		public UnityRecord(Recorder r)
		{
			this._r = r;
		}

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x060001F8 RID: 504 RVA: 0x0000C820 File Offset: 0x0000AA20
		public bool isValid
		{
			get
			{
				return this._r.isValid;
			}
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x060001F9 RID: 505 RVA: 0x0000C830 File Offset: 0x0000AA30
		public int sampleBlockCountX
		{
			get
			{
				return this._r.sampleBlockCountX;
			}
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x060001FA RID: 506 RVA: 0x0000C840 File Offset: 0x0000AA40
		public long elapsedNanosecondsX
		{
			get
			{
				return this._r.elapsedNanosecondsX;
			}
		}

		// Token: 0x040000FA RID: 250
		private Recorder _r;
	}
}
