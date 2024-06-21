using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace UWA.ICSharpCode.SharpZipLib.Tar
{
	// Token: 0x02000090 RID: 144
	[ComVisible(false)]
	public class TarArchive : IDisposable
	{
		// Token: 0x14000001 RID: 1
		// (add) Token: 0x06000668 RID: 1640 RVA: 0x0002C590 File Offset: 0x0002A790
		// (remove) Token: 0x06000669 RID: 1641 RVA: 0x0002C5CC File Offset: 0x0002A7CC
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event ProgressMessageHandler ProgressMessageEvent;

		// Token: 0x0600066A RID: 1642 RVA: 0x0002C608 File Offset: 0x0002A808
		protected virtual void OnProgressMessageEvent(TarEntry entry, string message)
		{
			ProgressMessageHandler progressMessageEvent = this.ProgressMessageEvent;
			bool flag = progressMessageEvent != null;
			if (flag)
			{
				progressMessageEvent(this, entry, message);
			}
		}

		// Token: 0x0600066B RID: 1643 RVA: 0x0002C638 File Offset: 0x0002A838
		protected TarArchive()
		{
		}

		// Token: 0x0600066C RID: 1644 RVA: 0x0002C658 File Offset: 0x0002A858
		protected TarArchive(TarInputStream stream)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			this.tarIn = stream;
		}

		// Token: 0x0600066D RID: 1645 RVA: 0x0002C6A8 File Offset: 0x0002A8A8
		protected TarArchive(TarOutputStream stream)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			this.tarOut = stream;
		}

		// Token: 0x0600066E RID: 1646 RVA: 0x0002C6F8 File Offset: 0x0002A8F8
		public static TarArchive CreateInputTarArchive(Stream inputStream)
		{
			bool flag = inputStream == null;
			if (flag)
			{
				throw new ArgumentNullException("inputStream");
			}
			TarInputStream tarInputStream = inputStream as TarInputStream;
			bool flag2 = tarInputStream != null;
			TarArchive result;
			if (flag2)
			{
				result = new TarArchive(tarInputStream);
			}
			else
			{
				result = TarArchive.CreateInputTarArchive(inputStream, 20);
			}
			return result;
		}

		// Token: 0x0600066F RID: 1647 RVA: 0x0002C758 File Offset: 0x0002A958
		public static TarArchive CreateInputTarArchive(Stream inputStream, int blockFactor)
		{
			bool flag = inputStream == null;
			if (flag)
			{
				throw new ArgumentNullException("inputStream");
			}
			bool flag2 = inputStream is TarInputStream;
			if (flag2)
			{
				throw new ArgumentException("TarInputStream not valid");
			}
			return new TarArchive(new TarInputStream(inputStream, blockFactor));
		}

		// Token: 0x06000670 RID: 1648 RVA: 0x0002C7B0 File Offset: 0x0002A9B0
		public static TarArchive CreateOutputTarArchive(Stream outputStream)
		{
			bool flag = outputStream == null;
			if (flag)
			{
				throw new ArgumentNullException("outputStream");
			}
			TarOutputStream tarOutputStream = outputStream as TarOutputStream;
			bool flag2 = tarOutputStream != null;
			TarArchive result;
			if (flag2)
			{
				result = new TarArchive(tarOutputStream);
			}
			else
			{
				result = TarArchive.CreateOutputTarArchive(outputStream, 20);
			}
			return result;
		}

		// Token: 0x06000671 RID: 1649 RVA: 0x0002C810 File Offset: 0x0002AA10
		public static TarArchive CreateOutputTarArchive(Stream outputStream, int blockFactor)
		{
			bool flag = outputStream == null;
			if (flag)
			{
				throw new ArgumentNullException("outputStream");
			}
			bool flag2 = outputStream is TarOutputStream;
			if (flag2)
			{
				throw new ArgumentException("TarOutputStream is not valid");
			}
			return new TarArchive(new TarOutputStream(outputStream, blockFactor));
		}

		// Token: 0x06000672 RID: 1650 RVA: 0x0002C868 File Offset: 0x0002AA68
		public void SetKeepOldFiles(bool keepExistingFiles)
		{
			bool flag = this.isDisposed;
			if (flag)
			{
				throw new ObjectDisposedException("TarArchive");
			}
			this.keepOldFiles = keepExistingFiles;
		}

		// Token: 0x170000F8 RID: 248
		// (get) Token: 0x06000673 RID: 1651 RVA: 0x0002C89C File Offset: 0x0002AA9C
		// (set) Token: 0x06000674 RID: 1652 RVA: 0x0002C8D4 File Offset: 0x0002AAD4
		public bool AsciiTranslate
		{
			get
			{
				bool flag = this.isDisposed;
				if (flag)
				{
					throw new ObjectDisposedException("TarArchive");
				}
				return this.asciiTranslate;
			}
			set
			{
				bool flag = this.isDisposed;
				if (flag)
				{
					throw new ObjectDisposedException("TarArchive");
				}
				this.asciiTranslate = value;
			}
		}

		// Token: 0x06000675 RID: 1653 RVA: 0x0002C908 File Offset: 0x0002AB08
		[Obsolete("Use the AsciiTranslate property")]
		public void SetAsciiTranslation(bool translateAsciiFiles)
		{
			bool flag = this.isDisposed;
			if (flag)
			{
				throw new ObjectDisposedException("TarArchive");
			}
			this.asciiTranslate = translateAsciiFiles;
		}

		// Token: 0x170000F9 RID: 249
		// (get) Token: 0x06000676 RID: 1654 RVA: 0x0002C93C File Offset: 0x0002AB3C
		// (set) Token: 0x06000677 RID: 1655 RVA: 0x0002C974 File Offset: 0x0002AB74
		public string PathPrefix
		{
			get
			{
				bool flag = this.isDisposed;
				if (flag)
				{
					throw new ObjectDisposedException("TarArchive");
				}
				return this.pathPrefix;
			}
			set
			{
				bool flag = this.isDisposed;
				if (flag)
				{
					throw new ObjectDisposedException("TarArchive");
				}
				this.pathPrefix = value;
			}
		}

		// Token: 0x170000FA RID: 250
		// (get) Token: 0x06000678 RID: 1656 RVA: 0x0002C9A8 File Offset: 0x0002ABA8
		// (set) Token: 0x06000679 RID: 1657 RVA: 0x0002C9E0 File Offset: 0x0002ABE0
		public string RootPath
		{
			get
			{
				bool flag = this.isDisposed;
				if (flag)
				{
					throw new ObjectDisposedException("TarArchive");
				}
				return this.rootPath;
			}
			set
			{
				bool flag = this.isDisposed;
				if (flag)
				{
					throw new ObjectDisposedException("TarArchive");
				}
				this.rootPath = value;
			}
		}

		// Token: 0x0600067A RID: 1658 RVA: 0x0002CA14 File Offset: 0x0002AC14
		public void SetUserInfo(int userId, string userName, int groupId, string groupName)
		{
			bool flag = this.isDisposed;
			if (flag)
			{
				throw new ObjectDisposedException("TarArchive");
			}
			this.userId = userId;
			this.userName = userName;
			this.groupId = groupId;
			this.groupName = groupName;
			this.applyUserInfoOverrides = true;
		}

		// Token: 0x170000FB RID: 251
		// (get) Token: 0x0600067B RID: 1659 RVA: 0x0002CA64 File Offset: 0x0002AC64
		// (set) Token: 0x0600067C RID: 1660 RVA: 0x0002CA9C File Offset: 0x0002AC9C
		public bool ApplyUserInfoOverrides
		{
			get
			{
				bool flag = this.isDisposed;
				if (flag)
				{
					throw new ObjectDisposedException("TarArchive");
				}
				return this.applyUserInfoOverrides;
			}
			set
			{
				bool flag = this.isDisposed;
				if (flag)
				{
					throw new ObjectDisposedException("TarArchive");
				}
				this.applyUserInfoOverrides = value;
			}
		}

		// Token: 0x170000FC RID: 252
		// (get) Token: 0x0600067D RID: 1661 RVA: 0x0002CAD0 File Offset: 0x0002ACD0
		public int UserId
		{
			get
			{
				bool flag = this.isDisposed;
				if (flag)
				{
					throw new ObjectDisposedException("TarArchive");
				}
				return this.userId;
			}
		}

		// Token: 0x170000FD RID: 253
		// (get) Token: 0x0600067E RID: 1662 RVA: 0x0002CB08 File Offset: 0x0002AD08
		public string UserName
		{
			get
			{
				bool flag = this.isDisposed;
				if (flag)
				{
					throw new ObjectDisposedException("TarArchive");
				}
				return this.userName;
			}
		}

		// Token: 0x170000FE RID: 254
		// (get) Token: 0x0600067F RID: 1663 RVA: 0x0002CB40 File Offset: 0x0002AD40
		public int GroupId
		{
			get
			{
				bool flag = this.isDisposed;
				if (flag)
				{
					throw new ObjectDisposedException("TarArchive");
				}
				return this.groupId;
			}
		}

		// Token: 0x170000FF RID: 255
		// (get) Token: 0x06000680 RID: 1664 RVA: 0x0002CB78 File Offset: 0x0002AD78
		public string GroupName
		{
			get
			{
				bool flag = this.isDisposed;
				if (flag)
				{
					throw new ObjectDisposedException("TarArchive");
				}
				return this.groupName;
			}
		}

		// Token: 0x17000100 RID: 256
		// (get) Token: 0x06000681 RID: 1665 RVA: 0x0002CBB0 File Offset: 0x0002ADB0
		public int RecordSize
		{
			get
			{
				bool flag = this.isDisposed;
				if (flag)
				{
					throw new ObjectDisposedException("TarArchive");
				}
				bool flag2 = this.tarIn != null;
				int result;
				if (flag2)
				{
					result = this.tarIn.RecordSize;
				}
				else
				{
					bool flag3 = this.tarOut != null;
					if (flag3)
					{
						result = this.tarOut.RecordSize;
					}
					else
					{
						result = 10240;
					}
				}
				return result;
			}
		}

		// Token: 0x17000101 RID: 257
		// (set) Token: 0x06000682 RID: 1666 RVA: 0x0002CC2C File Offset: 0x0002AE2C
		public bool IsStreamOwner
		{
			set
			{
				bool flag = this.tarIn != null;
				if (flag)
				{
					this.tarIn.IsStreamOwner = value;
				}
				else
				{
					this.tarOut.IsStreamOwner = value;
				}
			}
		}

		// Token: 0x06000683 RID: 1667 RVA: 0x0002CC74 File Offset: 0x0002AE74
		[Obsolete("Use Close instead")]
		public void CloseArchive()
		{
			this.Close();
		}

		// Token: 0x06000684 RID: 1668 RVA: 0x0002CC80 File Offset: 0x0002AE80
		public void ListContents()
		{
			bool flag = this.isDisposed;
			if (flag)
			{
				throw new ObjectDisposedException("TarArchive");
			}
			for (;;)
			{
				TarEntry nextEntry = this.tarIn.GetNextEntry();
				bool flag2 = nextEntry == null;
				if (flag2)
				{
					break;
				}
				this.OnProgressMessageEvent(nextEntry, null);
			}
		}

		// Token: 0x06000685 RID: 1669 RVA: 0x0002CCDC File Offset: 0x0002AEDC
		public void ExtractContents(string destinationDirectory)
		{
			bool flag = this.isDisposed;
			if (flag)
			{
				throw new ObjectDisposedException("TarArchive");
			}
			for (;;)
			{
				TarEntry nextEntry = this.tarIn.GetNextEntry();
				bool flag2 = nextEntry == null;
				if (flag2)
				{
					break;
				}
				this.ExtractEntry(destinationDirectory, nextEntry);
			}
		}

		// Token: 0x06000686 RID: 1670 RVA: 0x0002CD38 File Offset: 0x0002AF38
		private void ExtractEntry(string destDir, TarEntry entry)
		{
			this.OnProgressMessageEvent(entry, null);
			string text = entry.Name;
			bool flag = Path.IsPathRooted(text);
			if (flag)
			{
				text = text.Substring(Path.GetPathRoot(text).Length);
			}
			text = text.Replace('/', Path.DirectorySeparatorChar);
			string text2 = Path.Combine(destDir, text);
			bool isDirectory = entry.IsDirectory;
			if (isDirectory)
			{
				TarArchive.EnsureDirectoryExists(text2);
			}
			else
			{
				string directoryName = Path.GetDirectoryName(text2);
				TarArchive.EnsureDirectoryExists(directoryName);
				bool flag2 = true;
				FileInfo fileInfo = new FileInfo(text2);
				bool exists = fileInfo.Exists;
				if (exists)
				{
					bool flag3 = this.keepOldFiles;
					if (flag3)
					{
						this.OnProgressMessageEvent(entry, "Destination file already exists");
						flag2 = false;
					}
					else
					{
						bool flag4 = (fileInfo.Attributes & FileAttributes.ReadOnly) > (FileAttributes)0;
						if (flag4)
						{
							this.OnProgressMessageEvent(entry, "Destination file already exists, and is read-only");
							flag2 = false;
						}
					}
				}
				bool flag5 = flag2;
				if (flag5)
				{
					bool flag6 = false;
					Stream stream = File.Create(text2);
					bool flag7 = this.asciiTranslate;
					if (flag7)
					{
						flag6 = !TarArchive.IsBinary(text2);
					}
					StreamWriter streamWriter = null;
					bool flag8 = flag6;
					if (flag8)
					{
						streamWriter = new StreamWriter(stream);
					}
					byte[] array = new byte[32768];
					for (;;)
					{
						int num = this.tarIn.Read(array, 0, array.Length);
						bool flag9 = num <= 0;
						if (flag9)
						{
							break;
						}
						bool flag10 = flag6;
						if (flag10)
						{
							int num2 = 0;
							for (int i = 0; i < num; i++)
							{
								bool flag11 = array[i] == 10;
								if (flag11)
								{
									string @string = Encoding.ASCII.GetString(array, num2, i - num2);
									streamWriter.WriteLine(@string);
									num2 = i + 1;
								}
							}
						}
						else
						{
							stream.Write(array, 0, num);
						}
					}
					bool flag12 = flag6;
					if (flag12)
					{
						streamWriter.Close();
					}
					else
					{
						stream.Close();
					}
				}
			}
		}

		// Token: 0x06000687 RID: 1671 RVA: 0x0002CF58 File Offset: 0x0002B158
		public void WriteEntry(TarEntry sourceEntry, bool recurse)
		{
			bool flag = sourceEntry == null;
			if (flag)
			{
				throw new ArgumentNullException("sourceEntry");
			}
			bool flag2 = this.isDisposed;
			if (flag2)
			{
				throw new ObjectDisposedException("TarArchive");
			}
			try
			{
				if (recurse)
				{
					TarHeader.SetValueDefaults(sourceEntry.UserId, sourceEntry.UserName, sourceEntry.GroupId, sourceEntry.GroupName);
				}
				this.WriteEntryCore(sourceEntry, recurse);
			}
			finally
			{
				if (recurse)
				{
					TarHeader.RestoreSetValues();
				}
			}
		}

		// Token: 0x06000688 RID: 1672 RVA: 0x0002CFF4 File Offset: 0x0002B1F4
		private void WriteEntryCore(TarEntry sourceEntry, bool recurse)
		{
			string text = null;
			string text2 = sourceEntry.File;
			TarEntry tarEntry = (TarEntry)sourceEntry.Clone();
			bool flag = this.applyUserInfoOverrides;
			if (flag)
			{
				tarEntry.GroupId = this.groupId;
				tarEntry.GroupName = this.groupName;
				tarEntry.UserId = this.userId;
				tarEntry.UserName = this.userName;
			}
			this.OnProgressMessageEvent(tarEntry, null);
			bool flag2 = this.asciiTranslate && !tarEntry.IsDirectory;
			if (flag2)
			{
				bool flag3 = !TarArchive.IsBinary(text2);
				if (flag3)
				{
					text = Path.GetTempFileName();
					using (StreamReader streamReader = File.OpenText(text2))
					{
						using (Stream stream = File.Create(text))
						{
							for (;;)
							{
								string text3 = streamReader.ReadLine();
								bool flag4 = text3 == null;
								if (flag4)
								{
									break;
								}
								byte[] bytes = Encoding.ASCII.GetBytes(text3);
								stream.Write(bytes, 0, bytes.Length);
								stream.WriteByte(10);
							}
							stream.Flush();
						}
					}
					tarEntry.Size = new FileInfo(text).Length;
					text2 = text;
				}
			}
			string text4 = null;
			bool flag5 = this.rootPath != null;
			if (flag5)
			{
				bool flag6 = tarEntry.Name.StartsWith(this.rootPath);
				if (flag6)
				{
					text4 = tarEntry.Name.Substring(this.rootPath.Length + 1);
				}
			}
			bool flag7 = this.pathPrefix != null;
			if (flag7)
			{
				text4 = ((text4 == null) ? (this.pathPrefix + "/" + tarEntry.Name) : (this.pathPrefix + "/" + text4));
			}
			bool flag8 = text4 != null;
			if (flag8)
			{
				tarEntry.Name = text4;
			}
			this.tarOut.PutNextEntry(tarEntry);
			bool isDirectory = tarEntry.IsDirectory;
			if (isDirectory)
			{
				if (recurse)
				{
					TarEntry[] directoryEntries = tarEntry.GetDirectoryEntries();
					for (int i = 0; i < directoryEntries.Length; i++)
					{
						this.WriteEntryCore(directoryEntries[i], recurse);
					}
				}
			}
			else
			{
				using (Stream stream2 = File.OpenRead(text2))
				{
					byte[] array = new byte[32768];
					for (;;)
					{
						int num = stream2.Read(array, 0, array.Length);
						bool flag9 = num <= 0;
						if (flag9)
						{
							break;
						}
						this.tarOut.Write(array, 0, num);
					}
				}
				bool flag10 = text != null && text.Length > 0;
				if (flag10)
				{
					File.Delete(text);
				}
				this.tarOut.CloseEntry();
			}
		}

		// Token: 0x06000689 RID: 1673 RVA: 0x0002D330 File Offset: 0x0002B530
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x0600068A RID: 1674 RVA: 0x0002D344 File Offset: 0x0002B544
		protected virtual void Dispose(bool disposing)
		{
			bool flag = !this.isDisposed;
			if (flag)
			{
				this.isDisposed = true;
				if (disposing)
				{
					bool flag2 = this.tarOut != null;
					if (flag2)
					{
						this.tarOut.Flush();
						this.tarOut.Close();
					}
					bool flag3 = this.tarIn != null;
					if (flag3)
					{
						this.tarIn.Close();
					}
				}
			}
		}

		// Token: 0x0600068B RID: 1675 RVA: 0x0002D3C4 File Offset: 0x0002B5C4
		public virtual void Close()
		{
			this.Dispose(true);
		}

		// Token: 0x0600068C RID: 1676 RVA: 0x0002D3D0 File Offset: 0x0002B5D0
		~TarArchive()
		{
			this.Dispose(false);
		}

		// Token: 0x0600068D RID: 1677 RVA: 0x0002D404 File Offset: 0x0002B604
		private static void EnsureDirectoryExists(string directoryName)
		{
			bool flag = !Directory.Exists(directoryName);
			if (flag)
			{
				try
				{
					Directory.CreateDirectory(directoryName);
				}
				catch (Exception ex)
				{
					throw new TarException("Exception creating directory '" + directoryName + "', " + ex.Message, ex);
				}
			}
		}

		// Token: 0x0600068E RID: 1678 RVA: 0x0002D464 File Offset: 0x0002B664
		private static bool IsBinary(string filename)
		{
			using (FileStream fileStream = File.OpenRead(filename))
			{
				int num = Math.Min(4096, (int)fileStream.Length);
				byte[] array = new byte[num];
				int num2 = fileStream.Read(array, 0, num);
				for (int i = 0; i < num2; i++)
				{
					byte b = array[i];
					bool flag = b < 8 || (b > 13 && b < 32) || b == byte.MaxValue;
					if (flag)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x040003FF RID: 1023
		private bool keepOldFiles;

		// Token: 0x04000400 RID: 1024
		private bool asciiTranslate;

		// Token: 0x04000401 RID: 1025
		private int userId;

		// Token: 0x04000402 RID: 1026
		private string userName = string.Empty;

		// Token: 0x04000403 RID: 1027
		private int groupId;

		// Token: 0x04000404 RID: 1028
		private string groupName = string.Empty;

		// Token: 0x04000405 RID: 1029
		private string rootPath;

		// Token: 0x04000406 RID: 1030
		private string pathPrefix;

		// Token: 0x04000407 RID: 1031
		private bool applyUserInfoOverrides;

		// Token: 0x04000408 RID: 1032
		private TarInputStream tarIn;

		// Token: 0x04000409 RID: 1033
		private TarOutputStream tarOut;

		// Token: 0x0400040A RID: 1034
		private bool isDisposed;
	}
}
