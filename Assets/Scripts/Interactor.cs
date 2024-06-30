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

        if(numColliders > 0 ) {

            if (colliders[0].gameObject.TryGetComponent(out IHoldable holdable)) {
                OnHoldableInteracted(holdable);
            }
        }
    }


}
