using System;
using System.Collections.Generic;
using UnityEngine;

namespace UWASDK
{
	// Token: 0x02000028 RID: 40
	public class TextureLoader
	{
		// Token: 0x06000169 RID: 361 RVA: 0x00009664 File Offset: 0x00007864
		private TextureLoader()
		{
			this.resources = new Dictionary<string, Texture2D>
			{
				{
					"Interval",
					Resources.Load<Texture2D>("Textures/UWA.Interval")
				},
				{
					"SetLine-1",
					Resources.Load<Texture2D>("Textures/UWA.SetLine.1")
				},
				{
					"Gear",
					Resources.Load<Texture2D>("Textures/UWA.Gear")
				},
				{
					"BtnBG-1",
					Resources.Load<Texture2D>("Textures/UWA.BtnBG.1")
				},
				{
					"Close",
					Resources.Load<Texture2D>("Textures/UWA.Close")
				},
				{
					"SetLine-2",
					Resources.Load<Texture2D>("Textures/UWA.SetLine.2")
				},
				{
					"Checkbox-1",
					Resources.Load<Texture2D>("Textures/UWA.Checkbox.1")
				},
				{
					"Checkbox-2",
					Resources.Load<Texture2D>("Textures/UWA.Checkbox.2")
				},
				{
					"Checkround-1",
					Resources.Load<Texture2D>("Textures/UWA.Checkround.1")
				},
				{
					"Checkround-2",
					Resources.Load<Texture2D>("Textures/UWA.Checkround.2")
				},
				{
					"ServiceLine",
					Resources.Load<Texture2D>("Textures/UWA.ServiceLine")
				},
				{
					"Right",
					Resources.Load<Texture2D>("Textures/UWA.Right")
				},
				{
					"Left",
					Resources.Load<Texture2D>("Textures/UWA.Left")
				},
				{
					"Dump-Round",
					Resources.Load<Texture2D>("Textures/UWA.Dump.Round")
				},
				{
					"Snapshot-Round",
					Resources.Load<Texture2D>("Textures/UWA.Snapshot.Round")
				},
				{
					"ServiceLine-h",
					Resources.Load<Texture2D>("Textures/UWA.ServiceLine.h")
				},
				{
					"Stop-Round",
					Resources.Load<Texture2D>("Textures/UWA.Stop.Round")
				},
				{
					"Information",
					Resources.Load<Texture2D>("Textures/UWA.Information")
				},
				{
					"Detail",
					Resources.Load<Texture2D>("Textures/UWA.Detail")
				},
				{
					"Exit",
					Resources.Load<Texture2D>("Textures/UWA.Exit")
				},
				{
					"Tab",
					Resources.Load<Texture2D>("Textures/UWA.Tab")
				},
				{
					"Hint",
					Resources.Load<Texture2D>("Textures/UWA.Hint")
				},
				{
					"Switch",
					Resources.Load<Texture2D>("Textures/UWA.Switch")
				},
				{
					"Proj-Active",
					Resources.Load<Texture2D>("Textures/UWA.Proj.Active")
				},
				{
					"ShowItem",
					Resources.Load<Texture2D>("Textures/UWA.ShowItem")
				},
				{
					"Refresh",
					Resources.Load<Texture2D>("Textures/UWA.Refresh")
				},
				{
					"Return",
					Resources.Load<Texture2D>("Textures/UWA.Return")
				},
				{
					"ProgressBG",
					Resources.Load<Texture2D>("Textures/UWA.ProgressBG")
				},
				{
					"Progress-In-Left",
					Resources.Load<Texture2D>("Textures/UWA.Progress.In.Left")
				},
				{
					"Progress-In-Right",
					Resources.Load<Texture2D>("Textures/UWA.Progress.In.Right")
				},
				{
					"Progress-In-Mid",
					Resources.Load<Texture2D>("Textures/UWA.Progress.In.Mid")
				},
				{
					"Switch-1",
					Resources.Load<Texture2D>("Textures/UWA.Switch.1")
				},
				{
					"Switch-2",
					Resources.Load<Texture2D>("Textures/UWA.Switch.2")
				},
				{
					"BtnTab",
					Resources.Load<Texture2D>("Textures/UWA.BtnTab")
				},
				{
					"FoldLeft",
					Resources.Load<Texture2D>("Textures/UWA.FoldLeft")
				},
				{
					"FoldRight",
					Resources.Load<Texture2D>("Textures/UWA.FoldRight")
				}
			};
		}

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x0600016A RID: 362 RVA: 0x000099A0 File Offset: 0x00007BA0
		public static TextureLoader Instance
		{
			get
			{
				bool flag = TextureLoader._instance == null;
				if (flag)
				{
					TextureLoader._instance = new TextureLoader();
				}
				return TextureLoader._instance;
			}
		}

		// Token: 0x0600016B RID: 363 RVA: 0x000099DC File Offset: 0x00007BDC
		public Texture2D Get(string s)
		{
			Texture2D texture2D;
			bool flag = this.resources.TryGetValue(s, out texture2D) && texture2D != null;
			Texture2D result;
			if (flag)
			{
				result = texture2D;
			}
			else
			{
				result = Texture2D.whiteTexture;
			}
			return result;
		}

		// Token: 0x040000E9 RID: 233
		private Dictionary<string, Texture2D> resources;

		// Token: 0x040000EA RID: 234
		private static TextureLoader _instance;
	}
}
