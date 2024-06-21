using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Profiling
{
	// Token: 0x0200000B RID: 11
	[UsedByNativeCode]
	internal class Sampler
	{
		// Token: 0x06000085 RID: 133 RVA: 0x000053B8 File Offset: 0x000035B8
		internal Sampler()
		{
		}

		// Token: 0x06000086 RID: 134 RVA: 0x000053C0 File Offset: 0x000035C0
		internal Sampler(IntPtr ptr)
		{
			this.m_Ptr = ptr;
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000087 RID: 135 RVA: 0x000053D0 File Offset: 0x000035D0
		public bool isValid
		{
			get
			{
				return this.m_Ptr != IntPtr.Zero;
			}
		}

		// Token: 0x06000088 RID: 136 RVA: 0x000053E4 File Offset: 0x000035E4
		internal Recorder GetRecorder()
		{
			if (Recorder.v20183)
			{
				IntPtr recorderInternal = Sampler.GetRecorderInternal(this.m_Ptr);
				Recorder result;
				if (recorderInternal == IntPtr.Zero)
				{
					result = Recorder.s_InvalidRecorder;
				}
				else
				{
					result = new Recorder(recorderInternal);
				}
				return result;
			}
			Recorder recorderInternal2 = this.GetRecorderInternal();
			return recorderInternal2 ?? Recorder.s_InvalidRecorder;
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00005444 File Offset: 0x00003644
		public static Sampler Get(string name)
		{
			Sampler samplerInternal = Sampler.GetSamplerInternal(name);
			return samplerInternal ?? Sampler.s_InvalidSampler;
		}

		// Token: 0x0600008A RID: 138 RVA: 0x0000546C File Offset: 0x0000366C
		public static int GetNames(List<string> names)
		{
			return Sampler.GetSamplerNamesInternal(names);
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600008B RID: 139
		[ThreadAndSerializationSafe]
		public extern string name { [GeneratedByOldBindingsGenerator] [MethodImpl(MethodImplOptions.InternalCall)] get; }

		// Token: 0x0600008C RID: 140
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern string GetSamplerName();

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600008D RID: 141 RVA: 0x00005474 File Offset: 0x00003674
		public string name_v20183
		{
			get
			{
				if (this.isValid)
				{
					return this.GetSamplerName();
				}
				return null;
			}
		}

		// Token: 0x0600008E RID: 142
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Recorder GetRecorderInternal();

		// Token: 0x0600008F RID: 143
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetRecorderInternal(IntPtr ptr);

		// Token: 0x06000090 RID: 144
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Sampler GetSamplerInternal(string name);

		// Token: 0x06000091 RID: 145
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetSamplerNamesInternal(object namesScriptingPtr);

		// Token: 0x0400003B RID: 59
		internal IntPtr m_Ptr;

		// Token: 0x0400003C RID: 60
		internal static Sampler s_InvalidSampler = new Sampler();
	}
}
