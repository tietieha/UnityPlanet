using System;
using System.Collections.Generic;
using System.IO;

// Token: 0x0200000B RID: 11
public class UWACoreConfig
{
	// Token: 0x17000008 RID: 8
	// (get) Token: 0x06000052 RID: 82 RVA: 0x00003760 File Offset: 0x00001960
	// (set) Token: 0x06000053 RID: 83 RVA: 0x00003780 File Offset: 0x00001980
	public static string GUID
	{
		get
		{
			return UWACoreConfig._guid;
		}
		set
		{
			UWACoreConfig._guid = value;
			bool flag = value.Length > 3;
			if (flag)
			{
				int num = value.IndexOf("|", StringComparison.OrdinalIgnoreCase);
				bool flag2 = num != -1;
				if (flag2)
				{
					UWACoreConfig.REPORT_IP = value.Substring(0, num);
					UWACoreConfig.PROJECT_NAME = value.Substring(num + 1);
				}
			}
		}
	}

	// Token: 0x17000009 RID: 9
	// (get) Token: 0x06000054 RID: 84 RVA: 0x000037E4 File Offset: 0x000019E4
	public static string ConfigUrl
	{
		get
		{
			return "http://" + UWACoreConfig.IP + ":8081/UWAServerConfig.ini";
		}
	}

	// Token: 0x1700000A RID: 10
	// (get) Token: 0x06000055 RID: 85 RVA: 0x00003814 File Offset: 0x00001A14
	public static string DexUrl
	{
		get
		{
			return "http://" + UWACoreConfig.IP + ":8081/dex";
		}
	}

	// Token: 0x1700000B RID: 11
	// (get) Token: 0x06000056 RID: 86 RVA: 0x00003844 File Offset: 0x00001A44
	public static string LuaUrl
	{
		get
		{
			return "http://" + UWACoreConfig.IP + ":8081/lua";
		}
	}

	// Token: 0x06000057 RID: 87 RVA: 0x00003874 File Offset: 0x00001A74
	public static bool CheckIpValid()
	{
		string[] array = UWACoreConfig.IP.Split(new char[]
		{
			'.'
		});
		return array.Length == 4;
	}

	// Token: 0x06000058 RID: 88 RVA: 0x000038AC File Offset: 0x00001AAC
	public static void ParseConfigLocal()
	{
		UWACoreConfig.Config4Manager = new Dictionary<string, Dictionary<string, string>>();
		for (int i = 0; i < UWACoreConfig.defaultConfig.Length; i++)
		{
			string text = UWACoreConfig.defaultConfig[i];
			bool flag = text.StartsWith("//");
			if (!flag)
			{
				string[] array = text.Split(new char[]
				{
					':',
					','
				});
				bool flag2 = array.Length == 3;
				if (flag2)
				{
					bool flag3 = !UWACoreConfig.Config4Manager.ContainsKey(array[0]);
					if (flag3)
					{
						UWACoreConfig.Config4Manager.Add(array[0], new Dictionary<string, string>());
					}
					UWACoreConfig.Config4Manager[array[0]].Add(array[1], array[2]);
				}
			}
		}
	}

	// Token: 0x06000059 RID: 89 RVA: 0x00003990 File Offset: 0x00001B90
	public static void ParseConfig(byte[] cfg)
	{
		bool flag = cfg == null;
		if (!flag)
		{
			UWACoreConfig.Config4Manager = new Dictionary<string, Dictionary<string, string>>();
			StreamReader streamReader = new StreamReader(new MemoryStream(cfg));
			string text = streamReader.ReadLine();
			while (text != null)
			{
				bool flag2 = text.StartsWith("//");
				if (flag2)
				{
					text = streamReader.ReadLine();
				}
				else
				{
					string[] array = text.Split(new char[]
					{
						':',
						','
					});
					bool flag3 = array.Length == 3;
					if (flag3)
					{
						bool flag4 = !UWACoreConfig.Config4Manager.ContainsKey(array[0]);
						if (flag4)
						{
							UWACoreConfig.Config4Manager.Add(array[0], new Dictionary<string, string>());
						}
						UWACoreConfig.Config4Manager[array[0]].Add(array[1], array[2]);
					}
					text = streamReader.ReadLine();
				}
			}
			streamReader.Close();
		}
	}

	// Token: 0x04000038 RID: 56
	public static bool ToAddStarter = false;

	// Token: 0x04000039 RID: 57
	public static string IP = "192.168.1.1";

	// Token: 0x0400003A RID: 58
	public static string LOG_IP = "192.168.1.1";

	// Token: 0x0400003B RID: 59
	public static string REPORT_IP = "";

	// Token: 0x0400003C RID: 60
	public static string PROJECT_NAME = "";

	// Token: 0x0400003D RID: 61
	public static int PORT = 8885;

	// Token: 0x0400003E RID: 62
	public static string KEY = "NotSet4LegacyWrapper";

	// Token: 0x0400003F RID: 63
	public static string PLUGIN_VERSION = "0.0.0";

	// Token: 0x04000040 RID: 64
	public static string APP_NAME = "unknown app";

	// Token: 0x04000041 RID: 65
	public static string PKG_NAME = "com.not.set";

	// Token: 0x04000042 RID: 66
	public static string TIME_VERSION = "0000.00.00";

	// Token: 0x04000043 RID: 67
	private static string _guid;

	// Token: 0x04000044 RID: 68
	public const int BUFFER_SIZE = 8192;

	// Token: 0x04000045 RID: 69
	public const string SCREEN_SHOT_SUFFIX = ".jpg";

	// Token: 0x04000046 RID: 70
	public const string TEST_FLAG = "^=test=^";

	// Token: 0x04000047 RID: 71
	public const string START_FLAG = "^=start=^";

	// Token: 0x04000048 RID: 72
	public const string END_FLAG = "^=end=^";

	// Token: 0x04000049 RID: 73
	public const string SUCCEED_FLAG = "^=succeed=^";

	// Token: 0x0400004A RID: 74
	public const string RESEND_FLAG = "^=resend=^";

	// Token: 0x0400004B RID: 75
	public static string Remote_IP = "192.168.1.1";

	// Token: 0x0400004C RID: 76
	public static int Remote_PORT = 8123;

	// Token: 0x0400004D RID: 77
	public static Dictionary<string, Dictionary<string, string>> Config4Manager;

	// Token: 0x0400004E RID: 78
	private static string[] defaultConfig = new string[]
	{
		"// Manager",
		"// Tracker",
		"// useTimeTracker pluginVersion > 1.0.0.0",
		"UWAStarter:useTimeTracker,true",
		"UWAStarter:useScreenTracker,true",
		"UWAStarter:useAssetTracker,true",
		"UWAStarter:useLogTracker,true",
		"UWAStarter:useApiTracker,true",
		"// Tracker ",
		"UWAStarter:profilerTrackTimeInterval,2.3",
		"UWAStarter:timeTrackTimeInterval,2.1",
		"UWAStarter:screenTrackTimeInterval,2.2",
		"UWAStarter:assetTrackTimeInterval,3.9",
		"UWAStarter:logTrackTimeInterval,4.1",
		"UWAStarter:apiTrackTimeInterval,3.7",
		"// Asset Tracker ",
		"AssetTrackManager:AssetBundle,3.1",
		"AssetTrackManager:AnimationClip,3.2",
		"AssetTrackManager:AudioClip,5.1",
		"AssetTrackManager:Font,20.3",
		"AssetTrackManager:Material,3.3",
		"AssetTrackManager:Mesh,10.1",
		"AssetTrackManager:MeshCollider,10.2",
		"AssetTrackManager:ParticleSystem,3.4",
		"AssetTrackManager:ParticleRenderer,3.5",
		"AssetTrackManager:RenderTexture,10.3",
		"AssetTrackManager:Shader,20.1",
		"AssetTrackManager:Texture2D,10.4",
		"AssetTrackManager:SkinnedMeshRenderer,3.6",
		"AssetTrackManager:TextAsset,3.7",
		"AssetTrackManager:Animator,3.9",
		"AssetTrackManager:Animation,3.9",
		"// Log Tracker LogType",
		"LogTrackManager:Log,true",
		"LogTrackManager:Error,true",
		"LogTrackManager:Assert,true",
		"LogTrackManager:Warning,true",
		"LogTrackManager:Exception,true",
		"// Log Tracker Stack Trace",
		"LogTrackManager:outputStackTrace,false",
		"//  Hardware Tracker ",
		"HardwareTrackManager:Pss,2.2",
		"HardwareTrackManager:Network,1.0",
		"HardwareTrackManager:Battery,8.1",
		"HardwareTrackManager:Temperature,6.1",
		"HardwareTrackManager:GpuLoad,1",
		"HardwareTrackManager:CpuLoad,1",
		"//  Api Track",
		"ApiTrackManager:limitLogValueCount,30",
		"ApiTrackManager:limitRegisterValueCount,30",
		"ApiTrackManager:limitPushSampleCount,30",
		"AssetTrackManager:Enable,true",
		"AssetTrackManager:TimeInterval,5.1",
		"LogTrackManager:Enable,true",
		"LogTrackManager:TimeInterval,3.1",
		"ResInfoTrackManager:Enable,true",
		"ResInfoTrackManager:TimeInterval,4.3",
		"HardwareTrackManager:Enable,true",
		"HardwareTrackManager:TimeInterval,2.5",
		"TimeTrackManager:Enable,true",
		"TimeTrackManager:TimeInterval,2.4",
		"ScreenTrackManager:Enable,true",
		"ScreenTrackManager:TimeInterval,1",
		"ScreenTrackManager:Overdraw,false",
		"ApiTrackManager:Enable,true",
		"ApiTrackManager:TimeInterval,2.2",
		"DataTrackManager:Enable,true",
		"DataTrackManager:TimeInterval,2.1",
		"DataTrackManager:Log,truex",
		"MonoTrackManager:Enable,true",
		"MonoTrackManager:TimeInterval,2.1",
		"VRTrackManager:Enable,true",
		"VRTrackManager:TimeInterval,2.3",
		"DataTrackManager:U1,Profiler.get_enabled",
		"DataTrackManager:U2,Profiler.set_enabled",
		"DataTrackManager:U3,Profiler.get_enableBinaryLog",
		"DataTrackManager:U4,Profiler.set_enableBinaryLog",
		"DataTrackManager:U5,Profiler.get_logFile",
		"DataTrackManager:U6,Profiler.set_logFile",
		"DataTrackManager:N1,UProfiler.get_enabled",
		"DataTrackManager:N2,UProfiler.set_enabled",
		"DataTrackManager:N3,UProfiler.get_enableBinaryLog",
		"DataTrackManager:N4,UProfiler.set_enableBinaryLog",
		"DataTrackManager:N5,UProfiler.get_logFile",
		"DataTrackManager:N6,UProfiler.set_logFile"
	};
}
