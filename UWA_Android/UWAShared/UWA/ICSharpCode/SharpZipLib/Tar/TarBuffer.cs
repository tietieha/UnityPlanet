using System;
using System.IO;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Tar
{
	// Token: 0x020000A0 RID: 160
	[ComVisible(false)]
	public class TarBuffer
	{
		// Token: 0x17000156 RID: 342
		// (get) Token: 0x0600076B RID: 1899 RVA: 0x0003A2E0 File Offset: 0x000384E0
		public int RecordSize
		{
			get
			{
				return this.recordSize;
			}
		}

		// Token: 0x0600076C RID: 1900 RVA: 0x0003A300 File Offset: 0x00038500
		[Obsolete("Use RecordSize property instead")]
		public int GetRecordSize()
		{
			return this.recordSize;
		}

		// Token: 0x17000157 RID: 343
		// (get) Token: 0x0600076D RID: 1901 RVA: 0x0003A320 File Offset: 0x00038520
		public int BlockFactor
		{
			get
			{
				return this.blockFactor;
			}
		}

		// Token: 0x0600076E RID: 1902 RVA: 0x0003A340 File Offset: 0x00038540
		[Obsolete("Use BlockFactor property instead")]
		public int GetBlockFactor()
		{
			return this.blockFactor;
		}

		// Token: 0x0600076F RID: 1903 RVA: 0x0003A360 File Offset: 0x00038560
		protected TarBuffer()
		{
		}

		// Token: 0x06000770 RID: 1904 RVA: 0x0003A384 File Offset: 0x00038584
		public static TarBuffer CreateInputTarBuffer(Stream inputStream)
		{
			bool flag = inputStream == null;
			if (flag)
			{
				throw new ArgumentNullException("inputStream");
			}
			return TarBuffer.CreateInputTarBuffer(inputStream, 20);
		}

		// Token: 0x06000771 RID: 1905 RVA: 0x0003A3BC File Offset: 0x000385BC
		public static TarBuffer CreateInputTarBuffer(Stream inputStream, int blockFactor)
		{
			bool flag = inputStream == null;
			if (flag)
			{
				throw new ArgumentNullException("inputStream");
			}
			bool flag2 = blockFactor <= 0;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("blockFactor", "Factor cannot be negative");
			}
			TarBuffer tarBuffer = new TarBuffer();
			tarBuffer.inputStream = inputStream;
			tarBuffer.outputStream = null;
			tarBuffer.Initialize(blockFactor);
			return tarBuffer;
		}

		// Token: 0x06000772 RID: 1906 RVA: 0x0003A428 File Offset: 0x00038628
		public static TarBuffer CreateOutputTarBuffer(Stream outputStream)
		{
			bool flag = outputStream == null;
			if (flag)
			{
				throw new ArgumentNullException("outputStream");
			}
			return TarBuffer.CreateOutputTarBuffer(outputStream, 20);
		}

		// Token: 0x06000773 RID: 1907 RVA: 0x0003A460 File Offset: 0x00038660
		public static TarBuffer CreateOutputTarBuffer(Stream outputStream, int blockFactor)
		{
			bool flag = outputStream == null;
			if (flag)
			{
				throw new ArgumentNullException("outputStream");
			}
			bool flag2 = blockFactor <= 0;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("blockFactor", "Factor cannot be negative");
			}
			TarBuffer tarBuffer = new TarBuffer();
			tarBuffer.inputStream = null;
			tarBuffer.outputStream = outputStream;
			tarBuffer.Initialize(blockFactor);
			return tarBuffer;
		}

		// Token: 0x06000774 RID: 1908 RVA: 0x0003A4CC File Offset: 0x000386CC
		private void Initialize(int archiveBlockFactor)
		{
			this.blockFactor = archiveBlockFactor;
			this.recordSize = archiveBlockFactor * 512;
			this.recordBuffer = new byte[this.RecordSize];
			bool flag = this.inputStream != null;
			if (flag)
			{
				this.currentRecordIndex = -1;
				this.currentBlockIndex = this.BlockFactor;
			}
			else
			{
				this.currentRecordIndex = 0;
				this.currentBlockIndex = 0;
			}
		}

		// Token: 0x06000775 RID: 1909 RVA: 0x0003A540 File Offset: 0x00038740
		[Obsolete("Use IsEndOfArchiveBlock instead")]
		public bool IsEOFBlock(byte[] block)
		{
			bool flag = block == null;
			if (flag)
			{
				throw new ArgumentNullException("block");
			}
			bool flag2 = block.Length != 512;
			if (flag2)
			{
				throw new ArgumentException("block length is invalid");
			}
			for (int i = 0; i < 512; i++)
			{
				bool flag3 = block[i] > 0;
				if (flag3)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000776 RID: 1910 RVA: 0x0003A5C4 File Offset: 0x000387C4
		public static bool IsEndOfArchiveBlock(byte[] block)
		{
			bool flag = block == null;
			if (flag)
			{
				throw new ArgumentNullException("block");
			}
			bool flag2 = block.Length != 512;
			if (flag2)
			{
				throw new ArgumentException("block length is invalid");
			}
			for (int i = 0; i < 512; i++)
			{
				bool flag3 = block[i] > 0;
				if (flag3)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000777 RID: 1911 RVA: 0x0003A648 File Offset: 0x00038848
		public void SkipBlock()
		{
			bool flag = this.inputStream == null;
			if (flag)
			{
				throw new TarException("no input stream defined");
			}
			bool flag2 = this.currentBlockIndex >= this.BlockFactor;
			if (flag2)
			{
				bool flag3 = !this.ReadRecord();
				if (flag3)
				{
					throw new TarException("Failed to read a record");
				}
			}
			this.currentBlockIndex++;
		}

		// Token: 0x06000778 RID: 1912 RVA: 0x0003A6BC File Offset: 0x000388BC
		public byte[] ReadBlock()
		{
			bool flag = this.inputStream == null;
			if (flag)
			{
				throw new TarException("TarBuffer.ReadBlock - no input stream defined");
			}
			bool flag2 = this.currentBlockIndex >= this.BlockFactor;
			if (flag2)
			{
				bool flag3 = !this.ReadRecord();
				if (flag3)
				{
					throw new TarException("Failed to read a record");
				}
			}
			byte[] array = new byte[512];
			Array.Copy(this.recordBuffer, this.currentBlockIndex * 512, array, 0, 512);
			this.currentBlockIndex++;
			return array;
		}

		// Token: 0x06000779 RID: 1913 RVA: 0x0003A764 File Offset: 0x00038964
		private bool ReadRecord()
		{
			bool flag = this.inputStream == null;
			if (flag)
			{
				throw new TarException("no input stream stream defined");
			}
			this.currentBlockIndex = 0;
			int num = 0;
			long num2;
			for (int i = this.RecordSize; i > 0; i -= (int)num2)
			{
				num2 = (long)this.inputStream.Read(this.recordBuffer, num, i);
				bool flag2 = num2 <= 0L;
				if (flag2)
				{
					break;
				}
				num += (int)num2;
			}
			this.currentRecordIndex++;
			return true;
		}

		// Token: 0x17000158 RID: 344
		// (get) Token: 0x0600077A RID: 1914 RVA: 0x0003A804 File Offset: 0x00038A04
		public int CurrentBlock
		{
			get
			{
				return this.currentBlockIndex;
			}
		}

		// Token: 0x17000159 RID: 345
		// (get) Token: 0x0600077B RID: 1915 RVA: 0x0003A824 File Offset: 0x00038A24
		// (set) Token: 0x0600077C RID: 1916 RVA: 0x0003A844 File Offset: 0x00038A44
		public bool IsStreamOwner
		{
			get
			{
				return this.isStreamOwner_;
			}
			set
			{
				this.isStreamOwner_ = value;
			}
		}

		// Token: 0x0600077D RID: 1917 RVA: 0x0003A850 File Offset: 0x00038A50
		[Obsolete("Use CurrentBlock property instead")]
		public int GetCurrentBlockNum()
		{
			return this.currentBlockIndex;
		}

		// Token: 0x1700015A RID: 346
		// (get) Token: 0x0600077E RID: 1918 RVA: 0x0003A870 File Offset: 0x00038A70
		public int CurrentRecord
		{
			get
			{
				return this.currentRecordIndex;
			}
		}

		// Token: 0x0600077F RID: 1919 RVA: 0x0003A890 File Offset: 0x00038A90
		[Obsolete("Use CurrentRecord property instead")]
		public int GetCurrentRecordNum()
		{
			return this.currentRecordIndex;
		}

		// Token: 0x06000780 RID: 1920 RVA: 0x0003A8B0 File Offset: 0x00038AB0
		public void WriteBlock(byte[] block)
		{
			bool flag = block == null;
			if (flag)
			{
				throw new ArgumentNullException("block");
			}
			bool flag2 = this.outputStream == null;
			if (flag2)
			{
				throw new TarException("TarBuffer.WriteBlock - no output stream defined");
			}
			bool flag3 = block.Length != 512;
			if (flag3)
			{
				string message = string.Format("TarBuffer.WriteBlock - block to write has length '{0}' which is not the block size of '{1}'", block.Length, 512);
				throw new TarException(message);
			}
			bool flag4 = this.currentBlockIndex >= this.BlockFactor;
			if (flag4)
			{
				this.WriteRecord();
			}
			Array.Copy(block, 0, this.recordBuffer, this.currentBlockIndex * 512, 512);
			this.currentBlockIndex++;
		}

		// Token: 0x06000781 RID: 1921 RVA: 0x0003A980 File Offset: 0x00038B80
		public void WriteBlock(byte[] buffer, int offset)
		{
			bool flag = buffer == null;
			if (flag)
			{
				throw new ArgumentNullException("buffer");
			}
			bool flag2 = this.outputStream == null;
			if (flag2)
			{
				throw new TarException("TarBuffer.WriteBlock - no output stream stream defined");
			}
			bool flag3 = offset < 0 || offset >= buffer.Length;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			bool flag4 = offset + 512 > buffer.Length;
			if (flag4)
			{
				string message = string.Format("TarBuffer.WriteBlock - record has length '{0}' with offset '{1}' which is less than the record size of '{2}'", buffer.Length, offset, this.recordSize);
				throw new TarException(message);
			}
			bool flag5 = this.currentBlockIndex >= this.BlockFactor;
			if (flag5)
			{
				this.WriteRecord();
			}
			Array.Copy(buffer, offset, this.recordBuffer, this.currentBlockIndex * 512, 512);
			this.currentBlockIndex++;
		}

		// Token: 0x06000782 RID: 1922 RVA: 0x0003AA80 File Offset: 0x00038C80
		private void WriteRecord()
		{
			bool flag = this.outputStream == null;
			if (flag)
			{
				throw new TarException("TarBuffer.WriteRecord no output stream defined");
			}
			this.outputStream.Write(this.recordBuffer, 0, this.RecordSize);
			this.outputStream.Flush();
			this.currentBlockIndex = 0;
			this.currentRecordIndex++;
		}

		// Token: 0x06000783 RID: 1923 RVA: 0x0003AAE8 File Offset: 0x00038CE8
		private void WriteFinalRecord()
		{
			bool flag = this.outputStream == null;
			if (flag)
			{
				throw new TarException("TarBuffer.WriteFinalRecord no output stream defined");
			}
			bool flag2 = this.currentBlockIndex > 0;
			if (flag2)
			{
				int num = this.currentBlockIndex * 512;
				Array.Clear(this.recordBuffer, num, this.RecordSize - num);
				this.WriteRecord();
			}
			this.outputStream.Flush();
		}

		// Token: 0x06000784 RID: 1924 RVA: 0x0003AB60 File Offset: 0x00038D60
		public void Close()
		{
			bool flag = this.outputStream != null;
			if (flag)
			{
				this.WriteFinalRecord();
				bool flag2 = this.isStreamOwner_;
				if (flag2)
				{
					this.outputStream.Close();
				}
				this.outputStream = null;
			}
			else
			{
				bool flag3 = this.inputStream != null;
				if (flag3)
				{
					bool flag4 = this.isStreamOwner_;
					if (flag4)
					{
						this.inputStream.Close();
					}
					this.inputStream = null;
				}
			}
		}

		// Token: 0x0400047E RID: 1150
		public const int BlockSize = 512;

		// Token: 0x0400047F RID: 1151
		public const int DefaultBlockFactor = 20;

		// Token: 0x04000480 RID: 1152
		public const int DefaultRecordSize = 10240;

		// Token: 0x04000481 RID: 1153
		private Stream inputStream;

		// Token: 0x04000482 RID: 1154
		private Stream outputStream;

		// Token: 0x04000483 RID: 1155
		private byte[] recordBuffer;

		// Token: 0x04000484 RID: 1156
		private int currentBlockIndex;

		// Token: 0x04000485 RID: 1157
		private int currentRecordIndex;

		// Token: 0x04000486 RID: 1158
		private int recordSize = 10240;

		// Token: 0x04000487 RID: 1159
		private int blockFactor = 20;

		// Token: 0x04000488 RID: 1160
		private bool isStreamOwner_ = true;
	}
}
