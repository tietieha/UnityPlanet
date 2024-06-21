using System;
using System.Runtime.InteropServices;
using UWA.ICSharpCode.SharpZipLib.Core;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x02000057 RID: 87
	[ComVisible(false)]
	public class FastZipEvents
	{
		// Token: 0x060003D1 RID: 977 RVA: 0x0001B104 File Offset: 0x00019304
		public bool OnDirectoryFailure(string directory, Exception e)
		{
			bool result = false;
			UWA.ICSharpCode.SharpZipLib.Core.DirectoryFailureHandler directoryFailure = this.DirectoryFailure;
			bool flag = directoryFailure != null;
			if (flag)
			{
				UWA.ICSharpCode.SharpZipLib.Core.ScanFailureEventArgs scanFailureEventArgs = new UWA.ICSharpCode.SharpZipLib.Core.ScanFailureEventArgs(directory, e);
				directoryFailure(this, scanFailureEventArgs);
				result = scanFailureEventArgs.ContinueRunning;
			}
			return result;
		}

		// Token: 0x060003D2 RID: 978 RVA: 0x0001B150 File Offset: 0x00019350
		public bool OnFileFailure(string file, Exception e)
		{
			UWA.ICSharpCode.SharpZipLib.Core.FileFailureHandler fileFailure = this.FileFailure;
			bool flag = fileFailure != null;
			bool flag2 = flag;
			if (flag2)
			{
				UWA.ICSharpCode.SharpZipLib.Core.ScanFailureEventArgs scanFailureEventArgs = new UWA.ICSharpCode.SharpZipLib.Core.ScanFailureEventArgs(file, e);
				fileFailure(this, scanFailureEventArgs);
				flag = scanFailureEventArgs.ContinueRunning;
			}
			return flag;
		}

		// Token: 0x060003D3 RID: 979 RVA: 0x0001B19C File Offset: 0x0001939C
		public bool OnProcessFile(string file)
		{
			bool result = true;
			UWA.ICSharpCode.SharpZipLib.Core.ProcessFileHandler processFile = this.ProcessFile;
			bool flag = processFile != null;
			if (flag)
			{
				UWA.ICSharpCode.SharpZipLib.Core.ScanEventArgs scanEventArgs = new UWA.ICSharpCode.SharpZipLib.Core.ScanEventArgs(file);
				processFile(this, scanEventArgs);
				result = scanEventArgs.ContinueRunning;
			}
			return result;
		}

		// Token: 0x060003D4 RID: 980 RVA: 0x0001B1E8 File Offset: 0x000193E8
		public bool OnCompletedFile(string file)
		{
			bool result = true;
			UWA.ICSharpCode.SharpZipLib.Core.CompletedFileHandler completedFile = this.CompletedFile;
			bool flag = completedFile != null;
			if (flag)
			{
				UWA.ICSharpCode.SharpZipLib.Core.ScanEventArgs scanEventArgs = new UWA.ICSharpCode.SharpZipLib.Core.ScanEventArgs(file);
				completedFile(this, scanEventArgs);
				result = scanEventArgs.ContinueRunning;
			}
			return result;
		}

		// Token: 0x060003D5 RID: 981 RVA: 0x0001B234 File Offset: 0x00019434
		public bool OnProcessDirectory(string directory, bool hasMatchingFiles)
		{
			bool result = true;
			UWA.ICSharpCode.SharpZipLib.Core.ProcessDirectoryHandler processDirectory = this.ProcessDirectory;
			bool flag = processDirectory != null;
			if (flag)
			{
				UWA.ICSharpCode.SharpZipLib.Core.DirectoryEventArgs directoryEventArgs = new UWA.ICSharpCode.SharpZipLib.Core.DirectoryEventArgs(directory, hasMatchingFiles);
				processDirectory(this, directoryEventArgs);
				result = directoryEventArgs.ContinueRunning;
			}
			return result;
		}

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x060003D6 RID: 982 RVA: 0x0001B280 File Offset: 0x00019480
		// (set) Token: 0x060003D7 RID: 983 RVA: 0x0001B2A0 File Offset: 0x000194A0
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

		// Token: 0x04000245 RID: 581
		public UWA.ICSharpCode.SharpZipLib.Core.ProcessDirectoryHandler ProcessDirectory;

		// Token: 0x04000246 RID: 582
		public UWA.ICSharpCode.SharpZipLib.Core.ProcessFileHandler ProcessFile;

		// Token: 0x04000247 RID: 583
		public UWA.ICSharpCode.SharpZipLib.Core.ProgressHandler Progress;

		// Token: 0x04000248 RID: 584
		public UWA.ICSharpCode.SharpZipLib.Core.CompletedFileHandler CompletedFile;

		// Token: 0x04000249 RID: 585
		public UWA.ICSharpCode.SharpZipLib.Core.DirectoryFailureHandler DirectoryFailure;

		// Token: 0x0400024A RID: 586
		public UWA.ICSharpCode.SharpZipLib.Core.FileFailureHandler FileFailure;

		// Token: 0x0400024B RID: 587
		private TimeSpan progressInterval_ = TimeSpan.FromSeconds(3.0);
	}
}
