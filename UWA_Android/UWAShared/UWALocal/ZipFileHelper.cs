using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;
using UWA.ICSharpCode.SharpZipLib.Zip;

namespace UWALocal
{
	// Token: 0x0200004D RID: 77
	[ComVisible(false)]
	public class ZipFileHelper
	{
		// Token: 0x0600033B RID: 827 RVA: 0x0001BF8C File Offset: 0x0001A18C
		public ZipFileHelper(string path)
		{
			this.zipPath = path;
		}

		// Token: 0x0600033C RID: 828 RVA: 0x0001BFF4 File Offset: 0x0001A1F4
		public void AddFile(string path)
		{
			bool flag = File.Exists(path);
			if (flag)
			{
				this.addedFiles.Add(path);
			}
		}

		// Token: 0x0600033D RID: 829 RVA: 0x0001C024 File Offset: 0x0001A224
		public void Check()
		{
			bool flag = !this.doThread.IsAlive;
			if (flag)
			{
				this.doThread = new Thread(new ParameterizedThreadStart(this.ZipFiles))
				{
					IsBackground = false
				};
				this.doThread.Start();
			}
		}

		// Token: 0x0600033E RID: 830 RVA: 0x0001C078 File Offset: 0x0001A278
		public bool Do()
		{
			bool flag = Directory.Exists(this.zipPath);
			bool result;
			if (flag)
			{
				this.doThread = new Thread(new ParameterizedThreadStart(this.ZipFiles))
				{
					IsBackground = false
				};
				this.doThread.Start();
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x0600033F RID: 831 RVA: 0x0001C0D8 File Offset: 0x0001A2D8
		public float Progress
		{
			get
			{
				bool flag = this.totalFiles == 0L;
				float result;
				if (flag)
				{
					result = 0f;
				}
				else
				{
					bool flag2 = this.isDone != null && this.isDone.Value;
					if (flag2)
					{
						result = 1f;
					}
					else
					{
						result = (float)this.zippedFiles * 1f / (float)this.totalFiles;
					}
				}
				return result;
			}
		}

		// Token: 0x06000340 RID: 832 RVA: 0x0001C158 File Offset: 0x0001A358
		private void ZipFiles(object threadContext)
		{
			try
			{
				bool flag = File.Exists(this.zipPath + ".zip");
				if (flag)
				{
					File.Delete(this.zipPath + ".zip");
				}
			}
			catch (Exception)
			{
				this.isDone = new bool?(false);
				return;
			}
			bool flag2 = this.needProgress || this.addedFiles.Count != 0;
			if (flag2)
			{
				string[] files = Directory.GetFiles(this.zipPath, "*", SearchOption.AllDirectories);
				this.addedFiles.AddRange(files);
				this.totalFiles = 0L;
				for (int i = 0; i < this.addedFiles.Count; i++)
				{
					FileInfo fileInfo = new FileInfo(this.addedFiles[i]);
					this.totalFiles += fileInfo.Length;
				}
				this.zippedFiles = 0L;
				try
				{
					this.ZipWork(this.zipPath, this.addedFiles.ToArray());
				}
				catch (Exception ex)
				{
					Debug.Log("Zipwork Ex:" + ex.ToString());
					this.isDone = new bool?(false);
					return;
				}
			}
			else
			{
				this.totalFiles = 1L;
				this.zippedFiles = 0L;
				FastZip fastZip = new FastZip();
				fastZip.CreateZip(this.zipPath + ".zip", this.zipPath, true, "*");
				this.zippedFiles = 1L;
			}
			this.isDone = new bool?(true);
		}

		// Token: 0x06000341 RID: 833 RVA: 0x0001C310 File Offset: 0x0001A510
		private void ZipWork(string zipPath, string[] paths)
		{
			string text = Path.GetFullPath(zipPath) + "\\";
			string fileName = Path.GetFileName(zipPath);
			using (ZipOutputStream zipOutputStream = new ZipOutputStream(File.Create(zipPath + ".zip")))
			{
				zipOutputStream.SetLevel(4);
				byte[] array = new byte[4096];
				foreach (string text2 in paths)
				{
					FileInfo fileInfo = new FileInfo(text2);
					int num;
					bool flag = this.skipScreenshot && int.TryParse(Path.GetFileNameWithoutExtension(text2), out num) && text2.EndsWith(".jpg", StringComparison.Ordinal);
					if (flag)
					{
						this.zippedFiles += fileInfo.Length;
					}
					else
					{
						using (FileStream fileStream = fileInfo.OpenRead())
						{
							string relativePath = ZipFileHelper.GetRelativePath2(fileName, text2);
							zipOutputStream.PutNextEntry(new ZipEntry(relativePath)
							{
								DateTime = DateTime.Now
							});
							int num2;
							do
							{
								num2 = fileStream.Read(array, 0, array.Length);
								zipOutputStream.Write(array, 0, num2);
								this.zippedFiles += (long)num2;
							}
							while (num2 > 0);
						}
						zipOutputStream.CloseEntry();
					}
				}
			}
		}

		// Token: 0x06000342 RID: 834 RVA: 0x0001C4B8 File Offset: 0x0001A6B8
		private static string GetRelativePath2(string folderName, string targetPath)
		{
			int num = targetPath.IndexOf(folderName);
			bool flag = num == -1;
			string result;
			if (flag)
			{
				result = targetPath;
			}
			else
			{
				string text = targetPath.Substring(num + folderName.Length);
				while (text[0] == Path.DirectorySeparatorChar || text[0] == Path.AltDirectorySeparatorChar)
				{
					text = text.Substring(1);
				}
				result = text;
			}
			return result;
		}

		// Token: 0x06000343 RID: 835 RVA: 0x0001C538 File Offset: 0x0001A738
		private static string GetRelativePath(string basePath, string targetPath)
		{
			Uri uri = new Uri(basePath);
			Uri uri2 = new Uri(targetPath);
			Uri uri3 = uri.MakeRelativeUri(uri2);
			string text = Uri.UnescapeDataString(uri3.ToString());
			return text.Replace('/', '\\');
		}

		// Token: 0x04000245 RID: 581
		private long totalFiles = 0L;

		// Token: 0x04000246 RID: 582
		private long zippedFiles = 0L;

		// Token: 0x04000247 RID: 583
		private string zipPath = "";

		// Token: 0x04000248 RID: 584
		public bool? isDone = null;

		// Token: 0x04000249 RID: 585
		private Thread doThread = null;

		// Token: 0x0400024A RID: 586
		private List<string> addedFiles = new List<string>();

		// Token: 0x0400024B RID: 587
		public bool skipScreenshot = false;

		// Token: 0x0400024C RID: 588
		public bool needProgress = true;
	}
}
