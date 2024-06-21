using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace UWA.ICSharpCode.SharpZipLib.Encryption
{
	// Token: 0x020000B1 RID: 177
	[ComVisible(false)]
	public sealed class PkzipClassicManaged : PkzipClassic
	{
		// Token: 0x17000194 RID: 404
		// (get) Token: 0x06000859 RID: 2137 RVA: 0x0003ED70 File Offset: 0x0003CF70
		// (set) Token: 0x0600085A RID: 2138 RVA: 0x0003ED8C File Offset: 0x0003CF8C
		public override int BlockSize
		{
			get
			{
				return 8;
			}
			set
			{
				bool flag = value != 8;
				if (flag)
				{
					throw new CryptographicException("Block size is invalid");
				}
			}
		}

		// Token: 0x17000195 RID: 405
		// (get) Token: 0x0600085B RID: 2139 RVA: 0x0003EDB8 File Offset: 0x0003CFB8
		public override KeySizes[] LegalKeySizes
		{
			get
			{
				return new KeySizes[]
				{
					new KeySizes(96, 96, 0)
				};
			}
		}

		// Token: 0x0600085C RID: 2140 RVA: 0x0003EDEC File Offset: 0x0003CFEC
		public override void GenerateIV()
		{
		}

		// Token: 0x17000196 RID: 406
		// (get) Token: 0x0600085D RID: 2141 RVA: 0x0003EDF0 File Offset: 0x0003CFF0
		public override KeySizes[] LegalBlockSizes
		{
			get
			{
				return new KeySizes[]
				{
					new KeySizes(8, 8, 0)
				};
			}
		}

		// Token: 0x17000197 RID: 407
		// (get) Token: 0x0600085E RID: 2142 RVA: 0x0003EE20 File Offset: 0x0003D020
		// (set) Token: 0x0600085F RID: 2143 RVA: 0x0003EE64 File Offset: 0x0003D064
		public override byte[] Key
		{
			get
			{
				bool flag = this.key_ == null;
				if (flag)
				{
					this.GenerateKey();
				}
				return (byte[])this.key_.Clone();
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("value");
				}
				bool flag2 = value.Length != 12;
				if (flag2)
				{
					throw new CryptographicException("Key size is illegal");
				}
				this.key_ = (byte[])value.Clone();
			}
		}

		// Token: 0x06000860 RID: 2144 RVA: 0x0003EEBC File Offset: 0x0003D0BC
		public override void GenerateKey()
		{
			this.key_ = new byte[12];
			Random random = new Random();
			random.NextBytes(this.key_);
		}

		// Token: 0x06000861 RID: 2145 RVA: 0x0003EEF0 File Offset: 0x0003D0F0
		public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
		{
			this.key_ = rgbKey;
			return new PkzipClassicEncryptCryptoTransform(this.Key);
		}

		// Token: 0x06000862 RID: 2146 RVA: 0x0003EF1C File Offset: 0x0003D11C
		public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV)
		{
			this.key_ = rgbKey;
			return new PkzipClassicDecryptCryptoTransform(this.Key);
		}

		// Token: 0x04000507 RID: 1287
		private byte[] key_;
	}
}
