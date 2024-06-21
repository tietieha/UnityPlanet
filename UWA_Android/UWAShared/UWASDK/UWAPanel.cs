using System;
using System.Collections.Generic;
using UnityEngine;
using UWA;
using UWALocal;

namespace UWASDK
{
	// Token: 0x0200002A RID: 42
	public class UWAPanel : MonoBehaviour
	{
		// Token: 0x06000177 RID: 375 RVA: 0x00009BEC File Offset: 0x00007DEC
		private void Awake()
		{
			this.Init();
		}

		// Token: 0x06000178 RID: 376 RVA: 0x00009BF8 File Offset: 0x00007DF8
		private void Init()
		{
			bool inited = this.Inited;
			if (!inited)
			{
				UWAPanel.Inst = this;
				UWAPanel.st = base.gameObject.AddComponent<ScreenTouch>();
				this.LoadPanelSkin();
				this.WindowRectInitialize();
				this.GUIStyleInitialize();
				this.WindowInitialize();
				this._fullScreenWinFunc = new GUI.WindowFunction(this.FullScreenWindow);
				UWAPanel.LeftButton = new UButton(new Action(this.ButtonClicked), TextureLoader.Instance.Get("FoldLeft"), StyleController.Instance.GetStyle("Empty", "label"), new Rect((float)(Screen.width - 30), 25f, 60f, 180f), 1f);
				UWAPanel.RightButton = new UButton(new Action(this.ButtonClicked), TextureLoader.Instance.Get("FoldRight"), StyleController.Instance.GetStyle("Empty", "label"), new Rect(40f, 25f, 60f, 180f), 1f);
				SdkUIMgr.Get().CurState = ((Screen.width > Screen.height) ? WindowState.Horizontal : WindowState.Vertical);
				this.Inited = true;
			}
		}

		// Token: 0x06000179 RID: 377 RVA: 0x00009D3C File Offset: 0x00007F3C
		private void OnGUI()
		{
			bool flag = !this.Inited;
			if (flag)
			{
				this.Init();
			}
			bool enabled = GUI.enabled;
			GUISkin skin = GUI.skin;
			GUI.skin = this.CSkin;
			GUI.enabled = this.Interactable;
			this.LayoutUpdate();
			DataUploader.UploadState s = DataUploader.s;
			DataUploader.UploadState uploadState = s;
			if (uploadState > DataUploader.UploadState.DeleteOld)
			{
				if (uploadState != DataUploader.UploadState.Idle)
				{
					this.uploadWindow.View(19879823);
					this.uploadWindow.IPon = true;
					this.FullScreenRect = GUI.Window(19879813, this.FullScreenRect, this._fullScreenWinFunc, "", StyleController.Instance.GetStyle("Shade", "label"));
					GUI.BringWindowToFront(19879813);
					GUI.BringWindowToFront(19879823);
				}
				else
				{
					bool pocoOn = this.PocoOn;
					if (pocoOn)
					{
						bool flag2 = SdkUIMgr.Get().UiState == SdkUIMgr.UIState.SERVICE;
						if (flag2)
						{
							this.servicesWindow.View(19879817);
						}
					}
					else
					{
						WindowState windowState = (Screen.width >= Screen.height) ? WindowState.Horizontal : WindowState.Vertical;
						bool flag3 = windowState != SdkUIMgr.Get().CurState;
						if (flag3)
						{
							SdkUIMgr.Get().OldPos = new Vector2(float.MinValue, float.MinValue);
							SdkUIMgr.Get().ExitInitPos = new Vector2(float.MinValue, float.MinValue);
							SdkUIMgr.Get().CurState = windowState;
						}
						float num = (Screen.width > Screen.height) ? (this.exitWindow.GetWindowRect().width + (float)Screen.width * 0.05f) : 0f;
						switch (SdkUIMgr.Get().UiState)
						{
						case SdkUIMgr.UIState.MODE:
						{
							Vector2 windowPos = this.modeWindow.GetWindowPos();
							this.SyncMovementExit(this.modeWindow);
							float num2 = this.modeWindow.GetWindowRect().width + num;
							bool flag4 = windowPos.x > 0f - num2 * 0.3f && windowPos.x < (float)Screen.width - num2 * 0.7f;
							if (flag4)
							{
								this.exitWindow.View(19879822);
								this.modeWindow.View(19879814);
							}
							else
							{
								this.ViewDragButton(windowPos, -num2 * 0.3f, num2 * 0.7f);
							}
							this.ViewTargetWindow(this.modeWindow, windowPos, false, new object[]
							{
								0f - num2 * 0.3f,
								num2 * 0.7f,
								0f,
								1f * ((float)Screen.width - num2 * 1f)
							});
							break;
						}
						case SdkUIMgr.UIState.SELECT:
						{
							Vector2 windowPos = this.selectWindow.GetWindowPos();
							this.SyncMovementExit(this.selectWindow);
							float num3 = this.selectWindow.GetWindowRect().width + num;
							bool flag5 = windowPos.x > 0f - num3 * 0.3f && windowPos.x < (float)Screen.width - num3 * 0.7f;
							if (flag5)
							{
								this.exitWindow.View(19879822);
								this.selectWindow.View(19879815);
							}
							else
							{
								this.ViewDragButton(windowPos, -num3 * 0.3f, num3 * 0.7f);
							}
							this.ViewTargetWindow(this.selectWindow, windowPos, true, new object[]
							{
								0f - num3 * 0.3f,
								num3 * 0.7f,
								0f,
								1f * ((float)Screen.width - num3 * 1f)
							});
							break;
						}
						case SdkUIMgr.UIState.SET:
							this.FullScreenRect = GUI.Window(19879813, this.FullScreenRect, this._fullScreenWinFunc, "", StyleController.Instance.GetStyle("Shade", "label"));
							this.setWindow.View(19879816);
							GUI.BringWindowToFront(19879813);
							GUI.BringWindowToFront(19879816);
							break;
						case SdkUIMgr.UIState.INFO:
						{
							bool flag6 = this.CheckInfoWindow();
							if (flag6)
							{
								this.informationWindow.View(19879820);
							}
							else
							{
								bool flag7 = SdkUIMgr.Get().funcptr != null;
								if (flag7)
								{
									SdkUIMgr.Get().funcptr();
								}
							}
							break;
						}
						case SdkUIMgr.UIState.SERVICE:
						{
							Vector2 windowPos = this.servicesWindow.GetWindowPos();
							float width = this.servicesWindow.GetWindowRect().width;
							bool flag8 = windowPos.x > 0f - width * 0.3f && windowPos.x < (float)Screen.width - width * 0.7f;
							if (flag8)
							{
								this.servicesWindow.View(19879817);
							}
							else
							{
								this.ViewDragButton(windowPos, -width * 0.3f, width * 0.7f);
							}
							this.ViewTargetWindow(this.servicesWindow, windowPos, false, new object[]
							{
								0f - width * 0.3f,
								width * 0.7f,
								0f,
								1f * ((float)Screen.width - width * 1f)
							});
							break;
						}
						}
					}
				}
			}
			else
			{
				this.uploadMsgWindow.View(19879824);
				this.FullScreenRect = GUI.Window(19879813, this.FullScreenRect, this._fullScreenWinFunc, "", StyleController.Instance.GetStyle("Shade", "label"));
				GUI.BringWindowToFront(19879813);
				GUI.BringWindowToFront(19879824);
			}
			bool pocoOn2 = this.PocoOn;
			if (pocoOn2)
			{
				this.pocoTipWindow.View(19879825);
			}
			bool flag9 = this.detailOn && !this.PocoOn;
			if (flag9)
			{
				this.detailWindow.View(19879818);
				this.FullScreenRect = GUI.Window(19879813, this.FullScreenRect, this._fullScreenWinFunc, "", StyleController.Instance.GetStyle("Shade", "label"));
				GUI.BringWindowToFront(19879813);
				GUI.BringWindowToFront(19879818);
			}
			bool flag10 = this.messageOn && !this.PocoOn;
			if (flag10)
			{
				this.messageWindow.View(19879819);
				this.FullScreenRect = GUI.Window(19879813, this.FullScreenRect, this._fullScreenWinFunc, "", StyleController.Instance.GetStyle("Shade", "label"));
				GUI.BringWindowToFront(19879813);
				GUI.BringWindowToFront(19879819);
			}
			bool flag11 = this.informationOn && !this.PocoOn;
			if (flag11)
			{
				this.informationWindow.View(19879820);
				this.FullScreenRect = GUI.Window(19879813, this.FullScreenRect, this._fullScreenWinFunc, "", StyleController.Instance.GetStyle("Shade", "label"));
				GUI.BringWindowToFront(19879813);
				GUI.BringWindowToFront(19879820);
			}
			bool flag12 = this.checkOn && !this.PocoOn;
			if (flag12)
			{
				this.checkWindow.View(19879821);
				this.FullScreenRect = GUI.Window(19879813, this.FullScreenRect, this._fullScreenWinFunc, "", StyleController.Instance.GetStyle("Shade", "label"));
				GUI.BringWindowToFront(19879813);
				GUI.BringWindowToFront(19879821);
			}
			GUI.skin = skin;
			GUI.enabled = enabled;
		}

		// Token: 0x0600017A RID: 378 RVA: 0x0000A5FC File Offset: 0x000087FC
		private bool LoadPanelSkin()
		{
			this.UWASkin = Resources.Load<GUISkin>("GUISkin/PanelSkin");
			bool flag = this.UWASkin == null;
			bool result;
			if (flag)
			{
				StyleController.Instance.SetEmpty(true);
				result = false;
			}
			else
			{
				this.CSkin = Object.Instantiate<GUISkin>(this.UWASkin);
				result = true;
			}
			return result;
		}

		// Token: 0x0600017B RID: 379 RVA: 0x0000A660 File Offset: 0x00008860
		private void WindowRectInitialize()
		{
			this.ORects.Clear();
			Dictionary<string, Rect> value = new Dictionary<string, Rect>
			{
				{
					"ModeWindowRect",
					new Rect(1020f, 80f, 500f, 150f)
				},
				{
					"SelectWindowRect",
					new Rect(320f, 80f, 1200f, 150f)
				},
				{
					"NonSelectWindowRect",
					new Rect(1000f, 80f, 500f, 150f)
				},
				{
					"SetWindowRect",
					new Rect(200f, 100f, 1500f, 800f)
				},
				{
					"NonSetWindowRect",
					new Rect(200f, 300f, 1500f, 700f)
				},
				{
					"ServicesWindowRect",
					new Rect(60f, 80f, 320f, 180f)
				},
				{
					"ExitWindowRect",
					new Rect(1570f, 80f, 250f, 150f)
				},
				{
					"MessageWindowRect",
					new Rect(150f, 10f, 1620f, 120f)
				},
				{
					"DetailWindowRect",
					new Rect(460f, 300f, 1000f, 700f)
				},
				{
					"InformationWindowRect",
					new Rect(460f, 300f, 1000f, 700f)
				},
				{
					"CheckWindowRect",
					new Rect(460f, 300f, 1000f, 700f)
				},
				{
					"UploadWindowRect",
					new Rect(350f, 100f, 1200f, 800f)
				},
				{
					"UploadMsgWindowRect",
					new Rect(460f, 100f, 1000f, 550f)
				},
				{
					"PocoTipWindowRect",
					new Rect(1320f, 80f, 500f, 100f)
				}
			};
			this.ORects.Add(WindowState.Horizontal, value);
			Dictionary<string, Rect> value2 = new Dictionary<string, Rect>
			{
				{
					"ModeWindowRect",
					new Rect(780f, 250f, 250f, 400f)
				},
				{
					"SelectWindowRect",
					new Rect(780f, 250f, 250f, 800f)
				},
				{
					"NonSelectWindowRect",
					new Rect(780f, 250f, 250f, 400f)
				},
				{
					"SetWindowRect",
					new Rect(90f, 100f, 900f, 1500f)
				},
				{
					"NonSetWindowRect",
					new Rect(90f, 100f, 900f, 1500f)
				},
				{
					"ServicesWindowRect",
					new Rect(60f, 80f, 320f, 180f)
				},
				{
					"ExitWindowRect",
					new Rect(780f, 80f, 250f, 150f)
				},
				{
					"MessageWindowRect",
					new Rect(40f, 10f, 1000f, 240f)
				},
				{
					"DetailWindowRect",
					new Rect(40f, 300f, 1000f, 700f)
				},
				{
					"InformationWindowRect",
					new Rect(40f, 300f, 1000f, 700f)
				},
				{
					"CheckWindowRect",
					new Rect(40f, 300f, 1000f, 700f)
				},
				{
					"UploadWindowRect",
					new Rect(90f, 100f, 900f, 800f)
				},
				{
					"UploadMsgWindowRect",
					new Rect(190f, 100f, 700f, 550f)
				},
				{
					"PocoTipWindowRect",
					new Rect(530f, 80f, 500f, 100f)
				}
			};
			this.ORects.Add(WindowState.Vertical, value2);
		}

		// Token: 0x0600017C RID: 380 RVA: 0x0000AAB4 File Offset: 0x00008CB4
		private void GUIStyleInitialize()
		{
			bool flag = this.UWASkin == null;
			if (!flag)
			{
				this.oStyleList.Add(this.UWASkin.button);
				this.oStyleList.Add(this.UWASkin.toggle);
				this.oStyleList.Add(this.UWASkin.label);
				this.oStyleList.Add(this.UWASkin.textField);
				this.cStyleList.Add(this.CSkin.button);
				this.cStyleList.Add(this.CSkin.toggle);
				this.cStyleList.Add(this.CSkin.label);
				this.cStyleList.Add(this.CSkin.textField);
				for (int i = 0; i < this.CSkin.customStyles.Length; i++)
				{
					this.oStyleList.Add(this.UWASkin.customStyles[i]);
					this.cStyleList.Add(this.CSkin.customStyles[i]);
				}
			}
		}

		// Token: 0x0600017D RID: 381 RVA: 0x0000ABF4 File Offset: 0x00008DF4
		private void WindowInitialize()
		{
			this.modeWindow = new ModeWindow(this.ORects[WindowState.Horizontal]["ModeWindowRect"], this.ORects[WindowState.Vertical]["ModeWindowRect"], StyleController.Instance.GetStyle("window", "label"), "ModeWindow", UWindow.YAlign.Top, WindowState.Horizontal, 1f);
			this.windowList.Add(this.modeWindow);
			this.exitWindow = new ExitWindow(this.ORects[WindowState.Horizontal]["ExitWindowRect"], this.ORects[WindowState.Vertical]["ExitWindowRect"], StyleController.Instance.GetStyle("window", "label"), "ExitWindow", UWindow.YAlign.Top, WindowState.Horizontal, 1f);
			this.windowList.Add(this.exitWindow);
			bool dev = SharedUtils.Dev;
			if (dev)
			{
				this.selectWindow = new SelectWindow(this.ORects[WindowState.Horizontal]["SelectWindowRect"], this.ORects[WindowState.Vertical]["SelectWindowRect"], StyleController.Instance.GetStyle("window", "label"), "SelectWindow", UWindow.YAlign.Top, WindowState.Horizontal, 1f);
			}
			else
			{
				this.selectWindow = new SelectWindow(this.ORects[WindowState.Horizontal]["NonSelectWindowRect"], this.ORects[WindowState.Vertical]["NonSelectWindowRect"], StyleController.Instance.GetStyle("window", "label"), "SelectWindow", UWindow.YAlign.Top, WindowState.Horizontal, 1f);
			}
			this.windowList.Add(this.selectWindow);
			bool dev2 = SharedUtils.Dev;
			if (dev2)
			{
				this.setWindow = new SetWindow(this.ORects[WindowState.Horizontal]["SetWindowRect"], this.ORects[WindowState.Vertical]["SetWindowRect"], StyleController.Instance.GetStyle("window", "label"), "SetWindow", UWindow.YAlign.Center, WindowState.Horizontal, 1f);
			}
			else
			{
				this.setWindow = new SetWindow(this.ORects[WindowState.Horizontal]["NonSetWindowRect"], this.ORects[WindowState.Vertical]["NonSetWindowRect"], StyleController.Instance.GetStyle("window", "label"), "SetWindow", UWindow.YAlign.Center, WindowState.Horizontal, 1f);
			}
			this.windowList.Add(this.setWindow);
			this.servicesWindow = new ServicesWindow(this.ORects[WindowState.Horizontal]["ServicesWindowRect"], this.ORects[WindowState.Vertical]["ServicesWindowRect"], StyleController.Instance.GetStyle("Empty", "window"), "ServicesWindow", UWindow.YAlign.Top, WindowState.Horizontal, 1f);
			this.windowList.Add(this.servicesWindow);
			this.detailWindow = new DetailWindow(this.ORects[WindowState.Horizontal]["DetailWindowRect"], this.ORects[WindowState.Vertical]["DetailWindowRect"], StyleController.Instance.GetStyle("window", "label"), "DetailWindow", UWindow.YAlign.Center, WindowState.Horizontal, 1f);
			this.windowList.Add(this.detailWindow);
			this.messageWindow = new MessageWindow(this.ORects[WindowState.Horizontal]["MessageWindowRect"], this.ORects[WindowState.Vertical]["MessageWindowRect"], StyleController.Instance.GetStyle("window", "label"), "MessageWindow", UWindow.YAlign.Top, WindowState.Horizontal, 1f);
			this.windowList.Add(this.messageWindow);
			this.informationWindow = new InformationWindow(this.ORects[WindowState.Horizontal]["InformationWindowRect"], this.ORects[WindowState.Vertical]["InformationWindowRect"], StyleController.Instance.GetStyle("window", "label"), "InformationWindow", UWindow.YAlign.Center, WindowState.Horizontal, 1f);
			this.windowList.Add(this.informationWindow);
			this.checkWindow = new CheckWindow(this.ORects[WindowState.Horizontal]["CheckWindowRect"], this.ORects[WindowState.Vertical]["CheckWindowRect"], StyleController.Instance.GetStyle("window", "label"), "CheckWindow", UWindow.YAlign.Center, WindowState.Horizontal, 1f);
			this.windowList.Add(this.checkWindow);
			this.uploadWindow = new UploadWindow(this.ORects[WindowState.Horizontal]["UploadWindowRect"], this.ORects[WindowState.Vertical]["UploadWindowRect"], StyleController.Instance.GetStyle("window", "label"), "UploadWindow", UWindow.YAlign.Center, WindowState.Horizontal, 1f);
			this.windowList.Add(this.uploadWindow);
			this.uploadMsgWindow = new UploadMsgWindow(this.ORects[WindowState.Horizontal]["UploadMsgWindowRect"], this.ORects[WindowState.Vertical]["UploadMsgWindowRect"], StyleController.Instance.GetStyle("window", "label"), "UploadMsgWindow", UWindow.YAlign.Center, WindowState.Horizontal, 1f);
			this.windowList.Add(this.uploadMsgWindow);
			this.pocoTipWindow = new PocoTipWindow(this.ORects[WindowState.Horizontal]["PocoTipWindowRect"], this.ORects[WindowState.Vertical]["PocoTipWindowRect"], StyleController.Instance.GetStyle("window", "label"), "PocoTipWindow", UWindow.YAlign.Top, WindowState.Horizontal, 1f);
			this.windowList.Add(this.pocoTipWindow);
			for (int i = 0; i < this.windowList.Count; i++)
			{
				this.windowList[i].WindowInitialize();
			}
		}

		// Token: 0x0600017E RID: 382 RVA: 0x0000B1F4 File Offset: 0x000093F4
		private void LayoutUpdate()
		{
			bool flag = this.scWidth == Screen.width && this.scHeight == Screen.height;
			if (!flag)
			{
				this.scWidth = Screen.width;
				this.scHeight = Screen.height;
				this.sizeState = ((this.scWidth >= this.scHeight) ? WindowState.Horizontal : WindowState.Vertical);
				float rate = (this.sizeState == WindowState.Horizontal) ? ((float)this.scWidth / 1920f) : ((float)this.scWidth / 1080f);
				for (int i = 0; i < this.windowList.Count; i++)
				{
					this.windowList[i].State = this.sizeState;
					this.windowList[i].Rate = rate;
				}
				this.FullScreenRect = new Rect(0f, 0f, (float)Screen.width, (float)Screen.height);
				this.StyleUpdate(rate);
			}
		}

		// Token: 0x0600017F RID: 383 RVA: 0x0000B308 File Offset: 0x00009508
		private void StyleUpdate(float rate)
		{
			this.ControlUpdate(rate);
		}

		// Token: 0x06000180 RID: 384 RVA: 0x0000B314 File Offset: 0x00009514
		private void ControlUpdate(float rate)
		{
			bool flag = this.UWASkin == null;
			if (!flag)
			{
				for (int i = 0; i < this.cStyleList.Count; i++)
				{
					this.cStyleList[i].fontSize = (int)((float)this.oStyleList[i].fontSize * rate);
					this.cStyleList[i].border.left = (int)((float)this.oStyleList[i].border.left * rate);
				}
				GUIStyle style = this.CSkin.GetStyle("Toggle");
				GUIStyle style2 = this.UWASkin.GetStyle("Toggle");
				style.padding.top = (int)((float)style2.padding.top * rate);
				style.padding.left = (int)((float)style2.padding.left * rate);
				style = this.CSkin.GetStyle("OverdrawLuaToggle");
				style2 = this.UWASkin.GetStyle("OverdrawLuaToggle");
				style.padding.top = (int)((float)style2.padding.top * rate);
				style.padding.left = (int)((float)style2.padding.left * rate);
				style = this.CSkin.GetStyle("RecToggle");
				style2 = this.UWASkin.GetStyle("RecToggle");
				style.padding.top = (int)((float)style2.padding.top * rate);
				style.padding.left = (int)((float)style2.padding.left * rate);
				style = this.CSkin.GetStyle("SetToggle");
				style2 = this.UWASkin.GetStyle("SetToggle");
				style.padding.top = (int)((float)style2.padding.top * rate);
				style.padding.left = (int)((float)style2.padding.left * rate);
				style = this.CSkin.GetStyle("UploadInput");
				style2 = this.UWASkin.GetStyle("UploadInput");
				style.padding.left = (int)((float)style2.padding.left * rate);
				for (int j = 0; j < this.controlList.Count; j++)
				{
					this.controlList[j].Rate = rate;
				}
			}
		}

		// Token: 0x06000181 RID: 385 RVA: 0x0000B59C File Offset: 0x0000979C
		public void CreateMessage(string str)
		{
			bool flag = this.messageWindow != null;
			if (flag)
			{
				this.messageWindow.CreateMessage(str);
			}
		}

		// Token: 0x06000182 RID: 386 RVA: 0x0000B5CC File Offset: 0x000097CC
		public void CreateInformation(string str)
		{
			bool flag = this.informationWindow != null;
			if (flag)
			{
				this.informationWindow.CreateInformation(str);
			}
		}

		// Token: 0x06000183 RID: 387 RVA: 0x0000B5FC File Offset: 0x000097FC
		public void ClearMessage()
		{
			bool flag = this.messageWindow != null;
			if (flag)
			{
				this.messageWindow.ClearMessage();
			}
		}

		// Token: 0x06000184 RID: 388 RVA: 0x0000B62C File Offset: 0x0000982C
		public void ClearInformation()
		{
			bool flag = this.informationWindow != null;
			if (flag)
			{
				this.informationWindow.ClearInformation();
			}
		}

		// Token: 0x06000185 RID: 389 RVA: 0x0000B65C File Offset: 0x0000985C
		private void FullScreenWindow(int id)
		{
		}

		// Token: 0x06000186 RID: 390 RVA: 0x0000B660 File Offset: 0x00009860
		public void ButtonClicked()
		{
			this.isClicked = true;
		}

		// Token: 0x06000187 RID: 391 RVA: 0x0000B66C File Offset: 0x0000986C
		public void ViewDragButton(Vector2 Pos, float left, float right)
		{
			bool flag = Pos.x >= (float)Screen.width - right;
			if (flag)
			{
				UWAPanel.LeftButton.View(new Vector2((float)(Screen.width - 50), Pos.y));
			}
			else
			{
				bool flag2 = Pos.x <= left;
				if (flag2)
				{
					UWAPanel.RightButton.View(new Vector2(-5f, 50f));
				}
			}
		}

		// Token: 0x06000188 RID: 392 RVA: 0x0000B6F0 File Offset: 0x000098F0
		public void ViewTargetWindow(UWindow target, Vector2 oldPos, bool need, params object[] _params)
		{
			bool flag = _params.Length < 4;
			if (!flag)
			{
				bool flag2 = this.isClicked;
				if (flag2)
				{
					this.isClicked = false;
					bool flag3 = oldPos.x >= (float)Screen.width - (float)_params[1];
					if (flag3)
					{
						target.SetWindowPos(new Vector2((float)_params[3], oldPos.y));
					}
					else
					{
						bool flag4 = oldPos.x <= (float)_params[0];
						if (flag4)
						{
							target.SetWindowPos(new Vector2((float)_params[2], 50f));
						}
					}
				}
			}
		}

		// Token: 0x06000189 RID: 393 RVA: 0x0000B7A8 File Offset: 0x000099A8
		private void SyncMovementExit(UWindow target)
		{
			Vector2 windowPos = target.GetWindowPos();
			Rect windowRect = target.GetWindowRect();
			bool isDrag = this.exitWindow.isDrag;
			if (isDrag)
			{
				bool flag = target.State == WindowState.Horizontal;
				if (flag)
				{
					target.SetWindowPos(new Vector2(this.exitWindow.GetWindowPos().x - windowRect.width - (float)Screen.width * 0.05f, this.exitWindow.GetWindowPos().y));
				}
				else
				{
					target.SetWindowPos(new Vector2(this.exitWindow.GetWindowPos().x, this.exitWindow.GetWindowPos().y + 50f + this.exitWindow.GetWindowRect().height));
				}
			}
			else
			{
				bool flag2 = target.State == WindowState.Horizontal;
				if (flag2)
				{
					this.exitWindow.SetWindowPos(new Vector2(windowPos.x + windowRect.width + (float)Screen.width * 0.05f, windowPos.y));
				}
				else
				{
					this.exitWindow.SetWindowPos(new Vector2(windowPos.x, windowPos.y - 50f - this.exitWindow.GetWindowRect().height));
				}
			}
		}

		// Token: 0x0600018A RID: 394 RVA: 0x0000B904 File Offset: 0x00009B04
		private bool CheckInfoWindow()
		{
			bool flag = SdkUIMgr.Get().infoStates.Count == 1;
			bool result;
			if (flag)
			{
				result = !UWAPanel.ConfigInst.overview.engine_cpu_stack;
			}
			else
			{
				result = (SdkUIMgr.Get().infoStates.Count > 0);
			}
			return result;
		}

		// Token: 0x040000EF RID: 239
		private GUISkin UWASkin;

		// Token: 0x040000F0 RID: 240
		private GUISkin CSkin;

		// Token: 0x040000F1 RID: 241
		private List<GUIStyle> oStyleList = new List<GUIStyle>();

		// Token: 0x040000F2 RID: 242
		private List<GUIStyle> cStyleList = new List<GUIStyle>();

		// Token: 0x040000F3 RID: 243
		private List<UControl> controlList = new List<UControl>();

		// Token: 0x040000F4 RID: 244
		public const int oWidth = 1920;

		// Token: 0x040000F5 RID: 245
		public const int oHeight = 1080;

		// Token: 0x040000F6 RID: 246
		public static ScreenTouch st = null;

		// Token: 0x040000F7 RID: 247
		private int scWidth = 0;

		// Token: 0x040000F8 RID: 248
		private int scHeight = 0;

		// Token: 0x040000F9 RID: 249
		private bool isClicked = false;

		// Token: 0x040000FA RID: 250
		private WindowState sizeState = WindowState.Horizontal;

		// Token: 0x040000FB RID: 251
		public static SdkStartConfig ConfigInst = SdkStartConfig.Instance;

		// Token: 0x040000FC RID: 252
		public static SdkStartConfig BufConfigInst = new SdkStartConfig();

		// Token: 0x040000FD RID: 253
		public static bool UseTempSave = false;

		// Token: 0x040000FE RID: 254
		public Dictionary<WindowState, Dictionary<string, Rect>> ORects = new Dictionary<WindowState, Dictionary<string, Rect>>();

		// Token: 0x040000FF RID: 255
		private List<UWindow> windowList = new List<UWindow>();

		// Token: 0x04000100 RID: 256
		public ModeWindow modeWindow;

		// Token: 0x04000101 RID: 257
		public SelectWindow selectWindow;

		// Token: 0x04000102 RID: 258
		public SetWindow setWindow;

		// Token: 0x04000103 RID: 259
		public ServicesWindow servicesWindow;

		// Token: 0x04000104 RID: 260
		public DetailWindow detailWindow;

		// Token: 0x04000105 RID: 261
		public MessageWindow messageWindow;

		// Token: 0x04000106 RID: 262
		public InformationWindow informationWindow;

		// Token: 0x04000107 RID: 263
		public CheckWindow checkWindow;

		// Token: 0x04000108 RID: 264
		public ExitWindow exitWindow;

		// Token: 0x04000109 RID: 265
		public UploadWindow uploadWindow;

		// Token: 0x0400010A RID: 266
		public UploadMsgWindow uploadMsgWindow;

		// Token: 0x0400010B RID: 267
		public PocoTipWindow pocoTipWindow;

		// Token: 0x0400010C RID: 268
		private GUI.WindowFunction _fullScreenWinFunc = null;

		// Token: 0x0400010D RID: 269
		public static UButton LeftButton;

		// Token: 0x0400010E RID: 270
		public static UButton RightButton;

		// Token: 0x0400010F RID: 271
		public bool detailOn = false;

		// Token: 0x04000110 RID: 272
		public bool messageOn = false;

		// Token: 0x04000111 RID: 273
		public bool informationOn = false;

		// Token: 0x04000112 RID: 274
		public bool Inited = false;

		// Token: 0x04000113 RID: 275
		public bool checkOn = false;

		// Token: 0x04000114 RID: 276
		public bool PocoOn = false;

		// Token: 0x04000115 RID: 277
		public bool Interactable = true;

		// Token: 0x04000116 RID: 278
		public static UWAPanel Inst;

		// Token: 0x04000117 RID: 279
		private Rect FullScreenRect;

		// Token: 0x020000F5 RID: 245
		private enum UWindowID
		{
			// Token: 0x04000644 RID: 1604
			Shade = 19879813,
			// Token: 0x04000645 RID: 1605
			Mode,
			// Token: 0x04000646 RID: 1606
			Select,
			// Token: 0x04000647 RID: 1607
			Set,
			// Token: 0x04000648 RID: 1608
			Services,
			// Token: 0x04000649 RID: 1609
			Detail,
			// Token: 0x0400064A RID: 1610
			Message,
			// Token: 0x0400064B RID: 1611
			Information,
			// Token: 0x0400064C RID: 1612
			Check,
			// Token: 0x0400064D RID: 1613
			Exit,
			// Token: 0x0400064E RID: 1614
			Upload,
			// Token: 0x0400064F RID: 1615
			UploadMsg,
			// Token: 0x04000650 RID: 1616
			PocoTip
		}
	}
}
