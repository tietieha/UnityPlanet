using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using UWA.ICSharpCode.SharpZipLib.Encryption;

namespace UWA.ICSharpCode.SharpZipLib.Zip.Compression.Streams
{
	// Token: 0x02000089 RID: 137
	[ComVisible(false)]
	public class DeflaterOutputStream : Stream
	{
		// Token: 0x06000604 RID: 1540 RVA: 0x0002ABE4 File Offset: 0x00028DE4
		public DeflaterOutputStream(Stream baseOutputStream) : this(baseOutputStream, new Deflater(), 512)
		{
		}

		// Token: 0x06000605 RID: 1541 RVA: 0x0002ABFC File Offset: 0x00028DFC
		public DeflaterOutputStream(Stream baseOutputStream, Deflater deflater) : this(baseOutputStream, deflater, 512)
		{
		}

		// Token: 0x06000606 RID: 1542 RVA: 0x0002AC10 File Offset: 0x00028E10
		public DeflaterOutputStream(Stream baseOutputStream, Deflater deflater, int bufferSize)
		{
			bool flag = baseOutputStream == null;
			if (flag)
			{
				throw new ArgumentNullException("baseOutputStream");
			}
			bool flag2 = !baseOutputStream.CanWrite;
			if (flag2)
			{
				throw new ArgumentException("Must support writing", "baseOutputStream");
			}
			bool flag3 = deflater == null;
			if (flag3)
			{
				throw new ArgumentNullException("deflater");
			}
			bool flag4 = bufferSize < 512;
			if (flag4)
			{
				throw new ArgumentOutOfRangeException("bufferSize");
			}
			this.baseOutputStream_ = baseOutputStream;
			this.buffer_ = new byte[bufferSize];
			this.deflater_ = deflater;
		}

		// Token: 0x06000607 RID: 1543 RVA: 0x0002ACB4 File Offset: 0x00028EB4
		public virtual void Finish()
		{
			this.deflater_.Finish();
			while (!this.deflater_.IsFinished)
			{
				int num = this.deflater_.Deflate(this.buffer_, 0, this.buffer_.Length);
				bool flag = num <= 0;
				if (flag)
				{
					break;
				}
				bool flag2 = this.cryptoTransform_ != null;
				if (flag2)
				{
					this.EncryptBlock(this.buffer_, 0, num);
				}
				this.baseOutputStream_.Write(this.buffer_, 0, num);
			}
			bool flag3 = !this.deflater_.IsFinished;
			if (flag3)
			{
				throw new SharpZipBaseException("Can't deflate all input?");
			}
			this.baseOutputStream_.Flush();
			bool flag4 = this.cryptoTransform_ != null;
			if (flag4)
			{
				bool flag5 = this.cryptoTransform_ is ZipAESTransform;
				if (flag5)
				{
					this.AESAuthCode = ((ZipAESTransform)this.cryptoTransform_).GetAuthCode();
				}
				this.cryptoTransform_.Dispose();
				this.cryptoTransform_ = null;
			}
		}

		// Token: 0x170000E0 RID: 224
		// (get) Token: 0x06000608 RID: 1544 RVA: 0x0002ADD4 File Offset: 0x00028FD4
		// (set) Token: 0x06000609 RID: 1545 RVA: 0x0002ADF4 File Offset: 0x00028FF4
		public bool IsStreamOwner
		{
			get
			{
				return this.isStreamOwner_;
			}
			set
			{
				this.isStreamOwner_ = value;
			}
		}

		// Token: 0x170000E1 RID: 225
		// (get) Token: 0x0600060A RID: 1546 RVA: 0x0002AE00 File Offset: 0x00029000
		public bool CanPatchEntries
		{
			get
			{
				return this.baseOutputStream_.CanSeek;
			}
		}

		// Token: 0x170000E2 RID: 226
		// (get) Token: 0x0600060B RID: 1547 RVA: 0x0002AE24 File Offset: 0x00029024
		// (set) Token: 0x0600060C RID: 1548 RVA: 0x0002AE44 File Offset: 0x00029044
		public string Password
		{
			get
			{
				return this.password;
			}
			set
			{
				bool flag = value != null && value.Length == 0;
				if (flag)
				{
					this.password = null;
				}
				else
				{
					this.password = value;
				}
			}
		}

		// Token: 0x0600060D RID: 1549 RVA: 0x0002AE8C File Offset: 0x0002908C
		protected void EncryptBlock(byte[] buffer, int offset, int length)
		{
			this.cryptoTransform_.TransformBlock(buffer, 0, length, buffer, 0);
		}

		// Token: 0x0600060E RID: 1550 RVA: 0x0002AEA0 File Offset: 0x000290A0
		protected void InitializePassword(string password)
		{
			UWA.ICSharpCode.SharpZipLib.Encryption.PkzipClassicManaged pkzipClassicManaged = new UWA.ICSharpCode.SharpZipLib.Encryption.PkzipClassicManaged();
			byte[] rgbKey = UWA.ICSharpCode.SharpZipLib.Encryption.PkzipClassic.GenerateKeys(ZipConstants.ConvertToArray(password));
			this.cryptoTransform_ = pkzipClassicManaged.CreateEncryptor(rgbKey, null);
		}

		// Token: 0x0600060F RID: 1551 RVA: 0x0002AED4 File Offset: 0x000290D4
		protected void InitializeAESPassword(ZipEntry entry, string rawPassword, out byte[] salt, out byte[] pwdVerifier)
		{
			salt = new byte[entry.AESSaltLen];
			bool flag = DeflaterOutputStream._aesRnd == null;
			if (flag)
			{
				DeflaterOutputStream._aesRnd = new RNGCryptoServiceProvider();
			}
			DeflaterOutputStream._aesRnd.GetBytes(salt);
			int blockSize = entry.AESKeySize / 8;
			this.cryptoTransform_ = new ZipAESTransform(rawPassword, salt, blockSize, true);
			pwdVerifier = ((ZipAESTransform)this.cryptoTransform_).PwdVerifier;
		}

		// Token: 0x06000610 RID: 1552 RVA: 0x0002AF48 File Offset: 0x00029148
		protected void Deflate()
		{
			while (!this.deflater_.IsNeedingInput)
			{
				int num = this.deflater_.Deflate(this.buffer_, 0, this.buffer_.Length);
				bool flag = num <= 0;
				if (flag)
				{
					break;
				}
				bool flag2 = this.cryptoTransform_ != null;
				if (flag2)
				{
					this.EncryptBlock(this.buffer_, 0, num);
				}
				this.baseOutputStream_.Write(this.buffer_, 0, num);
			}
			bool flag3 = !this.deflater_.IsNeedingInput;
			if (flag3)
			{
				throw new SharpZipBaseException("DeflaterOutputStream can't deflate all input?");
			}
		}

		// Token: 0x170000E3 RID: 227
		// (get) Token: 0x06000611 RID: 1553 RVA: 0x0002AFFC File Offset: 0x000291FC
		public override bool CanRead
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170000E4 RID: 228
		// (get) Token: 0x06000612 RID: 1554 RVA: 0x0002B018 File Offset: 0x00029218
		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170000E5 RID: 229
		// (get) Token: 0x06000613 RID: 1555 RVA: 0x0002B034 File Offset: 0x00029234
		public override bool CanWrite
		{
			get
			{
				return this.baseOutputStream_.CanWrite;
			}
		}

		// Token: 0x170000E6 RID: 230
		// (get) Token: 0x06000614 RID: 1556 RVA: 0x0002B058 File Offset: 0x00029258
		public override long Length
		{
			get
			{
				return this.baseOutputStream_.Length;
			}
		}

		// Token: 0x170000E7 RID: 231
		// (get) Token: 0x06000615 RID: 1557 RVA: 0x0002B07C File Offset: 0x0002927C
		// (set) Token: 0x06000616 RID: 1558 RVA: 0x0002B0A0 File Offset: 0x000292A0
		public override long Position
		{
			get
			{
				return this.baseOutputStream_.Position;
			}
			set
			{
				throw new NotSupportedException("Position property not supported");
			}
		}

		// Token: 0x06000617 RID: 1559 RVA: 0x0002B0B0 File Offset: 0x000292B0
		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException("DeflaterOutputStream Seek not supported");
		}

		// Token: 0x06000618 RID: 1560 RVA: 0x0002B0C0 File Offset: 0x000292C0
		public override void SetLength(long value)
		{
			throw new NotSupportedException("DeflaterOutputStream SetLength not supported");
		}

		// Token: 0x06000619 RID: 1561 RVA: 0x0002B0D0 File Offset: 0x000292D0
		public override int ReadByte()
		{
			throw new NotSupportedException("DeflaterOutputStream ReadByte not supported");
		}

		// Token: 0x0600061A RID: 1562 RVA: 0x0002B0E0 File Offset: 0x000292E0
		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException("DeflaterOutputStream Read not supported");
		}

		// Token: 0x0600061B RID: 1563 RVA: 0x0002B0F0 File Offset: 0x000292F0
		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			throw new NotSupportedException("DeflaterOutputStream BeginRead not currently supported");
		}

		// Token: 0x0600061C RID: 1564 RVA: 0x0002B100 File Offset: 0x00029300
		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			throw new NotSupportedException("BeginWrite is not supported");
		}

		// Token: 0x0600061D RID: 1565 RVA: 0x0002B110 File Offset: 0x00029310
		public override void Flush()
		{
			this.deflater_.Flush();
			this.Deflate();
			this.baseOutputStream_.Flush();
		}

		// Token: 0x0600061E RID: 1566 RVA: 0x0002B134 File Offset: 0x00029334
		public override void Close()
		{
			bool flag = !this.isClosed_;
			if (flag)
			{
				this.isClosed_ = true;
				try
				{
					this.Finish();
					bool flag2 = this.cryptoTransform_ != null;
					if (flag2)
					{
						this.GetAuthCodeIfAES();
						this.cryptoTransform_.Dispose();
						this.cryptoTransform_ = null;
					}
				}
				finally
				{
					bool flag3 = this.isStreamOwner_;
					if (flag3)
					{
						this.baseOutputStream_.Close();
					}
				}
			}
		}

		// Token: 0x0600061F RID: 1567 RVA: 0x0002B1C4 File Offset: 0x000293C4
		private void GetAuthCodeIfAES()
		{
			bool flag = this.cryptoTransform_ is ZipAESTransform;
			if (flag)
			{
				this.AESAuthCode = ((ZipAESTransform)this.cryptoTransform_).GetAuthCode();
			}
		}

		// Token: 0x06000620 RID: 1568 RVA: 0x0002B204 File Offset: 0x00029404
		public override void WriteByte(byte value)
		{
			this.Write(new byte[]
			{
				value
			}, 0, 1);
		}

		// Token: 0x06000621 RID: 1569 RVA: 0x0002B22C File Offset: 0x0002942C
		public override void Write(byte[] buffer, int offset, int count)
		{
			this.deflater_.SetInput(buffer, offset, count);
			this.Deflate();
		}

		// Token: 0x040003DD RID: 989
		private string password;

		// Token: 0x040003DE RID: 990
		private ICryptoTransform cryptoTransform_;

		// Token: 0x040003DF RID: 991
		protected byte[] AESAuthCode;

		// Token: 0x040003E0 RID: 992
		private byte[] buffer_;

		// Token: 0x040003E1 RID: 993
		protected Deflater deflater_;

		// Token: 0x040003E2 RID: 994
		protected Stream baseOutputStream_;

		// Token: 0x040003E3 RID: 995
		private bool isClosed_;

		// Token: 0x040003E4 RID: 996
		private bool isStreamOwner_ = true;

		// Token: 0x040003E5 RID: 997
		private static RNGCryptoServiceProvider _aesRnd;
	}
}
