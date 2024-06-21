using System;
using System.IO;
using System.Threading;
using UWA;
using UWA.ICSharpCode.SharpZipLib.Zip;

namespace UWACore.Util
{
	// Token: 0x02000049 RID: 73
	internal class ZipFileHelper
	{
		// Token: 0x06000333 RID: 819 RVA: 0x00015ED8 File Offset: 0x000140D8
		public ZipFileHelper(string path)
		{
			this.zipPath = path;
		}

		// Token: 0x06000334 RID: 820 RVA: 0x00015F04 File Offset: 0x00014104
		public bool Do()
		{
			bool flag = Directory.Exists(this.zipPath);
			bool result;
			if (flag)
			{
				Thread thread = new Thread(new ParameterizedThreadStart(this.ZipFiles));
				thread.Start();
				result = true;
			}
			else
			{
				bool showLog = SharedUtils.ShowLog;
				if (showLog)
				{
					SharedUtils.Log("zipPath does not exist " + this.zipPath);
				}
				result = false;
			}
			return result;
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x06000335 RID: 821 RVA: 0x00015F78 File Offset: 0x00014178
		public float Progress
		{
			get
			{
				bool flag = this.totalFiles == 0;
				float result;
				if (flag)
				{
					result = 0f;
				}
				else
				{
					result = (float)this.zippedFiles * 1f / (float)this.totalFiles;
				}
				return result;
			}
		}

		// Token: 0x06000336 RID: 822 RVA: 0x00015FC4 File Offset: 0x000141C4
		private void ZipFiles(object threadContext)
		{
			using (UWA.ICSharpCode.SharpZipLib.Zip.ZipOutputStream zipOutputStream = new UWA.ICSharpCode.SharpZipLib.Zip.ZipOutputStream(File.Create(this.zipPath + ".zip")))
			{
				zipOutputStream.SetLevel(4);
				string[] files = Directory.GetFiles(this.zipPath);
				this.totalFiles = files.Length;
				this.zippedFiles = 0;
				byte[] array = new byte[4096];
				foreach (string path in files)
				{
					using (FileStream fileStream = File.OpenRead(path))
					{
						zipOutputStream.PutNextEntry(new UWA.ICSharpCode.SharpZipLib.Zip.ZipEntry(Path.GetFileName(path))
						{
							DateTime = DateTime.Now
						});
						int num;
						do
						{
							num = fileStream.Read(array, 0, array.Length);
							zipOutputStream.Write(array, 0, num);
						}
						while (num > 0);
					}
					zipOutputStream.CloseEntry();
					this.zippedFiles++;
				}
			}
		}

		// Token: 0x040001D5 RID: 469
		private int totalFiles = 0;

		// Token: 0x040001D6 RID: 470
		private int zippedFiles = 0;

		// Token: 0x040001D7 RID: 471
		private string zipPath = "";
	}
}
