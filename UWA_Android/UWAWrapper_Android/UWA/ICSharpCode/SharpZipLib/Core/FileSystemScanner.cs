using System;
using System.IO;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Core
{
	// Token: 0x020000AF RID: 175
	[ComVisible(false)]
	public class FileSystemScanner
	{
		// Token: 0x060007BF RID: 1983 RVA: 0x0003297C File Offset: 0x00030B7C
		public FileSystemScanner(string filter)
		{
			this.fileFilter_ = new PathFilter(filter);
		}

		// Token: 0x060007C0 RID: 1984 RVA: 0x00032994 File Offset: 0x00030B94
		public FileSystemScanner(string fileFilter, string directoryFilter)
		{
			this.fileFilter_ = new PathFilter(fileFilter);
			this.directoryFilter_ = new PathFilter(directoryFilter);
		}

		// Token: 0x060007C1 RID: 1985 RVA: 0x000329B8 File Offset: 0x00030BB8
		public FileSystemScanner(IScanFilter fileFilter)
		{
			this.fileFilter_ = fileFilter;
		}

		// Token: 0x060007C2 RID: 1986 RVA: 0x000329CC File Offset: 0x00030BCC
		public FileSystemScanner(IScanFilter fileFilter, IScanFilter directoryFilter)
		{
			this.fileFilter_ = fileFilter;
			this.directoryFilter_ = directoryFilter;
		}

		// Token: 0x060007C3 RID: 1987 RVA: 0x000329E4 File Offset: 0x00030BE4
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

		// Token: 0x060007C4 RID: 1988 RVA: 0x00032A34 File Offset: 0x00030C34
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

		// Token: 0x060007C5 RID: 1989 RVA: 0x00032A88 File Offset: 0x00030C88
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

		// Token: 0x060007C6 RID: 1990 RVA: 0x00032ACC File Offset: 0x00030CCC
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

		// Token: 0x060007C7 RID: 1991 RVA: 0x00032B10 File Offset: 0x00030D10
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

		// Token: 0x060007C8 RID: 1992 RVA: 0x00032B54 File Offset: 0x00030D54
		public void Scan(string directory, bool recurse)
		{
			this.alive_ = true;
			this.ScanDir(directory, recurse);
		}

		// Token: 0x060007C9 RID: 1993 RVA: 0x00032B68 File Offset: 0x00030D68
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

		// Token: 0x040004B3 RID: 1203
		public ProcessDirectoryHandler ProcessDirectory;

		// Token: 0x040004B4 RID: 1204
		public ProcessFileHandler ProcessFile;

		// Token: 0x040004B5 RID: 1205
		public CompletedFileHandler CompletedFile;

		// Token: 0x040004B6 RID: 1206
		public DirectoryFailureHandler DirectoryFailure;

		// Token: 0x040004B7 RID: 1207
		public FileFailureHandler FileFailure;

		// Token: 0x040004B8 RID: 1208
		private IScanFilter fileFilter_;

		// Token: 0x040004B9 RID: 1209
		private IScanFilter directoryFilter_;

		// Token: 0x040004BA RID: 1210
		private bool alive_;
	}
}
