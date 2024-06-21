using System;
using System.Security.Cryptography;

namespace UWA.ICSharpCode.SharpZipLib.Encryption
{
	// Token: 0x020000B3 RID: 179
	internal class ZipAESTransform : ICryptoTransform, IDisposable
	{
		// Token: 0x06000867 RID: 2151 RVA: 0x0003F1C4 File Offset: 0x0003D3C4
		public ZipAESTransform(string key, byte[] saltBytes, int blockSize, bool writeMode)
		{
			bool flag = blockSize != 16 && blockSize != 32;
			if (flag)
			{
				throw new Exception("Invalid blocksize " + blockSize.ToString() + ". Must be 16 or 32.");
			}
			bool flag2 = saltBytes.Length != blockSize / 2;
			if (flag2)
			{
				throw new Exception("Invalid salt len. Must be " + (blockSize / 2).ToString() + " for blocksize " + blockSize.ToString());
			}
			this._blockSize = blockSize;
			this._encryptBuffer = new byte[this._blockSize];
			this._encrPos = 16;
			Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(key, saltBytes, 1000);
			RijndaelManaged rijndaelManaged = new RijndaelManaged();
			rijndaelManaged.Mode = CipherMode.ECB;
			this._counterNonce = new byte[this._blockSize];
			byte[] bytes = rfc2898DeriveBytes.GetBytes(this._blockSize);
			byte[] bytes2 = rfc2898DeriveBytes.GetBytes(this._blockSize);
			this._encryptor = rijndaelManaged.CreateEncryptor(bytes, bytes2);
			this._pwdVerifier = rfc2898DeriveBytes.GetBytes(2);
			this._hmacsha1 = new HMACSHA1(bytes2);
			this._writeMode = writeMode;
		}

		// Token: 0x06000868 RID: 2152 RVA: 0x0003F2EC File Offset: 0x0003D4EC
		public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
		{
			bool flag = !this._writeMode;
			if (flag)
			{
				this._hmacsha1.TransformBlock(inputBuffer, inputOffset, inputCount, inputBuffer, inputOffset);
			}
			for (int i = 0; i < inputCount; i++)
			{
				bool flag2 = this._encrPos == 16;
				if (flag2)
				{
					int num = 0;
					for (;;)
					{
						byte[] counterNonce = this._counterNonce;
						int num2 = num;
						byte b = counterNonce[num2] + 1;
						counterNonce[num2] = b;
						if (b != 0)
						{
							break;
						}
						num++;
					}
					this._encryptor.TransformBlock(this._counterNonce, 0, this._blockSize, this._encryptBuffer, 0);
					this._encrPos = 0;
				}
				int num3 = i + outputOffset;
				byte b2 = inputBuffer[i + inputOffset];
				byte[] encryptBuffer = this._encryptBuffer;
				int encrPos = this._encrPos;
				this._encrPos = encrPos + 1;
				outputBuffer[num3] = (b2 ^ encryptBuffer[encrPos]);
			}
			bool writeMode = this._writeMode;
			if (writeMode)
			{
				this._hmacsha1.TransformBlock(outputBuffer, outputOffset, inputCount, outputBuffer, outputOffset);
			}
			return inputCount;
		}

		// Token: 0x17000198 RID: 408
		// (get) Token: 0x06000869 RID: 2153 RVA: 0x0003F3FC File Offset: 0x0003D5FC
		public byte[] PwdVerifier
		{
			get
			{
				return this._pwdVerifier;
			}
		}

		// Token: 0x0600086A RID: 2154 RVA: 0x0003F41C File Offset: 0x0003D61C
		public byte[] GetAuthCode()
		{
			bool flag = !this._finalised;
			if (flag)
			{
				byte[] inputBuffer = new byte[0];
				this._hmacsha1.TransformFinalBlock(inputBuffer, 0, 0);
				this._finalised = true;
			}
			return this._hmacsha1.Hash;
		}

		// Token: 0x0600086B RID: 2155 RVA: 0x0003F470 File Offset: 0x0003D670
		public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
		{
			throw new NotImplementedException("ZipAESTransform.TransformFinalBlock");
		}

		// Token: 0x17000199 RID: 409
		// (get) Token: 0x0600086C RID: 2156 RVA: 0x0003F480 File Offset: 0x0003D680
		public int InputBlockSize
		{
			get
			{
				return this._blockSize;
			}
		}

		// Token: 0x1700019A RID: 410
		// (get) Token: 0x0600086D RID: 2157 RVA: 0x0003F4A0 File Offset: 0x0003D6A0
		public int OutputBlockSize
		{
			get
			{
				return this._blockSize;
			}
		}

		// Token: 0x1700019B RID: 411
		// (get) Token: 0x0600086E RID: 2158 RVA: 0x0003F4C0 File Offset: 0x0003D6C0
		public bool CanTransformMultipleBlocks
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700019C RID: 412
		// (get) Token: 0x0600086F RID: 2159 RVA: 0x0003F4DC File Offset: 0x0003D6DC
		public bool CanReuseTransform
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06000870 RID: 2160 RVA: 0x0003F4F8 File Offset: 0x0003D6F8
		public void Dispose()
		{
			this._encryptor.Dispose();
		}

		// Token: 0x04000510 RID: 1296
		private const int PWD_VER_LENGTH = 2;

		// Token: 0x04000511 RID: 1297
		private const int KEY_ROUNDS = 1000;

		// Token: 0x04000512 RID: 1298
		private const int ENCRYPT_BLOCK = 16;

		// Token: 0x04000513 RID: 1299
		private int _blockSize;

		// Token: 0x04000514 RID: 1300
		private ICryptoTransform _encryptor;

		// Token: 0x04000515 RID: 1301
		private readonly byte[] _counterNonce;

		// Token: 0x04000516 RID: 1302
		private byte[] _encryptBuffer;

		// Token: 0x04000517 RID: 1303
		private int _encrPos;

		// Token: 0x04000518 RID: 1304
		private byte[] _pwdVerifier;

		// Token: 0x04000519 RID: 1305
		private HMACSHA1 _hmacsha1;

		// Token: 0x0400051A RID: 1306
		private bool _finalised;

		// Token: 0x0400051B RID: 1307
		private bool _writeMode;
	}
}
