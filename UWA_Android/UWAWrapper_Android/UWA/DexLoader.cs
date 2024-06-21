using System;
using System.IO;
using UnityEngine;

namespace UWA
{
	// Token: 0x02000050 RID: 80
	internal class DexLoader
	{
		// Token: 0x06000367 RID: 871 RVA: 0x00017404 File Offset: 0x00015604
		public DexLoader()
		{
			this._dexPath = Application.persistentDataPath + "/" + this._dexPName;
			File.WriteAllBytes(this._dexPath, UWAPublicCore50983.UwaDex.GetByte());
			string codeCacheDir = SharedUtils.GetCodeCacheDir();
			bool showLog = SharedUtils.ShowLog;
			if (showLog)
			{
				SharedUtils.Log("UWA dex opt path: " + codeCacheDir);
			}
			this.joDexClassLoader = new AndroidJavaObject("dalvik.system.DexClassLoader", new object[]
			{
				this._dexPath,
				codeCacheDir,
				null,
				SharedUtils.CurrentActivity.Call<AndroidJavaObject>("getClassLoader", new object[0])
			});
		}

		// Token: 0x06000368 RID: 872 RVA: 0x000174BC File Offset: 0x000156BC
		public AndroidJavaClass GetJavaClass(string className)
		{
			AndroidJavaClass result = null;
			try
			{
				result = this.joDexClassLoader.Call<AndroidJavaClass>("loadClass", new object[]
				{
					className
				});
			}
			catch (Exception ex)
			{
				string str = "DexLoader load class Failed: ";
				Exception ex2 = ex;
				SharedUtils.Log(str + ((ex2 != null) ? ex2.ToString() : null));
			}
			return result;
		}

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x06000369 RID: 873 RVA: 0x00017534 File Offset: 0x00015734
		public static DexLoader Instance
		{
			get
			{
				bool flag = DexLoader._instance == null;
				if (flag)
				{
					DexLoader._instance = new DexLoader();
				}
				return DexLoader._instance;
			}
		}

		// Token: 0x040001F9 RID: 505
		private AndroidJavaObject joDexClassLoader;

		// Token: 0x040001FA RID: 506
		private string _dexPath = null;

		// Token: 0x040001FB RID: 507
		private string _dexPName = "uwa.dex";

		// Token: 0x040001FC RID: 508
		private static DexLoader _instance;
	}
}
