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

    private void Update() {
        if (Input.GetKeyDown(KeyCode.E)) {
            Interact();
        }
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
