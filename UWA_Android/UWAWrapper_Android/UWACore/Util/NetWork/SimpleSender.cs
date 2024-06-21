using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UWA;

namespace UWACore.Util.NetWork
{
	// Token: 0x0200004A RID: 74
	internal static class SimpleSender
	{
		// Token: 0x06000337 RID: 823 RVA: 0x000160F4 File Offset: 0x000142F4
		public static string ConnectServer(object info, bool needReturn = false)
		{
			IPAddress address = IPAddress.Parse(UWACoreConfig.IP);
			Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			string result = null;
			try
			{
				socket.Connect(new IPEndPoint(address, 8082));
			}
			catch (Exception ex)
			{
				SharedUtils.Log("ConnectServer fail to connect server : " + ex.ToString());
				return result;
			}
			try
			{
				Thread.Sleep(100);
				socket.Send(Encoding.Unicode.GetBytes((string)info + "$" + (needReturn ? "uwacore-return$" : "")));
				if (needReturn)
				{
					Thread.Sleep(1000);
					byte[] array = new byte[100];
					socket.ReceiveTimeout = 3000;
					int num = socket.Receive(array);
					bool flag = num > 0;
					if (flag)
					{
						result = Encoding.Unicode.GetString(array);
					}
				}
			}
			catch (Exception ex2)
			{
				SharedUtils.Log("ConnectServer fail to connect server : " + ex2.ToString());
			}
			Thread.Sleep(500);
			socket.Shutdown(SocketShutdown.Both);
			socket.Close();
			return result;
		}

		// Token: 0x06000338 RID: 824 RVA: 0x00016248 File Offset: 0x00014448
		public static string SendAndRequestServer(object info)
		{
			return SimpleSender.ConnectServer(info, true);
		}

		// Token: 0x06000339 RID: 825 RVA: 0x00016268 File Offset: 0x00014468
		public static void SendToServer(object info)
		{
			SimpleSender.ConnectServer(info, false);
		}
	}
}
