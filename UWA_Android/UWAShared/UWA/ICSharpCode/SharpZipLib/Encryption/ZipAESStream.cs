using System;
using System.IO;
using System.Security.Cryptography;

namespace UWA.ICSharpCode.SharpZipLib.Encryption
{
	// Token: 0x020000B2 RID: 178
	internal class ZipAESStream : CryptoStream
	{
		// Token: 0x06000864 RID: 2148 RVA: 0x0003EF54 File Offset: 0x0003D154
		public ZipAESStream(Stream stream, ZipAESTransform transform, CryptoStreamMode mode) : base(stream, transform, mode)
		{
			this._stream = stream;
			this._transform = transform;
			this._slideBuffer = new byte[1024];
			this._blockAndAuth = 26;
			bool flag = mode > CryptoStreamMode.Read;
			if (flag)
			{
				throw new Exception("ZipAESStream only for read");
			}
		}

		// Token: 0x06000865 RID: 2149 RVA: 0x0003EFB0 File Offset: 0x0003D1B0
		public override int Read(byte[] outBuffer, int offset, int count)
		{
			int i = 0;
			while (i < count)
			{
				int num = this._slideBufFreePos - this._slideBufStartPos;
				int num2 = this._blockAndAuth - num;
				bool flag = this._slideBuffer.Length - this._slideBufFreePos < num2;
				if (flag)
				{
					int num3 = 0;
					int j = this._slideBufStartPos;
					while (j < this._slideBufFreePos)
					{
						this._slideBuffer[num3] = this._slideBuffer[j];
						j++;
						num3++;
					}
					this._slideBufFreePos -= this._slideBufStartPos;
					this._slideBufStartPos = 0;
				}
				int num4 = this._stream.Read(this._slideBuffer, this._slideBufFreePos, num2);
				this._slideBufFreePos += num4;
				num = this._slideBufFreePos - this._slideBufStartPos;
				bool flag2 = num >= this._blockAndAuth;
				if (!flag2)
				{
					bool flag3 = num > 10;
					if (flag3)
					{
						int num5 = num - 10;
						this._transform.TransformBlock(this._slideBuffer, this._slideBufStartPos, num5, outBuffer, offset);
						i += num5;
						this._slideBufStartPos += num5;
					}
					else
					{
						bool flag4 = num < 10;
						if (flag4)
						{
							throw new Exception("Internal error missed auth code");
						}
					}
					byte[] authCode = this._transform.GetAuthCode();
					for (int k = 0; k < 10; k++)
					{
						bool flag5 = authCode[k] != this._slideBuffer[this._slideBufStartPos + k];
						if (flag5)
						{
							throw new Exception("AES Authentication Code does not match. This is a super-CRC check on the data in the file after compression and encryption. \r\nThe file may be damaged.");
						}
					}
					break;
				}
				this._transform.TransformBlock(this._slideBuffer, this._slideBufStartPos, 16, outBuffer, offset);
				i += 16;
				offset += 16;
				this._slideBufStartPos += 16;
			}
			return i;
		}

		// Token: 0x06000866 RID: 2150 RVA: 0x0003F1BC File Offset: 0x0003D3BC
		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}

		// Token: 0x04000508 RID: 1288
		private const int AUTH_CODE_LENGTH = 10;

		// Token: 0x04000509 RID: 1289
		private Stream _stream;

		// Token: 0x0400050A RID: 1290
		private ZipAESTransform _transform;

		// Token: 0x0400050B RID: 1291
		private byte[] _slideBuffer;

		// Token: 0x0400050C RID: 1292
		private int _slideBufStartPos;

		// Token: 0x0400050D RID: 1293
		private int _slideBufFreePos;

		// Token: 0x0400050E RID: 1294
		private const int CRYPTO_BLOCK_SIZE = 16;

		// Token: 0x0400050F RID: 1295
		private int _blockAndAuth;
	}
}
