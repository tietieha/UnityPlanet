using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UWA;

namespace UWACore.Util.NetWork
{
	// Token: 0x0200004B RID: 75
	[ComVisible(false)]
	public class Client
	{
		// Token: 0x0600033A RID: 826 RVA: 0x00016274 File Offset: 0x00014474
		public Client(string filePath, string uploadName, int maxRetry = 2)
		{
			this.filePath = filePath;
			this.uploadName = uploadName;
			this.maxRetry = maxRetry;
			this.buffer = new byte[8192];
			this.socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			this.Failed = false;
		}

		// Token: 0x0600033B RID: 827 RVA: 0x000162DC File Offset: 0x000144DC
		public static void DoSocketTestAsync(Action<bool> cb)
		{
			Client._testClient = new Client("", "", 2);
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

		// Token: 0x0600033C RID: 828 RVA: 0x00016334 File Offset: 0x00014534
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
					SharedUtils.Log((this.Failed ? "Failed at last: " : "Succeed : ") + this.uploadName);
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
				string str = "SendFile exception: ";
				Exception ex2 = ex;
				SharedUtils.LogError(str + ((ex2 != null) ? ex2.ToString() : null));
				this.Failed = true;
			}
			finally
			{
				this.doneEvent.Set();
			}
		}

		// Token: 0x0600033D RID: 829 RVA: 0x00016468 File Offset: 0x00014668
		private bool ConnectToServerSync2(int timeoutMSec)
		{
			try
			{
				IPAddress address = IPAddress.Parse(UWACoreConfig.IP);
				IAsyncResult asyncResult = this.socketClient.BeginConnect(new IPEndPoint(address, UWACoreConfig.PORT), null, null);
				bool flag = asyncResult.AsyncWaitHandle.WaitOne(5000, true);
				return this.socketClient.Connected;
			}
			catch (Exception ex)
			{
			}
			return false;
		}

		// Token: 0x0600033E RID: 830 RVA: 0x000164E0 File Offset: 0x000146E0
		private bool ConnectToServerSync(int timeoutMSec)
		{
			Client.SyncTimeoutObject.Reset();
			try
			{
				IPAddress address = IPAddress.Parse(UWACoreConfig.IP);
				this.socketClient.BeginConnect(new IPEndPoint(address, UWACoreConfig.PORT), new AsyncCallback(Client.CallBackMethod), this.socketClient);
			}
			catch (Exception ex)
			{
				SharedUtils.LogError(string.Format("connect failed : {0}", this.filePath));
				return false;
			}
			return Client.SyncTimeoutObject.WaitOne(timeoutMSec, false);
		}

		// Token: 0x0600033F RID: 831 RVA: 0x00016578 File Offset: 0x00014778
		private static void CallBackMethod(IAsyncResult asyncresult)
		{
			Client.SyncTimeoutObject.Set();
		}

		// Token: 0x06000340 RID: 832 RVA: 0x00016588 File Offset: 0x00014788
		private bool ConnectToServer()
		{
			bool result;
			try
			{
				IPAddress address = IPAddress.Parse(UWACoreConfig.IP);
				this.socketClient.Connect(new IPEndPoint(address, UWACoreConfig.PORT));
				result = true;
			}
			catch (Exception ex)
			{
				SharedUtils.LogError(string.Format("connect failed : {0}", this.filePath));
				result = false;
			}
			return result;
		}

		// Token: 0x06000341 RID: 833 RVA: 0x000165F0 File Offset: 0x000147F0
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
				SharedUtils.LogError(string.Format("EchoStart failed: {0}", this.uploadName));
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x06000342 RID: 834 RVA: 0x00016674 File Offset: 0x00014874
		private void EchoEnd()
		{
			int num = 0;
			this.AddStringInfoBuffer("^=test=^", ref this.buffer, ref num);
			this.SendMessageInBuffer(this.socketClient, this.buffer);
			this.socketClient.Close();
		}

		// Token: 0x06000343 RID: 835 RVA: 0x000166BC File Offset: 0x000148BC
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
					SharedUtils.LogError("TestSending : !EchoStart");
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

		// Token: 0x06000344 RID: 836 RVA: 0x00016720 File Offset: 0x00014920
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

		// Token: 0x06000345 RID: 837 RVA: 0x0001676C File Offset: 0x0001496C
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
					SharedUtils.LogError("Failed in " + num.ToString() + " : " + this.uploadName.Split(new char[]
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
				SharedUtils.LogError("Failed at last : " + this.uploadName);
			}
			return flag;
		}

		// Token: 0x06000346 RID: 838 RVA: 0x0001683C File Offset: 0x00014A3C
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
				SharedUtils.Log(string.Format("TrySendFileOnce exception : {0}\n{1}", this.uploadName, ex.ToString()));
			}
			return result;
		}

		// Token: 0x06000347 RID: 839 RVA: 0x00016A14 File Offset: 0x00014C14
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

		// Token: 0x06000348 RID: 840 RVA: 0x00016A7C File Offset: 0x00014C7C
		private void AddStringInfoBuffer(string sourceStr, ref byte[] buffer, ref int startIndex)
		{
			byte[] bytes = Encoding.Unicode.GetBytes(sourceStr);
			byte[] bytes2 = BitConverter.GetBytes(bytes.Length);
			Array.Copy(bytes2, 0, buffer, startIndex, bytes2.Length);
			startIndex += bytes2.Length;
			Array.Copy(bytes, 0, buffer, startIndex, bytes.Length);
			startIndex += bytes.Length;
		}

		// Token: 0x06000349 RID: 841 RVA: 0x00016AD4 File Offset: 0x00014CD4
		private void GetMessageEnding(ref byte[] buffer)
		{
			byte[] bytes = Encoding.Unicode.GetBytes("^=end=^");
		}

		// Token: 0x0600034A RID: 842 RVA: 0x00016AF8 File Offset: 0x00014CF8
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

		// Token: 0x0600034B RID: 843 RVA: 0x00016B3C File Offset: 0x00014D3C
		private string GetEndFlag()
		{
			Client.ReceiveMessageIntoBuffer(this.socketClient, ref this.buffer);
			int num = 0;
			return CoreUtils.GetFlagFromBytesBuffer(this.buffer, ref num);
		}

		// Token: 0x0600034C RID: 844 RVA: 0x00016B78 File Offset: 0x00014D78
		private static void ReceiveMessageIntoBuffer(Socket receiveSocket, ref byte[] buffer)
		{
			int i = 0;
			int num = receiveSocket.Receive(buffer, i, 8192, SocketFlags.None);
			for (i += num; i < 8192; i += num)
			{
				num = receiveSocket.Receive(buffer, i, 8192 - i, SocketFlags.None);
			}
		}

		// Token: 0x0600034D RID: 845 RVA: 0x00016BCC File Offset: 0x00014DCC
		private void SendMessageInBuffer(Socket receiveSocket, byte[] buffer)
		{
			int i = 0;
			int num = receiveSocket.Send(buffer, i, 8192, SocketFlags.None);
			for (i += num; i < 8192; i += num)
			{
				num = receiveSocket.Send(buffer, i, 8192 - i, SocketFlags.None);
			}
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x0600034E RID: 846 RVA: 0x00016C1C File Offset: 0x00014E1C
		// (set) Token: 0x0600034F RID: 847 RVA: 0x00016C24 File Offset: 0x00014E24
		public bool Failed { get; private set; }

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x06000350 RID: 848 RVA: 0x00016C30 File Offset: 0x00014E30
		public float Progress
		{
			get
			{
				return this.progress;
			}
		}

		// Token: 0x040001D8 RID: 472
		private static readonly ManualResetEvent SyncTimeoutObject = new ManualResetEvent(false);

		// Token: 0x040001D9 RID: 473
		private static Client _testClient = null;

		// Token: 0x040001DA RID: 474
		private static Thread _testThread = null;

		// Token: 0x040001DC RID: 476
		private Socket socketClient;

		// Token: 0x040001DD RID: 477
		private string ip;

		// Token: 0x040001DE RID: 478
		private int port;

		// Token: 0x040001DF RID: 479
		private IPAddress ipAddress;

		// Token: 0x040001E0 RID: 480
		private Socket clientSocket;

		// Token: 0x040001E1 RID: 481
		private byte[] buffer;

		// Token: 0x040001E2 RID: 482
		private string filePath;

		// Token: 0x040001E3 RID: 483
		private string uploadName;

		// Token: 0x040001E4 RID: 484
		private int maxRetry = 0;

		// Token: 0x040001E5 RID: 485
		private float progress = 0f;

		// Token: 0x040001E6 RID: 486
		private ManualResetEvent doneEvent;
	}
}
