using System;
using System.Collections.Generic;
using System.Reflection;

namespace UWALocal.Reflect
{
	// Token: 0x02000028 RID: 40
	internal class Recorder
	{
		// Token: 0x060001EF RID: 495 RVA: 0x0000C6A4 File Offset: 0x0000A8A4
		public static Recorder Get(string name)
		{
			object obj = Recorder._recorderGet.Invoke(null, new object[]
			{
				name
			});
			if (obj == null)
			{
				return null;
			}
			if (Recorder._recorderMaps.ContainsKey(obj.GetHashCode()))
			{
				return Recorder._recorderMaps[obj.GetHashCode()];
			}
			Recorder recorder = new Recorder(obj);
			Recorder._recorderMaps.Add(obj.GetHashCode(), recorder);
			return recorder;
		}

		// Token: 0x060001F0 RID: 496 RVA: 0x0000C714 File Offset: 0x0000A914
		public Recorder(object obj)
		{
			this._recorderObj = obj;
			if (Recorder._recorderFilterToCurrentThread != null)
			{
				Recorder._recorderFilterToCurrentThread.Invoke(obj, null);
			}
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x060001F1 RID: 497 RVA: 0x0000C73C File Offset: 0x0000A93C
		public bool isValid
		{
			get
			{
				return (bool)Recorder._recorderInvalid.GetValue(this._recorderObj, null);
			}
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x060001F2 RID: 498 RVA: 0x0000C754 File Offset: 0x0000A954
		// (set) Token: 0x060001F3 RID: 499 RVA: 0x0000C76C File Offset: 0x0000A96C
		public bool enabledX
		{
			get
			{
				return (bool)Recorder._recorderEnabled.GetValue(this._recorderObj, null);
			}
			set
			{
				Recorder._recorderEnabled.SetValue(this._recorderObj, value, null);
			}
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x060001F4 RID: 500 RVA: 0x0000C788 File Offset: 0x0000A988
		public int sampleBlockCountX
		{
			get
			{
				return (int)Recorder._recordersampleBlockCount.GetValue(this._recorderObj, null);
			}
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x060001F5 RID: 501 RVA: 0x0000C7A0 File Offset: 0x0000A9A0
		public long elapsedNanosecondsX
		{
			get
			{
				return (long)Recorder._recorderelapsedNanoseconds.GetValue(this._recorderObj, null);
			}
		}

		// Token: 0x040000EE RID: 238
		private object _recorderObj;

		// Token: 0x040000EF RID: 239
		private static Dictionary<int, Recorder> _recorderMaps = new Dictionary<int, Recorder>();

		// Token: 0x040000F0 RID: 240
		public static PropertyInfo _recorderInvalid = null;

		// Token: 0x040000F1 RID: 241
		public static PropertyInfo _recorderEnabled = null;

		// Token: 0x040000F2 RID: 242
		public static PropertyInfo _recordersampleBlockCount = null;

		// Token: 0x040000F3 RID: 243
		public static PropertyInfo _recorderelapsedNanoseconds = null;

		// Token: 0x040000F4 RID: 244
		public static MethodInfo _recorderGet = null;

		// Token: 0x040000F5 RID: 245
		public static MethodInfo _recorderCollectFromAllThreads = null;

		// Token: 0x040000F6 RID: 246
		public static MethodInfo _recorderFilterToCurrentThread = null;

		// Token: 0x040000F7 RID: 247
		public static Type _recorderType = null;

		// Token: 0x040000F8 RID: 248
		public static bool v20183 = false;

		// Token: 0x040000F9 RID: 249
		public static bool allThread = true;
	}
}
