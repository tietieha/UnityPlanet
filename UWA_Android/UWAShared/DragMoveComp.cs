using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000002 RID: 2
public class DragMoveComp : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler
{
	// Token: 0x06000001 RID: 1 RVA: 0x00002048 File Offset: 0x00000248
	private void Awake()
	{
		this.rt = base.gameObject.GetComponent<RectTransform>();
		this.srcPos = this.rt.position;
	}

	// Token: 0x06000002 RID: 2 RVA: 0x00002070 File Offset: 0x00000270
	public void OnBeginDrag(PointerEventData eventData)
	{
		this.srcPos = this.rt.position;
		RectTransformUtility.ScreenPointToWorldPointInRectangle(this.rt, eventData.position, eventData.pressEventCamera, ref this.startPos);
	}

	// Token: 0x06000003 RID: 3 RVA: 0x000020B4 File Offset: 0x000002B4
	public void OnDrag(PointerEventData eventData)
	{
		this.SetCanvasPos(eventData);
	}

	// Token: 0x06000004 RID: 4 RVA: 0x000020C0 File Offset: 0x000002C0
	public void OnEndDrag(PointerEventData eventData)
	{
		this.SetCanvasPos(eventData);
	}

	// Token: 0x06000005 RID: 5 RVA: 0x000020CC File Offset: 0x000002CC
	private void SetCanvasPos(PointerEventData eventData)
	{
		Vector3 vector;
		RectTransformUtility.ScreenPointToWorldPointInRectangle(this.rt, eventData.position, eventData.pressEventCamera, ref vector);
		this.rt.position = this.srcPos + vector - this.startPos;
	}

	// Token: 0x04000001 RID: 1
	private RectTransform rt;

	// Token: 0x04000002 RID: 2
	private Vector3 srcPos;

	// Token: 0x04000003 RID: 3
	private Vector3 startPos;
}
