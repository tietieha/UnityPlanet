using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UWA;

namespace UWACore.TrackManagers
{
	// Token: 0x0200003F RID: 63
	internal class NativeJavaScreenTrackManager : ScreenTrackManager
	{
		// Token: 0x060002C6 RID: 710
		[DllImport("uwa", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool IsFeatureSupport(string s);

		// Token: 0x060002C7 RID: 711
		[DllImport("uwa", CallingConvention = CallingConvention.Cdecl)]
		public static extern void DoNativeCommand(string s, string p);

		// Token: 0x060002C8 RID: 712 RVA: 0x000137B8 File Offset: 0x000119B8
		public static bool Support()
		{
			try
			{
				bool flag = NativeJavaScreenTrackManager.IsFeatureSupport("android_screenshot");
				bool flag2 = flag;
				if (flag2)
				{
					return true;
				}
			}
			catch (Exception)
			{
			}
			return false;
		}

		// Token: 0x060002C9 RID: 713 RVA: 0x00013808 File Offset: 0x00011A08
		protected override void InitWithConfig(Dictionary<string, string> config)
		{
		}

		// Token: 0x060002CA RID: 714 RVA: 0x0001380C File Offset: 0x00011A0C
		protected override void Prepare()
		{
		}

		// Token: 0x060002CB RID: 715 RVA: 0x00013810 File Offset: 0x00011A10
		public override bool IsReady(out string error)
		{
			error = null;
			return true;
		}

		// Token: 0x060002CC RID: 716 RVA: 0x00013830 File Offset: 0x00011A30
		public override bool Shot(string path)
		{
			bool flag = SharedUtils.frameId - this._lastframe > 25;
			bool result;
			if (flag)
			{
				this._lastframe = SharedUtils.frameId;
				bool flag2 = false;
				try
				{
					NativeJavaScreenTrackManager.DoNativeCommand("android_screenshot", path);
				}
				catch (Exception)
				{
				}
				result = flag2;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x060002CD RID: 717 RVA: 0x000138A0 File Offset: 0x00011AA0
		// (set) Token: 0x060002CE RID: 718 RVA: 0x000138E0 File Offset: 0x00011AE0
		protected override string LogFile
		{
			get
			{
				return (base.Enabled && this._screenshotFileName != null) ? this._screenshotFileName : "";
			}
			set
			{
				bool enabled = base.Enabled;
				if (enabled)
				{
					this._screenshotFileName = value;
					bool flag = !this._screenshotFileName.Equals("");
					if (flag)
					{
						this.Shot(this._screenshotFileName);
					}
				}
			}
		}

		// Token: 0x04000198 RID: 408
		private int _lastframe = -50;

		// Token: 0x04000199 RID: 409
		private string _screenshotFileName = null;
	}
}
