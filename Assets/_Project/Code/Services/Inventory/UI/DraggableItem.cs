using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;
using R3;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public Transform ParentAfterDrag;

    [Inject] private readonly InputService _input;
    private Image _image;
    private RectTransform _root;

    private void Awake()
    {
        _image = GetComponentInChildren<Image>();
    }

    public void SetRoot(RectTransform rect)
    {
        _root = rect;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        var currentSlot = transform.parent.GetComponent<InventorySlot>();
        if (currentSlot != null && currentSlot.IsLocked)
        {
            Debug.LogWarning("Cannot drag item from a locked slot.");
            return;
        }

        SetMaskableAndActive(false);

        ParentAfterDrag = transform.parent;
        transform.SetParent(_root);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = _input.Point;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        SetMaskableAndActive(true);

        transform.SetParent(ParentAfterDrag);
        OnItemMoved?.Invoke(ParentAfterDrag.GetSiblingIndex());
    }

    private void SetMaskableAndActive(bool value)
    {
        _image.raycastTarget = value;
        _image.maskable = value;

        _image.enabled = false;
        _image.enabled = true;
    }

    public event Action<int> OnItemMoved;

    public Observable<int> OnItemMovedAsObservable()
    {
        return Observable.FromEvent<int>(
            handler => OnItemMoved += handler,
            handler => OnItemMoved -= handler
        );
    }
}