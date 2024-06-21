using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UWA;
using UWAEditor.Local;

// Token: 0x02000002 RID: 2
public class UWAWindow : EditorWindow
{
	// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
	[MenuItem("Tools/UWA SDK", false, 6)]
	public static void ShowWindow()
	{
		EditorWindow window = EditorWindow.GetWindow(typeof(UWAWindow), false, "UWA_SDK");
		window.minSize = new Vector2(225f, 200f);
	}

	// Token: 0x06000002 RID: 2 RVA: 0x0000208C File Offset: 0x0000028C
	private void OnGUI()
	{
		EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.FlexibleSpace();
		bool flag = BuildPrepareTool.UwaSdkUrl != "" && GUILayout.Button("Document", new GUILayoutOption[0]);
		if (flag)
		{
			Application.OpenURL(BuildPrepareTool.UwaSdkUrl);
		}
		EditorGUILayout.EndHorizontal();
		GUILayout.Space(5f);
		EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Space(5f);
		EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
		this.ShowSdkSupport();
		EditorGUILayout.EndVertical();
		GUILayout.Space(5f);
		EditorGUILayout.EndHorizontal();
	}

	// Token: 0x06000003 RID: 3 RVA: 0x0000213C File Offset: 0x0000033C
	public void OnEnable()
	{
		Localization.eLocale locale = Localization.eLocale.en_US;
		string @string = EditorPrefs.GetString("uwa_locale", "en_US");
		bool flag = Application.systemLanguage == 6;
		if (flag)
		{
			locale = Localization.eLocale.zh_CN;
			@string = EditorPrefs.GetString("uwa_locale", "zh_CN");
		}
		try
		{
			locale = (Localization.eLocale)Enum.Parse(typeof(Localization.eLocale), @string);
		}
		catch (Exception)
		{
		}
		Localization.Instance.SetLocale(locale);
		bool flag2 = this._sdkSupport == null;
		if (flag2)
		{
			string path = BuildPrepareTool.UwaFullPath + "/Runtime/Resources/uwasdksettings.bytes";
			string text = null;
			bool flag3 = File.Exists(path);
			if (flag3)
			{
				text = File.ReadAllText(path);
			}
			bool flag4 = text != null;
			if (flag4)
			{
				this._sdkSupport = new UWAWindow.UwaSdkSettings((JSONClass)JSON.Parse(text));
			}
		}
		bool flag5 = this._sdkSupport == null;
		if (flag5)
		{
			this._sdkSupport = new UWAWindow.UwaSdkSettings();
			string text2 = BuildPrepareTool.UwaFullPath + "/Runtime/Resources";
			bool flag6 = !Directory.Exists(text2);
			if (flag6)
			{
				Directory.CreateDirectory(text2);
			}
			File.WriteAllText(text2 + "/uwasdksettings.bytes", this._sdkSupport.ToJSON());
			AssetDatabase.Refresh();
		}
	}

	// Token: 0x06000004 RID: 4 RVA: 0x00002280 File Offset: 0x00000480
	public void ShowSdkSupport()
	{
		EditorGUILayout.LabelField(Localization.Instance.Get("SDK Settings"), EditorStyles.boldLabel, new GUILayoutOption[0]);
		EditorGUILayout.BeginVertical(BuildPrepareTool.textAreaStyle, new GUILayoutOption[]
		{
			GUILayout.MinHeight(10f)
		});
		GUI.changed = false;
		this._sdkSupport.AutoLaunch = EditorGUILayout.Toggle("Auto Launch", this._sdkSupport.AutoLaunch, new GUILayoutOption[0]);
		bool flag = !this._sdkSupport.AutoLaunch;
		if (flag)
		{
			EditorGUILayout.HelpBox("Launch with API: UWAEngine.StaticInit()", 1);
		}
		EditorGUILayout.EndVertical();
		EditorGUILayout.Space();
		EditorGUILayout.LabelField(Localization.Instance.Get("Mode Settings"), EditorStyles.boldLabel, new GUILayoutOption[0]);
		EditorGUILayout.BeginVertical(BuildPrepareTool.textAreaStyle, new GUILayoutOption[]
		{
			GUILayout.MinHeight(10f)
		});
		this._sdkSupport.EnableUWAGPM = EditorGUILayout.Toggle("UWA GPM", this._sdkSupport.EnableUWAGPM, new GUILayoutOption[0]);
		this._sdkSupport.EnableUWAGOT = EditorGUILayout.Toggle("UWA GOT", this._sdkSupport.EnableUWAGOT, new GUILayoutOption[0]);
		bool changed = GUI.changed;
		if (changed)
		{
			File.WriteAllText(BuildPrepareTool.UwaFullPath + "/Runtime/Resources/uwasdksettings.bytes", this._sdkSupport.ToJSON());
			AssetDatabase.Refresh();
		}
		EditorGUILayout.EndVertical();
	}

	// Token: 0x04000001 RID: 1
	private UWAWindow.UwaSdkSettings _sdkSupport = null;

	// Token: 0x0200000E RID: 14
	internal class UwaSdkSettings
	{
		// Token: 0x060000AA RID: 170 RVA: 0x00004E7C File Offset: 0x0000307C
		public UwaSdkSettings(JSONClass cls)
		{
			bool flag = cls.Contains("Auto_Launch");
			if (flag)
			{
				this.AutoLaunch = cls["Auto_Launch"].AsBool;
			}
			bool flag2 = cls.Contains("Enable_UWAGOT");
			if (flag2)
			{
				this.EnableUWAGOT = cls["Enable_UWAGOT"].AsBool;
			}
			bool flag3 = cls.Contains("Enable_UWAGPM");
			if (flag3)
			{
				this.EnableUWAGPM = cls["Enable_UWAGPM"].AsBool;
			}
		}

		// Token: 0x060000AB RID: 171 RVA: 0x00004F15 File Offset: 0x00003115
		public UwaSdkSettings()
		{
			this.AutoLaunch = true;
			this.EnableUWAGOT = true;
			this.EnableUWAGPM = true;
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00004F4C File Offset: 0x0000314C
		public string ToJSON()
		{
			JSONClass jsonclass = new JSONClass();
			jsonclass["Auto_Launch"] = new JSONData(this.AutoLaunch);
			jsonclass["Enable_UWAGOT"] = new JSONData(this.EnableUWAGOT);
			jsonclass["Enable_UWAGPM"] = new JSONData(this.EnableUWAGPM);
			return jsonclass.ToString();
		}

		// Token: 0x04000022 RID: 34
		public bool AutoLaunch = true;

		// Token: 0x04000023 RID: 35
		public bool EnableUWAGOT = true;

		// Token: 0x04000024 RID: 36
		public bool EnableUWAGPM = true;
	}
}
