using System;
using System.IO;

namespace UWACore.Util
{
	// Token: 0x0200002C RID: 44
	public class ByteArray
	{
		// Token: 0x06000200 RID: 512 RVA: 0x0000CAF8 File Offset: 0x0000ACF8
		public ByteArray()
		{
			this.stream = new MemoryStream();
			this.writer = new BinaryWriter(this.stream);
		}

		// Token: 0x06000201 RID: 513 RVA: 0x0000CB1C File Offset: 0x0000AD1C
		public void writeByte(byte value)
		{
			this.writer.Write(value);
		}

		// Token: 0x06000202 RID: 514 RVA: 0x0000CB2C File Offset: 0x0000AD2C
		public byte[] GetAllBytes()
		{
			byte[] array = new byte[this.stream.Length];
			this.stream.Position = 0L;
			this.stream.Read(array, 0, array.Length);
			return array;
		}

		// Token: 0x06000203 RID: 515 RVA: 0x0000CB70 File Offset: 0x0000AD70
		public void WriteTo(string path)
		{
			using (FileStream fileStream = File.Create(path))
			{
				if (this.stream.Length > (long)ByteArray.buffer.Length)
				{
					ByteArray.buffer = new byte[this.stream.Length];
				}
				this.stream.Seek(0L, SeekOrigin.Begin);
				this.stream.Read(ByteArray.buffer, 0, (int)this.stream.Length);
				fileStream.Write(ByteArray.buffer, 0, (int)this.stream.Length);
			}
		}

		// Token: 0x040000FC RID: 252
		private MemoryStream stream;

		// Token: 0x040000FD RID: 253
		private BinaryWriter writer;

		// Token: 0x040000FE RID: 254
		private static byte[] buffer = new byte[4096];
	}
}
