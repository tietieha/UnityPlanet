using System;
using System.IO;

namespace UWA
{
	// Token: 0x02000052 RID: 82
	internal class TrackWriter<T>
	{
		// Token: 0x060003A3 RID: 931 RVA: 0x00018DB0 File Offset: 0x00016FB0
		public TrackWriter(int bufferSize)
		{
			this.mWriterBuffer = new T[Math.Min(this.mBufferMaxLength, bufferSize)];
			this.mBufferMaxLength = ((this.mBufferMaxLength > bufferSize) ? bufferSize : this.mBufferMaxLength);
		}

		// Token: 0x060003A4 RID: 932 RVA: 0x00018E24 File Offset: 0x00017024
		protected override void Finalize()
		{
			try
			{
				bool flag = this.mLogWriter != null;
				if (flag)
				{
					this.mLogWriter.Close();
					this.mLogWriter = null;
				}
				this.mWriterBuffer = null;
				this.mBufferLength = 0;
			}
			finally
			{
				base.Finalize();
			}
		}

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x060003A5 RID: 933 RVA: 0x00018E84 File Offset: 0x00017084
		// (set) Token: 0x060003A6 RID: 934 RVA: 0x00018EA4 File Offset: 0x000170A4
		public string LogPath
		{
			get
			{
				return this.mLogPath;
			}
			set
			{
				bool flag = !value.Equals(this.mLogPath);
				if (flag)
				{
					bool flag2 = value.Equals("");
					if (flag2)
					{
						try
						{
							this.BufferToFile();
							bool flag3 = this.mLogWriter != null;
							if (flag3)
							{
								this.mLogWriter.Close();
								this.mLogWriter = null;
							}
						}
						catch (IOException ex)
						{
							SharedUtils.Log("exception occurs when setting log path to \"\": /n" + ex.StackTrace);
						}
					}
					bool flag4 = !this.mLogPath.Equals("") && !value.Equals("");
					if (flag4)
					{
						SharedUtils.Log("Change the logPath of assetTrackLog without a \"\" setting !");
					}
					this.mLogPath = value;
				}
			}
		}

		// Token: 0x060003A7 RID: 935 RVA: 0x00018F88 File Offset: 0x00017188
		public void WriteToBuffer(T str)
		{
			bool flag = this.mBufferLength >= this.mBufferMaxLength;
			if (flag)
			{
				this.BufferToFile();
			}
			this.mWriterBuffer[this.mBufferLength] = str;
			this.mBufferLength++;
		}

		// Token: 0x060003A8 RID: 936 RVA: 0x00018FDC File Offset: 0x000171DC
		public void BufferToFile()
		{
			bool flag = this.mBufferLength > 0;
			if (flag)
			{
				bool flag2 = this.mLogWriter == null;
				if (flag2)
				{
					this.mLogWriter = new StreamWriter(this.mLogPath);
				}
				for (int i = 0; i < this.mBufferLength; i++)
				{
					this.mLogWriter.WriteLine(this.mWriterBuffer[i].ToString());
				}
				this.mBufferLength = 0;
				this.mLogWriter.Flush();
			}
		}

		// Token: 0x060003A9 RID: 937 RVA: 0x00019078 File Offset: 0x00017278
		public void WriteLineToFile(T str)
		{
			bool flag = this.mLogWriter == null;
			if (flag)
			{
				this.mLogWriter = new StreamWriter(this.mLogPath);
			}
			this.mLogWriter.WriteLine(str.ToString());
			this.mLogWriter.Flush();
		}

		// Token: 0x04000215 RID: 533
		private string mLogPath = "";

		// Token: 0x04000216 RID: 534
		private StreamWriter mLogWriter = null;

		// Token: 0x04000217 RID: 535
		private T[] mWriterBuffer;

		// Token: 0x04000218 RID: 536
		private int mBufferMaxLength = 1000;

		// Token: 0x04000219 RID: 537
		private int mBufferLength = 0;
	}
}
