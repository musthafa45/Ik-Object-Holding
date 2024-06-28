using UnityEngine;

public class Item : MonoBehaviour, IHoldable {

    private ColliderEdgeDetector edgeDetector;
    private Rigidbody rb;
    [SerializeField] private float throwForce = 2f;

    private void Awake() {
        edgeDetector = GetComponent<ColliderEdgeDetector>();
        rb = GetComponent<Rigidbody>();
    }

    public Vector3 GetLeftHandIkTargetPosition() => edgeDetector.GetLeftHoldPoint();

    public Vector3 GetRightHandIkTargetPosition() => edgeDetector.GetRightHoldPoint();

    public Transform GetTransform() => transform;

    public void SetKinematic(bool active) => rb.isKinematic = active;

    public void SetParent(Transform target,bool resetLocalPos = true) {
        if(target != null) {
            transform.SetParent(target);
            if(resetLocalPos ) {
                transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            }
        }
        else {
            transform.SetParent(null);
        }
      
    }

    public void Throw(Transform objectHoldTransform) {
        SetKinematic(false);
        SetParent(null);
        GetComponent<Collider>().isTrigger = false;
        Vector3 dir = objectHoldTransform.position - transform.position;
        rb.AddForce(dir * throwForce,ForceMode.Impulse);
    }

    public Collider GetCollider() {
        if(TryGetComponent<Collider>(out Collider collider)) {
            return collider;
        }
        else {
            collider = GetComponentInParent<Collider>();
            if(collider != null) {
                return collider;
            }
            else {
                Debug.Log("Object Has not Any Attached Collider");
                return null;
            }
        }
    } 
}
