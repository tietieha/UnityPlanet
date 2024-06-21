using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UWA
{
	// Token: 0x02000004 RID: 4
	public class BuildPrepareTool
	{
		// Token: 0x06000009 RID: 9 RVA: 0x000023F6 File Offset: 0x000005F6
		public static void SetVersionDependTool(IVersionDependTool vdtool)
		{
			BuildPrepareTool.VDTool = vdtool;
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002400 File Offset: 0x00000600
		public static void Prepare(BuildTarget target, bool isAndroidTargetLargerThan28)
		{
			bool flag = BuildPrepareTool.IsSdkDisabled(target);
			Debug.Log("[UWA SDK] : Prepare UWA GOT SDK for build.");
			Debug.Log("[UWA SDK] : Target " + target.ToString());
			Debug.LogFormat("[UWA SDK] : Build {0} SDK ", new object[]
			{
				flag ? "without" : " with"
			});
			Debug.LogFormat("[UWA SDK] : AndroidTarget {0} 28", new object[]
			{
				isAndroidTargetLargerThan28 ? ">" : "<="
			});
			Debug.Log("[UWA SDK] : ManagedSetup.");
			try
			{
				BuildPrepareTool.ManagedSetup(flag);
			}
			catch (Exception ex)
			{
				Debug.Log("[UWA SDK] : ManagedSetup Ex\n" + ex.ToString());
			}
			Debug.Log("[UWA SDK] : NativePluginSetup.");
			try
			{
				BuildPrepareTool.NativePluginSetup(isAndroidTargetLargerThan28, flag);
			}
			catch (Exception ex2)
			{
				Debug.Log("[UWA SDK] : NativePluginSetup Ex\n" + ex2.ToString());
			}
			Debug.Log("[UWA SDK] : OtherSetup.");
			try
			{
				BuildPrepareTool.OtherSetup(target, flag);
			}
			catch (Exception ex3)
			{
				Debug.Log("[UWA SDK] : OtherSetup Ex\n" + ex3.ToString());
			}
			Debug.Log("[UWA SDK] : Prepare done.");
		}

		// Token: 0x0600000B RID: 11 RVA: 0x0000254C File Offset: 0x0000074C
		private static bool IsSdkDisabled(BuildTarget target)
		{
			BuildTargetGroup buildTargetGroup = 0;
			if (target <= 9)
			{
				if (target != 5)
				{
					if (target != 9)
					{
						goto IL_33;
					}
					buildTargetGroup = 4;
					goto IL_33;
				}
			}
			else
			{
				if (target == 13)
				{
					buildTargetGroup = 7;
					goto IL_33;
				}
				if (target != 19)
				{
					goto IL_33;
				}
			}
			buildTargetGroup = 1;
			IL_33:
			bool flag = buildTargetGroup == 0;
			return flag || PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup).Contains("DISABLE_UWA_SDK");
		}

		// Token: 0x0600000C RID: 12 RVA: 0x000025B0 File Offset: 0x000007B0
		private static void ManagedSetup(bool disable)
		{
			bool flag = BuildPrepareTool.VDTool != null && !disable;
			if (flag)
			{
				BuildPrepareTool.VDTool.PrepareManagedStrip();
			}
			if (disable)
			{
				BuildPrepareTool.SetPluginTarget(BuildPrepareTool.UwaUnityPath + "/Runtime/ManagedLibs/UWAShared.dll", -2, true, false, true);
				BuildPrepareTool.SetPluginTarget(BuildPrepareTool.UwaUnityPath + "/Runtime/ManagedLibs/UWAWrapper_Android.dll", -2, true, false, false);
				BuildPrepareTool.SetPluginTarget(BuildPrepareTool.UwaUnityPath + "/Runtime/ManagedLibs/UWAWrapper_iOS.dll", -2, true, false, false);
				BuildPrepareTool.SetPluginTarget(BuildPrepareTool.UwaUnityPath + "/Runtime/ManagedLibs/UWAWrapper_Windows.dll", -2, true, false, false);
			}
			else
			{
				BuildPrepareTool.SetPluginTarget(BuildPrepareTool.UwaUnityPath + "/Runtime/ManagedLibs/UWAShared.dll", -2, true, true, true);
				BuildPrepareTool.SetPluginTarget(BuildPrepareTool.UwaUnityPath + "/Runtime/ManagedLibs/UWAWrapper_Android.dll", 13, true, false, true);
				BuildPrepareTool.SetPluginTarget(BuildPrepareTool.UwaUnityPath + "/Runtime/ManagedLibs/UWAWrapper_iOS.dll", 9, true, false, true);
				BuildPrepareTool.SetPluginTarget(BuildPrepareTool.UwaUnityPath + "/Runtime/ManagedLibs/UWAWrapper_Windows.dll", 5, true, false, true);
				BuildPrepareTool.SetPluginTarget(BuildPrepareTool.UwaUnityPath + "/Runtime/ManagedLibs/UWAWrapper_Windows.dll", 19, false, false, true);
			}
		}

		// Token: 0x0600000D RID: 13 RVA: 0x000026D4 File Offset: 0x000008D4
		private static void NativePluginSetup(bool isAndroidTargetLargerThan28, bool disable)
		{
			BuildTarget target = -2;
			bool flag = !disable;
			if (flag)
			{
				target = 9;
			}
			BuildPrepareTool.SetPluginTarget(BuildPrepareTool.UwaUnityPath + "/Runtime/Plugins/iOS/libuwa.a", target, true, false, false);
			BuildPrepareTool.SetPluginTarget(BuildPrepareTool.UwaUnityPath + "/Runtime/Plugins/iOS/UwaTools.h", target, true, false, false);
			BuildPrepareTool.SetPluginTarget(BuildPrepareTool.UwaUnityPath + "/Runtime/Plugins/iOS/UwaTools.mm", target, true, false, false);
			bool flag2 = !disable;
			if (flag2)
			{
				target = 5;
			}
			BuildPrepareTool.SetPluginTarget(BuildPrepareTool.UwaUnityPath + "/Runtime/Plugins/x86/uwa.dll", target, true, false, false);
			bool flag3 = !disable;
			if (flag3)
			{
				target = 19;
			}
			BuildPrepareTool.SetPluginTarget(BuildPrepareTool.UwaUnityPath + "/Runtime/Plugins/x86_64/uwa.dll", target, true, false, false);
			bool flag4 = !disable;
			if (flag4)
			{
				target = 13;
			}
			BuildPrepareTool.SetPluginTarget(BuildPrepareTool.UwaUnityPath + "/Runtime/Plugins/Android/libs/arm64-v8a/libuwa.so", target, true, false, false);
			BuildPrepareTool.SetPluginTarget(BuildPrepareTool.UwaUnityPath + "/Runtime/Plugins/Android/libs/armeabi-v7a/libuwa.so", target, true, false, false);
			BuildPrepareTool.SetPluginTarget(BuildPrepareTool.UwaUnityPath + "/Runtime/Plugins/Android/libs/x86/libuwa.so", target, true, false, false);
			BuildPrepareTool.SetPluginTarget(BuildPrepareTool.UwaUnityPath + "/Runtime/Plugins/Android/uwa_pc.aar", target, true, false, false);
			if (isAndroidTargetLargerThan28)
			{
				BuildPrepareTool.SetPluginTarget(BuildPrepareTool.UwaUnityPath + "/Runtime/Plugins/Android/uwa_mp.aar", target, true, false, false);
				BuildPrepareTool.SetPluginTarget(BuildPrepareTool.UwaUnityPath + "/Runtime/Plugins/Android/uwa_mp28.aar", -2, true, false, false);
			}
			else
			{
				BuildPrepareTool.SetPluginTarget(BuildPrepareTool.UwaUnityPath + "/Runtime/Plugins/Android/uwa_mp.aar", -2, true, false, false);
				BuildPrepareTool.SetPluginTarget(BuildPrepareTool.UwaUnityPath + "/Runtime/Plugins/Android/uwa_mp28.aar", target, true, false, false);
			}
			bool flag5 = !disable;
			if (flag5)
			{
				BuildPrepareTool.SetPlatformData(BuildPrepareTool.UwaUnityPath + "/Runtime/Plugins/Android/libs/armeabi-v7a/libuwa.so", target, "CPU", "ARMv7");
				BuildPrepareTool.SetPlatformData(BuildPrepareTool.UwaUnityPath + "/Runtime/Plugins/Android/libs/x86/libuwa.so", target, "CPU", "x86");
				bool flag6 = !BuildPrepareTool.SetPlatformData(BuildPrepareTool.UwaUnityPath + "/Runtime/Plugins/Android/libs/arm64-v8a/libuwa.so", 13, "CPU", "ARM64");
				if (flag6)
				{
					BuildPrepareTool.SetPluginTarget(BuildPrepareTool.UwaUnityPath + "/Runtime/Plugins/Android/libs/arm64-v8a/libuwa.so", -2, true, false, false);
				}
			}
		}

		// Token: 0x0600000E RID: 14 RVA: 0x000028FC File Offset: 0x00000AFC
		private static void OtherSetup(BuildTarget target, bool disable)
		{
			bool flag = BuildPrepareTool.VDTool != null && !disable;
			if (flag)
			{
				BuildPrepareTool.VDTool.PrepareOtherSetup();
			}
		}

		// Token: 0x0600000F RID: 15 RVA: 0x00002928 File Offset: 0x00000B28
		private static bool SetPluginTarget(string pluginPath, BuildTarget target, bool disableOther = true, bool anyPlatform = false, bool withEditor = false)
		{
			PluginImporter pluginImporter = AssetImporter.GetAtPath(pluginPath) as PluginImporter;
			bool flag = pluginImporter == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				pluginImporter.SetCompatibleWithAnyPlatform(anyPlatform);
				pluginImporter.SetCompatibleWithEditor(withEditor);
				bool flag2 = !anyPlatform;
				if (flag2)
				{
					if (disableOther)
					{
						for (int i = 0; i < BuildPrepareTool.BuildTargets.Count; i++)
						{
							pluginImporter.SetCompatibleWithPlatform(BuildPrepareTool.BuildTargets[i], false);
						}
					}
					bool flag3 = target != -2;
					if (flag3)
					{
						pluginImporter.SetCompatibleWithPlatform(target, true);
					}
				}
				bool flag4 = BuildPrepareTool.VDTool != null;
				if (flag4)
				{
					BuildPrepareTool.VDTool.PrepareNativePlugin(pluginImporter);
				}
				pluginImporter.SaveAndReimport();
				result = true;
			}
			return result;
		}

		// Token: 0x06000010 RID: 16 RVA: 0x000029EC File Offset: 0x00000BEC
		private static bool SetPlatformData(string pluginPath, BuildTarget target, string key, string value)
		{
			PluginImporter pluginImporter = AssetImporter.GetAtPath(pluginPath) as PluginImporter;
			bool flag = pluginImporter == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				pluginImporter.SetPlatformData(target, key, value);
				result = (pluginImporter.GetPlatformData(target, key) == value);
			}
			return result;
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000011 RID: 17 RVA: 0x00002A34 File Offset: 0x00000C34
		public static GUIStyle textAreaStyle
		{
			get
			{
				bool flag = BuildPrepareTool._textAreaStyle == null;
				if (flag)
				{
					BuildPrepareTool._textAreaStyle = new GUIStyle("textField")
					{
						margin = new RectOffset(0, 0, 0, 0)
					};
				}
				return BuildPrepareTool._textAreaStyle;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000012 RID: 18 RVA: 0x00002A80 File Offset: 0x00000C80
		public static string UwaUnityPath
		{
			get
			{
				bool flag = BuildPrepareTool._uwaUnityPath == null;
				if (flag)
				{
					string[] directories = Directory.GetDirectories(Application.dataPath, "UWA_SDK", SearchOption.AllDirectories);
					bool flag2 = directories.Length == 0;
					if (flag2)
					{
						BuildPrepareTool._uwaUnityPath = "";
					}
					else
					{
						BuildPrepareTool._uwaUnityPath = directories[0].Replace(Application.dataPath, "Assets");
					}
				}
				return BuildPrepareTool._uwaUnityPath;
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000013 RID: 19 RVA: 0x00002AE8 File Offset: 0x00000CE8
		public static string UwaFullPath
		{
			get
			{
				bool flag = BuildPrepareTool._uwaFullPath == null;
				if (flag)
				{
					string[] directories = Directory.GetDirectories(Application.dataPath, "UWA_SDK", SearchOption.AllDirectories);
					bool flag2 = directories.Length == 0;
					if (flag2)
					{
						BuildPrepareTool._uwaFullPath = "";
					}
					else
					{
						BuildPrepareTool._uwaFullPath = directories[0];
					}
				}
				return BuildPrepareTool._uwaFullPath;
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000014 RID: 20 RVA: 0x00002B40 File Offset: 0x00000D40
		public static string UwaSdkUrl
		{
			get
			{
				bool flag = BuildPrepareTool._uwaSdkUrl == null;
				if (flag)
				{
					string text = BuildPrepareTool.UwaFullPath + "/Documentation/UWA_SDK_User_Guide(CN).pdf";
					bool flag2 = File.Exists(text);
					if (flag2)
					{
						BuildPrepareTool._uwaSdkUrl = ("file:///" + text).Replace(" ", "%20");
					}
					else
					{
						BuildPrepareTool._uwaSdkUrl = "";
					}
				}
				return BuildPrepareTool._uwaSdkUrl;
			}
		}

		// Token: 0x04000002 RID: 2
		private const string SDKRoot = "UWA_SDK";

		// Token: 0x04000003 RID: 3
		private static IVersionDependTool VDTool;

		// Token: 0x04000004 RID: 4
		private static List<BuildTarget> BuildTargets = new List<BuildTarget>
		{
			5,
			19,
			13,
			9
		};

		// Token: 0x04000005 RID: 5
		private static GUIStyle _textAreaStyle = null;

		// Token: 0x04000006 RID: 6
		private static string _uwaUnityPath = null;

		// Token: 0x04000007 RID: 7
		private static string _uwaFullPath = null;

		// Token: 0x04000008 RID: 8
		private static string _uwaSdkUrl = null;
	}
}
