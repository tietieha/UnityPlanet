using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Scripting;
using UWA;
using UWA.Android;
using UWASDK;

namespace UWALocal
{
	// Token: 0x02000019 RID: 25
	internal class PocoTool
	{
		// Token: 0x0600014D RID: 333 RVA: 0x00008668 File Offset: 0x00006868
		public static bool CheckPoco()
		{
			if (PocoTool.pocoChecked != null)
			{
				return PocoTool.pocoChecked.Value;
			}
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly assembly in assemblies)
			{
				PocoTool.PocoManagerType = assembly.GetType("PocoManager");
				if (PocoTool.PocoManagerType == null)
				{
					PocoTool.PocoManagerType = assembly.GetType("Poco.PocoManager");
				}
				if (PocoTool.PocoManagerType != null)
				{
					PocoTool.RpcField = PocoTool.PocoManagerType.GetField("rpc", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					PocoTool.RPCParserType = assembly.GetType("RPCParser");
					if (PocoTool.RPCParserType == null)
					{
						PocoTool.RPCParserType = assembly.GetType("Poco.RPCParser");
					}
					if (PocoTool.RpcField != null && PocoTool.RPCParserType != null)
					{
						if (SharedUtils.ShowLog)
						{
							SharedUtils.Log("RpcField & RPCParserType found");
						}
						if (SharedUtils.ShowLog)
						{
							SharedUtils.Log("CheckPoco Success");
						}
						PocoTool.pocoChecked = new bool?(true);
						return true;
					}
				}
			}
			if (SharedUtils.ShowLog)
			{
				SharedUtils.Log("CheckPoco Failed");
			}
			PocoTool.pocoChecked = new bool?(false);
			return false;
		}

		// Token: 0x0600014E RID: 334 RVA: 0x0000879C File Offset: 0x0000699C
		public static bool CheckPocoManager()
		{
			return PocoTool._pocoManagerExist;
		}

		// Token: 0x0600014F RID: 335 RVA: 0x000087A4 File Offset: 0x000069A4
		[Preserve]
		private static object Tag(List<object> param)
		{
			if (SharedUtils.ShowLog)
			{
				SharedUtils.Log("PocoRpc Tag:" + param.Count.ToString());
			}
			if (param.Count != 1 || !(param[0] is string))
			{
				return false;
			}
			UWAEngine.Tag((string)param[0]);
			return true;
		}

		// Token: 0x06000150 RID: 336 RVA: 0x00008818 File Offset: 0x00006A18
		[Preserve]
		private static object Note(List<object> param)
		{
			if (SharedUtils.ShowLog)
			{
				SharedUtils.Log("PocoRpc Note:" + param.Count.ToString());
			}
			if (param.Count != 1 || !(param[0] is string))
			{
				return false;
			}
			SharedUtils.SetNote((string)param[0]);
			return true;
		}

		// Token: 0x06000151 RID: 337 RVA: 0x0000888C File Offset: 0x00006A8C
		[Preserve]
		private static object Dump(List<object> param)
		{
			if (SharedUtils.ShowLog)
			{
				SharedUtils.Log("PocoRpc Dump:" + param.Count.ToString());
			}
			if (param.Count != 1)
			{
				return false;
			}
			string a = param[0].ToString().ToLower();
			if (a == "mono")
			{
				SdkCtrlData.Instance.TryDump |= eDumpType.ManagedHeap;
			}
			if (a == "lua")
			{
				SdkCtrlData.Instance.TryDump |= eDumpType.Lua;
			}
			if (a == "resources")
			{
				SdkCtrlData.Instance.TryDump |= eDumpType.Resources;
			}
			if (a == "overdraw")
			{
				SdkCtrlData.Instance.TryDump |= eDumpType.Overdraw;
			}
			return true;
		}

		// Token: 0x06000152 RID: 338 RVA: 0x00008978 File Offset: 0x00006B78
		[Preserve]
		private static object StatPoco(List<object> param)
		{
			if (SharedUtils.ShowLog)
			{
				SharedUtils.Log("PocoRpc StatPoco:" + param.Count.ToString());
			}
			if (param.Count != 1)
			{
				return false;
			}
			if (param[0].ToString().ToLower() == "true")
			{
				UwaLocalState.pocoStrip = false;
			}
			return true;
		}

		// Token: 0x06000153 RID: 339 RVA: 0x000089F0 File Offset: 0x00006BF0
		[Preserve]
		private static object Connect(List<object> param)
		{
			if (SharedUtils.ShowLog)
			{
				SharedUtils.Log("PocoRpc Connect:" + param.Count.ToString());
			}
			SdkCtrlData.Instance.PocoConnected = true;
			SdkUIMgr.Get().ClearDialog();
			SdkUIMgr.Get().ClearMsg();
			DataUploader.s = DataUploader.UploadState.Idle;
			return true;
		}

		// Token: 0x06000154 RID: 340 RVA: 0x00008A54 File Offset: 0x00006C54
		[Preserve]
		private static object Start(List<object> param)
		{
			if (SharedUtils.ShowLog)
			{
				SharedUtils.Log("PocoRpc Start:" + param.Count.ToString());
			}
			if ((param.Count != 1 || !(param[0] is string)) && (param.Count != 2 || !(param[0] is string) || !(param[1] is string)))
			{
				return false;
			}
			string text = (string)param[0];
			if (text == "Assets" || text == "assets")
			{
				text = "Resources";
			}
			string text2 = null;
			if (param.Count == 2)
			{
				text2 = (string)param[1];
			}
			if (SharedUtils.ShowLog)
			{
				SharedUtils.Log("PocoRpc Start:" + text + "," + text2);
			}
			UwaLocalStarter.StartWithPoco(text, text2);
			return true;
		}

		// Token: 0x06000155 RID: 341 RVA: 0x00008B5C File Offset: 0x00006D5C
		[Preserve]
		private static object Stop(List<object> param)
		{
			if (SharedUtils.ShowLog)
			{
				SharedUtils.Log("PocoRpc Stop:" + param.Count.ToString());
			}
			UwaLocalStarter.StopWithPoco();
			return true;
		}

		// Token: 0x06000156 RID: 342 RVA: 0x00008BA0 File Offset: 0x00006DA0
		[Preserve]
		private static object StartUploadUserpwd(List<object> param)
		{
			if (SharedUtils.ShowLog)
			{
				SharedUtils.Log("PocoRpc StartUploadUserpwd:" + param.Count.ToString());
			}
			if (param.Count != 4)
			{
				return false;
			}
			string[] array = new string[4];
			for (int i = 0; i < 4; i++)
			{
				array[i] = param[i].ToString();
			}
			PocoTool.StartUploadUserpwd_internal(array);
			return true;
		}

		// Token: 0x06000157 RID: 343 RVA: 0x00008C24 File Offset: 0x00006E24
		[Preserve]
		private static object StartUploadPoco(List<object> param)
		{
			if (SharedUtils.ShowLog)
			{
				SharedUtils.Log("PocoRpc StartUploadPoco:" + param.Count.ToString());
			}
			if (param.Count != 4)
			{
				return false;
			}
			string[] array = new string[4];
			for (int i = 0; i < 4; i++)
			{
				array[i] = param[i].ToString();
			}
			PocoTool.StartUploadPoco_internal(array);
			return true;
		}

		// Token: 0x06000158 RID: 344 RVA: 0x00008CA8 File Offset: 0x00006EA8
		[Preserve]
		private static object StartUploadPocoWithNote(List<object> param)
		{
			if (SharedUtils.ShowLog)
			{
				SharedUtils.Log("PocoRpc StartUploadPocoWithNote:" + param.Count.ToString());
			}
			if (param.Count != 5)
			{
				return false;
			}
			string[] array = new string[5];
			for (int i = 0; i < 5; i++)
			{
				array[i] = param[i].ToString();
			}
			PocoTool.StartUploadPoco_internal(array);
			return true;
		}

		// Token: 0x06000159 RID: 345 RVA: 0x00008D2C File Offset: 0x00006F2C
		[Preserve]
		private static object CheckUploadResult(List<object> param)
		{
			if (SharedUtils.ShowLog)
			{
				SharedUtils.Log("PocoRpc CheckUploadResult:" + param.Count.ToString());
			}
			if (PocoTool._uploadresult == null)
			{
				return new object[]
				{
					false
				};
			}
			return new object[]
			{
				true,
				PocoTool._uploadresult.Value,
				PocoTool._uploadinfo
			};
		}

		// Token: 0x0600015A RID: 346 RVA: 0x00008DAC File Offset: 0x00006FAC
		private static void UploadCallback(bool suc, string info)
		{
			PocoTool._uploadresult = new bool?(suc);
			PocoTool._uploadinfo = info;
			PocoTool._uploading = false;
		}

		// Token: 0x0600015B RID: 347 RVA: 0x00008DC8 File Offset: 0x00006FC8
		private static bool StartUploadUserpwd_internal(string[] ps)
		{
			PocoTool._uploadinfo = "";
			PocoTool._uploadresult = null;
			if (PocoTool._uploading)
			{
				PocoTool.UploadCallback(false, "Already uploading, or test is not stopped.");
				return false;
			}
			if (ps.Length == 4)
			{
				PocoTool._uploading = true;
				try
				{
					if (SharedUtils.ShowLog)
					{
						SharedUtils.Log("startuploaduserpwd:" + ps[0]);
					}
					DataUploader.TryOneKeyUploadUP(new Action<bool, string>(PocoTool.UploadCallback), new object[]
					{
						ps[0],
						ps[1],
						int.Parse(ps[2]),
						int.Parse(ps[3])
					});
					return true;
				}
				catch (Exception ex)
				{
					bool suc = false;
					string str = "TryOneKeyUpload Exception:";
					Exception ex2 = ex;
					PocoTool.UploadCallback(suc, str + ((ex2 != null) ? ex2.ToString() : null));
					return false;
				}
			}
			PocoTool.UploadCallback(false, "Wrong Param Count.");
			return false;
		}

		// Token: 0x0600015C RID: 348 RVA: 0x00008ED4 File Offset: 0x000070D4
		private static bool StartUploadPoco_internal(string[] ps)
		{
			PocoTool._uploadinfo = "";
			PocoTool._uploadresult = null;
			if (PocoTool._uploading)
			{
				PocoTool.UploadCallback(false, "Already uploading, or test is not stopped.");
				return false;
			}
			if (UwaLocalStarter.Get().TestState != UwaLocalStarter.eTestState.Stopped)
			{
				PocoTool.UploadCallback(false, "Test is not stopped.");
				return false;
			}
			if (ps.Length == 4 || ps.Length == 5)
			{
				PocoTool._uploading = true;
				try
				{
					if (SharedUtils.ShowLog)
					{
						SharedUtils.Log("startupload:" + ps[0]);
					}
					if (ps.Length == 4)
					{
						DataUploader.TryOneKeyUploadPoco(new Action<bool, string>(PocoTool.UploadCallback), new object[]
						{
							ps[0],
							ps[1],
							int.Parse(ps[2]),
							int.Parse(ps[3])
						});
					}
					if (ps.Length == 5)
					{
						DataUploader.TryOneKeyUploadPoco(new Action<bool, string>(PocoTool.UploadCallback), new object[]
						{
							ps[0],
							ps[1],
							int.Parse(ps[2]),
							int.Parse(ps[3]),
							ps[4]
						});
					}
					return true;
				}
				catch (Exception ex)
				{
					bool suc = false;
					string str = "TryOneKeyUploadPoco Exception:";
					Exception ex2 = ex;
					PocoTool.UploadCallback(suc, str + ((ex2 != null) ? ex2.ToString() : null));
					return false;
				}
			}
			PocoTool.UploadCallback(false, "Wrong Param Count.");
			return false;
		}

		// Token: 0x0600015D RID: 349 RVA: 0x00009074 File Offset: 0x00007274
		public static bool AddGOTRPC()
		{
			PocoTool.PocoManagerObj = null;
			object[] array = Resources.FindObjectsOfTypeAll(PocoTool.PocoManagerType);
			object[] array2 = array;
			try
			{
				for (int i = 0; i < array2.Length; i++)
				{
					if (array2[i] != null && PocoTool.RpcField.GetValue(array2[i]) != null)
					{
						PocoTool.PocoManagerObj = array2[i];
						break;
					}
				}
			}
			catch (Exception)
			{
			}
			if (SharedUtils.ShowLog)
			{
				string str = "PocoManagerObj : ";
				object pocoManagerObj = PocoTool.PocoManagerObj;
				SharedUtils.Log(str + ((pocoManagerObj != null) ? pocoManagerObj.ToString() : null));
			}
			if (PocoTool.PocoManagerObj != null)
			{
				object value = PocoTool.RpcField.GetValue(PocoTool.PocoManagerObj);
				MethodInfo method = PocoTool.RPCParserType.GetMethod("addRpcMethod");
				Type nestedType = PocoTool.RPCParserType.GetNestedType("RpcMethod");
				Delegate @delegate = Delegate.CreateDelegate(nestedType, typeof(PocoTool), "Connect");
				Delegate delegate2 = Delegate.CreateDelegate(nestedType, typeof(PocoTool), "Tag");
				Delegate delegate3 = Delegate.CreateDelegate(nestedType, typeof(PocoTool), "Note");
				Delegate delegate4 = Delegate.CreateDelegate(nestedType, typeof(PocoTool), "Dump");
				Delegate delegate5 = Delegate.CreateDelegate(nestedType, typeof(PocoTool), "StatPoco");
				Delegate delegate6 = Delegate.CreateDelegate(nestedType, typeof(PocoTool), "Start");
				Delegate delegate7 = Delegate.CreateDelegate(nestedType, typeof(PocoTool), "Stop");
				Delegate delegate8 = Delegate.CreateDelegate(nestedType, typeof(PocoTool), "StartUploadUserpwd");
				Delegate delegate9 = Delegate.CreateDelegate(nestedType, typeof(PocoTool), "StartUploadPoco");
				Delegate delegate10 = Delegate.CreateDelegate(nestedType, typeof(PocoTool), "StartUploadPocoWithNote");
				Delegate delegate11 = Delegate.CreateDelegate(nestedType, typeof(PocoTool), "CheckUploadResult");
				method.Invoke(value, new object[]
				{
					"UWA.Connect",
					@delegate
				});
				method.Invoke(value, new object[]
				{
					"UWA.Tag",
					delegate2
				});
				method.Invoke(value, new object[]
				{
					"UWA.Note",
					delegate3
				});
				method.Invoke(value, new object[]
				{
					"UWA.Dump",
					delegate4
				});
				method.Invoke(value, new object[]
				{
					"UWA.StatPoco",
					delegate5
				});
				method.Invoke(value, new object[]
				{
					"UWA.Start",
					delegate6
				});
				method.Invoke(value, new object[]
				{
					"UWA.Stop",
					delegate7
				});
				method.Invoke(value, new object[]
				{
					"UWA.StartUploadUserpwd",
					delegate8
				});
				method.Invoke(value, new object[]
				{
					"UWA.StartUploadPoco",
					delegate9
				});
				method.Invoke(value, new object[]
				{
					"UWA.StartUploadPocoWithNote",
					delegate10
				});
				method.Invoke(value, new object[]
				{
					"UWA.CheckUploadResult",
					delegate11
				});
				return true;
			}
			return false;
		}

		// Token: 0x04000099 RID: 153
		private static bool? pocoChecked = null;

		// Token: 0x0400009A RID: 154
		private static bool _pocoManagerExist = false;

		// Token: 0x0400009B RID: 155
		internal static bool _uploading = false;

		// Token: 0x0400009C RID: 156
		internal static bool? _uploadresult = null;

		// Token: 0x0400009D RID: 157
		internal static string _uploadinfo = "";

		// Token: 0x0400009E RID: 158
		private static object PocoManagerObj = null;

		// Token: 0x0400009F RID: 159
		private static FieldInfo RpcField = null;

		// Token: 0x040000A0 RID: 160
		private static Type PocoManagerType = null;

		// Token: 0x040000A1 RID: 161
		private static Type RPCParserType = null;
	}
}
