using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace UWACore.Util
{
	// Token: 0x02000046 RID: 70
	[ComVisible(false)]
	public class BitmapData
	{
		// Token: 0x06000316 RID: 790 RVA: 0x00014894 File Offset: 0x00012A94
		public BitmapData(Color32[] _pixels, int _width, int _height)
		{
			this.height = _height;
			this.width = _width;
			this.pixels = _pixels;
		}

		// Token: 0x06000317 RID: 791 RVA: 0x000148B4 File Offset: 0x00012AB4
		public BitmapData(Texture2D texture)
		{
			this.height = texture.height;
			this.width = texture.width;
			this.pixels = texture.GetPixels32();
		}

		// Token: 0x06000318 RID: 792 RVA: 0x000148F4 File Offset: 0x00012AF4
		public Color32 getPixelColor32(int x, int y)
		{
			bool flag = x >= this.width;
			if (flag)
			{
				x = this.width - 1;
			}
			bool flag2 = y >= this.height;
			if (flag2)
			{
				y = this.height - 1;
			}
			bool flag3 = x < 0;
			if (flag3)
			{
				x = 0;
			}
			bool flag4 = y < 0;
			if (flag4)
			{
				y = 0;
			}
			return this.pixels[y * this.width + x];
		}

		// Token: 0x040001B0 RID: 432
		public int height;

		// Token: 0x040001B1 RID: 433
		public int width;

		// Token: 0x040001B2 RID: 434
		private Color32[] pixels;
	}
}
