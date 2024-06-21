using System;
using System.IO;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Tar
{
	// Token: 0x02000091 RID: 145
	[ComVisible(false)]
	public class TarBuffer
	{
		// Token: 0x17000102 RID: 258
		// (get) Token: 0x0600068F RID: 1679 RVA: 0x0002D528 File Offset: 0x0002B728
		public int RecordSize
		{
			get
			{
				return this.recordSize;
			}
		}

		// Token: 0x06000690 RID: 1680 RVA: 0x0002D548 File Offset: 0x0002B748
		[Obsolete("Use RecordSize property instead")]
		public int GetRecordSize()
		{
			return this.recordSize;
		}

		// Token: 0x17000103 RID: 259
		// (get) Token: 0x06000691 RID: 1681 RVA: 0x0002D568 File Offset: 0x0002B768
		public int BlockFactor
		{
			get
			{
				return this.blockFactor;
			}
		}

		// Token: 0x06000692 RID: 1682 RVA: 0x0002D588 File Offset: 0x0002B788
		[Obsolete("Use BlockFactor property instead")]
		public int GetBlockFactor()
		{
			return this.blockFactor;
		}

		// Token: 0x06000693 RID: 1683 RVA: 0x0002D5A8 File Offset: 0x0002B7A8
		protected TarBuffer()
		{
		}

		// Token: 0x06000694 RID: 1684 RVA: 0x0002D5CC File Offset: 0x0002B7CC
		public static TarBuffer CreateInputTarBuffer(Stream inputStream)
		{
			bool flag = inputStream == null;
			if (flag)
			{
				throw new ArgumentNullException("inputStream");
			}
			return TarBuffer.CreateInputTarBuffer(inputStream, 20);
		}

		// Token: 0x06000695 RID: 1685 RVA: 0x0002D604 File Offset: 0x0002B804
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

		// Token: 0x06000696 RID: 1686 RVA: 0x0002D670 File Offset: 0x0002B870
		public static TarBuffer CreateOutputTarBuffer(Stream outputStream)
		{
			bool flag = outputStream == null;
			if (flag)
			{
				throw new ArgumentNullException("outputStream");
			}
			return TarBuffer.CreateOutputTarBuffer(outputStream, 20);
		}

		// Token: 0x06000697 RID: 1687 RVA: 0x0002D6A8 File Offset: 0x0002B8A8
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

		// Token: 0x06000698 RID: 1688 RVA: 0x0002D714 File Offset: 0x0002B914
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

		// Token: 0x06000699 RID: 1689 RVA: 0x0002D788 File Offset: 0x0002B988
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

		// Token: 0x0600069A RID: 1690 RVA: 0x0002D80C File Offset: 0x0002BA0C
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

		// Token: 0x0600069B RID: 1691 RVA: 0x0002D890 File Offset: 0x0002BA90
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

		// Token: 0x0600069C RID: 1692 RVA: 0x0002D904 File Offset: 0x0002BB04
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

		// Token: 0x0600069D RID: 1693 RVA: 0x0002D9AC File Offset: 0x0002BBAC
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

		// Token: 0x17000104 RID: 260
		// (get) Token: 0x0600069E RID: 1694 RVA: 0x0002DA4C File Offset: 0x0002BC4C
		public int CurrentBlock
		{
			get
			{
				return this.currentBlockIndex;
			}
		}

		// Token: 0x17000105 RID: 261
		// (get) Token: 0x0600069F RID: 1695 RVA: 0x0002DA6C File Offset: 0x0002BC6C
		// (set) Token: 0x060006A0 RID: 1696 RVA: 0x0002DA8C File Offset: 0x0002BC8C
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

		// Token: 0x060006A1 RID: 1697 RVA: 0x0002DA98 File Offset: 0x0002BC98
		[Obsolete("Use CurrentBlock property instead")]
		public int GetCurrentBlockNum()
		{
			return this.currentBlockIndex;
		}

		// Token: 0x17000106 RID: 262
		// (get) Token: 0x060006A2 RID: 1698 RVA: 0x0002DAB8 File Offset: 0x0002BCB8
		public int CurrentRecord
		{
			get
			{
				return this.currentRecordIndex;
			}
		}

		// Token: 0x060006A3 RID: 1699 RVA: 0x0002DAD8 File Offset: 0x0002BCD8
		[Obsolete("Use CurrentRecord property instead")]
		public int GetCurrentRecordNum()
		{
			return this.currentRecordIndex;
		}

		// Token: 0x060006A4 RID: 1700 RVA: 0x0002DAF8 File Offset: 0x0002BCF8
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

		// Token: 0x060006A5 RID: 1701 RVA: 0x0002DBC8 File Offset: 0x0002BDC8
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

		// Token: 0x060006A6 RID: 1702 RVA: 0x0002DCC8 File Offset: 0x0002BEC8
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

		// Token: 0x060006A7 RID: 1703 RVA: 0x0002DD30 File Offset: 0x0002BF30
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

		// Token: 0x060006A8 RID: 1704 RVA: 0x0002DDA8 File Offset: 0x0002BFA8
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

		// Token: 0x0400040B RID: 1035
		public const int BlockSize = 512;

		// Token: 0x0400040C RID: 1036
		public const int DefaultBlockFactor = 20;

		// Token: 0x0400040D RID: 1037
		public const int DefaultRecordSize = 10240;

		// Token: 0x0400040E RID: 1038
		private Stream inputStream;

		// Token: 0x0400040F RID: 1039
		private Stream outputStream;

		// Token: 0x04000410 RID: 1040
		private byte[] recordBuffer;

		// Token: 0x04000411 RID: 1041
		private int currentBlockIndex;

		// Token: 0x04000412 RID: 1042
		private int currentRecordIndex;

		// Token: 0x04000413 RID: 1043
		private int recordSize = 10240;

		// Token: 0x04000414 RID: 1044
		private int blockFactor = 20;

		// Token: 0x04000415 RID: 1045
		private bool isStreamOwner_ = true;
	}
}
