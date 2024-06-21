using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Profiling
{
	// Token: 0x0200000A RID: 10
	[UsedByNativeCode]
	internal sealed class Recorder
	{
		// Token: 0x0600006A RID: 106 RVA: 0x000051D0 File Offset: 0x000033D0
		internal Recorder()
		{
		}

		// Token: 0x0600006B RID: 107 RVA: 0x000051D8 File Offset: 0x000033D8
		internal Recorder(IntPtr ptr)
		{
			this.m_Ptr = ptr;
		}

		// Token: 0x0600006C RID: 108 RVA: 0x000051E8 File Offset: 0x000033E8
		~Recorder()
		{
			if (this.m_Ptr != IntPtr.Zero)
			{
				if (Recorder.v20183)
				{
					Recorder.DisposeNative(this.m_Ptr);
				}
				else
				{
					this.DisposeNative();
				}
			}
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00005248 File Offset: 0x00003448
		public static Recorder Get(string samplerName)
		{
			if (Recorder.v20183)
			{
				IntPtr @internal = Recorder.GetInternal(samplerName);
				Recorder recorder;
				if (@internal == IntPtr.Zero)
				{
					recorder = Recorder.s_InvalidRecorder;
				}
				else
				{
					recorder = new Recorder(@internal);
					try
					{
						recorder.FilterToCurrentThread();
						Recorder.allThread = false;
					}
					catch (Exception ex)
					{
					}
				}
				return recorder;
			}
			Sampler sampler = Sampler.Get(samplerName);
			return sampler.GetRecorder();
		}

		// Token: 0x0600006E RID: 110
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetInternal(string samplerName);

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600006F RID: 111 RVA: 0x000052C0 File Offset: 0x000034C0
		public bool isValid
		{
			get
			{
				return this.m_Ptr != IntPtr.Zero;
			}
		}

		// Token: 0x06000070 RID: 112
		[GeneratedByOldBindingsGenerator]
		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void DisposeNative();

		// Token: 0x06000071 RID: 113
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DisposeNative(IntPtr ptr);

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000072 RID: 114 RVA: 0x000052D4 File Offset: 0x000034D4
		// (set) Token: 0x06000073 RID: 115 RVA: 0x000052F0 File Offset: 0x000034F0
		public bool enabledX
		{
			get
			{
				if (Recorder.v20183)
				{
					return this.enabled_v20183;
				}
				return this.enabled;
			}
			set
			{
				if (Recorder.v20183)
				{
					this.enabled_v20183 = value;
					return;
				}
				this.enabled = value;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000074 RID: 116
		// (set) Token: 0x06000075 RID: 117
		[ThreadAndSerializationSafe]
		public extern bool enabled { [GeneratedByOldBindingsGenerator] [MethodImpl(MethodImplOptions.InternalCall)] get; [GeneratedByOldBindingsGenerator] [MethodImpl(MethodImplOptions.InternalCall)] set; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000076 RID: 118 RVA: 0x0000530C File Offset: 0x0000350C
		// (set) Token: 0x06000077 RID: 119 RVA: 0x00005324 File Offset: 0x00003524
		public bool enabled_v20183
		{
			get
			{
				return this.isValid && this.IsEnabled();
			}
			set
			{
				if (this.isValid)
				{
					this.SetEnabled(value);
				}
			}
		}

		// Token: 0x06000078 RID: 120
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool IsEnabled();

		// Token: 0x06000079 RID: 121
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetEnabled(bool enabled);

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600007A RID: 122 RVA: 0x00005338 File Offset: 0x00003538
		public long elapsedNanosecondsX
		{
			get
			{
				if (Recorder.v20183)
				{
					return this.elapsedNanoseconds_v20183;
				}
				return this.elapsedNanoseconds;
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600007B RID: 123
		[ThreadAndSerializationSafe]
		public extern long elapsedNanoseconds { [GeneratedByOldBindingsGenerator] [MethodImpl(MethodImplOptions.InternalCall)] get; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600007C RID: 124 RVA: 0x00005354 File Offset: 0x00003554
		public long elapsedNanoseconds_v20183
		{
			get
			{
				if (this.isValid)
				{
					return this.GetElapsedNanoseconds();
				}
				return 0L;
			}
		}

		// Token: 0x0600007D RID: 125
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern long GetElapsedNanoseconds();

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600007E RID: 126 RVA: 0x0000536C File Offset: 0x0000356C
		public int sampleBlockCountX
		{
			get
			{
				if (Recorder.v20183)
				{
					return this.sampleBlockCount_v20183;
				}
				return this.sampleBlockCount;
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600007F RID: 127
		[ThreadAndSerializationSafe]
		public extern int sampleBlockCount { [GeneratedByOldBindingsGenerator] [MethodImpl(MethodImplOptions.InternalCall)] get; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000080 RID: 128 RVA: 0x00005388 File Offset: 0x00003588
		public int sampleBlockCount_v20183
		{
			get
			{
				if (this.isValid)
				{
					return this.GetSampleBlockCount();
				}
				return 0;
			}
		}

		// Token: 0x06000081 RID: 129
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int GetSampleBlockCount();

		// Token: 0x06000082 RID: 130
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void CollectFromAllThreads();

		// Token: 0x06000083 RID: 131
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void FilterToCurrentThread();

		// Token: 0x04000037 RID: 55
		public static bool v20183 = false;

		// Token: 0x04000038 RID: 56
		public static bool allThread = true;

		// Token: 0x04000039 RID: 57
		internal IntPtr m_Ptr;

		// Token: 0x0400003A RID: 58
		internal static Recorder s_InvalidRecorder = new Recorder();
	}
}
