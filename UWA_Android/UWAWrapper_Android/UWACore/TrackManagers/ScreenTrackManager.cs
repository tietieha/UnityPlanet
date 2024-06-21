using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UWA;
using UWACore.Util;

namespace UWACore.TrackManagers
{
	// Token: 0x0200003C RID: 60
	internal abstract class ScreenTrackManager : BaseTrackerManager
	{
		// Token: 0x060002AD RID: 685 RVA: 0x00012FB4 File Offset: 0x000111B4
		public static ScreenTrackManager Get()
		{
			bool flag = UWAState.ScreenMethod == UWAState.eScreenshotMethod.Disabled;
			if (flag)
			{
				ScreenTrackManager._screenTrackManager = new EmptyScreenTrackManager();
			}
			else
			{
				bool flag2 = SharedUtils.IsPicoVR();
				bool flag3 = flag2;
				if (flag3)
				{
					Debug.LogWarning("UWAGOT: Screenshot on PicoVR is not supported");
				}
				bool flag4 = CoreUtils.IsAndroid4() || flag2;
				if (flag4)
				{
					UWAState.ScreenMethod = UWAState.eScreenshotMethod.Adb;
				}
				else
				{
					bool flag5 = NativeJavaScreenTrackManager.Support();
					if (flag5)
					{
						UWAState.ScreenMethod = UWAState.eScreenshotMethod.Java;
					}
				}
			}
			bool flag6 = UWAState.ScreenMethod == UWAState.eScreenshotMethod.Java;
			if (flag6)
			{
				bool flag7 = NativeJavaScreenTrackManager.Support();
				if (flag7)
				{
					ScreenTrackManager._screenTrackManager = new NativeJavaScreenTrackManager();
				}
				else
				{
					ScreenTrackManager._screenTrackManager = new JavaMediaProjectionScreenTrackManager();
				}
			}
			bool flag8 = UWAState.ScreenMethod == UWAState.eScreenshotMethod.Adb;
			if (flag8)
			{
				ScreenTrackManager._screenTrackManager = new AdbScreenTrackManager();
			}
			return ScreenTrackManager._screenTrackManager;
		}

		// Token: 0x060002AE RID: 686 RVA: 0x000130A0 File Offset: 0x000112A0
		public static void DoPrepare()
		{
			ScreenTrackManager._screenTrackManager.Prepare();
		}

		// Token: 0x060002AF RID: 687 RVA: 0x000130B0 File Offset: 0x000112B0
		protected ScreenTrackManager() : base(".jpg", 200)
		{
		}

		// Token: 0x060002B0 RID: 688 RVA: 0x000130C4 File Offset: 0x000112C4
		protected override void InitWithConfig(Dictionary<string, string> config)
		{
		}

		// Token: 0x060002B1 RID: 689 RVA: 0x000130C8 File Offset: 0x000112C8
		public override void SwitchLogFile(bool end = false)
		{
			base.SwitchLogFile(end);
			bool flag = !end;
			if (flag)
			{
				this.LogFile = this.GetTargetLogName();
			}
		}

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x060002B2 RID: 690 RVA: 0x000130FC File Offset: 0x000112FC
		// (set) Token: 0x060002B3 RID: 691 RVA: 0x00013104 File Offset: 0x00011304
		protected override string LogFile { get; set; }

		// Token: 0x060002B4 RID: 692 RVA: 0x00013110 File Offset: 0x00011310
		public void BeginDrawScene()
		{
		}

		// Token: 0x060002B5 RID: 693 RVA: 0x00013114 File Offset: 0x00011314
		public void EndDrawScene()
		{
		}

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x060002B6 RID: 694 RVA: 0x00013118 File Offset: 0x00011318
		// (set) Token: 0x060002B7 RID: 695 RVA: 0x00013120 File Offset: 0x00011320
		public bool ServiceDeleted { get; set; }

		// Token: 0x060002B8 RID: 696 RVA: 0x0001312C File Offset: 0x0001132C
		public virtual bool Shot(string path)
		{
			return false;
		}

		// Token: 0x060002B9 RID: 697 RVA: 0x00013148 File Offset: 0x00011348
		private static void InitMethod()
		{
			bool methodInit = ScreenTrackManager._methodInit;
			if (!methodInit)
			{
				bool flag = ScreenTrackManager._captureMethod == null;
				if (flag)
				{
					Type type = null;
					Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
					foreach (Assembly assembly in assemblies)
					{
						bool flag2 = !assembly.FullName.Contains("ScreenCapture");
						if (!flag2)
						{
							type = assembly.GetType("UnityEngine.ScreenCapture");
							bool flag3 = type != null;
							if (flag3)
							{
								break;
							}
						}
					}
					bool flag4 = type != null;
					if (flag4)
					{
						ScreenTrackManager._captureMethod = type.GetMethod("CaptureScreenshot", new Type[]
						{
							typeof(string),
							typeof(int)
						});
					}
				}
				bool flag5 = ScreenTrackManager._captureMethod == null;
				if (flag5)
				{
					Type type2 = typeof(GameObject).Assembly.GetType("UnityEngine.ScreenCapture");
					bool flag6 = type2 != null;
					if (flag6)
					{
						ScreenTrackManager._captureMethod = type2.GetMethod("CaptureScreenshot", new Type[]
						{
							typeof(string),
							typeof(int)
						});
					}
				}
				bool flag7 = ScreenTrackManager._captureMethod == null && Application.unityVersion.StartsWith("2018");
				if (flag7)
				{
					ScreenTrackManager._captureMethod = typeof(Application).GetMethod("CaptureScreenshot", new Type[]
					{
						typeof(string),
						typeof(int)
					});
				}
				bool flag8 = ScreenTrackManager._encodeJpgMethod == null;
				if (flag8)
				{
					Type type3 = null;
					Assembly[] assemblies2 = AppDomain.CurrentDomain.GetAssemblies();
					foreach (Assembly assembly2 in assemblies2)
					{
						bool flag9 = !assembly2.FullName.Contains("ImageConversion");
						if (!flag9)
						{
							type3 = assembly2.GetType("UnityEngine.ImageConversion");
							bool flag10 = type3 != null;
							if (flag10)
							{
								break;
							}
						}
					}
					bool flag11 = type3 != null;
					if (flag11)
					{
						ScreenTrackManager._encodeJpgMethod = type3.GetMethod("EncodeToJPG", new Type[]
						{
							typeof(Texture2D),
							typeof(int)
						});
					}
				}
				bool flag12 = ScreenTrackManager._encodeJpgMethod == null;
				if (flag12)
				{
					Type type4 = typeof(GameObject).Assembly.GetType("UnityEngine.ImageConversion");
					bool flag13 = type4 != null;
					if (flag13)
					{
						ScreenTrackManager._encodeJpgMethod = type4.GetMethod("EncodeToJPG", new Type[]
						{
							typeof(Texture2D),
							typeof(int)
						});
					}
				}
				bool flag14 = ScreenTrackManager._getpixels32Method == null;
				if (flag14)
				{
					ScreenTrackManager._getpixels32Method = typeof(Texture2D).GetMethod("GetPixels32", new Type[]
					{
						typeof(int)
					});
				}
				string str = "_captureMethod =  ";
				MethodInfo captureMethod = ScreenTrackManager._captureMethod;
				Debug.Log(str + ((captureMethod != null) ? captureMethod.ToString() : null));
				string str2 = "_encodeJpgMethod =  ";
				MethodInfo encodeJpgMethod = ScreenTrackManager._encodeJpgMethod;
				Debug.Log(str2 + ((encodeJpgMethod != null) ? encodeJpgMethod.ToString() : null));
				string str3 = "_getpixels32Method =  ";
				MethodInfo getpixels32Method = ScreenTrackManager._getpixels32Method;
				Debug.Log(str3 + ((getpixels32Method != null) ? getpixels32Method.ToString() : null));
				ScreenTrackManager._methodInit = true;
			}
		}

		// Token: 0x060002BA RID: 698 RVA: 0x000134F4 File Offset: 0x000116F4
		public static IEnumerator DelayScreenshot(string path)
		{
			yield return new WaitForEndOfFrame();
			ScreenTrackManager.InitMethod();
			string fileName = Path.GetFileName(path);
			bool captured = false;
			try
			{
				bool flag = ScreenTrackManager._captureMethod != null;
				if (flag)
				{
					ScreenTrackManager._captureMethod.Invoke(null, new object[]
					{
						fileName,
						0
					});
					captured = true;
				}
			}
			catch (Exception ex)
			{
				Exception e = ex;
				SharedUtils.Log("_captureMethod.Invoke Ex : " + e.ToString());
			}
			bool flag2 = !captured;
			if (flag2)
			{
				bool flag3 = ScreenTrackManager._encodeJpgMethod != null || ScreenTrackManager._getpixels32Method != null;
				if (flag3)
				{
					Texture2D tex = new Texture2D(Screen.width, Screen.height, 3, false);
					tex.ReadPixels(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), 0, 0, false);
					tex.Apply();
					bool flag4 = ScreenTrackManager._encodeJpgMethod != null;
					if (flag4)
					{
						try
						{
							byte[] jpg = ScreenTrackManager._encodeJpgMethod.Invoke(null, new object[]
							{
								tex,
								70
							}) as byte[];
							File.WriteAllBytes(path, jpg);
							captured = true;
							jpg = null;
						}
						catch (Exception ex)
						{
							Exception e2 = ex;
							SharedUtils.Log("_encodeJpgMethod.Invoke Ex : " + e2.ToString());
						}
					}
					bool flag5 = !captured && ScreenTrackManager._getpixels32Method != null;
					if (flag5)
					{
						try
						{
							Color32[] colors = ScreenTrackManager._getpixels32Method.Invoke(tex, new object[]
							{
								0
							}) as Color32[];
							UWAPublicCore50983.JPGEncoder encoder = new UWAPublicCore50983.JPGEncoder(colors, tex.width, tex.height, 50f);
							encoder.doEncoding();
							encoder.WriteTo(path);
							captured = true;
							colors = null;
							encoder = null;
						}
						catch (Exception ex)
						{
							Exception e3 = ex;
							SharedUtils.Log("_getpixels32Method.Invoke Ex : " + e3.ToString());
						}
					}
					Object.DestroyImmediate(tex);
					tex = null;
				}
				UWAState.ScreencapLost = !captured;
			}
			yield return new WaitForSeconds(5f);
			string DirName = Path.GetDirectoryName(path);
			bool flag6 = !string.IsNullOrEmpty(DirName) && Directory.Exists(DirName);
			if (flag6)
			{
				bool flag7 = File.Exists(Application.persistentDataPath + "/" + fileName);
				if (flag7)
				{
					File.Move(Application.persistentDataPath + "/" + fileName, path);
				}
				bool flag8 = File.Exists(Application.dataPath + "/" + fileName);
				if (flag8)
				{
					File.Move(Application.dataPath + "/" + fileName, path);
				}
			}
			yield break;
		}

		// Token: 0x0400018C RID: 396
		private static ScreenTrackManager _screenTrackManager;

		// Token: 0x0400018D RID: 397
		public static ScreenTrackManager.ETrackMode TrackMode;

		// Token: 0x04000190 RID: 400
		private static MethodInfo _captureMethod;

		// Token: 0x04000191 RID: 401
		private static MethodInfo _encodeJpgMethod;

		// Token: 0x04000192 RID: 402
		private static MethodInfo _getpixels32Method;

		// Token: 0x04000193 RID: 403
		private static bool _methodInit;

		// Token: 0x020000FD RID: 253
		public enum ETrackMode
		{
			// Token: 0x04000651 RID: 1617
			Image,
			// Token: 0x04000652 RID: 1618
			Video
		}
	}
}
