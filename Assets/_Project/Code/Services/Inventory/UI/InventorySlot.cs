using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0)
        {
            GameObject dropped = eventData.pointerDrag;
            if (dropped.TryGetComponent<DraggableItem>(out var draggableItem))
            {
                draggableItem.ParentAfterDrag = transform;
            }  
        }
    }
}
