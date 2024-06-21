using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using UWA.ICSharpCode.SharpZipLib.Core;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x02000067 RID: 103
	[ComVisible(false)]
	public class FastZip
	{
		// Token: 0x060004B5 RID: 1205 RVA: 0x00028084 File Offset: 0x00026284
		public FastZip()
		{
		}

		// Token: 0x060004B6 RID: 1206 RVA: 0x000280A0 File Offset: 0x000262A0
		public FastZip(FastZipEvents events)
		{
			this.events_ = events;
		}

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x060004B7 RID: 1207 RVA: 0x000280C4 File Offset: 0x000262C4
		// (set) Token: 0x060004B8 RID: 1208 RVA: 0x000280E4 File Offset: 0x000262E4
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

		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x060004B9 RID: 1209 RVA: 0x000280F0 File Offset: 0x000262F0
		// (set) Token: 0x060004BA RID: 1210 RVA: 0x00028110 File Offset: 0x00026310
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

		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x060004BB RID: 1211 RVA: 0x0002811C File Offset: 0x0002631C
		// (set) Token: 0x060004BC RID: 1212 RVA: 0x00028140 File Offset: 0x00026340
		public INameTransform NameTransform
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

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x060004BD RID: 1213 RVA: 0x00028150 File Offset: 0x00026350
		// (set) Token: 0x060004BE RID: 1214 RVA: 0x00028170 File Offset: 0x00026370
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

		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x060004BF RID: 1215 RVA: 0x000281A8 File Offset: 0x000263A8
		// (set) Token: 0x060004C0 RID: 1216 RVA: 0x000281C8 File Offset: 0x000263C8
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

		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x060004C1 RID: 1217 RVA: 0x000281D4 File Offset: 0x000263D4
		// (set) Token: 0x060004C2 RID: 1218 RVA: 0x000281F4 File Offset: 0x000263F4
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

		// Token: 0x170000BA RID: 186
		// (get) Token: 0x060004C3 RID: 1219 RVA: 0x00028200 File Offset: 0x00026400
		// (set) Token: 0x060004C4 RID: 1220 RVA: 0x00028220 File Offset: 0x00026420
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

		// Token: 0x060004C5 RID: 1221 RVA: 0x0002822C File Offset: 0x0002642C
		public void CreateZip(string zipFileName, string sourceDirectory, bool recurse, string fileFilter, string directoryFilter)
		{
			this.CreateZip(File.Create(zipFileName), sourceDirectory, recurse, fileFilter, directoryFilter);
		}

		// Token: 0x060004C6 RID: 1222 RVA: 0x00028244 File Offset: 0x00026444
		public void CreateZip(string zipFileName, string sourceDirectory, bool recurse, string fileFilter)
		{
			this.CreateZip(File.Create(zipFileName), sourceDirectory, recurse, fileFilter, null);
		}

		// Token: 0x060004C7 RID: 1223 RVA: 0x0002825C File Offset: 0x0002645C
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
				FileSystemScanner fileSystemScanner = new FileSystemScanner(fileFilter, directoryFilter);
				FileSystemScanner fileSystemScanner2 = fileSystemScanner;
				fileSystemScanner2.ProcessFile = (ProcessFileHandler)Delegate.Combine(fileSystemScanner2.ProcessFile, new ProcessFileHandler(this.ProcessFile));
				bool createEmptyDirectories = this.CreateEmptyDirectories;
				if (createEmptyDirectories)
				{
					FileSystemScanner fileSystemScanner3 = fileSystemScanner;
					fileSystemScanner3.ProcessDirectory = (ProcessDirectoryHandler)Delegate.Combine(fileSystemScanner3.ProcessDirectory, new ProcessDirectoryHandler(this.ProcessDirectory));
				}
				bool flag2 = this.events_ != null;
				if (flag2)
				{
					bool flag3 = this.events_.FileFailure != null;
					if (flag3)
					{
						FileSystemScanner fileSystemScanner4 = fileSystemScanner;
						fileSystemScanner4.FileFailure = (FileFailureHandler)Delegate.Combine(fileSystemScanner4.FileFailure, this.events_.FileFailure);
					}
					bool flag4 = this.events_.DirectoryFailure != null;
					if (flag4)
					{
						FileSystemScanner fileSystemScanner5 = fileSystemScanner;
						fileSystemScanner5.DirectoryFailure = (DirectoryFailureHandler)Delegate.Combine(fileSystemScanner5.DirectoryFailure, this.events_.DirectoryFailure);
					}
				}
				fileSystemScanner.Scan(sourceDirectory, recurse);
			}
		}

		// Token: 0x060004C8 RID: 1224 RVA: 0x000283E8 File Offset: 0x000265E8
		public void ExtractZip(string zipFileName, string targetDirectory, string fileFilter)
		{
			this.ExtractZip(zipFileName, targetDirectory, FastZip.Overwrite.Always, null, fileFilter, null, this.restoreDateTimeOnExtract_);
		}

		// Token: 0x060004C9 RID: 1225 RVA: 0x00028410 File Offset: 0x00026610
		public void ExtractZip(string zipFileName, string targetDirectory, FastZip.Overwrite overwrite, FastZip.ConfirmOverwriteDelegate confirmDelegate, string fileFilter, string directoryFilter, bool restoreDateTime)
		{
			Stream inputStream = File.Open(zipFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
			this.ExtractZip(inputStream, targetDirectory, overwrite, confirmDelegate, fileFilter, directoryFilter, restoreDateTime, true);
		}

		// Token: 0x060004CA RID: 1226 RVA: 0x00028440 File Offset: 0x00026640
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
			this.fileFilter_ = new NameFilter(fileFilter);
			this.directoryFilter_ = new NameFilter(directoryFilter);
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

		// Token: 0x060004CB RID: 1227 RVA: 0x0002861C File Offset: 0x0002681C
		private void ProcessDirectory(object sender, DirectoryEventArgs e)
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

		// Token: 0x060004CC RID: 1228 RVA: 0x000286C4 File Offset: 0x000268C4
		private void ProcessFile(object sender, ScanEventArgs e)
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

		// Token: 0x060004CD RID: 1229 RVA: 0x000287D8 File Offset: 0x000269D8
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
				StreamUtils.Copy(stream, this.outputStream_, this.buffer_, this.events_.Progress, this.events_.ProgressInterval, this, name);
			}
			else
			{
				StreamUtils.Copy(stream, this.outputStream_, this.buffer_);
			}
			bool flag4 = this.events_ != null;
			if (flag4)
			{
				this.continueRunning_ = this.events_.OnCompletedFile(name);
			}
		}

		// Token: 0x060004CE RID: 1230 RVA: 0x000288B4 File Offset: 0x00026AB4
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
								StreamUtils.Copy(this.zipFile_.GetInputStream(entry), fileStream, this.buffer_, this.events_.Progress, this.events_.ProgressInterval, this, entry.Name, entry.Size);
							}
							else
							{
								StreamUtils.Copy(this.zipFile_.GetInputStream(entry), fileStream, this.buffer_);
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

		// Token: 0x060004CF RID: 1231 RVA: 0x00028B48 File Offset: 0x00026D48
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

		// Token: 0x060004D0 RID: 1232 RVA: 0x00028CF0 File Offset: 0x00026EF0
		private static int MakeExternalAttributes(FileInfo info)
		{
			return (int)info.Attributes;
		}

		// Token: 0x060004D1 RID: 1233 RVA: 0x00028D10 File Offset: 0x00026F10
		private static bool NameIsValid(string name)
		{
			return name != null && name.Length > 0 && name.IndexOfAny(Path.GetInvalidPathChars()) < 0;
		}

		// Token: 0x040002BF RID: 703
		private bool continueRunning_;

		// Token: 0x040002C0 RID: 704
		private byte[] buffer_;

		// Token: 0x040002C1 RID: 705
		private ZipOutputStream outputStream_;

		// Token: 0x040002C2 RID: 706
		private ZipFile zipFile_;

		// Token: 0x040002C3 RID: 707
		private string sourceDirectory_;

		// Token: 0x040002C4 RID: 708
		private NameFilter fileFilter_;

		// Token: 0x040002C5 RID: 709
		private NameFilter directoryFilter_;

		// Token: 0x040002C6 RID: 710
		private FastZip.Overwrite overwrite_;

		// Token: 0x040002C7 RID: 711
		private FastZip.ConfirmOverwriteDelegate confirmDelegate_;

		// Token: 0x040002C8 RID: 712
		private bool restoreDateTimeOnExtract_;

		// Token: 0x040002C9 RID: 713
		private bool restoreAttributesOnExtract_;

		// Token: 0x040002CA RID: 714
		private bool createEmptyDirectories_;

		// Token: 0x040002CB RID: 715
		private FastZipEvents events_;

		// Token: 0x040002CC RID: 716
		private IEntryFactory entryFactory_ = new ZipEntryFactory();

		// Token: 0x040002CD RID: 717
		private INameTransform extractNameTransform_;

		// Token: 0x040002CE RID: 718
		private UseZip64 useZip64_ = UseZip64.Dynamic;

		// Token: 0x040002CF RID: 719
		private string password_;

		// Token: 0x02000148 RID: 328
		public enum Overwrite
		{
			// Token: 0x04000795 RID: 1941
			Prompt,
			// Token: 0x04000796 RID: 1942
			Never,
			// Token: 0x04000797 RID: 1943
			Always
		}

		// Token: 0x02000149 RID: 329
		// (Invoke) Token: 0x06000AB7 RID: 2743
		public delegate bool ConfirmOverwriteDelegate(string fileName);
	}
}
