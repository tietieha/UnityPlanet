using System;
using System.IO;
using UWA;

namespace UWALocal
{
	// Token: 0x0200001A RID: 26
	internal class TrackWriter<T>
	{
		// Token: 0x06000160 RID: 352 RVA: 0x000093F4 File Offset: 0x000075F4
		public TrackWriter(int bufferSize)
		{
			this.mWriterBuffer = new T[Math.Min(this.mBufferMaxLength, bufferSize)];
			this.mBufferMaxLength = ((this.mBufferMaxLength > bufferSize) ? bufferSize : this.mBufferMaxLength);
		}

		// Token: 0x06000161 RID: 353 RVA: 0x00009458 File Offset: 0x00007658
		~TrackWriter()
		{
			if (this.mLogWriter != null)
			{
				this.mLogWriter.Close();
				this.mLogWriter = null;
			}
			this.mWriterBuffer = null;
			this.mBufferLength = 0;
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x06000162 RID: 354 RVA: 0x000094AC File Offset: 0x000076AC
		// (set) Token: 0x06000163 RID: 355 RVA: 0x000094B4 File Offset: 0x000076B4
		public string LogPath
		{
			get
			{
				return this.mLogPath;
			}
			set
			{
				if (!value.Equals(this.mLogPath))
				{
					if (value.Equals(""))
					{
						try
						{
							this.BufferToFile();
							if (this.mLogWriter != null)
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
					if (!this.mLogPath.Equals("") && !value.Equals(""))
					{
						SharedUtils.Log("Change the logPath of assetTrackLog without a \"\" setting !");
					}
					this.mLogPath = value;
				}
			}
		}

		// Token: 0x06000164 RID: 356 RVA: 0x0000956C File Offset: 0x0000776C
		public void WriteToBuffer(T str)
		{
			if (this.mBufferLength >= this.mBufferMaxLength)
			{
				this.BufferToFile();
			}
			this.mWriterBuffer[this.mBufferLength] = str;
			this.mBufferLength++;
		}

		// Token: 0x06000165 RID: 357 RVA: 0x000095A8 File Offset: 0x000077A8
		public void BufferToFile()
		{
			if (this.mBufferLength > 0)
			{
				if (this.mLogWriter == null)
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

		// Token: 0x06000166 RID: 358 RVA: 0x0000962C File Offset: 0x0000782C
		public void WriteLineToFile(T str)
		{
			if (this.mLogWriter == null)
			{
				this.mLogWriter = new StreamWriter(this.mLogPath);
			}
			this.mLogWriter.WriteLine(str.ToString());
			this.mLogWriter.Flush();
		}

		// Token: 0x040000A2 RID: 162
		private string mLogPath = "";

		// Token: 0x040000A3 RID: 163
		private StreamWriter mLogWriter;

		// Token: 0x040000A4 RID: 164
		private T[] mWriterBuffer;

		// Token: 0x040000A5 RID: 165
		private int mBufferMaxLength = 1000;

		// Token: 0x040000A6 RID: 166
		private int mBufferLength;
	}
}
