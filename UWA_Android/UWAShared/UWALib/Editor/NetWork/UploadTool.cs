using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace UWALib.Editor.NetWork
{
	// Token: 0x0200004E RID: 78
	[ComVisible(false)]
	public class UploadTool
	{
		// Token: 0x06000344 RID: 836 RVA: 0x0001C584 File Offset: 0x0001A784
		public static string GetMD5HashFromFile(string fileName)
		{
			string result;
			try
			{
				FileStream fileStream = new FileStream(fileName, FileMode.Open);
				MD5 md = new MD5CryptoServiceProvider();
				byte[] array = md.ComputeHash(fileStream);
				fileStream.Close();
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < array.Length; i++)
				{
					stringBuilder.Append(array[i].ToString("x2"));
				}
				result = stringBuilder.ToString();
			}
			catch (Exception ex)
			{
				throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
			}
			return result;
		}

		// Token: 0x06000345 RID: 837 RVA: 0x0001C628 File Offset: 0x0001A828
		public static DateTime? GetBeijingTime()
		{
			DateTime? result = null;
			WebRequest webRequest = null;
			WebResponse webResponse = null;
			try
			{
				webRequest = WebRequest.Create("http://worldtimeapi.org/api/timezone/Asia/Shanghai");
				webResponse = webRequest.GetResponse();
				string jsondata = string.Empty;
				using (Stream responseStream = webResponse.GetResponseStream())
				{
					using (StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8))
					{
						jsondata = streamReader.ReadToEnd();
					}
				}
				JSONValue jsonvalue = JSONParser.SimpleParse(jsondata);
				bool flag = jsonvalue.IsDict();
				if (flag)
				{
					JSONValue jsonvalue2 = jsonvalue["datetime"];
					bool flag2 = jsonvalue2.IsString();
					if (flag2)
					{
						result = new DateTime?(DateTime.Parse(jsonvalue2.AsString(false)));
					}
				}
			}
			catch (WebException)
			{
				return null;
			}
			catch (Exception)
			{
				return null;
			}
			finally
			{
				bool flag3 = webResponse != null;
				if (flag3)
				{
					webResponse.Close();
				}
				bool flag4 = webRequest != null;
				if (flag4)
				{
					webRequest.Abort();
				}
			}
			return result;
		}

		// Token: 0x06000346 RID: 838 RVA: 0x0001C79C File Offset: 0x0001A99C
		private static DateTime GetTime(string timeStamp)
		{
			DateTime dateTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
			long ticks = long.Parse(timeStamp + "0000000");
			TimeSpan value = new TimeSpan(ticks);
			return dateTime.Add(value);
		}

		// Token: 0x06000347 RID: 839 RVA: 0x0001C7F0 File Offset: 0x0001A9F0
		public static int CalculatePartCount(long totalSize, int partSize)
		{
			int num = (int)(totalSize / (long)partSize);
			bool flag = totalSize % (long)partSize != 0L;
			if (flag)
			{
				num++;
			}
			return num;
		}

		// Token: 0x02000108 RID: 264
		public class UploadCompletedEventArgs
		{
			// Token: 0x06000A01 RID: 2561 RVA: 0x000484E8 File Offset: 0x000466E8
			internal UploadCompletedEventArgs(string etag, bool skip)
			{
				this.ETag = etag;
				this.Skip = skip;
			}

			// Token: 0x04000695 RID: 1685
			public string ETag;

			// Token: 0x04000696 RID: 1686
			public bool Skip;
		}

		// Token: 0x02000109 RID: 265
		public class UploadMultipartCompletedEventArgs
		{
			// Token: 0x06000A02 RID: 2562 RVA: 0x00048500 File Offset: 0x00046700
			internal UploadMultipartCompletedEventArgs(float percent)
			{
				this.Percent = percent;
			}

			// Token: 0x04000697 RID: 1687
			public float Percent;
		}

		// Token: 0x0200010A RID: 266
		// (Invoke) Token: 0x06000A04 RID: 2564
		public delegate void UploadCompletedEventHandler(object sender, UploadTool.UploadCompletedEventArgs e);

		// Token: 0x0200010B RID: 267
		// (Invoke) Token: 0x06000A08 RID: 2568
		public delegate void UploadMultipartCompletedEventHandler(object sender, UploadTool.UploadMultipartCompletedEventArgs e);
	}
}
