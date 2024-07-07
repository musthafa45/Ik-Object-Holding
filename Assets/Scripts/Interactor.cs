using System;
using UnityEngine;

public class Interactor : MonoBehaviour {
    public static Interactor Instance { get; private set; }

    public event Action<IHoldable> OnHoldableInteracted;

    [SerializeField] private float interactionRadius = 2f;
    [SerializeField] private LayerMask interactionLayer;
    private Collider[] colliders = new Collider[10];

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        InputManager.Instance.OnInteractionKeyPerformed += InputManager_Instance_OnInteractionKeyPerformed;
    }

    private void InputManager_Instance_OnInteractionKeyPerformed(object sender, EventArgs e) {
        Interact();
    }

    private void Interact() {

        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, interactionRadius, colliders, interactionLayer);

        if(numColliders > 0) {
            var go = colliders[0].gameObject;
            if (go.TryGetComponent(out IHoldable holdable) && HasLineOfSight(transform.position,go.transform.position)) {
                OnHoldableInteracted?.Invoke(holdable);
            }
        }
    }

    private bool HasLineOfSight(Vector3 position1, Vector3 position2) {
        // Draw a debug line to visualize the line of sight check
       

        // Perform the linecast
        if (Physics.Linecast(position1, position2, out RaycastHit hit)) {
            // Check if the hit object is not the target
            if (hit.transform.position == position2) {
                //target Inside Player LOS
                Debug.DrawLine(position1, position2, Color.green, 2f);
                return true;
            }
        }
        //target Behind Walls
        Debug.DrawLine(position1, position2, Color.red, 2f);
        return false;
    }
}
