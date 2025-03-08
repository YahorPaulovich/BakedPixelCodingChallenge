using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class InventorySlot : MonoBehaviour
    , IPoolable<IMemoryPool>, IDisposable
    , IDropHandler
{
    public class Factory : PlaceholderFactory<InventorySlot>
    {
    }

    private IMemoryPool _pool;

    [SerializeField] private GameObject _locker;

    public bool IsLocked { get; private set; } = false;

    public void OnSpawned(IMemoryPool pool)
    {
        _pool = pool;
    }

    public void OnDespawned()
    {
        if (transform.childCount > 0)
        {
            Destroy(transform.GetChild(0).gameObject);
        }
    }

    public void Dispose()
    {
        if (_pool != null)
        {
            _pool.Despawn(this);
            _pool = null;
        }
    }

    public void Lock()
    {
        IsLocked = true;
        _locker.gameObject.SetActive(true);
    }

    public void Unlock()
    {
        IsLocked = false;
        _locker.gameObject.SetActive(false);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (IsLocked)
        {
            Debug.LogWarning("Cannot drop item into a locked slot.");
            return;
        }

        if (transform.childCount <= 1)
        {
            GameObject dropped = eventData.pointerDrag;
            if (dropped.TryGetComponent<DraggableItem>(out var draggableItem))
            {
                draggableItem.ParentAfterDrag = transform;
            }
        }
    }
}