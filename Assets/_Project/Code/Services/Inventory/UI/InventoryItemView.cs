using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Zenject;

public sealed class InventoryItemView : MonoBehaviour
    , IPoolable<InventoryItem, int, IMemoryPool>, IDisposable
    , IPointerClickHandler
{
    public class Factory : PlaceholderFactory<InventoryItem, int, InventoryItemView>
    {
    }

    private IMemoryPool _pool;

    [SerializeField]
    private Image _iconImage;

    [SerializeField]
    private TextMeshProUGUI _stackSizeText;

    [SerializeField]
    private GameObject _locker;

public void OnSpawned(InventoryItem item, int count, IMemoryPool pool)
{
    _pool = pool;

    if (item != null)
    {
        _iconImage.sprite = item.Image;
        _iconImage.rectTransform.localScale = item.LocalScale;
    }
    if (_iconImage.sprite == null)
    {
        _iconImage.enabled = false;
    }

    if (count > 1)
    {
        _stackSizeText.text = count.ToString();
    }
    else
    {
        _stackSizeText.text = string.Empty;
    }
}

    public void OnDespawned()
    {
        
    }

    public void Dispose()
    {
        if (_pool != null)
        {
            _pool.Despawn(this);
            _pool = null;
        }

        OnClicked = null;
    }

    public void Lock()
    {
        _locker.gameObject.SetActive(true);
    }

    public void Unlock()
    {
        _locker.gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClicked?.Invoke();
    }

    public event Action OnClicked;
}
