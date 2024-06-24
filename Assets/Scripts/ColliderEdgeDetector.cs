using UnityEngine;

public class ColliderEdgeDetector : MonoBehaviour {
    public Color gizmoColor = Color.red;
    public Color rayCastColor = Color.red;
    public bool showRayCast = true;
    public Transform playerTransform; // Reference to the player
    public float rayLength = 2f; // Length of the raycasts
    public LayerMask layerMask; // LayerMask to filter which layers to consider
    private Vector3 leftGripPoint;
    private Vector3 rightGripPoint;

    private void OnDrawGizmos() {
        Collider collider = GetComponent<Collider>();
        if (collider != null && playerTransform != null) {
            CalculateGripPoints(collider);
            Gizmos.color = gizmoColor;
            Gizmos.DrawSphere(leftGripPoint, 0.07f);
            Gizmos.DrawSphere(rightGripPoint, 0.07f);
        }
    }

    private void CalculateGripPoints(Collider collider) {
        Vector3 objectCenter = collider.bounds.center;
        Vector3 directionToPlayer = (playerTransform.position - objectCenter).normalized;

        // Calculate the right and left directions relative to the player
        Vector3 rightDirection = Vector3.Cross(Vector3.up, directionToPlayer).normalized;
        Vector3 leftDirection = -rightDirection;

        // Cast rays from outside the object towards its center
        RaycastHit hit;

        // Right grip point
        if (Physics.Raycast(objectCenter + rightDirection * rayLength, -rightDirection, out hit, rayLength, layerMask)) {
            if (hit.collider == collider) {
                rightGripPoint = hit.point;
                if (showRayCast) Debug.DrawRay(objectCenter + rightDirection * rayLength, -rightDirection * rayLength, rayCastColor);
            }
            else {
                rightGripPoint = objectCenter + rightDirection * rayLength;
            }
        }
        else {
            rightGripPoint = objectCenter + rightDirection * rayLength;
        }

        // Left grip point
        if (Physics.Raycast(objectCenter + leftDirection * rayLength, -leftDirection, out hit, rayLength, layerMask)) {
            if (hit.collider == collider) {
                leftGripPoint = hit.point;
                if (showRayCast) Debug.DrawRay(objectCenter + leftDirection * rayLength, -leftDirection * rayLength, rayCastColor);
            }
            else {
                leftGripPoint = objectCenter + leftDirection * rayLength;
            }
        }
        else {
            leftGripPoint = objectCenter + leftDirection * rayLength;
        }
    }
}
