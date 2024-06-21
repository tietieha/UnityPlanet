using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace UWA.ICSharpCode.SharpZipLib.Encryption
{
	// Token: 0x020000A2 RID: 162
	[ComVisible(false)]
	public sealed class PkzipClassicManaged : PkzipClassic
	{
		// Token: 0x17000140 RID: 320
		// (get) Token: 0x0600077D RID: 1917 RVA: 0x00031FB8 File Offset: 0x000301B8
		// (set) Token: 0x0600077E RID: 1918 RVA: 0x00031FD4 File Offset: 0x000301D4
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

		// Token: 0x17000141 RID: 321
		// (get) Token: 0x0600077F RID: 1919 RVA: 0x00032000 File Offset: 0x00030200
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

		// Token: 0x06000780 RID: 1920 RVA: 0x00032034 File Offset: 0x00030234
		public override void GenerateIV()
		{
		}

		// Token: 0x17000142 RID: 322
		// (get) Token: 0x06000781 RID: 1921 RVA: 0x00032038 File Offset: 0x00030238
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

		// Token: 0x17000143 RID: 323
		// (get) Token: 0x06000782 RID: 1922 RVA: 0x00032068 File Offset: 0x00030268
		// (set) Token: 0x06000783 RID: 1923 RVA: 0x000320AC File Offset: 0x000302AC
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

		// Token: 0x06000784 RID: 1924 RVA: 0x00032104 File Offset: 0x00030304
		public override void GenerateKey()
		{
			this.key_ = new byte[12];
			Random random = new Random();
			random.NextBytes(this.key_);
		}

		// Token: 0x06000785 RID: 1925 RVA: 0x00032138 File Offset: 0x00030338
		public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
		{
			this.key_ = rgbKey;
			return new PkzipClassicEncryptCryptoTransform(this.Key);
		}

		// Token: 0x06000786 RID: 1926 RVA: 0x00032164 File Offset: 0x00030364
		public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV)
		{
			this.key_ = rgbKey;
			return new PkzipClassicDecryptCryptoTransform(this.Key);
		}

		// Token: 0x04000494 RID: 1172
		private byte[] key_;
	}
}
