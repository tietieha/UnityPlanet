using System;
using UWA.ICSharpCode.SharpZipLib.Checksums;

namespace UWA.ICSharpCode.SharpZipLib.Encryption
{
	// Token: 0x0200009F RID: 159
	internal class PkzipClassicCryptoBase
	{
		// Token: 0x06000768 RID: 1896 RVA: 0x00031C04 File Offset: 0x0002FE04
		protected byte TransformByte()
		{
			uint num = (this.keys[2] & 65535U) | 2U;
			return (byte)(num * (num ^ 1U) >> 8);
		}

		// Token: 0x06000769 RID: 1897 RVA: 0x00031C38 File Offset: 0x0002FE38
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

		// Token: 0x0600076A RID: 1898 RVA: 0x00031CEC File Offset: 0x0002FEEC
		protected void UpdateKeys(byte ch)
		{
			this.keys[0] = UWA.ICSharpCode.SharpZipLib.Checksums.Crc32.ComputeCrc32(this.keys[0], ch);
			this.keys[1] = this.keys[1] + (uint)((byte)this.keys[0]);
			this.keys[1] = this.keys[1] * 134775813U + 1U;
			this.keys[2] = UWA.ICSharpCode.SharpZipLib.Checksums.Crc32.ComputeCrc32(this.keys[2], (byte)(this.keys[1] >> 24));
		}

		// Token: 0x0600076B RID: 1899 RVA: 0x00031D68 File Offset: 0x0002FF68
		protected void Reset()
		{
			this.keys[0] = 0U;
			this.keys[1] = 0U;
			this.keys[2] = 0U;
		}

		// Token: 0x04000493 RID: 1171
		private uint[] keys;
	}
}
