using UnityEngine;

public class Item : MonoBehaviour, IHoldable {

    private ColliderEdgeDetector edgeDetector;
    private Rigidbody rb;
    private Collider collider;

    [SerializeField] private float throwForce = 2f;

    private void Awake() {
        edgeDetector = GetComponent<ColliderEdgeDetector>();
        collider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
    }

    public Transform GetLeftHandIkTargetTransform() => edgeDetector.GetLeftHoldPoint();

    public Transform GetRightHandIkTargetTransform() => edgeDetector.GetRightHoldPoint();

    public Transform GetTransform() => transform;

    public void SetKinematic(bool active) => rb.isKinematic = active;

    public void SetParent(Transform target) {
        transform.SetParent(target);
    }

    public void Throw(Transform objectHoldTransform) {
        SetKinematic(false);
        SetParent(null);
        GetCollider().isTrigger = false;
        Vector3 dir = objectHoldTransform.position - transform.position;
        rb.AddForce(dir * throwForce,ForceMode.Impulse);
    }

    public Collider GetCollider() => collider; 
}
