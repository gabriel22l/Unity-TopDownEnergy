using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Transform ogParent;
    private Canvas canvas;
    public InventoryUISlot sourceSlot  { get; private set; }

    [SerializeField] private GameObject dragObject;
    [SerializeField]private CanvasGroup dragCanvasGroup;
    
    public bool IsDragging { get; private set; } = false;
    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
    }
    private void OnDisable()
    {
        ResetDrag();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        IsDragging = true;
        
        sourceSlot = GetComponent<InventoryUISlot>();
        ogParent = dragObject.transform.parent;
        if(canvas != null)
            dragObject.transform.SetParent(canvas.transform);
        dragCanvasGroup.blocksRaycasts = false;
        dragCanvasGroup.alpha = 0.6f;
    }
    public void OnDrag(PointerEventData eventData)
    {
        if(IsDragging) dragObject.transform.position = eventData.position;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if(!IsDragging) return;
        ResetDrag();
        
        if (sourceSlot != null && eventData.pointerEnter == null)
        {
            sourceSlot.OnItemDropOut();
        }
    }
    private void ResetDrag()
    {
        if (!IsDragging) return;
        IsDragging = false;
        
        if (ogParent != null)
        {
            dragObject.transform.SetParent(ogParent);
            dragObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }

        dragCanvasGroup.blocksRaycasts = true;
        dragCanvasGroup.alpha = 1f;
    }
}