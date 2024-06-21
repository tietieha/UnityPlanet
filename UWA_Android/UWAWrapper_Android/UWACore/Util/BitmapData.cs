using System;
using UnityEngine;

namespace UWACore.Util
{
	// Token: 0x0200002E RID: 46
	public class BitmapData
	{
		// Token: 0x06000206 RID: 518 RVA: 0x0000CC38 File Offset: 0x0000AE38
		public BitmapData(Color32[] _pixels, int _width, int _height)
		{
			this.height = _height;
			this.width = _width;
			this.pixels = _pixels;
		}

		// Token: 0x06000207 RID: 519 RVA: 0x0000CC58 File Offset: 0x0000AE58
		public BitmapData(Texture2D texture)
		{
			this.height = texture.height;
			this.width = texture.width;
			this.pixels = texture.GetPixels32();
		}

		// Token: 0x06000208 RID: 520 RVA: 0x0000CC94 File Offset: 0x0000AE94
		public Color32 getPixelColor32(int x, int y)
		{
			if (x >= this.width)
			{
				x = this.width - 1;
			}
			if (y >= this.height)
			{
				y = this.height - 1;
			}
			if (x < 0)
			{
				x = 0;
			}
			if (y < 0)
			{
				y = 0;
			}
			return this.pixels[y * this.width + x];
		}

		// Token: 0x04000101 RID: 257
		public int height;

		// Token: 0x04000102 RID: 258
		public int width;

		// Token: 0x04000103 RID: 259
		private Color32[] pixels;
	}
}
