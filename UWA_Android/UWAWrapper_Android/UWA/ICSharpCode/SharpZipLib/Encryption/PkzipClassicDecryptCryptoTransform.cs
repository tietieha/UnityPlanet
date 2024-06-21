using System;
using System.Security.Cryptography;

namespace UWA.ICSharpCode.SharpZipLib.Encryption
{
	// Token: 0x020000A1 RID: 161
	internal class PkzipClassicDecryptCryptoTransform : PkzipClassicCryptoBase, ICryptoTransform, IDisposable
	{
		// Token: 0x06000775 RID: 1909 RVA: 0x00031EA8 File Offset: 0x000300A8
		internal PkzipClassicDecryptCryptoTransform(byte[] keyBlock)
		{
			base.SetKeys(keyBlock);
		}

		// Token: 0x06000776 RID: 1910 RVA: 0x00031EBC File Offset: 0x000300BC
		public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
		{
			byte[] array = new byte[inputCount];
			this.TransformBlock(inputBuffer, inputOffset, inputCount, array, 0);
			return array;
		}

		// Token: 0x06000777 RID: 1911 RVA: 0x00031EEC File Offset: 0x000300EC
		public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
		{
			for (int i = inputOffset; i < inputOffset + inputCount; i++)
			{
				byte b = inputBuffer[i] ^ base.TransformByte();
				outputBuffer[outputOffset++] = b;
				base.UpdateKeys(b);
			}
			return inputCount;
		}

		// Token: 0x1700013C RID: 316
		// (get) Token: 0x06000778 RID: 1912 RVA: 0x00031F3C File Offset: 0x0003013C
		public bool CanReuseTransform
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700013D RID: 317
		// (get) Token: 0x06000779 RID: 1913 RVA: 0x00031F58 File Offset: 0x00030158
		public int InputBlockSize
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x1700013E RID: 318
		// (get) Token: 0x0600077A RID: 1914 RVA: 0x00031F74 File Offset: 0x00030174
		public int OutputBlockSize
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x1700013F RID: 319
		// (get) Token: 0x0600077B RID: 1915 RVA: 0x00031F90 File Offset: 0x00030190
		public bool CanTransformMultipleBlocks
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600077C RID: 1916 RVA: 0x00031FAC File Offset: 0x000301AC
		public void Dispose()
		{
			base.Reset();
		}
	}
}
