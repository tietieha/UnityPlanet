using System;
using System.Security.Cryptography;

namespace UWA.ICSharpCode.SharpZipLib.Encryption
{
	// Token: 0x020000AF RID: 175
	internal class PkzipClassicEncryptCryptoTransform : PkzipClassicCryptoBase, ICryptoTransform, IDisposable
	{
		// Token: 0x06000849 RID: 2121 RVA: 0x0003EB4C File Offset: 0x0003CD4C
		internal PkzipClassicEncryptCryptoTransform(byte[] keyBlock)
		{
			base.SetKeys(keyBlock);
		}

		// Token: 0x0600084A RID: 2122 RVA: 0x0003EB60 File Offset: 0x0003CD60
		public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
		{
			byte[] array = new byte[inputCount];
			this.TransformBlock(inputBuffer, inputOffset, inputCount, array, 0);
			return array;
		}

		// Token: 0x0600084B RID: 2123 RVA: 0x0003EB90 File Offset: 0x0003CD90
		public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
		{
			for (int i = inputOffset; i < inputOffset + inputCount; i++)
			{
				byte ch = inputBuffer[i];
				outputBuffer[outputOffset++] = (inputBuffer[i] ^ base.TransformByte());
				base.UpdateKeys(ch);
			}
			return inputCount;
		}

		// Token: 0x1700018C RID: 396
		// (get) Token: 0x0600084C RID: 2124 RVA: 0x0003EBE4 File Offset: 0x0003CDE4
		public bool CanReuseTransform
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700018D RID: 397
		// (get) Token: 0x0600084D RID: 2125 RVA: 0x0003EC00 File Offset: 0x0003CE00
		public int InputBlockSize
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x1700018E RID: 398
		// (get) Token: 0x0600084E RID: 2126 RVA: 0x0003EC1C File Offset: 0x0003CE1C
		public int OutputBlockSize
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x1700018F RID: 399
		// (get) Token: 0x0600084F RID: 2127 RVA: 0x0003EC38 File Offset: 0x0003CE38
		public bool CanTransformMultipleBlocks
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06000850 RID: 2128 RVA: 0x0003EC54 File Offset: 0x0003CE54
		public void Dispose()
		{
			base.Reset();
		}
	}
}
