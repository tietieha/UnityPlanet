using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UWA;

namespace UWALocal
{
	// Token: 0x02000011 RID: 17
	public class ObjectGetType
	{
		// Token: 0x060000D2 RID: 210 RVA: 0x00005D6C File Offset: 0x00003F6C
		public ObjectGetType(string idpath)
		{
			string data = typeof(object).ToString();
			this._idmapPath = idpath;
			this.root = new ObjNode(data, 0);
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x00005DC0 File Offset: 0x00003FC0
		private void AddListToTree(bool alive, List<string> list, ObjNode root)
		{
			ObjNode objNode = root;
			for (int i = 1; i < list.Count; i++)
			{
				bool flag = false;
				for (int j = 0; j < objNode.Children.Count; j++)
				{
					if (objNode.Children[j].Data == list[i])
					{
						objNode = objNode.Children[j];
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					ObjNode objNode2 = new ObjNode(list[i], objNode.Depth + 1);
					objNode.Children.Add(objNode2);
					objNode = objNode2;
				}
			}
			ObjNode objNode3 = objNode;
			int num = objNode3.TotalCount;
			objNode3.TotalCount = num + 1;
			if (!alive)
			{
				ObjNode objNode4 = objNode;
				num = objNode4.DestroyedCount;
				objNode4.DestroyedCount = num + 1;
			}
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x00005E94 File Offset: 0x00004094
		private void Printorder(ObjNode node, ref StreamWriter sw)
		{
			int num = this.TryGetId(node.Data);
			sw.WriteLine(string.Concat(new string[]
			{
				node.Depth.ToString(),
				",",
				num.ToString(),
				",",
				0.ToString(),
				",",
				node.TotalCount.ToString(),
				",",
				node.DestroyedCount.ToString()
			}));
			for (int i = 0; i < node.Children.Count; i++)
			{
				this.Printorder(node.Children[i], ref sw);
			}
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x00005F5C File Offset: 0x0000415C
		private int TryGetId(string type)
		{
			int num;
			if (!this._idmap.TryGetValue(type, out num))
			{
				int idmapId = this._idmapId;
				this._idmapId = idmapId + 1;
				num = idmapId;
				this._idmap.Add(type, num);
				this._tw.WriteLine(num.ToString() + "," + type);
			}
			return num;
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x00005FC0 File Offset: 0x000041C0
		public void ObjectGet(Dictionary<object, int> objectsBackMap, string path)
		{
			if (objectsBackMap == null || this._idmapPath == null)
			{
				return;
			}
			if (this._tw == null)
			{
				this._tw = new StreamWriter(this._idmapPath);
				this._tw.WriteLine("0," + this.root.Data);
			}
			this.root.Clear();
			foreach (KeyValuePair<object, int> keyValuePair in objectsBackMap)
			{
				object key = keyValuePair.Key;
				Type type = key.GetType();
				List<string> list = new List<string>();
				bool alive = true;
				if (type.IsSubclassOf(typeof(Object)))
				{
					Object @object = key as Object;
					alive = (@object != null);
				}
				while (type != null)
				{
					list.Add(type.ToString());
					type = type.BaseType;
				}
				list.Reverse();
				this.AddListToTree(alive, list, this.root);
			}
			StreamWriter streamWriter = new StreamWriter(path + ".lm", true);
			streamWriter.WriteLine("#" + SharedUtils.frameId.ToString());
			this.Printorder(this.root, ref streamWriter);
			streamWriter.Close();
			this._tw.Flush();
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x0000612C File Offset: 0x0000432C
		public void Stop()
		{
			if (this._tw != null)
			{
				this._tw.Close();
			}
		}

		// Token: 0x04000052 RID: 82
		private Dictionary<string, int> _idmap = new Dictionary<string, int>();

		// Token: 0x04000053 RID: 83
		private int _idmapId = 10000000;

		// Token: 0x04000054 RID: 84
		private string _idmapPath;

		// Token: 0x04000055 RID: 85
		private StreamWriter _tw;

		// Token: 0x04000056 RID: 86
		private ObjNode root;
	}
}
