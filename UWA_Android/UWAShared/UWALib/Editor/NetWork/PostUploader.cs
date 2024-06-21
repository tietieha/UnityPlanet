using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace UWALib.Editor.NetWork
{
	// Token: 0x0200004F RID: 79
	internal class PostUploader
	{
		// Token: 0x06000349 RID: 841 RVA: 0x0001C834 File Offset: 0x0001AA34
		public PostUploader(PostUploader.Target t)
		{
			this.UploadTarget = t;
		}

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x0600034A RID: 842 RVA: 0x0001C84C File Offset: 0x0001AA4C
		// (remove) Token: 0x0600034B RID: 843 RVA: 0x0001C888 File Offset: 0x0001AA88
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event UploadTool.UploadCompletedEventHandler UploadCompleted;

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x0600034C RID: 844 RVA: 0x0001C8C4 File Offset: 0x0001AAC4
		// (remove) Token: 0x0600034D RID: 845 RVA: 0x0001C900 File Offset: 0x0001AB00
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event UploadTool.UploadMultipartCompletedEventHandler UploadMultipartCompleted;

		// Token: 0x0600034E RID: 846 RVA: 0x0001C93C File Offset: 0x0001AB3C
		public void MultipartUploadProgressAsync(string url, JSONValue rspdata, string filePath, object userToken)
		{
			UwaWebsiteClient.Log("MultipartUpload Thread.");
			this.uploadingThread = new Thread(new ParameterizedThreadStart(this.MultipartUploadProgressThreaded))
			{
				IsBackground = false
			};
			this.uploadingThread.Start(new object[]
			{
				url,
				rspdata,
				filePath,
				userToken
			});
		}

		// Token: 0x0600034F RID: 847 RVA: 0x0001C9A0 File Offset: 0x0001ABA0
		private void MultipartUploadProgressThreaded(object param)
		{
			object[] array = param as object[];
			UwaWebsiteClient.Log("MultipartUploadProgress.");
			try
			{
				this.MultipartUploadProgress(array[0] as string, (JSONValue)array[1], array[2] as string, array[3]);
			}
			catch (WebException ex)
			{
				try
				{
					using (Stream responseStream = ex.Response.GetResponseStream())
					{
						using (StreamReader streamReader = new StreamReader(responseStream))
						{
							string str = streamReader.ReadToEnd().Trim();
							UwaWebsiteClient.Log("MultipartUploadProgress failed: " + str);
						}
					}
				}
				catch (Exception)
				{
				}
				string str2 = "MultipartUploadProgress failed: ";
				Uri responseUri = ex.Response.ResponseUri;
				UwaWebsiteClient.Log(str2 + ((responseUri != null) ? responseUri.ToString() : null));
				UwaWebsiteClient.Log("MultipartUploadProgress failed: " + ex.Message);
				string str3 = "Upload WebException : ";
				WebException ex2 = ex;
				UwaWebsiteClient.Log(str3 + ((ex2 != null) ? ex2.ToString() : null));
				bool flag = this.UploadCompleted != null;
				if (flag)
				{
					this.UploadCompleted(null, new UploadTool.UploadCompletedEventArgs(null, false));
				}
				this.UploadMultipartCompleted = null;
				this.UploadCompleted = null;
			}
			catch (Exception ex3)
			{
				string str4 = "Upload Exception : ";
				Exception ex4 = ex3;
				UwaWebsiteClient.Log(str4 + ((ex4 != null) ? ex4.ToString() : null));
				bool flag2 = this.UploadCompleted != null;
				if (flag2)
				{
					this.UploadCompleted(null, new UploadTool.UploadCompletedEventArgs(null, false));
				}
				this.UploadMultipartCompleted = null;
				this.UploadCompleted = null;
			}
		}

		// Token: 0x06000350 RID: 848 RVA: 0x0001CBD4 File Offset: 0x0001ADD4
		private static string GetContentStr(string key, string value)
		{
			return string.Concat(new string[]
			{
				"--",
				PostUploader.boundary,
				PostUploader.enter,
				"Content-Disposition: form-data; name=\"",
				key,
				"\"",
				PostUploader.enter,
				PostUploader.enter,
				value,
				PostUploader.enter
			});
		}

		// Token: 0x06000351 RID: 849 RVA: 0x0001CC44 File Offset: 0x0001AE44
		private List<byte[]> RspData2ByteList(JSONValue jsonvalue)
		{
			Dictionary<string, JSONValue> dictionary = jsonvalue.AsDict(false);
			List<byte[]> list = new List<byte[]>();
			foreach (KeyValuePair<string, JSONValue> keyValuePair in dictionary)
			{
				string contentStr = PostUploader.GetContentStr(keyValuePair.Key, keyValuePair.Value);
				UwaWebsiteClient.Log(contentStr);
				list.Add(Encoding.UTF8.GetBytes(contentStr));
			}
			return list;
		}

		// Token: 0x06000352 RID: 850 RVA: 0x0001CCE4 File Offset: 0x0001AEE4
		private static string GetOssPostPolicy(string bucket, string start)
		{
			bool flag = bucket == "uwa-private";
			string result;
			if (flag)
			{
				result = (UwaWebsiteClient.Debug ? PostUploader.uwaprivatedebugpolicy : PostUploader.uwaprivatepolicy);
			}
			else
			{
				bool flag2 = bucket == "uwa-test";
				if (flag2)
				{
					result = PostUploader.uwatestpolicy;
				}
				else
				{
					bool flag3 = bucket == "uwa-pipeline";
					if (flag3)
					{
						result = (UwaWebsiteClient.Debug ? PostUploader.uwapipelinedebugpolicy : PostUploader.uwapipelinepolicy);
					}
					else
					{
						bool flag4 = bucket == "uwa-gpm";
						if (flag4)
						{
							result = (UwaWebsiteClient.Debug ? PostUploader.uwagpmdebugpolicy : PostUploader.uwagpmpolicy);
						}
						else
						{
							result = PostUploader.uwatestpolicy;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000353 RID: 851 RVA: 0x0001CDB4 File Offset: 0x0001AFB4
		private static string ComputeSignature(string key, string data)
		{
			string result;
			using (KeyedHashAlgorithm keyedHashAlgorithm = KeyedHashAlgorithm.Create("HmacSHA1".ToUpperInvariant()))
			{
				keyedHashAlgorithm.Key = Encoding.UTF8.GetBytes(key.ToCharArray());
				result = Convert.ToBase64String(keyedHashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(data.ToCharArray())));
			}
			return result;
		}

		// Token: 0x06000354 RID: 852 RVA: 0x0001CE2C File Offset: 0x0001B02C
		private static void AddContentToStream(ref Stream s, string c1, string c2)
		{
			string contentStr = PostUploader.GetContentStr(c1, c2);
			UwaWebsiteClient.Log(contentStr);
			byte[] bytes = Encoding.UTF8.GetBytes(contentStr);
			s.Write(bytes, 0, bytes.Length);
		}

		// Token: 0x06000355 RID: 853 RVA: 0x0001CE68 File Offset: 0x0001B068
		private void MakeGZip(string srcPath, string targetPath)
		{
			bool flag = File.Exists(targetPath);
			if (flag)
			{
				File.Delete(targetPath);
			}
			using (FileStream fileStream = File.OpenRead(srcPath))
			{
				using (FileStream fileStream2 = File.OpenWrite(targetPath))
				{
					using (GZipStream gzipStream = new GZipStream(fileStream2, CompressionMode.Compress))
					{
						byte[] array = new byte[1048576];
						for (int i = fileStream.Read(array, 0, array.Length); i > 0; i = fileStream.Read(array, 0, i))
						{
							gzipStream.Write(array, 0, i);
						}
					}
				}
			}
		}

		// Token: 0x06000356 RID: 854 RVA: 0x0001CF50 File Offset: 0x0001B150
		public void MultipartUploadProgress(string url, JSONValue jsonvalue, string filePath, object userToken)
		{
			UwaWebsiteClient.Log("Check File.");
			bool flag = !File.Exists(filePath);
			if (!flag)
			{
				bool flag2 = jsonvalue["uploadPath"].AsString(false).EndsWith(".json");
				string text = filePath;
				bool flag3 = flag2;
				if (flag3)
				{
					try
					{
						this.MakeGZip(filePath, filePath + ".gzip");
						text = filePath + ".gzip";
					}
					catch
					{
					}
				}
				UwaWebsiteClient.Log(url);
				FileInfo fileInfo = new FileInfo(text);
				long length = fileInfo.Length;
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
				httpWebRequest.AllowWriteStreamBuffering = false;
				httpWebRequest.SendChunked = true;
				httpWebRequest.Method = "POST";
				httpWebRequest.KeepAlive = false;
				httpWebRequest.ProtocolVersion = HttpVersion.Version11;
				httpWebRequest.Credentials = CredentialCache.DefaultCredentials;
				httpWebRequest.ContentType = "multipart/form-data; boundary=" + PostUploader.boundary;
				httpWebRequest.Timeout = 6000000;
				string text2 = null;
				string c = null;
				string text3 = "application/zip";
				string value = "";
				bool flag4 = flag2;
				if (flag4)
				{
					text3 = "application/json";
					value = "gzip";
				}
				bool flag5 = this.UploadTarget == PostUploader.Target.AWS;
				if (flag5)
				{
					httpWebRequest.Headers.Set("Access-Control-Allow-Origin", "*");
					httpWebRequest.Headers.Set("x-amz-algorithm", jsonvalue["signature"]["x-amz-algorithm"]);
					httpWebRequest.Headers.Set("x-amz-date", jsonvalue["signature"]["x-amz-date"]);
					httpWebRequest.Headers.Set("x-amz-server-side-encryption", jsonvalue["signature"]["x-amz-server-side-encryption"]);
					httpWebRequest.Headers.Set("x-amz-signature", jsonvalue["signature"]["x-amz-signature"]);
				}
				else
				{
					bool flag6 = this.UploadTarget == PostUploader.Target.OSS;
					if (flag6)
					{
						string value2 = jsonvalue["accessKeyId"].AsString(false);
						string value3 = jsonvalue["meta"].AsString(false);
						string value4 = jsonvalue["securityToken"].AsString(false);
						c = jsonvalue["policy"].AsString(false);
						bool flag7 = jsonvalue.ContainsKey("src_signature");
						if (flag7)
						{
							text2 = jsonvalue["src_signature"].AsString(false);
						}
						else
						{
							text2 = jsonvalue["signature"].AsString(false);
							jsonvalue["src_signature"] = text2;
						}
						jsonvalue["signature"] = JSONValue.NewDict();
						jsonvalue["signature"].Set("x-oss-security-token", value4);
						jsonvalue["signature"].Set("OSSAccessKeyId", value2);
						bool flag8 = !string.IsNullOrEmpty(value3);
						if (flag8)
						{
							jsonvalue["signature"].Set("x-oss-meta-datadesc", value3);
						}
						httpWebRequest.Headers.Set("Access-Control-Allow-Origin", "*");
						httpWebRequest.Headers.Set("x-oss-security-token", value4);
						httpWebRequest.Headers.Set("Authorization", text2);
					}
				}
				bool flag9 = length == 0L;
				if (!flag9)
				{
					List<byte[]> list = this.RspData2ByteList(jsonvalue["signature"]);
					Stream requestStream = httpWebRequest.GetRequestStream();
					for (int i = 0; i < list.Count; i++)
					{
						requestStream.Write(list[i], 0, list[i].Length);
					}
					bool flag10 = this.UploadTarget == PostUploader.Target.OSS;
					if (flag10)
					{
						PostUploader.AddContentToStream(ref requestStream, "Signature", text2);
						PostUploader.AddContentToStream(ref requestStream, "policy", c);
						PostUploader.AddContentToStream(ref requestStream, "key", jsonvalue["uploadPath"]);
						PostUploader.AddContentToStream(ref requestStream, "region", "oss-cn-beijing.aliyuncs.com");
						PostUploader.AddContentToStream(ref requestStream, "keyStart", Path.GetDirectoryName(jsonvalue["uploadPath"]));
						PostUploader.AddContentToStream(ref requestStream, "fileName", Path.GetFileName(jsonvalue["uploadPath"]));
					}
					bool flag11 = !string.IsNullOrEmpty(value);
					if (flag11)
					{
						string contentStr = PostUploader.GetContentStr("Content-Encoding", value);
						UwaWebsiteClient.Log(contentStr);
						byte[] bytes = Encoding.UTF8.GetBytes(contentStr);
						requestStream.Write(bytes, 0, bytes.Length);
					}
					string contentStr2 = PostUploader.GetContentStr("id", "WU_FILE_0");
					UwaWebsiteClient.Log(contentStr2);
					byte[] bytes2 = Encoding.UTF8.GetBytes(contentStr2);
					requestStream.Write(bytes2, 0, bytes2.Length);
					string contentStr3 = PostUploader.GetContentStr("name", Path.GetFileName(filePath));
					UwaWebsiteClient.Log(contentStr3);
					byte[] bytes3 = Encoding.UTF8.GetBytes(contentStr3);
					requestStream.Write(bytes3, 0, bytes3.Length);
					string contentStr4 = PostUploader.GetContentStr("type", text3);
					UwaWebsiteClient.Log(contentStr4);
					byte[] bytes4 = Encoding.UTF8.GetBytes(contentStr4);
					requestStream.Write(bytes4, 0, bytes4.Length);
					string contentStr5 = PostUploader.GetContentStr("size", length.ToString());
					UwaWebsiteClient.Log(contentStr5);
					byte[] bytes5 = Encoding.UTF8.GetBytes(contentStr5);
					requestStream.Write(bytes5, 0, bytes5.Length);
					string contentStr6 = PostUploader.GetContentStr("lastModifiedDate", File.GetLastWriteTime(filePath).ToString());
					UwaWebsiteClient.Log(contentStr6);
					byte[] bytes6 = Encoding.UTF8.GetBytes(contentStr6);
					requestStream.Write(bytes6, 0, bytes6.Length);
					string text4 = string.Concat(new string[]
					{
						"--",
						PostUploader.boundary,
						PostUploader.enter,
						"Content-Disposition: form-data; name=\"file\"; filename=\"",
						Path.GetFileName(filePath),
						"\"",
						PostUploader.enter,
						"Content-Type: ",
						text3,
						PostUploader.enter,
						PostUploader.enter
					});
					UwaWebsiteClient.Log(text4);
					byte[] bytes7 = Encoding.UTF8.GetBytes(text4);
					requestStream.Write(bytes7, 0, bytes7.Length);
					try
					{
						using (FileStream fileStream = new FileStream(text, FileMode.Open, FileAccess.Read))
						{
							byte[] array = new byte[10240];
							long length2 = fileStream.Length;
							long num = 0L;
							while (num < length2)
							{
								int num2 = fileStream.Read(array, 0, array.Length);
								bool flag12 = num2 > 0;
								if (!flag12)
								{
									break;
								}
								requestStream.Write(array, 0, num2);
								num += (long)num2;
								bool flag13 = this.UploadMultipartCompleted != null;
								if (flag13)
								{
									this.UploadMultipartCompleted(null, new UploadTool.UploadMultipartCompletedEventArgs(1f * (float)num / (float)length2 * 0.6f));
								}
								Array.Clear(array, 0, array.Length);
							}
						}
					}
					catch (Exception ex)
					{
						string str = "Upload ex : ";
						Exception ex2 = ex;
						UwaWebsiteClient.Log(str + ((ex2 != null) ? ex2.ToString() : null));
						bool flag14 = this.UploadCompleted != null;
						if (flag14)
						{
							this.UploadCompleted(null, new UploadTool.UploadCompletedEventArgs(null, false));
						}
						this.UploadMultipartCompleted = null;
						this.UploadCompleted = null;
						return;
					}
					string text5 = string.Concat(new string[]
					{
						PostUploader.enter,
						"--",
						PostUploader.boundary,
						"--",
						PostUploader.enter
					});
					UwaWebsiteClient.Log(text5);
					byte[] bytes8 = Encoding.UTF8.GetBytes(text5);
					requestStream.Write(bytes8, 0, bytes8.Length);
					requestStream.Close();
					Thread thread = new Thread(new ThreadStart(this.UpdatePercentWhenWaiting));
					thread.Start();
					HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
					UwaWebsiteClient.Log("GetResponse");
					string responseHeader = httpWebResponse.GetResponseHeader("ETag");
					UwaWebsiteClient.Log("GetResponse Etag : " + responseHeader);
					PostUploader.quit = true;
					Thread.Sleep(3000);
					try
					{
						bool isAlive = thread.IsAlive;
						if (isAlive)
						{
							thread.Abort();
						}
					}
					catch (Exception)
					{
					}
					bool flag15 = this.UploadCompleted != null;
					if (flag15)
					{
						this.UploadCompleted(null, new UploadTool.UploadCompletedEventArgs(responseHeader, false));
					}
					this.UploadMultipartCompleted = null;
					this.UploadCompleted = null;
				}
			}
		}

		// Token: 0x06000357 RID: 855 RVA: 0x0001D8A4 File Offset: 0x0001BAA4
		private void UpdatePercentWhenWaiting()
		{
			try
			{
				int num = 0;
				float num2 = 0.009999999f;
				float num3 = 0.6f;
				for (;;)
				{
					bool flag = PostUploader.quit;
					if (flag)
					{
						break;
					}
					Thread.Sleep(1000);
					num++;
					bool flag2 = num == 15;
					if (flag2)
					{
						num2 /= 2f;
					}
					bool flag3 = num == 30;
					if (flag3)
					{
						num2 /= 2f;
					}
					bool flag4 = num == 60;
					if (flag4)
					{
						num2 /= 2f;
					}
					bool flag5 = num == 120;
					if (flag5)
					{
						num2 /= 2f;
					}
					bool flag6 = num == 180;
					if (flag6)
					{
						num2 /= 2f;
					}
					bool flag7 = num == 240;
					if (flag7)
					{
						num2 /= 2f;
					}
					bool flag8 = num == 300;
					if (flag8)
					{
						num2 /= 2f;
					}
					num3 += num2;
					bool flag9 = num3 > 0.9f;
					if (flag9)
					{
						break;
					}
					bool flag10 = this.UploadMultipartCompleted != null;
					if (flag10)
					{
						this.UploadMultipartCompleted(null, new UploadTool.UploadMultipartCompletedEventArgs(num3));
					}
				}
			}
			catch (ThreadAbortException ex)
			{
			}
		}

		// Token: 0x0400024D RID: 589
		public PostUploader.Target UploadTarget = PostUploader.Target.Unset;

		// Token: 0x04000250 RID: 592
		public Thread uploadingThread;

		// Token: 0x04000251 RID: 593
		private static string enter = "\r\n";

		// Token: 0x04000252 RID: 594
		private static string boundary = "uwa0cn0got";

		// Token: 0x04000253 RID: 595
		private static string uwaprivatedebugpolicy = "{ \"expiration\": \"2050-12-01T12:00:00.000Z\", \"conditions\": [ {\"bucket\": \"uwa-private\" }, [\"starts-with\", \"$key\", \"projectGroup/file/test/\"] ] }";

		// Token: 0x04000254 RID: 596
		private static string uwaprivatepolicy = "{ \"expiration\": \"2050-12-01T12:00:00.000Z\", \"conditions\": [ {\"bucket\": \"uwa-private\" }, [\"starts-with\", \"$key\", \"projectGroup/file/online/\"] ] }";

		// Token: 0x04000255 RID: 597
		private static string uwatestpolicy = "{ \"expiration\": \"2050-12-01T12:00:00.000Z\", \"conditions\": [ {\"bucket\": \"uwa-test\" }, [\"starts-with\", \"$key\", \"got/\"] ] }";

		// Token: 0x04000256 RID: 598
		private static string uwapipelinepolicy = "{ \"expiration\": \"2050-12-01T12:00:00.000Z\", \"conditions\": [ {\"bucket\": \"uwa-pipeline\" }, [\"starts-with\", \"$key\", \"release/\"] ] }";

		// Token: 0x04000257 RID: 599
		private static string uwapipelinedebugpolicy = "{ \"expiration\": \"2050-12-01T12:00:00.000Z\", \"conditions\": [ {\"bucket\": \"uwa-pipeline\" }, [\"starts-with\", \"$key\", \"test/\"] ] }";

		// Token: 0x04000258 RID: 600
		private static string uwagpmpolicy = "{ \"expiration\": \"2050-12-01T12:00:00.000Z\", \"conditions\": [ {\"bucket\": \"uwa-gpm\" }, [\"starts-with\", \"$key\", \"release/\"] ] }";

		// Token: 0x04000259 RID: 601
		private static string uwagpmdebugpolicy = "{ \"expiration\": \"2050-12-01T12:00:00.000Z\", \"conditions\": [ {\"bucket\": \"uwa-gpm\" }, [\"starts-with\", \"$key\", \"test/\"] ] }";

		// Token: 0x0400025A RID: 602
		private static bool quit = false;

		// Token: 0x0200010C RID: 268
		public enum Target
		{
			// Token: 0x04000699 RID: 1689
			AWS,
			// Token: 0x0400069A RID: 1690
			OSS,
			// Token: 0x0400069B RID: 1691
			Unset
		}
	}
}
