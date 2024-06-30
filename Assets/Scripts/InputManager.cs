using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public event EventHandler OnInteractionKeyPerformed;
    public event EventHandler OnDropItemKeyPerformed;
    public event EventHandler OnThrowKeyPerformed;

    private void Awake() {
        Instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) {
            OnInteractionKeyPerformed?.Invoke(this, EventArgs.Empty);
        }

        if (Input.GetKeyDown(KeyCode.G)) {
            OnThrowKeyPerformed?.Invoke(this, EventArgs.Empty);
        }

        if (Input.GetKeyDown(KeyCode.X)) {
            OnDropItemKeyPerformed?.Invoke(this, EventArgs.Empty);
        }
    }
}
