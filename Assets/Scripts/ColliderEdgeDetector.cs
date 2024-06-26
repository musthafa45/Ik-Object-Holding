using UnityEngine;

public class ColliderEdgeDetector : MonoBehaviour {

    public Color gizmoColor = Color.red;
    public Color rayCastColor = Color.red;

    public bool showRayCast = true;

    public Transform playerTransform; // Reference to the player

    public float rayLength = 2f; // Length of the raycasts
    public LayerMask layerMask;  // LayerMask to filter which layers to consider
    public Vector3 rayOffset = Vector3.zero; //offset of Ray Orgin
    [Space]
    private Vector3 leftHoldPoint;
    private Vector3 rightHoldPoint;
    [Space]

    [SerializeField] private Vector3 leftHoldPointOffset;
    [SerializeField] private Vector3 rightHoldPointOffset;

    private Collider baseCollider;
    private void Awake() {
        baseCollider = GetComponent<Collider>();
    }
    private void OnDrawGizmos() {
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(leftHoldPoint, 0.07f);
        Gizmos.DrawSphere(rightHoldPoint, 0.07f);
    }

    private void FixedUpdate() {

        if (baseCollider != null && playerTransform != null) {
            CalculateHoldPoints(baseCollider);
        }
    }

    private void CalculateHoldPoints(Collider collider) {
        Vector3 objectCenter = collider.bounds.center;
        Vector3 directionToPlayer = (playerTransform.position - objectCenter).normalized;

        // Calculate the right and left directions relative to the player
        Vector3 rightDirection = Vector3.Cross(Vector3.up, directionToPlayer).normalized;
        Vector3 leftDirection = -rightDirection;

        // Cast rays from outside the object towards its center

        // Right grip point
        if (Physics.Raycast(objectCenter + rightDirection + rayOffset * rayLength, -rightDirection, out RaycastHit hit, rayLength, layerMask)) {
            if (hit.collider == collider) {
                leftHoldPoint = hit.point + leftHoldPointOffset;

                if (showRayCast) Debug.DrawRay(objectCenter + rightDirection + rayOffset * rayLength, -rightDirection * rayLength, rayCastColor);
            }
            else {
                leftHoldPoint = objectCenter + rightDirection + leftHoldPointOffset * rayLength;
            }
        }
        else {
            leftHoldPoint = objectCenter + rightDirection + leftHoldPointOffset * rayLength;
        }

        // Left grip point
        if (Physics.Raycast(objectCenter + leftDirection + rayOffset * rayLength, -leftDirection, out hit, rayLength, layerMask)) {
            if (hit.collider == collider) {
                rightHoldPoint = hit.point + rightHoldPointOffset;

                if (showRayCast) Debug.DrawRay(objectCenter + leftDirection + rayOffset * rayLength, -leftDirection * rayLength, rayCastColor);
            }
            else {
                rightHoldPoint = objectCenter + leftDirection + rightHoldPointOffset * rayLength;
            }
        }
        else {
            rightHoldPoint = objectCenter + leftDirection + rightHoldPointOffset * rayLength;
        }
    }

    public Vector3 GetLeftHoldPoint() => leftHoldPoint;

    public Vector3 GetRightHoldPoint() => rightHoldPoint;
}
