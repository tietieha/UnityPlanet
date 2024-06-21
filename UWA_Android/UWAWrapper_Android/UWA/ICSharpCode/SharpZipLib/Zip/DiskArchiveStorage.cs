using System;
using System.IO;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Zip
{
	// Token: 0x02000077 RID: 119
	[ComVisible(false)]
	public class DiskArchiveStorage : BaseArchiveStorage
	{
		// Token: 0x06000534 RID: 1332 RVA: 0x00023640 File Offset: 0x00021840
		public DiskArchiveStorage(ZipFile file, FileUpdateMode updateMode) : base(updateMode)
		{
			bool flag = file.Name == null;
			if (flag)
			{
				throw new ZipException("Cant handle non file archives");
			}
			this.fileName_ = file.Name;
		}

		// Token: 0x06000535 RID: 1333 RVA: 0x00023684 File Offset: 0x00021884
		public DiskArchiveStorage(ZipFile file) : this(file, FileUpdateMode.Safe)
		{
		}

		// Token: 0x06000536 RID: 1334 RVA: 0x00023690 File Offset: 0x00021890
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

		// Token: 0x06000537 RID: 1335 RVA: 0x00023710 File Offset: 0x00021910
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

		// Token: 0x06000538 RID: 1336 RVA: 0x000237E0 File Offset: 0x000219E0
		public override Stream MakeTemporaryCopy(Stream stream)
		{
			stream.Close();
			this.temporaryName_ = DiskArchiveStorage.GetTempFileName(this.fileName_, true);
			File.Copy(this.fileName_, this.temporaryName_, true);
			this.temporaryStream_ = new FileStream(this.temporaryName_, FileMode.Open, FileAccess.ReadWrite);
			return this.temporaryStream_;
		}

		// Token: 0x06000539 RID: 1337 RVA: 0x00023840 File Offset: 0x00021A40
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

		// Token: 0x0600053A RID: 1338 RVA: 0x000238A4 File Offset: 0x00021AA4
		public override void Dispose()
		{
			bool flag = this.temporaryStream_ != null;
			if (flag)
			{
				this.temporaryStream_.Close();
			}
		}

		// Token: 0x0600053B RID: 1339 RVA: 0x000238D4 File Offset: 0x00021AD4
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

		// Token: 0x0400031B RID: 795
		private Stream temporaryStream_;

		// Token: 0x0400031C RID: 796
		private string fileName_;

		// Token: 0x0400031D RID: 797
		private string temporaryName_;
	}
}
