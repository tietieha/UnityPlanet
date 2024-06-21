using System;
using System.IO;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Core
{
	// Token: 0x020000BE RID: 190
	[ComVisible(false)]
	public class FileSystemScanner
	{
		// Token: 0x0600089B RID: 2203 RVA: 0x0003F734 File Offset: 0x0003D934
		public FileSystemScanner(string filter)
		{
			this.fileFilter_ = new PathFilter(filter);
		}

		// Token: 0x0600089C RID: 2204 RVA: 0x0003F74C File Offset: 0x0003D94C
		public FileSystemScanner(string fileFilter, string directoryFilter)
		{
			this.fileFilter_ = new PathFilter(fileFilter);
			this.directoryFilter_ = new PathFilter(directoryFilter);
		}

		// Token: 0x0600089D RID: 2205 RVA: 0x0003F770 File Offset: 0x0003D970
		public FileSystemScanner(IScanFilter fileFilter)
		{
			this.fileFilter_ = fileFilter;
		}

		// Token: 0x0600089E RID: 2206 RVA: 0x0003F784 File Offset: 0x0003D984
		public FileSystemScanner(IScanFilter fileFilter, IScanFilter directoryFilter)
		{
			this.fileFilter_ = fileFilter;
			this.directoryFilter_ = directoryFilter;
		}

		// Token: 0x0600089F RID: 2207 RVA: 0x0003F79C File Offset: 0x0003D99C
		private bool OnDirectoryFailure(string directory, Exception e)
		{
			DirectoryFailureHandler directoryFailure = this.DirectoryFailure;
			bool flag = directoryFailure != null;
			bool flag2 = flag;
			if (flag2)
			{
				ScanFailureEventArgs scanFailureEventArgs = new ScanFailureEventArgs(directory, e);
				directoryFailure(this, scanFailureEventArgs);
				this.alive_ = scanFailureEventArgs.ContinueRunning;
			}
			return flag;
		}

		// Token: 0x060008A0 RID: 2208 RVA: 0x0003F7EC File Offset: 0x0003D9EC
		private bool OnFileFailure(string file, Exception e)
		{
			FileFailureHandler fileFailure = this.FileFailure;
			bool flag = fileFailure != null;
			bool flag2 = flag;
			if (flag2)
			{
				ScanFailureEventArgs scanFailureEventArgs = new ScanFailureEventArgs(file, e);
				this.FileFailure(this, scanFailureEventArgs);
				this.alive_ = scanFailureEventArgs.ContinueRunning;
			}
			return flag;
		}

		// Token: 0x060008A1 RID: 2209 RVA: 0x0003F840 File Offset: 0x0003DA40
		private void OnProcessFile(string file)
		{
			ProcessFileHandler processFile = this.ProcessFile;
			bool flag = processFile != null;
			if (flag)
			{
				ScanEventArgs scanEventArgs = new ScanEventArgs(file);
				processFile(this, scanEventArgs);
				this.alive_ = scanEventArgs.ContinueRunning;
			}
		}

		// Token: 0x060008A2 RID: 2210 RVA: 0x0003F884 File Offset: 0x0003DA84
		private void OnCompleteFile(string file)
		{
			CompletedFileHandler completedFile = this.CompletedFile;
			bool flag = completedFile != null;
			if (flag)
			{
				ScanEventArgs scanEventArgs = new ScanEventArgs(file);
				completedFile(this, scanEventArgs);
				this.alive_ = scanEventArgs.ContinueRunning;
			}
		}

		// Token: 0x060008A3 RID: 2211 RVA: 0x0003F8C8 File Offset: 0x0003DAC8
		private void OnProcessDirectory(string directory, bool hasMatchingFiles)
		{
			ProcessDirectoryHandler processDirectory = this.ProcessDirectory;
			bool flag = processDirectory != null;
			if (flag)
			{
				DirectoryEventArgs directoryEventArgs = new DirectoryEventArgs(directory, hasMatchingFiles);
				processDirectory(this, directoryEventArgs);
				this.alive_ = directoryEventArgs.ContinueRunning;
			}
		}

		// Token: 0x060008A4 RID: 2212 RVA: 0x0003F90C File Offset: 0x0003DB0C
		public void Scan(string directory, bool recurse)
		{
			this.alive_ = true;
			this.ScanDir(directory, recurse);
		}

		// Token: 0x060008A5 RID: 2213 RVA: 0x0003F920 File Offset: 0x0003DB20
		private void ScanDir(string directory, bool recurse)
		{
			try
			{
				string[] files = Directory.GetFiles(directory);
				bool flag = false;
				for (int i = 0; i < files.Length; i++)
				{
					bool flag2 = !this.fileFilter_.IsMatch(files[i]);
					if (flag2)
					{
						files[i] = null;
					}
					else
					{
						flag = true;
					}
				}
				this.OnProcessDirectory(directory, flag);
				bool flag3 = this.alive_ && flag;
				if (flag3)
				{
					foreach (string text in files)
					{
						try
						{
							bool flag4 = text != null;
							if (flag4)
							{
								this.OnProcessFile(text);
								bool flag5 = !this.alive_;
								if (flag5)
								{
									break;
								}
							}
						}
						catch (Exception e)
						{
							bool flag6 = !this.OnFileFailure(text, e);
							if (flag6)
							{
								throw;
							}
						}
					}
				}
			}
			catch (Exception e2)
			{
				bool flag7 = !this.OnDirectoryFailure(directory, e2);
				if (flag7)
				{
					throw;
				}
			}
			bool flag8 = this.alive_ && recurse;
			if (flag8)
			{
				try
				{
					string[] directories = Directory.GetDirectories(directory);
					foreach (string text2 in directories)
					{
						bool flag9 = this.directoryFilter_ == null || this.directoryFilter_.IsMatch(text2);
						if (flag9)
						{
							this.ScanDir(text2, true);
							bool flag10 = !this.alive_;
							if (flag10)
							{
								break;
							}
						}
					}
				}
				catch (Exception e3)
				{
					bool flag11 = !this.OnDirectoryFailure(directory, e3);
					if (flag11)
					{
						throw;
					}
				}
			}
		}

		// Token: 0x04000526 RID: 1318
		public ProcessDirectoryHandler ProcessDirectory;

		// Token: 0x04000527 RID: 1319
		public ProcessFileHandler ProcessFile;

		// Token: 0x04000528 RID: 1320
		public CompletedFileHandler CompletedFile;

		// Token: 0x04000529 RID: 1321
		public DirectoryFailureHandler DirectoryFailure;

		// Token: 0x0400052A RID: 1322
		public FileFailureHandler FileFailure;

		// Token: 0x0400052B RID: 1323
		private IScanFilter fileFilter_;

		// Token: 0x0400052C RID: 1324
		private IScanFilter directoryFilter_;

		// Token: 0x0400052D RID: 1325
		private bool alive_;
	}
}
