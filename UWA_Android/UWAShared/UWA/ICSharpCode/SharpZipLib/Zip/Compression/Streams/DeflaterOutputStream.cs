using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using UWA.ICSharpCode.SharpZipLib.Encryption;

namespace UWA.ICSharpCode.SharpZipLib.Zip.Compression.Streams
{
	// Token: 0x02000098 RID: 152
	[ComVisible(false)]
	public class DeflaterOutputStream : Stream
	{
		// Token: 0x060006E0 RID: 1760 RVA: 0x0003799C File Offset: 0x00035B9C
		public DeflaterOutputStream(Stream baseOutputStream) : this(baseOutputStream, new Deflater(), 512)
		{
		}

		// Token: 0x060006E1 RID: 1761 RVA: 0x000379B4 File Offset: 0x00035BB4
		public DeflaterOutputStream(Stream baseOutputStream, Deflater deflater) : this(baseOutputStream, deflater, 512)
		{
		}

		// Token: 0x060006E2 RID: 1762 RVA: 0x000379C8 File Offset: 0x00035BC8
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

		// Token: 0x060006E3 RID: 1763 RVA: 0x00037A6C File Offset: 0x00035C6C
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

		// Token: 0x17000134 RID: 308
		// (get) Token: 0x060006E4 RID: 1764 RVA: 0x00037B8C File Offset: 0x00035D8C
		// (set) Token: 0x060006E5 RID: 1765 RVA: 0x00037BAC File Offset: 0x00035DAC
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

		// Token: 0x17000135 RID: 309
		// (get) Token: 0x060006E6 RID: 1766 RVA: 0x00037BB8 File Offset: 0x00035DB8
		public bool CanPatchEntries
		{
			get
			{
				return this.baseOutputStream_.CanSeek;
			}
		}

		// Token: 0x17000136 RID: 310
		// (get) Token: 0x060006E7 RID: 1767 RVA: 0x00037BDC File Offset: 0x00035DDC
		// (set) Token: 0x060006E8 RID: 1768 RVA: 0x00037BFC File Offset: 0x00035DFC
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

		// Token: 0x060006E9 RID: 1769 RVA: 0x00037C44 File Offset: 0x00035E44
		protected void EncryptBlock(byte[] buffer, int offset, int length)
		{
			this.cryptoTransform_.TransformBlock(buffer, 0, length, buffer, 0);
		}

		// Token: 0x060006EA RID: 1770 RVA: 0x00037C58 File Offset: 0x00035E58
		protected void InitializePassword(string password)
		{
			PkzipClassicManaged pkzipClassicManaged = new PkzipClassicManaged();
			byte[] rgbKey = PkzipClassic.GenerateKeys(ZipConstants.ConvertToArray(password));
			this.cryptoTransform_ = pkzipClassicManaged.CreateEncryptor(rgbKey, null);
		}

		// Token: 0x060006EB RID: 1771 RVA: 0x00037C8C File Offset: 0x00035E8C
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

		// Token: 0x060006EC RID: 1772 RVA: 0x00037D00 File Offset: 0x00035F00
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

		// Token: 0x17000137 RID: 311
		// (get) Token: 0x060006ED RID: 1773 RVA: 0x00037DB4 File Offset: 0x00035FB4
		public override bool CanRead
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000138 RID: 312
		// (get) Token: 0x060006EE RID: 1774 RVA: 0x00037DD0 File Offset: 0x00035FD0
		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000139 RID: 313
		// (get) Token: 0x060006EF RID: 1775 RVA: 0x00037DEC File Offset: 0x00035FEC
		public override bool CanWrite
		{
			get
			{
				return this.baseOutputStream_.CanWrite;
			}
		}

		// Token: 0x1700013A RID: 314
		// (get) Token: 0x060006F0 RID: 1776 RVA: 0x00037E10 File Offset: 0x00036010
		public override long Length
		{
			get
			{
				return this.baseOutputStream_.Length;
			}
		}

		// Token: 0x1700013B RID: 315
		// (get) Token: 0x060006F1 RID: 1777 RVA: 0x00037E34 File Offset: 0x00036034
		// (set) Token: 0x060006F2 RID: 1778 RVA: 0x00037E58 File Offset: 0x00036058
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

		// Token: 0x060006F3 RID: 1779 RVA: 0x00037E68 File Offset: 0x00036068
		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException("DeflaterOutputStream Seek not supported");
		}

		// Token: 0x060006F4 RID: 1780 RVA: 0x00037E78 File Offset: 0x00036078
		public override void SetLength(long value)
		{
			throw new NotSupportedException("DeflaterOutputStream SetLength not supported");
		}

		// Token: 0x060006F5 RID: 1781 RVA: 0x00037E88 File Offset: 0x00036088
		public override int ReadByte()
		{
			throw new NotSupportedException("DeflaterOutputStream ReadByte not supported");
		}

		// Token: 0x060006F6 RID: 1782 RVA: 0x00037E98 File Offset: 0x00036098
		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException("DeflaterOutputStream Read not supported");
		}

		// Token: 0x060006F7 RID: 1783 RVA: 0x00037EA8 File Offset: 0x000360A8
		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			throw new NotSupportedException("DeflaterOutputStream BeginRead not currently supported");
		}

		// Token: 0x060006F8 RID: 1784 RVA: 0x00037EB8 File Offset: 0x000360B8
		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			throw new NotSupportedException("BeginWrite is not supported");
		}

		// Token: 0x060006F9 RID: 1785 RVA: 0x00037EC8 File Offset: 0x000360C8
		public override void Flush()
		{
			this.deflater_.Flush();
			this.Deflate();
			this.baseOutputStream_.Flush();
		}

		// Token: 0x060006FA RID: 1786 RVA: 0x00037EEC File Offset: 0x000360EC
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

		// Token: 0x060006FB RID: 1787 RVA: 0x00037F7C File Offset: 0x0003617C
		private void GetAuthCodeIfAES()
		{
			bool flag = this.cryptoTransform_ is ZipAESTransform;
			if (flag)
			{
				this.AESAuthCode = ((ZipAESTransform)this.cryptoTransform_).GetAuthCode();
			}
		}

		// Token: 0x060006FC RID: 1788 RVA: 0x00037FBC File Offset: 0x000361BC
		public override void WriteByte(byte value)
		{
			this.Write(new byte[]
			{
				value
			}, 0, 1);
		}

		// Token: 0x060006FD RID: 1789 RVA: 0x00037FE4 File Offset: 0x000361E4
		public override void Write(byte[] buffer, int offset, int count)
		{
			this.deflater_.SetInput(buffer, offset, count);
			this.Deflate();
		}

		// Token: 0x04000450 RID: 1104
		private string password;

		// Token: 0x04000451 RID: 1105
		private ICryptoTransform cryptoTransform_;

		// Token: 0x04000452 RID: 1106
		protected byte[] AESAuthCode;

		// Token: 0x04000453 RID: 1107
		private byte[] buffer_;

		// Token: 0x04000454 RID: 1108
		protected Deflater deflater_;

		// Token: 0x04000455 RID: 1109
		protected Stream baseOutputStream_;

		// Token: 0x04000456 RID: 1110
		private bool isClosed_;

		// Token: 0x04000457 RID: 1111
		private bool isStreamOwner_ = true;

		// Token: 0x04000458 RID: 1112
		private static RNGCryptoServiceProvider _aesRnd;
	}
}
