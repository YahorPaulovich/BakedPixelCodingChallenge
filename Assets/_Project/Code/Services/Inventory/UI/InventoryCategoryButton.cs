using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(Button))]
public sealed class InventoryCategoryButton : MonoBehaviour, IPoolable<InventoryCategory, IMemoryPool>, IDisposable
{
    public class Factory : PlaceholderFactory<InventoryCategory, InventoryCategoryButton>
    {
    }

    private Button _button;
    private IMemoryPool _pool;

    [SerializeField] 
    private TextMeshProUGUI _text;

    public InventoryCategory Category { get; private set; }

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(() => OnClicked?.Invoke());
    }

    public void OnSpawned(InventoryCategory category, IMemoryPool pool)
    {
        _pool = pool;
        _text.text = category.Name;
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

    public event Action OnClicked;
}
