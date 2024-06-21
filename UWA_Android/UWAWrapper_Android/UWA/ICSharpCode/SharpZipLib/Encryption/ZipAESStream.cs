using System;
using System.IO;
using System.Security.Cryptography;

namespace UWA.ICSharpCode.SharpZipLib.Encryption
{
	// Token: 0x020000A3 RID: 163
	internal class ZipAESStream : CryptoStream
	{
		// Token: 0x06000788 RID: 1928 RVA: 0x0003219C File Offset: 0x0003039C
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

		// Token: 0x06000789 RID: 1929 RVA: 0x000321F8 File Offset: 0x000303F8
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

		// Token: 0x0600078A RID: 1930 RVA: 0x00032404 File Offset: 0x00030604
		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}

		// Token: 0x04000495 RID: 1173
		private const int AUTH_CODE_LENGTH = 10;

		// Token: 0x04000496 RID: 1174
		private Stream _stream;

		// Token: 0x04000497 RID: 1175
		private ZipAESTransform _transform;

		// Token: 0x04000498 RID: 1176
		private byte[] _slideBuffer;

		// Token: 0x04000499 RID: 1177
		private int _slideBufStartPos;

		// Token: 0x0400049A RID: 1178
		private int _slideBufFreePos;

		// Token: 0x0400049B RID: 1179
		private const int CRYPTO_BLOCK_SIZE = 16;

		// Token: 0x0400049C RID: 1180
		private int _blockAndAuth;
	}
}
