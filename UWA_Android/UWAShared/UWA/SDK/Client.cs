using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using HotFix;
using UnityEngine;

namespace UWA.SDK
{
	// Token: 0x020000D0 RID: 208
	[ComVisible(false)]
	public class Client
	{
		// Token: 0x0600094B RID: 2379 RVA: 0x00045AAC File Offset: 0x00043CAC
		public Client(string filePath, string uploadName, int maxRetry = 0)
		{
			this.filePath = filePath;
			this.uploadName = uploadName;
			this.maxRetry = maxRetry;
			this.buffer = new byte[8192];
			this.socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			this.Failed = false;
		}

		// Token: 0x0600094C RID: 2380 RVA: 0x00045B14 File Offset: 0x00043D14
		public static void DoSocketTestAsync(Action<bool> cb)
		{
			Client._testClient = new Client("", "", 0);
			Client._testThread = new Thread(delegate()
			{
				bool obj = false;
				try
				{
					obj = Client._testClient.TestSending();
				}
				catch (Exception ex)
				{
				}
				cb(obj);
			});
			Client._testThread.Start();
		}

		// Token: 0x0600094D RID: 2381 RVA: 0x00045B6C File Offset: 0x00043D6C
		public void SendFile(object threadContext)
		{
			try
			{
				this.Failed = false;
				this.doneEvent = (ManualResetEvent)threadContext;
				bool flag = this.PrepaireSending();
				int num = 1;
				while (!flag && num <= this.maxRetry)
				{
					Thread.Sleep(60000);
					flag = this.PrepaireSending();
				}
				bool flag2 = flag;
				if (flag2)
				{
					this.Failed = !this.TrySendFile();
					Debug.Log((this.Failed ? "Failed at last: " : "Succeed : ") + this.uploadName);
				}
				else
				{
					this.Failed = true;
				}
				Thread.Sleep(1000);
				this.CloseSocket();
			}
			catch (Exception ex)
			{
				this.Failed = true;
			}
			finally
			{
				this.doneEvent.Set();
			}
		}

		// Token: 0x0600094E RID: 2382 RVA: 0x00045C7C File Offset: 0x00043E7C
		private bool ConnectToServerSync2(int timeoutMSec)
		{
			try
			{
				IPAddress address = IPAddress.Parse(UWAConfig.IP);
				IAsyncResult asyncResult = this.socketClient.BeginConnect(new IPEndPoint(address, UWAConfig.PORT), null, null);
				bool flag = asyncResult.AsyncWaitHandle.WaitOne(5000, true);
				return this.socketClient.Connected;
			}
			catch (Exception ex)
			{
			}
			return false;
		}

		// Token: 0x0600094F RID: 2383 RVA: 0x00045CF4 File Offset: 0x00043EF4
		private bool ConnectToServerSync(int timeoutMSec)
		{
			Client.SyncTimeoutObject.Reset();
			try
			{
				IPAddress address = IPAddress.Parse(UWAConfig.IP);
				this.socketClient.BeginConnect(new IPEndPoint(address, UWAConfig.PORT), new AsyncCallback(Client.CallBackMethod), this.socketClient);
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("connect failed : {0}", this.filePath));
				return false;
			}
			return Client.SyncTimeoutObject.WaitOne(timeoutMSec, false);
		}

		// Token: 0x06000950 RID: 2384 RVA: 0x00045D8C File Offset: 0x00043F8C
		private static void CallBackMethod(IAsyncResult asyncresult)
		{
			Client.SyncTimeoutObject.Set();
		}

		// Token: 0x06000951 RID: 2385 RVA: 0x00045D9C File Offset: 0x00043F9C
		private bool ConnectToServer()
		{
			bool result;
			try
			{
				IPAddress address = IPAddress.Parse(UWAConfig.IP);
				this.socketClient.Connect(new IPEndPoint(address, UWAConfig.PORT));
				result = true;
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("connect failed : {0}", this.filePath));
				result = false;
			}
			return result;
		}

		// Token: 0x06000952 RID: 2386 RVA: 0x00045E04 File Offset: 0x00044004
		private bool EchoStart()
		{
			Array.Clear(this.buffer, 0, 8192);
			Client.ReceiveMessageIntoBuffer(this.socketClient, ref this.buffer);
			int num = 0;
			string flagFromBytesBuffer = CoreUtils.GetFlagFromBytesBuffer(this.buffer, ref num);
			bool flag = !flagFromBytesBuffer.Equals("^=start=^");
			bool result;
			if (flag)
			{
				Debug.LogError(string.Format("EchoStart failed: {0}", this.uploadName));
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x06000953 RID: 2387 RVA: 0x00045E88 File Offset: 0x00044088
		private void EchoEnd()
		{
			int num = 0;
			this.AddStringInfoBuffer("^=test=^", ref this.buffer, ref num);
			this.SendMessageInBuffer(this.socketClient, this.buffer);
			this.socketClient.Close();
		}

		// Token: 0x06000954 RID: 2388 RVA: 0x00045ED0 File Offset: 0x000440D0
		public bool TestSending()
		{
			bool flag = !this.ConnectToServerSync2(5000);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = !this.EchoStart();
				if (flag2)
				{
					Debug.LogError("TestSending : !EchoStart");
					result = false;
				}
				else
				{
					this.EchoEnd();
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06000955 RID: 2389 RVA: 0x00045F34 File Offset: 0x00044134
		public bool PrepaireSending()
		{
			bool flag = !this.ConnectToServer();
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = !this.EchoStart();
				result = !flag2;
			}
			return result;
		}

		// Token: 0x06000956 RID: 2390 RVA: 0x00045F80 File Offset: 0x00044180
		private bool TrySendFile()
		{
			int num = 0;
			bool flag = this.TrySendFileOnce();
			num++;
			while (num <= this.maxRetry && !flag)
			{
				flag = this.TrySendFileOnce();
				bool flag2 = !flag;
				if (flag2)
				{
					Debug.LogWarning("Failed in " + num.ToString() + " : " + this.uploadName.Split(new char[]
					{
						'$'
					})[2]);
					Thread.Sleep(10000);
				}
				num++;
			}
			bool flag3 = flag;
			if (!flag3)
			{
				Debug.LogError("Failed at last : " + this.uploadName);
			}
			return flag;
		}

		// Token: 0x06000957 RID: 2391 RVA: 0x00046050 File Offset: 0x00044250
		private bool TrySendFileOnce()
		{
			int num = 0;
			this.progress = 0f;
			bool result = false;
			try
			{
				this.GetMessageHeader(this.uploadName, ref this.buffer);
				this.SendMessageInBuffer(this.socketClient, this.buffer);
				FileStream fileStream = new FileStream(this.filePath, FileMode.Open);
				FileInfo fileInfo = new FileInfo(this.filePath);
				long length = fileInfo.Length;
				long num2 = 0L;
				for (int i = fileStream.Read(this.buffer, 4, 8188); i > 0; i = fileStream.Read(this.buffer, 4, 8188))
				{
					byte[] bytes = BitConverter.GetBytes(i);
					Array.Copy(bytes, 0, this.buffer, 0, bytes.Length);
					this.SendMessageInBuffer(this.socketClient, this.buffer);
					num2 += (long)i;
					this.progress = (float)num2 * 1f / (float)length * 0.99f;
					Array.Clear(this.buffer, 0, this.buffer.Length);
				}
				num = 0;
				this.AddStringInfoBuffer("^=end=^", ref this.buffer, ref num);
				this.SendMessageInBuffer(this.socketClient, this.buffer);
				Client.ReceiveMessageIntoBuffer(this.socketClient, ref this.buffer);
				num = 0;
				string flagFromBytesBuffer = CoreUtils.GetFlagFromBytesBuffer(this.buffer, ref num);
				bool flag = flagFromBytesBuffer.Equals("^=succeed=^");
				if (flag)
				{
					this.progress = 1f;
					result = true;
				}
				fileStream.Close();
			}
			catch (Exception ex)
			{
				Debug.Log(string.Format("TrySendFileOnce exception : {0}\n{1}", this.uploadName, ex.ToString()));
			}
			return result;
		}

		// Token: 0x06000958 RID: 2392 RVA: 0x00046228 File Offset: 0x00044428
		private void GetMessageHeader(string fileName, ref byte[] buffer)
		{
			int num = 0;
			this.AddStringInfoBuffer("^=start=^", ref buffer, ref num);
			this.AddStringInfoBuffer(fileName, ref buffer, ref num);
			byte[] md5ForFile = CoreUtils.GetMD5ForFile(this.filePath);
			int value = md5ForFile.Length;
			byte[] bytes = BitConverter.GetBytes(value);
			Array.Copy(bytes, 0, buffer, num, bytes.Length);
			num += bytes.Length;
			Array.Copy(md5ForFile, 0, buffer, num, md5ForFile.Length);
		}

		// Token: 0x06000959 RID: 2393 RVA: 0x00046290 File Offset: 0x00044490
		private void AddStringInfoBuffer(string sourceStr, ref byte[] buffer, ref int startIndex)
		{
			byte[] bytes = Encoding.Unicode.GetBytes(sourceStr);
			byte[] bytes2 = BitConverter.GetBytes(bytes.Length);
			Array.Copy(bytes2, 0, buffer, startIndex, bytes2.Length);
			startIndex += bytes2.Length;
			Array.Copy(bytes, 0, buffer, startIndex, bytes.Length);
			startIndex += bytes.Length;
		}

		// Token: 0x0600095A RID: 2394 RVA: 0x000462E8 File Offset: 0x000444E8
		private void GetMessageEnding(ref byte[] buffer)
		{
			byte[] bytes = Encoding.Unicode.GetBytes("^=end=^");
		}

		// Token: 0x0600095B RID: 2395 RVA: 0x0004630C File Offset: 0x0004450C
		private void CloseSocket()
		{
			bool flag = this.socketClient != null;
			if (flag)
			{
				this.socketClient.Shutdown(SocketShutdown.Both);
				this.socketClient.Close();
				this.socketClient = null;
			}
		}

		// Token: 0x0600095C RID: 2396 RVA: 0x00046350 File Offset: 0x00044550
		private string GetEndFlag()
		{
			Client.ReceiveMessageIntoBuffer(this.socketClient, ref this.buffer);
			int num = 0;
			return CoreUtils.GetFlagFromBytesBuffer(this.buffer, ref num);
		}

		// Token: 0x0600095D RID: 2397 RVA: 0x0004638C File Offset: 0x0004458C
		private static void ReceiveMessageIntoBuffer(Socket receiveSocket, ref byte[] buffer)
		{
			int i = 0;
			int num = receiveSocket.Receive(buffer, i, 8192, SocketFlags.None);
			for (i += num; i < 8192; i += num)
			{
				num = receiveSocket.Receive(buffer, i, 8192 - i, SocketFlags.None);
			}
		}

		// Token: 0x0600095E RID: 2398 RVA: 0x000463E0 File Offset: 0x000445E0
		private void SendMessageInBuffer(Socket receiveSocket, byte[] buffer)
		{
			int i = 0;
			int num = receiveSocket.Send(buffer, i, 8192, SocketFlags.None);
			for (i += num; i < 8192; i += num)
			{
				num = receiveSocket.Send(buffer, i, 8192 - i, SocketFlags.None);
			}
		}

		// Token: 0x170001BF RID: 447
		// (get) Token: 0x0600095F RID: 2399 RVA: 0x00046430 File Offset: 0x00044630
		// (set) Token: 0x06000960 RID: 2400 RVA: 0x00046438 File Offset: 0x00044638
		public bool Failed { get; private set; }

		// Token: 0x170001C0 RID: 448
		// (get) Token: 0x06000961 RID: 2401 RVA: 0x00046444 File Offset: 0x00044644
		public float Progress
		{
			get
			{
				return this.progress;
			}
		}

		// Token: 0x040005A1 RID: 1441
		private static readonly ManualResetEvent SyncTimeoutObject = new ManualResetEvent(false);

		// Token: 0x040005A2 RID: 1442
		private static Client _testClient = null;

		// Token: 0x040005A3 RID: 1443
		private static Thread _testThread = null;

		// Token: 0x040005A5 RID: 1445
		private Socket socketClient;

		// Token: 0x040005A6 RID: 1446
		private string ip;

		// Token: 0x040005A7 RID: 1447
		private int port;

		// Token: 0x040005A8 RID: 1448
		private IPAddress ipAddress;

		// Token: 0x040005A9 RID: 1449
		private Socket clientSocket;

		// Token: 0x040005AA RID: 1450
		private byte[] buffer;

		// Token: 0x040005AB RID: 1451
		private string filePath;

		// Token: 0x040005AC RID: 1452
		private string uploadName;

		// Token: 0x040005AD RID: 1453
		private int maxRetry = 0;

		// Token: 0x040005AE RID: 1454
		private float progress = 0f;

		// Token: 0x040005AF RID: 1455
		private ManualResetEvent doneEvent;
	}
}
