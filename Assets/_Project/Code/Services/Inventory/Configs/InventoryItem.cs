using System;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryItem", menuName = "Configs/Inventory/InventoryItem", order = 0)]
public class InventoryItem : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public InventoryCategory Category { get; private set; }
    [field: SerializeField] public string Label { get; private set; }
    [field: SerializeField, TextArea] public string Description { get; private set; }
    [field: SerializeField] public Sprite Image { get; private set; }
    [field: SerializeField, Range(0, 10000000)] public ulong Price { get; private set; }
    [field: SerializeField, Range(0f, 10000000f)] public float Weight { get; private set; }

    [field: SerializeField, Range(0.01f, 2f)]
    private float _localScale = 1;
    public Vector3 LocalScale => new Vector3(_localScale, _localScale, _localScale);

    [SerializeField, Range(1, 10000000)]
    private int _stackSize = 1;

    [SerializeField, Range(1, 10000000)]
    private int _maxStackSize = 64;

    public int StackSize
    {
        get => _stackSize;
        set
        {
            _stackSize = math.min(value, _maxStackSize);
        }
    }

    public int MaxStackSize
    {
        get => _maxStackSize;
        set
        {
            _maxStackSize = value;

            if (_stackSize > _maxStackSize)
            {
                _stackSize = _maxStackSize;
            }
        }
    }

    private void OnValidate()
    {
        MaxStackSize = _maxStackSize;
        StackSize = _stackSize;
    }

    public bool LessThan(InventoryItem other)
    {
        return Name.CompareTo(other.Name) < 0;
    }
}
