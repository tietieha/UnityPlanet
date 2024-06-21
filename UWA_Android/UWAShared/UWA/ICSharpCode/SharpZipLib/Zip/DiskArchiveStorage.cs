using System;
using System.IO;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x02000086 RID: 134
	[ComVisible(false)]
	public class DiskArchiveStorage : BaseArchiveStorage
	{
		// Token: 0x06000610 RID: 1552 RVA: 0x000303F8 File Offset: 0x0002E5F8
		public DiskArchiveStorage(ZipFile file, FileUpdateMode updateMode) : base(updateMode)
		{
			bool flag = file.Name == null;
			if (flag)
			{
				throw new ZipException("Cant handle non file archives");
			}
			this.fileName_ = file.Name;
		}

		// Token: 0x06000611 RID: 1553 RVA: 0x0003043C File Offset: 0x0002E63C
		public DiskArchiveStorage(ZipFile file) : this(file, FileUpdateMode.Safe)
		{
		}

		// Token: 0x06000612 RID: 1554 RVA: 0x00030448 File Offset: 0x0002E648
		public override Stream GetTemporaryOutput()
		{
			bool flag = this.temporaryName_ != null;
			if (flag)
			{
				this.temporaryName_ = DiskArchiveStorage.GetTempFileName(this.temporaryName_, true);
				this.temporaryStream_ = File.Open(this.temporaryName_, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
			}
			else
			{
				this.temporaryName_ = Path.GetTempFileName();
				this.temporaryStream_ = File.Open(this.temporaryName_, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
			}
			return this.temporaryStream_;
		}

		// Token: 0x06000613 RID: 1555 RVA: 0x000304C8 File Offset: 0x0002E6C8
		public override Stream ConvertTemporaryToFinal()
		{
			bool flag = this.temporaryStream_ == null;
			if (flag)
			{
				throw new ZipException("No temporary stream has been created");
			}
			Stream result = null;
			string tempFileName = DiskArchiveStorage.GetTempFileName(this.fileName_, false);
			bool flag2 = false;
			try
			{
				this.temporaryStream_.Close();
				File.Move(this.fileName_, tempFileName);
				File.Move(this.temporaryName_, this.fileName_);
				flag2 = true;
				File.Delete(tempFileName);
				result = File.Open(this.fileName_, FileMode.Open, FileAccess.Read, FileShare.Read);
			}
			catch (Exception)
			{
				result = null;
				bool flag3 = !flag2;
				if (flag3)
				{
					File.Move(tempFileName, this.fileName_);
					File.Delete(this.temporaryName_);
				}
				throw;
			}
			return result;
		}

		// Token: 0x06000614 RID: 1556 RVA: 0x00030598 File Offset: 0x0002E798
		public override Stream MakeTemporaryCopy(Stream stream)
		{
			stream.Close();
			this.temporaryName_ = DiskArchiveStorage.GetTempFileName(this.fileName_, true);
			File.Copy(this.fileName_, this.temporaryName_, true);
			this.temporaryStream_ = new FileStream(this.temporaryName_, FileMode.Open, FileAccess.ReadWrite);
			return this.temporaryStream_;
		}

		// Token: 0x06000615 RID: 1557 RVA: 0x000305F8 File Offset: 0x0002E7F8
		public override Stream OpenForDirectUpdate(Stream stream)
		{
			bool flag = stream == null || !stream.CanWrite;
			Stream result;
			if (flag)
			{
				bool flag2 = stream != null;
				if (flag2)
				{
					stream.Close();
				}
				result = new FileStream(this.fileName_, FileMode.Open, FileAccess.ReadWrite);
			}
			else
			{
				result = stream;
			}
			return result;
		}

		// Token: 0x06000616 RID: 1558 RVA: 0x0003065C File Offset: 0x0002E85C
		public override void Dispose()
		{
			bool flag = this.temporaryStream_ != null;
			if (flag)
			{
				this.temporaryStream_.Close();
			}
		}

		// Token: 0x06000617 RID: 1559 RVA: 0x0003068C File Offset: 0x0002E88C
		private static string GetTempFileName(string original, bool makeTempFile)
		{
			string text = null;
			bool flag = original == null;
			if (flag)
			{
				text = Path.GetTempFileName();
			}
			else
			{
				int num = 0;
				int second = DateTime.Now.Second;
				while (text == null)
				{
					num++;
					string text2 = string.Format("{0}.{1}{2}.tmp", original, second, num);
					bool flag2 = !File.Exists(text2);
					if (flag2)
					{
						if (makeTempFile)
						{
							try
							{
								using (File.Create(text2))
								{
								}
								text = text2;
							}
							catch
							{
								second = DateTime.Now.Second;
							}
						}
						else
						{
							text = text2;
						}
					}
				}
			}
			return text;
		}

		// Token: 0x0400038E RID: 910
		private Stream temporaryStream_;

		// Token: 0x0400038F RID: 911
		private string fileName_;

		// Token: 0x04000390 RID: 912
		private string temporaryName_;
	}
}
