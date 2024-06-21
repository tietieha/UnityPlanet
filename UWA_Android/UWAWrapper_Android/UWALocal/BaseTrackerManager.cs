using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UWA;

namespace UWALocal
{
	// Token: 0x0200000E RID: 14
	internal abstract class BaseTrackerManager
	{
		// Token: 0x06000098 RID: 152
		[DllImport("uwa", CallingConvention = CallingConvention.Cdecl)]
		public static extern void Dump(int mode);

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000099 RID: 153 RVA: 0x000054CC File Offset: 0x000036CC
		public string Name
		{
			get
			{
				if (this._name == null)
				{
					this._name = base.GetType().Name;
				}
				return this._name;
			}
		}

		// Token: 0x0600009A RID: 154 RVA: 0x000054F0 File Offset: 0x000036F0
		public static void PrepareAll()
		{
			for (int i = 0; i < BaseTrackerManager.AllTrackManager.Count; i++)
			{
				BaseTrackerManager baseTrackerManager = BaseTrackerManager.AllTrackManager[i];
				baseTrackerManager.Prepare();
			}
		}

		// Token: 0x0600009B RID: 155 RVA: 0x0000552C File Offset: 0x0000372C
		public static void StartTrackAll()
		{
			for (int i = 0; i < BaseTrackerManager.AllTrackManager.Count; i++)
			{
				BaseTrackerManager baseTrackerManager = BaseTrackerManager.AllTrackManager[i];
				baseTrackerManager.StartTrack();
			}
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00005568 File Offset: 0x00003768
		public static void StopTrackAll()
		{
			for (int i = 0; i < BaseTrackerManager.AllTrackManager.Count; i++)
			{
				BaseTrackerManager baseTrackerManager = BaseTrackerManager.AllTrackManager[i];
				baseTrackerManager.StopTrack();
			}
		}

		// Token: 0x0600009D RID: 157 RVA: 0x000055A4 File Offset: 0x000037A4
		public static void ClearAll()
		{
			BaseTrackerManager.AllTrackManager.Clear();
		}

		// Token: 0x0600009E RID: 158 RVA: 0x000055B0 File Offset: 0x000037B0
		public static void CheckTimeAndSwitchLogFileAll()
		{
			for (int i = 0; i < BaseTrackerManager.AllTrackManager.Count; i++)
			{
				BaseTrackerManager baseTrackerManager = BaseTrackerManager.AllTrackManager[i];
				baseTrackerManager.CheckTimeAndSwitchLogFile();
			}
		}

		// Token: 0x0600009F RID: 159 RVA: 0x000055EC File Offset: 0x000037EC
		public static void UpdateAtEndAll()
		{
			for (int i = 0; i < BaseTrackerManager.AllTrackManager.Count; i++)
			{
				BaseTrackerManager baseTrackerManager = BaseTrackerManager.AllTrackManager[i];
				baseTrackerManager.UpdateAtEnd();
			}
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00005628 File Offset: 0x00003828
		public static void LateUpdateAll()
		{
			for (int i = 0; i < BaseTrackerManager.AllTrackManager.Count; i++)
			{
				BaseTrackerManager baseTrackerManager = BaseTrackerManager.AllTrackManager[i];
				baseTrackerManager.LateUpdate();
			}
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00005664 File Offset: 0x00003864
		public static void SwitchLogFileAll()
		{
			for (int i = 0; i < BaseTrackerManager.AllTrackManager.Count; i++)
			{
				BaseTrackerManager baseTrackerManager = BaseTrackerManager.AllTrackManager[i];
				baseTrackerManager.SwitchLogFile(false);
			}
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x000056A0 File Offset: 0x000038A0
		public static bool IsAllReady(out string errors)
		{
			errors = null;
			bool flag = true;
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < BaseTrackerManager.AllTrackManager.Count; i++)
			{
				BaseTrackerManager baseTrackerManager = BaseTrackerManager.AllTrackManager[i];
				string value = null;
				if (!baseTrackerManager.IsReady(out value))
				{
					stringBuilder.AppendLine(value);
					flag = false;
				}
			}
			if (!flag)
			{
				errors = stringBuilder.ToString();
			}
			return flag;
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x0000570C File Offset: 0x0000390C
		protected BaseTrackerManager(string extension, int bufferSize = 200)
		{
			this._ext = extension;
			this._trackTimeInterval = 2.1474836E+09f;
			this._trackFrameInterval = 2.1474836E+09f;
			this._trackTimeSum = 0f;
			this._trackFrameSum = 0f;
			this.Inited = false;
			this.Enabled = false;
			this.Tracking = false;
			this.TrackWriter = (this._ext.Equals("") ? null : new TrackWriter<string>(bufferSize));
			BaseTrackerManager.AllTrackManager.Add(this);
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x000057A0 File Offset: 0x000039A0
		public void Init(Dictionary<string, string> config)
		{
			if (this.Inited)
			{
				return;
			}
			if (config.ContainsKey("Enable"))
			{
				this.Enabled = config["Enable"].Equals("true", StringComparison.OrdinalIgnoreCase);
			}
			if (this.Enabled)
			{
				if (config.ContainsKey("TimeInterval"))
				{
					this._trackTimeInterval = float.Parse(config["TimeInterval"]);
					this._trackFrameInterval = this._trackTimeInterval * 60f;
				}
				this.InitWithConfig(config);
			}
			this.Inited = true;
		}

		// Token: 0x060000A5 RID: 165
		protected abstract void InitWithConfig(Dictionary<string, string> config);

		// Token: 0x060000A6 RID: 166 RVA: 0x0000583C File Offset: 0x00003A3C
		protected virtual void Prepare()
		{
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00005840 File Offset: 0x00003A40
		public virtual void StartTrack()
		{
			if (!this.Enabled)
			{
				return;
			}
			if (this.Tracking)
			{
				return;
			}
			this.Tracking = true;
			if (string.IsNullOrEmpty(this.LogFile))
			{
				this.LogFile = this.GetTargetLogName();
			}
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x0000588C File Offset: 0x00003A8C
		public virtual void StopTrack()
		{
			if (!this.Enabled)
			{
				return;
			}
			if (!this.Tracking)
			{
				return;
			}
			this.SwitchLogFile(true);
			this.Tracking = false;
			this.Inited = false;
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x000058CC File Offset: 0x00003ACC
		protected virtual string GetTargetLogName()
		{
			return CoreUtils.GetFrameIdWithExtFilePath(this._ext);
		}

		// Token: 0x060000AA RID: 170 RVA: 0x000058DC File Offset: 0x00003ADC
		public void UpdateAtEnd()
		{
			if (!this.Enabled)
			{
				return;
			}
			if (!this.Tracking)
			{
				return;
			}
			this.DoUpdateAtEnd();
		}

		// Token: 0x060000AB RID: 171 RVA: 0x000058FC File Offset: 0x00003AFC
		protected virtual void DoUpdateAtEnd()
		{
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00005900 File Offset: 0x00003B00
		public void LateUpdate()
		{
			if (!this.Enabled)
			{
				return;
			}
			if (!this.Tracking)
			{
				return;
			}
			this.DoLateUpdate();
		}

		// Token: 0x060000AD RID: 173 RVA: 0x00005920 File Offset: 0x00003B20
		protected virtual void DoLateUpdate()
		{
		}

		// Token: 0x060000AE RID: 174 RVA: 0x00005924 File Offset: 0x00003B24
		public void CheckTimeAndSwitchLogFile()
		{
			if (!this.Enabled)
			{
				return;
			}
			if (!this.Tracking)
			{
				return;
			}
			this._trackTimeSum += Time.unscaledDeltaTime;
			this._trackFrameSum += 1f;
			if (this.UseFrameInterval)
			{
				if (this._trackFrameSum > this._trackFrameInterval)
				{
					this.SwitchLogFile(false);
					this._trackFrameSum = 0f;
					return;
				}
			}
			else if (this._trackTimeSum > this._trackTimeInterval)
			{
				this.SwitchLogFile(false);
				this._trackTimeSum = 0f;
			}
		}

		// Token: 0x060000AF RID: 175 RVA: 0x000059C4 File Offset: 0x00003BC4
		public virtual void SwitchLogFile(bool end = false)
		{
			if (!this.Enabled)
			{
				return;
			}
			if (!this.Tracking)
			{
				return;
			}
			if (string.IsNullOrEmpty(this.LogFile))
			{
				return;
			}
			if (end)
			{
				this.LogFile = "";
			}
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x00005A00 File Offset: 0x00003C00
		public virtual int GetLogFileCount()
		{
			string[] files = Directory.GetFiles(SharedUtils.FinalDataPath, "*" + this._ext);
			return files.Length;
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x00005A30 File Offset: 0x00003C30
		public virtual long GetLogFileSize()
		{
			string[] files = Directory.GetFiles(SharedUtils.FinalDataPath, "*" + this._ext);
			long num = 0L;
			for (int i = 0; i < files.Length; i++)
			{
				FileInfo fileInfo = new FileInfo(files[i]);
				num += fileInfo.Length;
			}
			return num;
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x00005A88 File Offset: 0x00003C88
		public virtual bool IsReady(out string error)
		{
			error = null;
			return true;
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x060000B3 RID: 179 RVA: 0x00005A90 File Offset: 0x00003C90
		// (set) Token: 0x060000B4 RID: 180 RVA: 0x00005AC4 File Offset: 0x00003CC4
		protected virtual string LogFile
		{
			get
			{
				if (!this.Enabled || !this.Tracking || this.TrackWriter == null)
				{
					return "";
				}
				return this.TrackWriter.LogPath;
			}
			set
			{
				if (this.Enabled && this.Tracking && this.TrackWriter != null)
				{
					this.TrackWriter.LogPath = value;
				}
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x060000B5 RID: 181 RVA: 0x00005AF4 File Offset: 0x00003CF4
		// (set) Token: 0x060000B6 RID: 182 RVA: 0x00005AFC File Offset: 0x00003CFC
		public bool Enabled { get; set; }

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x060000B7 RID: 183 RVA: 0x00005B08 File Offset: 0x00003D08
		// (set) Token: 0x060000B8 RID: 184 RVA: 0x00005B10 File Offset: 0x00003D10
		public bool Tracking { get; private set; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x060000B9 RID: 185 RVA: 0x00005B1C File Offset: 0x00003D1C
		// (set) Token: 0x060000BA RID: 186 RVA: 0x00005B24 File Offset: 0x00003D24
		protected bool UseFrameInterval { get; set; }

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x060000BB RID: 187 RVA: 0x00005B30 File Offset: 0x00003D30
		// (set) Token: 0x060000BC RID: 188 RVA: 0x00005B38 File Offset: 0x00003D38
		private protected TrackWriter<string> TrackWriter { protected get; private set; }

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x060000BD RID: 189 RVA: 0x00005B44 File Offset: 0x00003D44
		// (set) Token: 0x060000BE RID: 190 RVA: 0x00005B4C File Offset: 0x00003D4C
		private protected bool Inited { protected get; private set; }

		// Token: 0x0400003E RID: 62
		private string _name;

		// Token: 0x0400003F RID: 63
		private static readonly List<BaseTrackerManager> AllTrackManager = new List<BaseTrackerManager>();

		// Token: 0x04000045 RID: 69
		private string _ext;

		// Token: 0x04000046 RID: 70
		protected float _trackTimeInterval;

		// Token: 0x04000047 RID: 71
		private float _trackTimeSum;

		// Token: 0x04000048 RID: 72
		protected float _trackFrameInterval;

		// Token: 0x04000049 RID: 73
		private float _trackFrameSum;
	}
}
