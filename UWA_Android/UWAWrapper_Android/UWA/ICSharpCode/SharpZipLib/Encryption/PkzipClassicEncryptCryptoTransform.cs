using System;
using System.Security.Cryptography;

namespace UWA.ICSharpCode.SharpZipLib.Encryption
{
	// Token: 0x020000A0 RID: 160
	internal class PkzipClassicEncryptCryptoTransform : PkzipClassicCryptoBase, ICryptoTransform, IDisposable
	{
		// Token: 0x0600076D RID: 1901 RVA: 0x00031D94 File Offset: 0x0002FF94
		internal PkzipClassicEncryptCryptoTransform(byte[] keyBlock)
		{
			base.SetKeys(keyBlock);
		}

		// Token: 0x0600076E RID: 1902 RVA: 0x00031DA8 File Offset: 0x0002FFA8
		public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
		{
			byte[] array = new byte[inputCount];
			this.TransformBlock(inputBuffer, inputOffset, inputCount, array, 0);
			return array;
		}

		// Token: 0x0600076F RID: 1903 RVA: 0x00031DD8 File Offset: 0x0002FFD8
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

		// Token: 0x17000138 RID: 312
		// (get) Token: 0x06000770 RID: 1904 RVA: 0x00031E2C File Offset: 0x0003002C
		public bool CanReuseTransform
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000139 RID: 313
		// (get) Token: 0x06000771 RID: 1905 RVA: 0x00031E48 File Offset: 0x00030048
		public int InputBlockSize
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x1700013A RID: 314
		// (get) Token: 0x06000772 RID: 1906 RVA: 0x00031E64 File Offset: 0x00030064
		public int OutputBlockSize
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x1700013B RID: 315
		// (get) Token: 0x06000773 RID: 1907 RVA: 0x00031E80 File Offset: 0x00030080
		public bool CanTransformMultipleBlocks
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06000774 RID: 1908 RVA: 0x00031E9C File Offset: 0x0003009C
		public void Dispose()
		{
			base.Reset();
		}
	}
}
