using System;
using System.Security.Cryptography;

namespace UWA.ICSharpCode.SharpZipLib.Encryption
{
	// Token: 0x020000A4 RID: 164
	internal class ZipAESTransform : ICryptoTransform, IDisposable
	{
		// Token: 0x0600078B RID: 1931 RVA: 0x0003240C File Offset: 0x0003060C
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

		// Token: 0x0600078C RID: 1932 RVA: 0x00032534 File Offset: 0x00030734
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

		// Token: 0x17000144 RID: 324
		// (get) Token: 0x0600078D RID: 1933 RVA: 0x00032644 File Offset: 0x00030844
		public byte[] PwdVerifier
		{
			get
			{
				return this._pwdVerifier;
			}
		}

		// Token: 0x0600078E RID: 1934 RVA: 0x00032664 File Offset: 0x00030864
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

		// Token: 0x0600078F RID: 1935 RVA: 0x000326B8 File Offset: 0x000308B8
		public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
		{
			throw new NotImplementedException("ZipAESTransform.TransformFinalBlock");
		}

		// Token: 0x17000145 RID: 325
		// (get) Token: 0x06000790 RID: 1936 RVA: 0x000326C8 File Offset: 0x000308C8
		public int InputBlockSize
		{
			get
			{
				return this._blockSize;
			}
		}

		// Token: 0x17000146 RID: 326
		// (get) Token: 0x06000791 RID: 1937 RVA: 0x000326E8 File Offset: 0x000308E8
		public int OutputBlockSize
		{
			get
			{
				return this._blockSize;
			}
		}

		// Token: 0x17000147 RID: 327
		// (get) Token: 0x06000792 RID: 1938 RVA: 0x00032708 File Offset: 0x00030908
		public bool CanTransformMultipleBlocks
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000148 RID: 328
		// (get) Token: 0x06000793 RID: 1939 RVA: 0x00032724 File Offset: 0x00030924
		public bool CanReuseTransform
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06000794 RID: 1940 RVA: 0x00032740 File Offset: 0x00030940
		public void Dispose()
		{
			this._encryptor.Dispose();
		}

		// Token: 0x0400049D RID: 1181
		private const int PWD_VER_LENGTH = 2;

		// Token: 0x0400049E RID: 1182
		private const int KEY_ROUNDS = 1000;

		// Token: 0x0400049F RID: 1183
		private const int ENCRYPT_BLOCK = 16;

		// Token: 0x040004A0 RID: 1184
		private int _blockSize;

		// Token: 0x040004A1 RID: 1185
		private ICryptoTransform _encryptor;

		// Token: 0x040004A2 RID: 1186
		private readonly byte[] _counterNonce;

		// Token: 0x040004A3 RID: 1187
		private byte[] _encryptBuffer;

		// Token: 0x040004A4 RID: 1188
		private int _encrPos;

		// Token: 0x040004A5 RID: 1189
		private byte[] _pwdVerifier;

		// Token: 0x040004A6 RID: 1190
		private HMACSHA1 _hmacsha1;

		// Token: 0x040004A7 RID: 1191
		private bool _finalised;

		// Token: 0x040004A8 RID: 1192
		private bool _writeMode;
	}
}
