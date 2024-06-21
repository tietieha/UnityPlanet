using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace UWA.ICSharpCode.SharpZipLib.Tar
{
	// Token: 0x0200009F RID: 159
	[ComVisible(false)]
	public class TarArchive : IDisposable
	{
		// Token: 0x14000003 RID: 3
		// (add) Token: 0x06000744 RID: 1860 RVA: 0x00039348 File Offset: 0x00037548
		// (remove) Token: 0x06000745 RID: 1861 RVA: 0x00039384 File Offset: 0x00037584
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event ProgressMessageHandler ProgressMessageEvent;

		// Token: 0x06000746 RID: 1862 RVA: 0x000393C0 File Offset: 0x000375C0
		protected virtual void OnProgressMessageEvent(TarEntry entry, string message)
		{
			ProgressMessageHandler progressMessageEvent = this.ProgressMessageEvent;
			bool flag = progressMessageEvent != null;
			if (flag)
			{
				progressMessageEvent(this, entry, message);
			}
		}

		// Token: 0x06000747 RID: 1863 RVA: 0x000393F0 File Offset: 0x000375F0
		protected TarArchive()
		{
		}

		// Token: 0x06000748 RID: 1864 RVA: 0x00039410 File Offset: 0x00037610
		protected TarArchive(TarInputStream stream)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			this.tarIn = stream;
		}

		// Token: 0x06000749 RID: 1865 RVA: 0x00039460 File Offset: 0x00037660
		protected TarArchive(TarOutputStream stream)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			this.tarOut = stream;
		}

		// Token: 0x0600074A RID: 1866 RVA: 0x000394B0 File Offset: 0x000376B0
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

		// Token: 0x0600074B RID: 1867 RVA: 0x00039510 File Offset: 0x00037710
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

		// Token: 0x0600074C RID: 1868 RVA: 0x00039568 File Offset: 0x00037768
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

		// Token: 0x0600074D RID: 1869 RVA: 0x000395C8 File Offset: 0x000377C8
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

		// Token: 0x0600074E RID: 1870 RVA: 0x00039620 File Offset: 0x00037820
		public void SetKeepOldFiles(bool keepExistingFiles)
		{
			bool flag = this.isDisposed;
			if (flag)
			{
				throw new ObjectDisposedException("TarArchive");
			}
			this.keepOldFiles = keepExistingFiles;
		}

		// Token: 0x1700014C RID: 332
		// (get) Token: 0x0600074F RID: 1871 RVA: 0x00039654 File Offset: 0x00037854
		// (set) Token: 0x06000750 RID: 1872 RVA: 0x0003968C File Offset: 0x0003788C
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

		// Token: 0x06000751 RID: 1873 RVA: 0x000396C0 File Offset: 0x000378C0
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

		// Token: 0x1700014D RID: 333
		// (get) Token: 0x06000752 RID: 1874 RVA: 0x000396F4 File Offset: 0x000378F4
		// (set) Token: 0x06000753 RID: 1875 RVA: 0x0003972C File Offset: 0x0003792C
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

		// Token: 0x1700014E RID: 334
		// (get) Token: 0x06000754 RID: 1876 RVA: 0x00039760 File Offset: 0x00037960
		// (set) Token: 0x06000755 RID: 1877 RVA: 0x00039798 File Offset: 0x00037998
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

		// Token: 0x06000756 RID: 1878 RVA: 0x000397CC File Offset: 0x000379CC
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

		// Token: 0x1700014F RID: 335
		// (get) Token: 0x06000757 RID: 1879 RVA: 0x0003981C File Offset: 0x00037A1C
		// (set) Token: 0x06000758 RID: 1880 RVA: 0x00039854 File Offset: 0x00037A54
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

		// Token: 0x17000150 RID: 336
		// (get) Token: 0x06000759 RID: 1881 RVA: 0x00039888 File Offset: 0x00037A88
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

		// Token: 0x17000151 RID: 337
		// (get) Token: 0x0600075A RID: 1882 RVA: 0x000398C0 File Offset: 0x00037AC0
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

		// Token: 0x17000152 RID: 338
		// (get) Token: 0x0600075B RID: 1883 RVA: 0x000398F8 File Offset: 0x00037AF8
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

		// Token: 0x17000153 RID: 339
		// (get) Token: 0x0600075C RID: 1884 RVA: 0x00039930 File Offset: 0x00037B30
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

		// Token: 0x17000154 RID: 340
		// (get) Token: 0x0600075D RID: 1885 RVA: 0x00039968 File Offset: 0x00037B68
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

		// Token: 0x17000155 RID: 341
		// (set) Token: 0x0600075E RID: 1886 RVA: 0x000399E4 File Offset: 0x00037BE4
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

		// Token: 0x0600075F RID: 1887 RVA: 0x00039A2C File Offset: 0x00037C2C
		[Obsolete("Use Close instead")]
		public void CloseArchive()
		{
			this.Close();
		}

		// Token: 0x06000760 RID: 1888 RVA: 0x00039A38 File Offset: 0x00037C38
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

		// Token: 0x06000761 RID: 1889 RVA: 0x00039A94 File Offset: 0x00037C94
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

		// Token: 0x06000762 RID: 1890 RVA: 0x00039AF0 File Offset: 0x00037CF0
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

		// Token: 0x06000763 RID: 1891 RVA: 0x00039D10 File Offset: 0x00037F10
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

		// Token: 0x06000764 RID: 1892 RVA: 0x00039DAC File Offset: 0x00037FAC
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

		// Token: 0x06000765 RID: 1893 RVA: 0x0003A0E8 File Offset: 0x000382E8
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x06000766 RID: 1894 RVA: 0x0003A0FC File Offset: 0x000382FC
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

		// Token: 0x06000767 RID: 1895 RVA: 0x0003A17C File Offset: 0x0003837C
		public virtual void Close()
		{
			this.Dispose(true);
		}

		// Token: 0x06000768 RID: 1896 RVA: 0x0003A188 File Offset: 0x00038388
		~TarArchive()
		{
			this.Dispose(false);
		}

		// Token: 0x06000769 RID: 1897 RVA: 0x0003A1BC File Offset: 0x000383BC
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

		// Token: 0x0600076A RID: 1898 RVA: 0x0003A21C File Offset: 0x0003841C
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

		// Token: 0x04000472 RID: 1138
		private bool keepOldFiles;

		// Token: 0x04000473 RID: 1139
		private bool asciiTranslate;

		// Token: 0x04000474 RID: 1140
		private int userId;

		// Token: 0x04000475 RID: 1141
		private string userName = string.Empty;

		// Token: 0x04000476 RID: 1142
		private int groupId;

		// Token: 0x04000477 RID: 1143
		private string groupName = string.Empty;

		// Token: 0x04000478 RID: 1144
		private string rootPath;

		// Token: 0x04000479 RID: 1145
		private string pathPrefix;

		// Token: 0x0400047A RID: 1146
		private bool applyUserInfoOverrides;

		// Token: 0x0400047B RID: 1147
		private TarInputStream tarIn;

		// Token: 0x0400047C RID: 1148
		private TarOutputStream tarOut;

		// Token: 0x0400047D RID: 1149
		private bool isDisposed;
	}
}
