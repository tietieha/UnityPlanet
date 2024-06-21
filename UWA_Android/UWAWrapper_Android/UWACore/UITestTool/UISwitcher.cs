using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UWA;
using UWACore.TrackManagers;
using UWACore.Util;

namespace UWACore.UITestTool
{
	// Token: 0x0200004E RID: 78
	internal class UISwitcher : MonoBehaviour
	{
		// Token: 0x0600035C RID: 860 RVA: 0x0001702C File Offset: 0x0001522C
		private void Start()
		{
			this._toolWindowRect = SharedUtils.GroupRect;
			this.dataRoot = SharedUtils.FinalDataPath;
		}

		// Token: 0x0600035D RID: 861 RVA: 0x00017048 File Offset: 0x00015248
		private void OnGUI()
		{
			bool showGUI = this._showGUI;
			if (showGUI)
			{
				this._toolWindowRect = GUI.Window(102938, this._toolWindowRect, new GUI.WindowFunction(this.DoUiSwitcher), "UWA UI Switch");
			}
		}

		// Token: 0x0600035E RID: 862 RVA: 0x00017090 File Offset: 0x00015290
		private void DoUiSwitcher(int id)
		{
			GUILayout.Space(30f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			bool mainFirst = this._mainFirst;
			if (mainFirst)
			{
				GUILayout.Label("主界面", SharedUtils.GetSuitableOption(0.2f, 0.5f));
			}
			else
			{
				this._otherPanel = GUILayout.TextField(this._otherPanel, SharedUtils.GetSuitableOption(0.2f, 0.5f));
			}
			bool flag = GUILayout.Button(" <=> ", SharedUtils.GetSuitableOption(0.1f, 0.5f));
			if (flag)
			{
				this._mainFirst = !this._mainFirst;
			}
			bool mainFirst2 = this._mainFirst;
			if (mainFirst2)
			{
				this._otherPanel = GUILayout.TextField(this._otherPanel, SharedUtils.GetSuitableOption(0.2f, 0.5f));
			}
			else
			{
				GUILayout.Label("主界面", SharedUtils.GetSuitableOption(0.2f, 0.5f));
			}
			bool flag2 = this._currentTask == null;
			if (flag2)
			{
				GUI.enabled = this._done;
				bool flag3 = GUILayout.Button("Start", SharedUtils.GetSuitableOption(0.2f, 0.5f));
				if (flag3)
				{
					this._showGUI = false;
					base.StartCoroutine(this.StartLog());
				}
				GUI.enabled = true;
			}
			bool flag4 = this._currentTask != null && GUILayout.Button("Stop", SharedUtils.GetSuitableOption(0.2f, 0.5f));
			if (flag4)
			{
				this._showGUI = false;
				base.StartCoroutine(this.StopLog());
			}
			bool flag5 = GUILayout.Button(this._currentFrame.ToString(), SharedUtils.GetSuitableOption(0.1f, 0.5f)) && this._currentTask == null;
			if (flag5)
			{
				GC.Collect();
			}
			GUILayout.EndHorizontal();
			GUI.DragWindow();
		}

		// Token: 0x0600035F RID: 863 RVA: 0x00017278 File Offset: 0x00015478
		private IEnumerator StartLog()
		{
			this._currentFrame = 0;
			this._done = false;
			yield return new WaitForEndOfFrame();
			UISwitcher.SwitchTask task = new UISwitcher.SwitchTask
			{
				DataId = DateTime.Now.ToString("yyyyMMddHHmmss"),
				PanelA = (this._mainFirst ? "主界面" : this._otherPanel),
				PanelB = ((!this._mainFirst) ? "主界面" : this._otherPanel)
			};
			this._dataMap.Add(task.DataId, task);
			this._currentTask = task;
			UProfiler.BeginRecord(this.dataRoot + "/" + this._currentTask.DataId);
			base.StartCoroutine(ScreenTrackManager.DelayScreenshot(this.dataRoot + "/" + this._currentTask.DataId + ".A.jpg"));
			yield break;
		}

		// Token: 0x06000360 RID: 864 RVA: 0x00017288 File Offset: 0x00015488
		private IEnumerator StopLog()
		{
			this._currentFrame = 0;
			yield return new WaitForEndOfFrame();
			UProfiler.EndRecord();
			base.StartCoroutine(ScreenTrackManager.DelayScreenshot(this.dataRoot + "/" + this._currentTask.DataId + ".B.jpg"));
			this._currentTask = null;
			this._mainFirst = !this._mainFirst;
			base.StartCoroutine(this.Done());
			yield break;
		}

		// Token: 0x06000361 RID: 865 RVA: 0x00017298 File Offset: 0x00015498
		private IEnumerator Done()
		{
			yield return new WaitForEndOfFrame();
			List<string> output = new List<string>();
			foreach (KeyValuePair<string, UISwitcher.SwitchTask> pair in this._dataMap)
			{
				string info = string.Concat(new string[]
				{
					pair.Key,
					",",
					pair.Value.PanelA,
					",",
					pair.Value.PanelB
				});
				output.Add(info);
				info = null;
				pair = default(KeyValuePair<string, UISwitcher.SwitchTask>);
			}
			Dictionary<string, UISwitcher.SwitchTask>.Enumerator enumerator = default(Dictionary<string, UISwitcher.SwitchTask>.Enumerator);
			string path = this.dataRoot + this.file;
			bool flag = File.Exists(path);
			if (flag)
			{
				File.Delete(path);
			}
			StreamWriter swData = new StreamWriter(path);
			foreach (string item in output)
			{
				swData.WriteLine(item);
				item = null;
			}
			List<string>.Enumerator enumerator2 = default(List<string>.Enumerator);
			swData.Close();
			this._done = true;
			yield break;
		}

		// Token: 0x06000362 RID: 866 RVA: 0x000172A8 File Offset: 0x000154A8
		private void Update()
		{
			bool flag = !this._showGUI;
			if (flag)
			{
				bool flag2 = this._currentHideGUITime > 0.5f;
				if (flag2)
				{
					this._currentHideGUITime = 0f;
					this._showGUI = true;
				}
				this._currentHideGUITime += Time.unscaledDeltaTime;
			}
			bool flag3 = this._currentTask != null;
			if (flag3)
			{
				this._currentFrame++;
			}
		}

		// Token: 0x040001EA RID: 490
		private string dataRoot = "";

		// Token: 0x040001EB RID: 491
		private string _otherPanel = "";

		// Token: 0x040001EC RID: 492
		private Rect _toolWindowRect;

		// Token: 0x040001ED RID: 493
		private bool _showGUI;

		// Token: 0x040001EE RID: 494
		private float _currentHideGUITime = 0f;

		// Token: 0x040001EF RID: 495
		private const float HideGUITime = 0.5f;

		// Token: 0x040001F0 RID: 496
		private bool _mainFirst = true;

		// Token: 0x040001F1 RID: 497
		private readonly Dictionary<string, UISwitcher.SwitchTask> _dataMap = new Dictionary<string, UISwitcher.SwitchTask>();

		// Token: 0x040001F2 RID: 498
		private UISwitcher.SwitchTask _currentTask = null;

		// Token: 0x040001F3 RID: 499
		private int _currentFrame = 0;

		// Token: 0x040001F4 RID: 500
		private bool _done = true;

		// Token: 0x040001F5 RID: 501
		private string file = "/dataInfo-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";

		// Token: 0x040001F6 RID: 502
		private Thread _thread;

		// Token: 0x040001F7 RID: 503
		private int dataIndex = 1;

		// Token: 0x02000103 RID: 259
		private class SwitchTask
		{
			// Token: 0x04000682 RID: 1666
			public string PanelA;

			// Token: 0x04000683 RID: 1667
			public string PanelB;

			// Token: 0x04000684 RID: 1668
			public string DataId;
		}
	}
}
