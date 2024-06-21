using System;
using UWA.ICSharpCode.SharpZipLib.Checksums;

namespace UWA.ICSharpCode.SharpZipLib.Encryption
{
	// Token: 0x020000AE RID: 174
	internal class PkzipClassicCryptoBase
	{
		// Token: 0x06000844 RID: 2116 RVA: 0x0003E9BC File Offset: 0x0003CBBC
		protected byte TransformByte()
		{
			uint num = (this.keys[2] & 65535U) | 2U;
			return (byte)(num * (num ^ 1U) >> 8);
		}

		// Token: 0x06000845 RID: 2117 RVA: 0x0003E9F0 File Offset: 0x0003CBF0
		protected void SetKeys(byte[] keyData)
		{
			bool flag = keyData == null;
			if (flag)
			{
				throw new ArgumentNullException("keyData");
			}
			bool flag2 = keyData.Length != 12;
			if (flag2)
			{
				throw new InvalidOperationException("Key length is not valid");
			}
			this.keys = new uint[3];
			this.keys[0] = (uint)((int)keyData[3] << 24 | (int)keyData[2] << 16 | (int)keyData[1] << 8 | (int)keyData[0]);
			this.keys[1] = (uint)((int)keyData[7] << 24 | (int)keyData[6] << 16 | (int)keyData[5] << 8 | (int)keyData[4]);
			this.keys[2] = (uint)((int)keyData[11] << 24 | (int)keyData[10] << 16 | (int)keyData[9] << 8 | (int)keyData[8]);
		}

		// Token: 0x06000846 RID: 2118 RVA: 0x0003EAA4 File Offset: 0x0003CCA4
		protected void UpdateKeys(byte ch)
		{
			this.keys[0] = Crc32.ComputeCrc32(this.keys[0], ch);
			this.keys[1] = this.keys[1] + (uint)((byte)this.keys[0]);
			this.keys[1] = this.keys[1] * 134775813U + 1U;
			this.keys[2] = Crc32.ComputeCrc32(this.keys[2], (byte)(this.keys[1] >> 24));
		}

		// Token: 0x06000847 RID: 2119 RVA: 0x0003EB20 File Offset: 0x0003CD20
		protected void Reset()
		{
			this.keys[0] = 0U;
			this.keys[1] = 0U;
			this.keys[2] = 0U;
		}

		// Token: 0x04000506 RID: 1286
		private uint[] keys;
	}
}
