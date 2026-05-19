using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private CanvasGroup canvasGroup;
    private Transform ogParent;
    private Canvas canvas;
    public InventoryUISlot sourceSlot  { get; private set; }
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        sourceSlot = GetComponentInParent<InventoryUISlot>();
        ogParent = transform.parent;
        if(canvas != null)transform.SetParent(canvas.transform);
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;
    }
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(ogParent);
        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1;
        
        if (sourceSlot != null && eventData.pointerEnter == null)
        {
            sourceSlot.OnItemDropOut();
        }
    }
}