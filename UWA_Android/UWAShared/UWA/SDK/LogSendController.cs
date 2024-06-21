using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

namespace UWA.SDK
{
	// Token: 0x020000D1 RID: 209
	[ComVisible(false)]
	public class LogSendController
	{
		// Token: 0x06000963 RID: 2403 RVA: 0x00046480 File Offset: 0x00044680
		public static Client AddTaskToSend(string filePath, string uploadName)
		{
			bool flag = File.Exists(filePath);
			Client result;
			if (flag)
			{
				Client client = new Client(filePath, uploadName, 0);
				ThreadPool.QueueUserWorkItem(new WaitCallback(client.SendFile), new ManualResetEvent(false));
				result = client;
			}
			else
			{
				Debug.LogError("filePath does not exist " + filePath);
				result = null;
			}
			return result;
		}
	}
}
