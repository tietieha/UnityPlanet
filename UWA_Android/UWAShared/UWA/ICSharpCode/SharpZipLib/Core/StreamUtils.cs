using System;
using System.IO;
using System.Runtime.InteropServices;

namespace UWA.ICSharpCode.SharpZipLib.Core
{
	// Token: 0x020000C5 RID: 197
	[ComVisible(false)]
	public sealed class StreamUtils
	{
		// Token: 0x060008C5 RID: 2245 RVA: 0x0004047C File Offset: 0x0003E67C
		public static void ReadFully(Stream stream, byte[] buffer)
		{
			StreamUtils.ReadFully(stream, buffer, 0, buffer.Length);
		}

		// Token: 0x060008C6 RID: 2246 RVA: 0x0004048C File Offset: 0x0003E68C
		public static void ReadFully(Stream stream, byte[] buffer, int offset, int count)
		{
			bool flag = stream == null;
			if (flag)
			{
				throw new ArgumentNullException("stream");
			}
			bool flag2 = buffer == null;
			if (flag2)
			{
				throw new ArgumentNullException("buffer");
			}
			bool flag3 = offset < 0 || offset > buffer.Length;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			bool flag4 = count < 0 || offset + count > buffer.Length;
			if (flag4)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			while (count > 0)
			{
				int num = stream.Read(buffer, offset, count);
				bool flag5 = num <= 0;
				if (flag5)
				{
					throw new EndOfStreamException();
				}
				offset += num;
				count -= num;
			}
		}

		// Token: 0x060008C7 RID: 2247 RVA: 0x0004055C File Offset: 0x0003E75C
		public static void Copy(Stream source, Stream destination, byte[] buffer)
		{
			bool flag = source == null;
			if (flag)
			{
				throw new ArgumentNullException("source");
			}
			bool flag2 = destination == null;
			if (flag2)
			{
				throw new ArgumentNullException("destination");
			}
			bool flag3 = buffer == null;
			if (flag3)
			{
				throw new ArgumentNullException("buffer");
			}
			bool flag4 = buffer.Length < 128;
			if (flag4)
			{
				throw new ArgumentException("Buffer is too small", "buffer");
			}
			bool flag5 = true;
			while (flag5)
			{
				int num = source.Read(buffer, 0, buffer.Length);
				bool flag6 = num > 0;
				if (flag6)
				{
					destination.Write(buffer, 0, num);
				}
				else
				{
					destination.Flush();
					flag5 = false;
				}
			}
		}

		// Token: 0x060008C8 RID: 2248 RVA: 0x00040620 File Offset: 0x0003E820
		public static void Copy(Stream source, Stream destination, byte[] buffer, ProgressHandler progressHandler, TimeSpan updateInterval, object sender, string name)
		{
			StreamUtils.Copy(source, destination, buffer, progressHandler, updateInterval, sender, name, -1L);
		}

		// Token: 0x060008C9 RID: 2249 RVA: 0x00040638 File Offset: 0x0003E838
		public static void Copy(Stream source, Stream destination, byte[] buffer, ProgressHandler progressHandler, TimeSpan updateInterval, object sender, string name, long fixedTarget)
		{
			bool flag = source == null;
			if (flag)
			{
				throw new ArgumentNullException("source");
			}
			bool flag2 = destination == null;
			if (flag2)
			{
				throw new ArgumentNullException("destination");
			}
			bool flag3 = buffer == null;
			if (flag3)
			{
				throw new ArgumentNullException("buffer");
			}
			bool flag4 = buffer.Length < 128;
			if (flag4)
			{
				throw new ArgumentException("Buffer is too small", "buffer");
			}
			bool flag5 = progressHandler == null;
			if (flag5)
			{
				throw new ArgumentNullException("progressHandler");
			}
			bool flag6 = true;
			DateTime now = DateTime.Now;
			long num = 0L;
			long target = 0L;
			bool flag7 = fixedTarget >= 0L;
			if (flag7)
			{
				target = fixedTarget;
			}
			else
			{
				bool canSeek = source.CanSeek;
				if (canSeek)
				{
					target = source.Length - source.Position;
				}
			}
			ProgressEventArgs progressEventArgs = new ProgressEventArgs(name, num, target);
			progressHandler(sender, progressEventArgs);
			bool flag8 = true;
			while (flag6)
			{
				int num2 = source.Read(buffer, 0, buffer.Length);
				bool flag9 = num2 > 0;
				if (flag9)
				{
					num += (long)num2;
					flag8 = false;
					destination.Write(buffer, 0, num2);
				}
				else
				{
					destination.Flush();
					flag6 = false;
				}
				bool flag10 = DateTime.Now - now > updateInterval;
				if (flag10)
				{
					flag8 = true;
					now = DateTime.Now;
					progressEventArgs = new ProgressEventArgs(name, num, target);
					progressHandler(sender, progressEventArgs);
					flag6 = progressEventArgs.ContinueRunning;
				}
			}
			bool flag11 = !flag8;
			if (flag11)
			{
				progressEventArgs = new ProgressEventArgs(name, num, target);
				progressHandler(sender, progressEventArgs);
			}
		}

		// Token: 0x060008CA RID: 2250 RVA: 0x000407FC File Offset: 0x0003E9FC
		private StreamUtils()
		{
		}
	}
}
