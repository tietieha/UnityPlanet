using System;
using System.Security.Cryptography;

namespace UWA.ICSharpCode.SharpZipLib.Encryption
{
	// Token: 0x020000B0 RID: 176
	internal class PkzipClassicDecryptCryptoTransform : PkzipClassicCryptoBase, ICryptoTransform, IDisposable
	{
		// Token: 0x06000851 RID: 2129 RVA: 0x0003EC60 File Offset: 0x0003CE60
		internal PkzipClassicDecryptCryptoTransform(byte[] keyBlock)
		{
			base.SetKeys(keyBlock);
		}

		// Token: 0x06000852 RID: 2130 RVA: 0x0003EC74 File Offset: 0x0003CE74
		public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
		{
			byte[] array = new byte[inputCount];
			this.TransformBlock(inputBuffer, inputOffset, inputCount, array, 0);
			return array;
		}

		// Token: 0x06000853 RID: 2131 RVA: 0x0003ECA4 File Offset: 0x0003CEA4
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

		// Token: 0x17000190 RID: 400
		// (get) Token: 0x06000854 RID: 2132 RVA: 0x0003ECF4 File Offset: 0x0003CEF4
		public bool CanReuseTransform
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000191 RID: 401
		// (get) Token: 0x06000855 RID: 2133 RVA: 0x0003ED10 File Offset: 0x0003CF10
		public int InputBlockSize
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x17000192 RID: 402
		// (get) Token: 0x06000856 RID: 2134 RVA: 0x0003ED2C File Offset: 0x0003CF2C
		public int OutputBlockSize
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x17000193 RID: 403
		// (get) Token: 0x06000857 RID: 2135 RVA: 0x0003ED48 File Offset: 0x0003CF48
		public bool CanTransformMultipleBlocks
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06000858 RID: 2136 RVA: 0x0003ED64 File Offset: 0x0003CF64
		public void Dispose()
		{
			base.Reset();
		}
	}
}
