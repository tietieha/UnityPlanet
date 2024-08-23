// // **********************************************************
// // *		                .-"""-.							*
// // *		               / .===. \			            *
// // *		               \/ 6 6 \/			            *
// // *		     ______ooo__\__=__/_____________			*
// // *		    / @author     Leon			   /			*
// // *		   / @Modified   2024-06-26       /			    *
// // *		  /_____________________ooo______/			    *
// // *		  			    |_ | _|			                *
// // *		  			    /-'Y'-\			                *
// // *		  			   (__/ \__)			            *
// // **********************************************************
//
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using Sirenix.OdinInspector.Editor;
// using Sirenix.Utilities.Editor;
// using UnityEditor;
// using UnityEngine;
//
// namespace AssetProcessor
// {
// 	public class AssetProcessorEditor : OdinMenuEditorWindow
// 	{
// 		private AssetProcessorDataCenter _configDataCenter;
// 		private OdinMenuItem _curOdinMenuItem;
// 		
// 		[MenuItem("TATools/文件导入设置", priority = 1000)]
// 		private static void OpenWindow()
// 		{
// 			var window = GetWindow<AssetProcessorEditor>();
// 			window.titleContent = new GUIContent("AssetPreprocessor Editor");
// 			window.Show();
// 		}
//
// 		protected override void Initialize()
// 		{
// 			base.Initialize();
// 			_configDataCenter = new AssetProcessorDataCenter();
// 			MenuWidth = 300;
// 			Selection.selectionChanged += OnProjectSelectionChanged;
// 		}
// 		
// 		protected override OdinMenuTree BuildMenuTree()
// 		{
// 			var tree = new OdinMenuTree(false);
// 			tree.Config = new OdinMenuTreeDrawingConfig()
// 			{
// 				DrawScrollView = true,
// 				DrawSearchToolbar = true,
// 				AutoHandleKeyboardNavigation = true,
// 				AutoScrollOnSelectionChanged = true
// 			};
// 			tree.Selection.SelectionConfirmed -= OnSelectionConfirmed;
// 			tree.Selection.SelectionConfirmed += OnSelectionConfirmed;
// 			tree.Selection.OnSelectionChanged -= OnMenuSelectionChanged;
// 			tree.Selection.OnSelectionChanged += OnMenuSelectionChanged;
// 			tree.Add("Default", null);
// 			tree.Add("Global", null);
// 			
// 			
// 			// 获取Assets文件夹下的所有子文件夹（递归）
// 			string assetsFolderPath = "Assets/GameAsset";
// 			PopulateMenuTree(tree, assetsFolderPath);
// 			return tree;
// 		}
// 		
// 		private void PopulateMenuTree(OdinMenuTree tree, string folderPath)
// 		{
// 			var folderPage = new AssetProcessorEditorItem(folderPath, _configDataCenter);
// 			tree.Add(folderPath, folderPage, EditorIcons.Folder);
// 			
// 			string[] subFolders = AssetDatabase.GetSubFolders(folderPath);
// 			foreach (string subFolder in subFolders)
// 			{
// 				PopulateMenuTree(tree, subFolder);
// 			}
// 		}
// 		
// 		#region GUI
// 		protected override void OnBeginDrawEditors()
// 		{
// 			var selected = this.MenuTree.Selection.FirstOrDefault();
// 			var toolbarHeight = this.MenuTree.Config.SearchToolbarHeight;
//
// 			SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
// 			{
// 				// SirenixEditorGUI.Title("配置文件", "", TextAlignment.Left, false, true);
// 				_configDataCenter.ConfigFile = (AssetProcessorConfig) SirenixEditorGUI.ObjectField(new GUIContent("配置文件"), _configDataCenter.ConfigFile, typeof(AssetProcessorConfig), false);
// 				
// 				if (SirenixEditorGUI.ToolbarButton("Add Config"))
// 				{
// 					var projectFolderPage = selected?.Value as AssetProcessorEditorItem;
// 					projectFolderPage?.CreateConfig();
// 				}
// 			}
// 			SirenixEditorGUI.EndHorizontalToolbar();
//
// 			if (_curOdinMenuItem != null)
// 			{
// 				var curFolderProcessor = _curOdinMenuItem.Value as AssetProcessorEditorItem;
// 				if (curFolderProcessor == null || !curFolderProcessor.HasConfig())
// 				{
// 					EditorGUILayout.LabelField("Nothing", GUIHelper.CenterLargeLabel, GUILayout.Height(this.position.height - 100));
// 				}
// 			}
// 		}
//
// 		protected override void OnEndDrawEditors()
// 		{
// 			GUILayout.FlexibleSpace();
// 			EditorGUILayout.BeginHorizontal();
// 			{
// 				if (GUIHelper.Button("Execute", Color.green, GUILayout.Height(50)))
// 				{
// 					var selected = this.MenuTree.Selection.FirstOrDefault();
// 					var projectFolderPage = selected?.Value as AssetProcessorEditorItem;
// 					projectFolderPage?.Execute();
// 				}
// 			}
// 			EditorGUILayout.EndHorizontal();
// 		}
// 		#endregion
//
// 		#region 事件
// 		private void OnSelectionConfirmed(OdinMenuTreeSelection selection)
// 		{
// 			var selectedItem = selection.FirstOrDefault();
// 			if (selectedItem != null && selectedItem.ChildMenuItems.Count > 0)
// 			{
// 				selectedItem.Toggled = !selectedItem.Toggled;
// 			}
// 		}
//
// 		private void OnMenuSelectionChanged()
// 		{
// 			var selectedItem = MenuTree.Selection.FirstOrDefault();
// 			if (selectedItem != null && _curOdinMenuItem != selectedItem)
// 			{
// 				_curOdinMenuItem = selectedItem;
// 				var projectFolderPage = selectedItem.Value as AssetProcessorEditorItem;
// 				projectFolderPage?.Refresh();
// 			}
// 		}
//
// 		private void OnProjectSelectionChanged()
// 		{
// 			var selectedObject = Selection.activeObject;
// 			if (selectedObject != null)
// 			{ 
// 				string assetPath = AssetDatabase.GetAssetPath(selectedObject);
// 				string folderPath;
//
// 				if (AssetDatabase.IsValidFolder(assetPath))
// 				{
// 					folderPath = assetPath;
// 				}
// 				else
// 				{
// 					folderPath = System.IO.Path.GetDirectoryName(assetPath);
// 				}
//
// 				if (!string.IsNullOrEmpty(folderPath))
// 				{
// 					folderPath = folderPath.Replace("\\", "/");
// 					var menuItem = MenuTree.GetMenuItem(folderPath);
// 					if (menuItem != null)
// 					{
// 						menuItem.Select();
// 					}
// 				}
// 			}
// 		}
// 		#endregion
// 		
// 		protected override void OnDestroy()
// 		{
// 			Selection.selectionChanged -= OnProjectSelectionChanged;
// 			// Unsubscribe from the event to prevent memory leaks
// 			if (this.MenuTree != null && this.MenuTree.Selection != null)
// 			{
// 				this.MenuTree.Selection.SelectionConfirmed -= OnSelectionConfirmed;
// 			}
// 			base.OnDestroy();
// 		}
// 	}
// 	
// 	// =================================================================================================================
// 	public class AssetProcessorDataCenter
// 	{
// 		private AssetProcessorConfig _configFile;
// 		public AssetProcessorConfig ConfigFile
// 		{
// 			get
// 			{
// 				if(_configFile == null)
// 					_configFile = AssetProcessorConfig.DefaultFile;
// 				return _configFile;
// 			}
// 			set => _configFile = value;
// 		}
//
// 		public NTreeNode<AssetProcessorConfigNode> ConfigTreeRoot;
//
// 		private Dictionary<string, NTreeNode<AssetProcessorConfigNode>> _configTreeNodes;
//
// 		public Dictionary<string, NTreeNode<AssetProcessorConfigNode>> ConfigTreeNodes
// 		{
// 			get
// 			{
// 				if (_configTreeNodes == null)
// 				{
// 					_configTreeNodes = ConfigFile.ConfigNodes
// 						.Where(node => !string.IsNullOrEmpty(node.Path))
// 						.ToDictionary(node => node.Path, node => new NTreeNode<AssetProcessorConfigNode>(node));
// 				}
// 				return _configTreeNodes;
// 			}
// 		}
// 		
// 		private void BuildConfigTree()
// 		{
// 			
// 		}
//
// 		public void MakeDirty()
// 		{
// 			_configTreeNodes = null;
// 			// 重构tree
// 		}
// 	}
// }