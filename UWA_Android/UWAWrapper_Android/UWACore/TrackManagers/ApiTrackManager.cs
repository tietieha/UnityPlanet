using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Profiling;
using UWA;

namespace UWACore.TrackManagers
{
	// Token: 0x02000036 RID: 54
	internal class ApiTrackManager : BaseTrackerManager, IUWAAPI, IUWAAPIInternal
	{
		// Token: 0x06000261 RID: 609 RVA: 0x0000FE14 File Offset: 0x0000E014
		public ApiTrackManager(string extension) : base(extension, 200)
		{
			this._limitLogValueCount = 0;
			this._limitRegisterValueCount = 0;
			this._limitPushSampleCount = 0;
			this._loggedValue = new HashSet<string>();
			this._pushedSamples = new HashSet<string>();
		}

		// Token: 0x06000262 RID: 610 RVA: 0x0000FE74 File Offset: 0x0000E074
		protected override void InitWithConfig(Dictionary<string, string> config)
		{
			bool flag = config.Count == 0;
			if (!flag)
			{
				this._limitLogValueCount = int.Parse(config["limitLogValueCount"]);
				this._limitRegisterValueCount = int.Parse(config["limitRegisterValueCount"]);
				this._limitPushSampleCount = int.Parse(config["limitPushSampleCount"]);
				ApiTrackManager._customApiFile = SharedUtils.FinalDataPath + "/customApi";
				this.ReadCustomApiFile();
			}
		}

		// Token: 0x06000263 RID: 611 RVA: 0x0000FEF8 File Offset: 0x0000E0F8
		private void ReadCustomApiFile()
		{
			bool flag = File.Exists(ApiTrackManager._customApiFile);
			if (flag)
			{
				string[] array = File.ReadAllLines(ApiTrackManager._customApiFile);
				foreach (string text in array)
				{
					string item = text.Replace(UWAEngineInternal.SamplePrefix, "");
					bool flag2 = !this._pushedSamples.Contains(item);
					if (flag2)
					{
						this._pushedSamples.Add(item);
					}
				}
			}
		}

		// Token: 0x06000264 RID: 612 RVA: 0x0000FF84 File Offset: 0x0000E184
		public override void StartTrack()
		{
			base.StartTrack();
			bool enabled = base.Enabled;
			if (enabled)
			{
				Type wrapperType = CoreUtils.GetWrapperType("UWAEngine");
				bool flag = wrapperType != null;
				if (flag)
				{
					FieldInfo field = wrapperType.GetField("_instance", BindingFlags.Static | BindingFlags.Public);
					bool flag2 = field != null;
					if (flag2)
					{
						field.SetValue(null, this);
					}
					MethodInfo method = wrapperType.GetMethod("Set", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
					bool flag3 = method != null;
					if (flag3)
					{
						method.Invoke(null, new object[]
						{
							this
						});
					}
				}
				UWAEngineInternal.Instance = this;
			}
		}

		// Token: 0x06000265 RID: 613 RVA: 0x00010020 File Offset: 0x0000E220
		public override void StopTrack()
		{
			base.StopTrack();
			Type wrapperType = CoreUtils.GetWrapperType("UWAEngine");
			bool flag = wrapperType != null;
			if (flag)
			{
				FieldInfo field = wrapperType.GetField("_instance", BindingFlags.Static | BindingFlags.Public);
				bool flag2 = field != null;
				if (flag2)
				{
					field.SetValue(null, null);
				}
				MethodInfo method = wrapperType.GetMethod("Set", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				bool flag3 = method != null;
				if (flag3)
				{
					method.Invoke(null, new object[1]);
				}
			}
			UWAEngineInternal.Instance = null;
		}

		// Token: 0x06000266 RID: 614 RVA: 0x000100A8 File Offset: 0x0000E2A8
		protected override void DoUpdateAtEnd()
		{
			this._pushSampleStack.Clear();
		}

		// Token: 0x06000267 RID: 615 RVA: 0x000100B8 File Offset: 0x0000E2B8
		public void SetUIActive(bool active)
		{
		}

		// Token: 0x06000268 RID: 616 RVA: 0x000100BC File Offset: 0x0000E2BC
		public void Tag(string tag)
		{
		}

		// Token: 0x06000269 RID: 617 RVA: 0x000100C0 File Offset: 0x0000E2C0
		public void Note(string note)
		{
		}

		// Token: 0x0600026A RID: 618 RVA: 0x000100C4 File Offset: 0x0000E2C4
		public void PushSample(string sampleName)
		{
			this._pushSampleStack.Push(sampleName);
			Profiler.BeginSample("UWAAPI");
			string text = this.CheckSampleToPush(sampleName);
			Profiler.EndSample();
			bool flag = text == null;
			if (!flag)
			{
				Profiler.BeginSample(text);
			}
		}

		// Token: 0x0600026B RID: 619 RVA: 0x00010114 File Offset: 0x0000E314
		private void WriteCustomApi(string api)
		{
			StreamWriter streamWriter = new StreamWriter(ApiTrackManager._customApiFile, true);
			streamWriter.WriteLine(api);
			streamWriter.Close();
		}

		// Token: 0x0600026C RID: 620 RVA: 0x00010144 File Offset: 0x0000E344
		public void PopSample()
		{
			bool flag = this._pushSampleStack.Count <= 0;
			if (!flag)
			{
				string item = this._pushSampleStack.Pop();
				bool flag2 = !this._pushedSamples.Contains(item);
				if (!flag2)
				{
					Profiler.EndSample();
				}
			}
		}

		// Token: 0x0600026D RID: 621 RVA: 0x000101A0 File Offset: 0x0000E3A0
		private void LogRegisterValue(string valueName, object value, ApiTrackManager.TrackTag tag)
		{
			string text = "";
			bool flag = value is int;
			if (flag)
			{
				text = "I";
			}
			bool flag2 = value is bool;
			if (flag2)
			{
				text = "B";
			}
			bool flag3 = value is float;
			if (flag3)
			{
				text = "F";
			}
			bool flag4 = value is Vector3;
			if (flag4)
			{
				text = "V";
			}
			base.TrackWriter.WriteToBuffer(string.Format("{0},{1},{2},{3},{4}", new object[]
			{
				tag,
				SharedUtils.frameId,
				text,
				valueName,
				value
			}));
		}

		// Token: 0x0600026E RID: 622 RVA: 0x00010250 File Offset: 0x0000E450
		public void LogValue(string valueName, float value)
		{
			bool flag = !this.CheckValueToLog(valueName);
			if (!flag)
			{
				this.LogValueInternal(valueName, value, ApiTrackManager.TrackTag.Custom);
			}
		}

		// Token: 0x0600026F RID: 623 RVA: 0x00010284 File Offset: 0x0000E484
		public void LogValue(string valueName, int value)
		{
			bool flag = !this.CheckValueToLog(valueName);
			if (!flag)
			{
				this.LogValueInternal(valueName, value, ApiTrackManager.TrackTag.Custom);
			}
		}

		// Token: 0x06000270 RID: 624 RVA: 0x000102B8 File Offset: 0x0000E4B8
		public void LogValue(string valueName, bool value)
		{
			bool flag = !this.CheckValueToLog(valueName);
			if (!flag)
			{
				this.LogValueInternal(valueName, value, ApiTrackManager.TrackTag.Custom);
			}
		}

		// Token: 0x06000271 RID: 625 RVA: 0x000102EC File Offset: 0x0000E4EC
		public void LogValue(string valueName, Vector3 value)
		{
			bool flag = !this.CheckValueToLog(valueName);
			if (!flag)
			{
				this.LogValueInternal(valueName, value, ApiTrackManager.TrackTag.Custom);
			}
		}

		// Token: 0x06000272 RID: 626 RVA: 0x00010320 File Offset: 0x0000E520
		public void AddMarker(string valuename)
		{
		}

		// Token: 0x06000273 RID: 627 RVA: 0x00010324 File Offset: 0x0000E524
		public void LogValueInternal(string valueName, float value, ApiTrackManager.TrackTag tag)
		{
			base.TrackWriter.WriteToBuffer(string.Format("{0},{1},F,{2},{3}", new object[]
			{
				tag,
				SharedUtils.frameId,
				valueName,
				value
			}));
		}

		// Token: 0x06000274 RID: 628 RVA: 0x00010378 File Offset: 0x0000E578
		public void LogValueInternal(string valueName, int value, ApiTrackManager.TrackTag tag)
		{
			base.TrackWriter.WriteToBuffer(string.Format("{0},{1},I,{2},{3}", new object[]
			{
				tag,
				SharedUtils.frameId,
				valueName,
				value
			}));
		}

		// Token: 0x06000275 RID: 629 RVA: 0x000103CC File Offset: 0x0000E5CC
		public void LogValueInternal(string valueName, bool value, ApiTrackManager.TrackTag tag)
		{
			base.TrackWriter.WriteToBuffer(string.Format("{0},{1},B,{2},{3}", new object[]
			{
				tag,
				SharedUtils.frameId,
				valueName,
				value
			}));
		}

		// Token: 0x06000276 RID: 630 RVA: 0x00010420 File Offset: 0x0000E620
		public void LogValueInternal(string valueName, Vector3 value, ApiTrackManager.TrackTag tag)
		{
			base.TrackWriter.WriteToBuffer(string.Format("{0},{1},V,{2},{3},{4},{5}", new object[]
			{
				tag,
				SharedUtils.frameId,
				valueName,
				value.x,
				value.y,
				value.z
			}));
		}

		// Token: 0x06000277 RID: 631 RVA: 0x00010494 File Offset: 0x0000E694
		private bool CheckValueToLog(string valueName)
		{
			bool flag = this._loggedValue.Contains(valueName);
			bool flag2 = flag;
			bool result;
			if (flag2)
			{
				result = true;
			}
			else
			{
				bool flag3 = this._loggedValue.Count < this._limitLogValueCount;
				if (flag3)
				{
					this._loggedValue.Add(valueName);
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		// Token: 0x06000278 RID: 632 RVA: 0x000104FC File Offset: 0x0000E6FC
		private string CheckSampleToPush(string sampleName)
		{
			bool flag = this._pushedSamples.Contains(sampleName);
			bool flag2 = flag && this._uwaSampleName.ContainsKey(sampleName);
			string result;
			if (flag2)
			{
				result = this._uwaSampleName[sampleName];
			}
			else
			{
				bool flag3 = this._pushedSamples.Count < this._limitPushSampleCount;
				if (flag3)
				{
					string text = UWAEngineInternal.SamplePrefix + sampleName;
					this._pushedSamples.Add(sampleName);
					this._uwaSampleName.Add(sampleName, text);
					this.WriteCustomApi(text);
					result = text;
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		// Token: 0x06000279 RID: 633 RVA: 0x000105AC File Offset: 0x0000E7AC
		public void Register(Type classType, string fieldName, float updateInterval)
		{
		}

		// Token: 0x0600027A RID: 634 RVA: 0x000105B0 File Offset: 0x0000E7B0
		public void Register(object classObj, string instanceName, string fieldName, float updateInterval)
		{
		}

		// Token: 0x0400015B RID: 347
		private readonly Dictionary<string, string> _uwaSampleName = new Dictionary<string, string>();

		// Token: 0x0400015C RID: 348
		private static readonly Type[] SupportedTypes = new Type[]
		{
			typeof(int),
			typeof(Vector3),
			typeof(bool),
			typeof(float)
		};

		// Token: 0x0400015D RID: 349
		private Stack<string> _pushSampleStack = new Stack<string>();

		// Token: 0x0400015E RID: 350
		private int _limitLogValueCount;

		// Token: 0x0400015F RID: 351
		private int _limitRegisterValueCount;

		// Token: 0x04000160 RID: 352
		private int _limitPushSampleCount;

		// Token: 0x04000161 RID: 353
		private static string _customApiFile;

		// Token: 0x04000162 RID: 354
		private HashSet<string> _loggedValue;

		// Token: 0x04000163 RID: 355
		private HashSet<string> _pushedSamples;

		// Token: 0x020000F5 RID: 245
		public enum TrackTag
		{
			// Token: 0x04000627 RID: 1575
			Ngui,
			// Token: 0x04000628 RID: 1576
			Ugui,
			// Token: 0x04000629 RID: 1577
			Custom
		}
	}
}
