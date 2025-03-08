using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public sealed class InputService : IInputService, PlayerInputActions.IUIActions, IInitializable, IDisposable
{
    public Vector2 Point { get; private set; }
    private PlayerInputActions _input;

    public void OnPoint(InputAction.CallbackContext context)
    {
        Point = context.ReadValue<Vector2>();
    }

    public void Initialize()
    {
        _input = new PlayerInputActions();
        _input.UI.SetCallbacks(this);
        _input.Enable();
    }

    public void Dispose()
    {
        if (_input != null)
        {
            _input.Disable();
            _input.UI.RemoveCallbacks(this);
            _input = null;
        }
    }

    ~InputService()
    {
        Dispose();
    }
}
