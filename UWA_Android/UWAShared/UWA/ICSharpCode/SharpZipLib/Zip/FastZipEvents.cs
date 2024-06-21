using System;
using System.Runtime.InteropServices;
using UWA.ICSharpCode.SharpZipLib.Core;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x02000066 RID: 102
	[ComVisible(false)]
	public class FastZipEvents
	{
		// Token: 0x060004AD RID: 1197 RVA: 0x00027EBC File Offset: 0x000260BC
		public bool OnDirectoryFailure(string directory, Exception e)
		{
			bool result = false;
			DirectoryFailureHandler directoryFailure = this.DirectoryFailure;
			bool flag = directoryFailure != null;
			if (flag)
			{
				ScanFailureEventArgs scanFailureEventArgs = new ScanFailureEventArgs(directory, e);
				directoryFailure(this, scanFailureEventArgs);
				result = scanFailureEventArgs.ContinueRunning;
			}
			return result;
		}

		// Token: 0x060004AE RID: 1198 RVA: 0x00027F08 File Offset: 0x00026108
		public bool OnFileFailure(string file, Exception e)
		{
			FileFailureHandler fileFailure = this.FileFailure;
			bool flag = fileFailure != null;
			bool flag2 = flag;
			if (flag2)
			{
				ScanFailureEventArgs scanFailureEventArgs = new ScanFailureEventArgs(file, e);
				fileFailure(this, scanFailureEventArgs);
				flag = scanFailureEventArgs.ContinueRunning;
			}
			return flag;
		}

		// Token: 0x060004AF RID: 1199 RVA: 0x00027F54 File Offset: 0x00026154
		public bool OnProcessFile(string file)
		{
			bool result = true;
			ProcessFileHandler processFile = this.ProcessFile;
			bool flag = processFile != null;
			if (flag)
			{
				ScanEventArgs scanEventArgs = new ScanEventArgs(file);
				processFile(this, scanEventArgs);
				result = scanEventArgs.ContinueRunning;
			}
			return result;
		}

		// Token: 0x060004B0 RID: 1200 RVA: 0x00027FA0 File Offset: 0x000261A0
		public bool OnCompletedFile(string file)
		{
			bool result = true;
			CompletedFileHandler completedFile = this.CompletedFile;
			bool flag = completedFile != null;
			if (flag)
			{
				ScanEventArgs scanEventArgs = new ScanEventArgs(file);
				completedFile(this, scanEventArgs);
				result = scanEventArgs.ContinueRunning;
			}
			return result;
		}

		// Token: 0x060004B1 RID: 1201 RVA: 0x00027FEC File Offset: 0x000261EC
		public bool OnProcessDirectory(string directory, bool hasMatchingFiles)
		{
			bool result = true;
			ProcessDirectoryHandler processDirectory = this.ProcessDirectory;
			bool flag = processDirectory != null;
			if (flag)
			{
				DirectoryEventArgs directoryEventArgs = new DirectoryEventArgs(directory, hasMatchingFiles);
				processDirectory(this, directoryEventArgs);
				result = directoryEventArgs.ContinueRunning;
			}
			return result;
		}

		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x060004B2 RID: 1202 RVA: 0x00028038 File Offset: 0x00026238
		// (set) Token: 0x060004B3 RID: 1203 RVA: 0x00028058 File Offset: 0x00026258
		public TimeSpan ProgressInterval
		{
			get
			{
				return this.progressInterval_;
			}
			set
			{
				this.progressInterval_ = value;
			}
		}

		// Token: 0x040002B8 RID: 696
		public ProcessDirectoryHandler ProcessDirectory;

		// Token: 0x040002B9 RID: 697
		public ProcessFileHandler ProcessFile;

		// Token: 0x040002BA RID: 698
		public ProgressHandler Progress;

		// Token: 0x040002BB RID: 699
		public CompletedFileHandler CompletedFile;

		// Token: 0x040002BC RID: 700
		public DirectoryFailureHandler DirectoryFailure;

		// Token: 0x040002BD RID: 701
		public FileFailureHandler FileFailure;

		// Token: 0x040002BE RID: 702
		private TimeSpan progressInterval_ = TimeSpan.FromSeconds(3.0);
	}
}
