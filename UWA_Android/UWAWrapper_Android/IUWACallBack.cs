using System;

// Token: 0x02000003 RID: 3
internal interface IUWACallBack
{
	// Token: 0x06000002 RID: 2
	void Enter(string flag, object inObj, string inString, bool inBool);

	// Token: 0x06000003 RID: 3
	void Leave(string flag, object outObj);
}
