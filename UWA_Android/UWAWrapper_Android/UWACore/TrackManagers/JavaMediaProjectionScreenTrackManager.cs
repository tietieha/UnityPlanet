using System;
using System.Collections.Generic;
using UnityEngine;
using UWA;

namespace UWACore.TrackManagers
{
	// Token: 0x0200003E RID: 62
	internal class JavaMediaProjectionScreenTrackManager : ScreenTrackManager
	{
		// Token: 0x060002BD RID: 701 RVA: 0x0001351C File Offset: 0x0001171C
		protected override void InitWithConfig(Dictionary<string, string> config)
		{
		}

		// Token: 0x060002BE RID: 702 RVA: 0x00013520 File Offset: 0x00011720
		protected override void Prepare()
		{
			this.LocalLoadDex();
		}

		// Token: 0x060002BF RID: 703 RVA: 0x0001352C File Offset: 0x0001172C
		public override bool IsReady(out string error)
		{
			error = null;
			bool flag = !this._dexLoaded;
			bool result;
			if (flag)
			{
				error = "Screenshot is not loaded.";
				result = false;
			}
			else
			{
				bool flag2 = !this._jcSsManager.CallStatic<bool>("isReady", new object[0]);
				if (flag2)
				{
					error = "Screenshot is not ready.";
					result = false;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		// Token: 0x060002C0 RID: 704 RVA: 0x00013598 File Offset: 0x00011798
		private void LocalLoadDex()
		{
			this._dexLoaded = this.LoadSsManager();
			bool showLog = SharedUtils.ShowLog;
			if (showLog)
			{
				SharedUtils.Log("LoadSsManager " + this._dexLoaded.ToString());
			}
		}

		// Token: 0x060002C1 RID: 705 RVA: 0x000135E0 File Offset: 0x000117E0
		private bool LoadSsManager()
		{
			this._jcSsManager = DexLoader.Instance.GetJavaClass("com.uwa.uwascreen.ScreenCaptureForUnity");
			bool flag = this._jcSsManager != null && SharedUtils.CurrentActivity != null;
			bool result;
			if (flag)
			{
				this._jcSsManager.CallStatic("ScreenCaptureInit", new object[]
				{
					405,
					SharedUtils.CurrentActivity
				});
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x060002C2 RID: 706 RVA: 0x00013664 File Offset: 0x00011864
		public override bool Shot(string path)
		{
			bool flag = this._jcSsManager.CallStatic<bool>("isReady", new object[0]);
			bool flag2 = SharedUtils.frameId - this._lastframe > 25 && flag;
			bool result;
			if (flag2)
			{
				this._lastframe = SharedUtils.frameId;
				bool flag3 = false;
				try
				{
					flag3 = this._jcSsManager.CallStatic<bool>("ScreenCapture", new object[]
					{
						path,
						70
					});
				}
				catch (Exception)
				{
				}
				result = flag3;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x060002C3 RID: 707 RVA: 0x00013704 File Offset: 0x00011904
		// (set) Token: 0x060002C4 RID: 708 RVA: 0x00013744 File Offset: 0x00011944
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

		// Token: 0x04000194 RID: 404
		private bool _dexLoaded = false;

		// Token: 0x04000195 RID: 405
		private AndroidJavaClass _jcSsManager;

		// Token: 0x04000196 RID: 406
		private int _lastframe = -50;

		// Token: 0x04000197 RID: 407
		private string _screenshotFileName = null;
	}
}
