using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using UWA.ICSharpCode.SharpZipLib.Core;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x02000058 RID: 88
	[ComVisible(false)]
	public class FastZip
	{
		// Token: 0x060003D9 RID: 985 RVA: 0x0001B2CC File Offset: 0x000194CC
		public FastZip()
		{
		}

		// Token: 0x060003DA RID: 986 RVA: 0x0001B2E8 File Offset: 0x000194E8
		public FastZip(FastZipEvents events)
		{
			this.events_ = events;
		}

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x060003DB RID: 987 RVA: 0x0001B30C File Offset: 0x0001950C
		// (set) Token: 0x060003DC RID: 988 RVA: 0x0001B32C File Offset: 0x0001952C
		public bool CreateEmptyDirectories
		{
			get
			{
				return this.createEmptyDirectories_;
			}
			set
			{
				this.createEmptyDirectories_ = value;
			}
		}

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x060003DD RID: 989 RVA: 0x0001B338 File Offset: 0x00019538
		// (set) Token: 0x060003DE RID: 990 RVA: 0x0001B358 File Offset: 0x00019558
		public string Password
		{
			get
			{
				return this.password_;
			}
			set
			{
				this.password_ = value;
			}
		}

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x060003DF RID: 991 RVA: 0x0001B364 File Offset: 0x00019564
		// (set) Token: 0x060003E0 RID: 992 RVA: 0x0001B388 File Offset: 0x00019588
		public UWA.ICSharpCode.SharpZipLib.Core.INameTransform NameTransform
		{
			get
			{
				return this.entryFactory_.NameTransform;
			}
			set
			{
				this.entryFactory_.NameTransform = value;
			}
		}

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x060003E1 RID: 993 RVA: 0x0001B398 File Offset: 0x00019598
		// (set) Token: 0x060003E2 RID: 994 RVA: 0x0001B3B8 File Offset: 0x000195B8
		public IEntryFactory EntryFactory
		{
			get
			{
				return this.entryFactory_;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					this.entryFactory_ = new ZipEntryFactory();
				}
				else
				{
					this.entryFactory_ = value;
				}
			}
		}

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x060003E3 RID: 995 RVA: 0x0001B3F0 File Offset: 0x000195F0
		// (set) Token: 0x060003E4 RID: 996 RVA: 0x0001B410 File Offset: 0x00019610
		public UseZip64 UseZip64
		{
			get
			{
				return this.useZip64_;
			}
			set
			{
				this.useZip64_ = value;
			}
		}

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x060003E5 RID: 997 RVA: 0x0001B41C File Offset: 0x0001961C
		// (set) Token: 0x060003E6 RID: 998 RVA: 0x0001B43C File Offset: 0x0001963C
		public bool RestoreDateTimeOnExtract
		{
			get
			{
				return this.restoreDateTimeOnExtract_;
			}
			set
			{
				this.restoreDateTimeOnExtract_ = value;
			}
		}

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x060003E7 RID: 999 RVA: 0x0001B448 File Offset: 0x00019648
		// (set) Token: 0x060003E8 RID: 1000 RVA: 0x0001B468 File Offset: 0x00019668
		public bool RestoreAttributesOnExtract
		{
			get
			{
				return this.restoreAttributesOnExtract_;
			}
			set
			{
				this.restoreAttributesOnExtract_ = value;
			}
		}

		// Token: 0x060003E9 RID: 1001 RVA: 0x0001B474 File Offset: 0x00019674
		public void CreateZip(string zipFileName, string sourceDirectory, bool recurse, string fileFilter, string directoryFilter)
		{
			this.CreateZip(File.Create(zipFileName), sourceDirectory, recurse, fileFilter, directoryFilter);
		}

		// Token: 0x060003EA RID: 1002 RVA: 0x0001B48C File Offset: 0x0001968C
		public void CreateZip(string zipFileName, string sourceDirectory, bool recurse, string fileFilter)
		{
			this.CreateZip(File.Create(zipFileName), sourceDirectory, recurse, fileFilter, null);
		}

		// Token: 0x060003EB RID: 1003 RVA: 0x0001B4A4 File Offset: 0x000196A4
		public void CreateZip(Stream outputStream, string sourceDirectory, bool recurse, string fileFilter, string directoryFilter)
		{
			this.NameTransform = new ZipNameTransform(sourceDirectory);
			this.sourceDirectory_ = sourceDirectory;
			using (this.outputStream_ = new ZipOutputStream(outputStream))
			{
				bool flag = this.password_ != null;
				if (flag)
				{
					this.outputStream_.Password = this.password_;
				}
				this.outputStream_.UseZip64 = this.UseZip64;
				UWA.ICSharpCode.SharpZipLib.Core.FileSystemScanner fileSystemScanner = new UWA.ICSharpCode.SharpZipLib.Core.FileSystemScanner(fileFilter, directoryFilter);
				UWA.ICSharpCode.SharpZipLib.Core.FileSystemScanner fileSystemScanner2 = fileSystemScanner;
				fileSystemScanner2.ProcessFile = (UWA.ICSharpCode.SharpZipLib.Core.ProcessFileHandler)Delegate.Combine(fileSystemScanner2.ProcessFile, new UWA.ICSharpCode.SharpZipLib.Core.ProcessFileHandler(this.ProcessFile));
				bool createEmptyDirectories = this.CreateEmptyDirectories;
				if (createEmptyDirectories)
				{
					UWA.ICSharpCode.SharpZipLib.Core.FileSystemScanner fileSystemScanner3 = fileSystemScanner;
					fileSystemScanner3.ProcessDirectory = (UWA.ICSharpCode.SharpZipLib.Core.ProcessDirectoryHandler)Delegate.Combine(fileSystemScanner3.ProcessDirectory, new UWA.ICSharpCode.SharpZipLib.Core.ProcessDirectoryHandler(this.ProcessDirectory));
				}
				bool flag2 = this.events_ != null;
				if (flag2)
				{
					bool flag3 = this.events_.FileFailure != null;
					if (flag3)
					{
						UWA.ICSharpCode.SharpZipLib.Core.FileSystemScanner fileSystemScanner4 = fileSystemScanner;
						fileSystemScanner4.FileFailure = (UWA.ICSharpCode.SharpZipLib.Core.FileFailureHandler)Delegate.Combine(fileSystemScanner4.FileFailure, this.events_.FileFailure);
					}
					bool flag4 = this.events_.DirectoryFailure != null;
					if (flag4)
					{
						UWA.ICSharpCode.SharpZipLib.Core.FileSystemScanner fileSystemScanner5 = fileSystemScanner;
						fileSystemScanner5.DirectoryFailure = (UWA.ICSharpCode.SharpZipLib.Core.DirectoryFailureHandler)Delegate.Combine(fileSystemScanner5.DirectoryFailure, this.events_.DirectoryFailure);
					}
				}
				fileSystemScanner.Scan(sourceDirectory, recurse);
			}
		}

		// Token: 0x060003EC RID: 1004 RVA: 0x0001B630 File Offset: 0x00019830
		public void ExtractZip(string zipFileName, string targetDirectory, string fileFilter)
		{
			this.ExtractZip(zipFileName, targetDirectory, FastZip.Overwrite.Always, null, fileFilter, null, this.restoreDateTimeOnExtract_);
		}

		// Token: 0x060003ED RID: 1005 RVA: 0x0001B658 File Offset: 0x00019858
		public void ExtractZip(string zipFileName, string targetDirectory, FastZip.Overwrite overwrite, FastZip.ConfirmOverwriteDelegate confirmDelegate, string fileFilter, string directoryFilter, bool restoreDateTime)
		{
			Stream inputStream = File.Open(zipFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
			this.ExtractZip(inputStream, targetDirectory, overwrite, confirmDelegate, fileFilter, directoryFilter, restoreDateTime, true);
		}

		// Token: 0x060003EE RID: 1006 RVA: 0x0001B688 File Offset: 0x00019888
		public void ExtractZip(Stream inputStream, string targetDirectory, FastZip.Overwrite overwrite, FastZip.ConfirmOverwriteDelegate confirmDelegate, string fileFilter, string directoryFilter, bool restoreDateTime, bool isStreamOwner)
		{
			bool flag = overwrite == FastZip.Overwrite.Prompt && confirmDelegate == null;
			if (flag)
			{
				throw new ArgumentNullException("confirmDelegate");
			}
			this.continueRunning_ = true;
			this.overwrite_ = overwrite;
			this.confirmDelegate_ = confirmDelegate;
			this.extractNameTransform_ = new WindowsNameTransform(targetDirectory);
			this.fileFilter_ = new UWA.ICSharpCode.SharpZipLib.Core.NameFilter(fileFilter);
			this.directoryFilter_ = new UWA.ICSharpCode.SharpZipLib.Core.NameFilter(directoryFilter);
			this.restoreDateTimeOnExtract_ = restoreDateTime;
			using (this.zipFile_ = new ZipFile(inputStream))
			{
				bool flag2 = this.password_ != null;
				if (flag2)
				{
					this.zipFile_.Password = this.password_;
				}
				this.zipFile_.IsStreamOwner = isStreamOwner;
				IEnumerator enumerator = this.zipFile_.GetEnumerator();
				while (this.continueRunning_ && enumerator.MoveNext())
				{
					ZipEntry zipEntry = (ZipEntry)enumerator.Current;
					bool isFile = zipEntry.IsFile;
					if (isFile)
					{
						bool flag3 = this.directoryFilter_.IsMatch(Path.GetDirectoryName(zipEntry.Name)) && this.fileFilter_.IsMatch(zipEntry.Name);
						if (flag3)
						{
							this.ExtractEntry(zipEntry);
						}
					}
					else
					{
						bool isDirectory = zipEntry.IsDirectory;
						if (isDirectory)
						{
							bool flag4 = this.directoryFilter_.IsMatch(zipEntry.Name) && this.CreateEmptyDirectories;
							if (flag4)
							{
								this.ExtractEntry(zipEntry);
							}
						}
					}
				}
			}
		}

		// Token: 0x060003EF RID: 1007 RVA: 0x0001B864 File Offset: 0x00019A64
		private void ProcessDirectory(object sender, UWA.ICSharpCode.SharpZipLib.Core.DirectoryEventArgs e)
		{
			bool flag = !e.HasMatchingFiles && this.CreateEmptyDirectories;
			if (flag)
			{
				bool flag2 = this.events_ != null;
				if (flag2)
				{
					this.events_.OnProcessDirectory(e.Name, e.HasMatchingFiles);
				}
				bool continueRunning = e.ContinueRunning;
				if (continueRunning)
				{
					bool flag3 = e.Name != this.sourceDirectory_;
					if (flag3)
					{
						ZipEntry entry = this.entryFactory_.MakeDirectoryEntry(e.Name);
						this.outputStream_.PutNextEntry(entry);
					}
				}
			}
		}

		// Token: 0x060003F0 RID: 1008 RVA: 0x0001B90C File Offset: 0x00019B0C
		private void ProcessFile(object sender, UWA.ICSharpCode.SharpZipLib.Core.ScanEventArgs e)
		{
			bool flag = this.events_ != null && this.events_.ProcessFile != null;
			if (flag)
			{
				this.events_.ProcessFile(sender, e);
			}
			bool continueRunning = e.ContinueRunning;
			if (continueRunning)
			{
				try
				{
					using (FileStream fileStream = File.Open(e.Name, FileMode.Open, FileAccess.Read, FileShare.Read))
					{
						ZipEntry entry = this.entryFactory_.MakeFileEntry(e.Name);
						this.outputStream_.PutNextEntry(entry);
						this.AddFileContents(e.Name, fileStream);
					}
				}
				catch (Exception e2)
				{
					bool flag2 = this.events_ != null;
					if (!flag2)
					{
						this.continueRunning_ = false;
						throw;
					}
					this.continueRunning_ = this.events_.OnFileFailure(e.Name, e2);
				}
			}
		}

		// Token: 0x060003F1 RID: 1009 RVA: 0x0001BA20 File Offset: 0x00019C20
		private void AddFileContents(string name, Stream stream)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			bool flag2 = this.buffer_ == null;
			if (flag2)
			{
				this.buffer_ = new byte[4096];
			}
			bool flag3 = this.events_ != null && this.events_.Progress != null;
			if (flag3)
			{
				UWA.ICSharpCode.SharpZipLib.Core.StreamUtils.Copy(stream, this.outputStream_, this.buffer_, this.events_.Progress, this.events_.ProgressInterval, this, name);
			}
			else
			{
				UWA.ICSharpCode.SharpZipLib.Core.StreamUtils.Copy(stream, this.outputStream_, this.buffer_);
			}
			bool flag4 = this.events_ != null;
			if (flag4)
			{
				this.continueRunning_ = this.events_.OnCompletedFile(name);
			}
		}

		// Token: 0x060003F2 RID: 1010 RVA: 0x0001BAFC File Offset: 0x00019CFC
		private void ExtractFileEntry(ZipEntry entry, string targetName)
		{
			bool flag = true;
			bool flag2 = this.overwrite_ != FastZip.Overwrite.Always;
			if (flag2)
			{
				bool flag3 = File.Exists(targetName);
				if (flag3)
				{
					bool flag4 = this.overwrite_ == FastZip.Overwrite.Prompt && this.confirmDelegate_ != null;
					flag = (flag4 && this.confirmDelegate_(targetName));
				}
			}
			bool flag5 = flag;
			if (flag5)
			{
				bool flag6 = this.events_ != null;
				if (flag6)
				{
					this.continueRunning_ = this.events_.OnProcessFile(entry.Name);
				}
				bool flag7 = this.continueRunning_;
				if (flag7)
				{
					try
					{
						using (FileStream fileStream = File.Create(targetName))
						{
							bool flag8 = this.buffer_ == null;
							if (flag8)
							{
								this.buffer_ = new byte[4096];
							}
							bool flag9 = this.events_ != null && this.events_.Progress != null;
							if (flag9)
							{
								UWA.ICSharpCode.SharpZipLib.Core.StreamUtils.Copy(this.zipFile_.GetInputStream(entry), fileStream, this.buffer_, this.events_.Progress, this.events_.ProgressInterval, this, entry.Name, entry.Size);
							}
							else
							{
								UWA.ICSharpCode.SharpZipLib.Core.StreamUtils.Copy(this.zipFile_.GetInputStream(entry), fileStream, this.buffer_);
							}
							bool flag10 = this.events_ != null;
							if (flag10)
							{
								this.continueRunning_ = this.events_.OnCompletedFile(entry.Name);
							}
						}
						bool flag11 = this.restoreDateTimeOnExtract_;
						if (flag11)
						{
							File.SetLastWriteTime(targetName, entry.DateTime);
						}
						bool flag12 = this.RestoreAttributesOnExtract && entry.IsDOSEntry && entry.ExternalFileAttributes != -1;
						if (flag12)
						{
							FileAttributes fileAttributes = (FileAttributes)entry.ExternalFileAttributes;
							fileAttributes &= (FileAttributes.ReadOnly | FileAttributes.Hidden | FileAttributes.Archive | FileAttributes.Normal);
							File.SetAttributes(targetName, fileAttributes);
						}
					}
					catch (Exception e)
					{
						bool flag13 = this.events_ != null;
						if (!flag13)
						{
							this.continueRunning_ = false;
							throw;
						}
						this.continueRunning_ = this.events_.OnFileFailure(targetName, e);
					}
				}
			}
		}

		// Token: 0x060003F3 RID: 1011 RVA: 0x0001BD90 File Offset: 0x00019F90
		private void ExtractEntry(ZipEntry entry)
		{
			bool flag = entry.IsCompressionMethodSupported();
			string text = entry.Name;
			bool flag2 = flag;
			if (flag2)
			{
				bool isFile = entry.IsFile;
				if (isFile)
				{
					text = this.extractNameTransform_.TransformFile(text);
				}
				else
				{
					bool isDirectory = entry.IsDirectory;
					if (isDirectory)
					{
						text = this.extractNameTransform_.TransformDirectory(text);
					}
				}
				flag = (text != null && text.Length != 0);
			}
			string path = null;
			bool flag3 = flag;
			if (flag3)
			{
				bool isDirectory2 = entry.IsDirectory;
				if (isDirectory2)
				{
					path = text;
				}
				else
				{
					path = Path.GetDirectoryName(Path.GetFullPath(text));
				}
			}
			bool flag4 = flag && !Directory.Exists(path);
			if (flag4)
			{
				bool flag5 = !entry.IsDirectory || this.CreateEmptyDirectories;
				if (flag5)
				{
					try
					{
						Directory.CreateDirectory(path);
					}
					catch (Exception e)
					{
						flag = false;
						bool flag6 = this.events_ != null;
						if (!flag6)
						{
							this.continueRunning_ = false;
							throw;
						}
						bool isDirectory3 = entry.IsDirectory;
						if (isDirectory3)
						{
							this.continueRunning_ = this.events_.OnDirectoryFailure(text, e);
						}
						else
						{
							this.continueRunning_ = this.events_.OnFileFailure(text, e);
						}
					}
				}
			}
			bool flag7 = flag && entry.IsFile;
			if (flag7)
			{
				this.ExtractFileEntry(entry, text);
			}
		}

		// Token: 0x060003F4 RID: 1012 RVA: 0x0001BF38 File Offset: 0x0001A138
		private static int MakeExternalAttributes(FileInfo info)
		{
			return (int)info.Attributes;
		}

		// Token: 0x060003F5 RID: 1013 RVA: 0x0001BF58 File Offset: 0x0001A158
		private static bool NameIsValid(string name)
		{
			return name != null && name.Length > 0 && name.IndexOfAny(Path.GetInvalidPathChars()) < 0;
		}

		// Token: 0x0400024C RID: 588
		private bool continueRunning_;

		// Token: 0x0400024D RID: 589
		private byte[] buffer_;

		// Token: 0x0400024E RID: 590
		private ZipOutputStream outputStream_;

		// Token: 0x0400024F RID: 591
		private ZipFile zipFile_;

		// Token: 0x04000250 RID: 592
		private string sourceDirectory_;

		// Token: 0x04000251 RID: 593
		private UWA.ICSharpCode.SharpZipLib.Core.NameFilter fileFilter_;

		// Token: 0x04000252 RID: 594
		private UWA.ICSharpCode.SharpZipLib.Core.NameFilter directoryFilter_;

		// Token: 0x04000253 RID: 595
		private FastZip.Overwrite overwrite_;

		// Token: 0x04000254 RID: 596
		private FastZip.ConfirmOverwriteDelegate confirmDelegate_;

		// Token: 0x04000255 RID: 597
		private bool restoreDateTimeOnExtract_;

		// Token: 0x04000256 RID: 598
		private bool restoreAttributesOnExtract_;

		// Token: 0x04000257 RID: 599
		private bool createEmptyDirectories_;

		// Token: 0x04000258 RID: 600
		private FastZipEvents events_;

		// Token: 0x04000259 RID: 601
		private IEntryFactory entryFactory_ = new ZipEntryFactory();

		// Token: 0x0400025A RID: 602
		private UWA.ICSharpCode.SharpZipLib.Core.INameTransform extractNameTransform_;

		// Token: 0x0400025B RID: 603
		private UseZip64 useZip64_ = UseZip64.Dynamic;

		// Token: 0x0400025C RID: 604
		private string password_;

		// Token: 0x02000112 RID: 274
		public enum Overwrite
		{
			// Token: 0x040006D8 RID: 1752
			Prompt,
			// Token: 0x040006D9 RID: 1753
			Never,
			// Token: 0x040006DA RID: 1754
			Always
		}

		// Token: 0x02000113 RID: 275
		// (Invoke) Token: 0x06000961 RID: 2401
		public delegate bool ConfirmOverwriteDelegate(string fileName);
	}
}
