using System;
using System.IO;
using System.Runtime.InteropServices;

namespace UWACore.Util
{
	// Token: 0x02000044 RID: 68
	[ComVisible(false)]
	public class ByteArray
	{
		// Token: 0x06000310 RID: 784 RVA: 0x00014730 File Offset: 0x00012930
		public ByteArray()
		{
			this.stream = new MemoryStream();
			this.writer = new BinaryWriter(this.stream);
		}

		// Token: 0x06000311 RID: 785 RVA: 0x00014758 File Offset: 0x00012958
		public void writeByte(byte value)
		{
			this.writer.Write(value);
		}

		// Token: 0x06000312 RID: 786 RVA: 0x00014768 File Offset: 0x00012968
		public byte[] GetAllBytes()
		{
			byte[] array = new byte[this.stream.Length];
			this.stream.Position = 0L;
			this.stream.Read(array, 0, array.Length);
			return array;
		}

		// Token: 0x06000313 RID: 787 RVA: 0x000147B4 File Offset: 0x000129B4
		public void WriteTo(string path)
		{
			using (FileStream fileStream = File.Create(path))
			{
				bool flag = this.stream.Length > (long)UWAPublicCore50983.ByteArray.buffer.Length;
				if (flag)
				{
					UWAPublicCore50983.ByteArray.buffer = new byte[this.stream.Length];
				}
				this.stream.Seek(0L, SeekOrigin.Begin);
				this.stream.Read(UWAPublicCore50983.ByteArray.buffer, 0, (int)this.stream.Length);
				fileStream.Write(UWAPublicCore50983.ByteArray.buffer, 0, (int)this.stream.Length);
			}
		}

		// Token: 0x040001AB RID: 427
		private MemoryStream stream;

		// Token: 0x040001AC RID: 428
		private BinaryWriter writer;

		// Token: 0x040001AD RID: 429
		private static byte[] buffer = new byte[4096];
	}
}
