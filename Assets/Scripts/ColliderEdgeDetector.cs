using UnityEngine;

public class ColliderEdgeDetector : MonoBehaviour {
    public Color gizmoColor = Color.red;
    public Color rayCastColor = Color.red;

    public bool showRayCast = true;

    public Transform playerTransform; // Reference to the player

    public float rayLength = 2f; // Length of the raycasts
    public LayerMask layerMask;  // LayerMask to filter which layers to consider
    public Vector3 rayOffset = Vector3.zero; // Offset of Ray Origin

    [Space]
    private Vector3 leftHoldPoint;
    private Vector3 rightHoldPoint;

    [Space]
    [SerializeField] private Vector3 leftHoldPointOffset;
    [SerializeField] private Vector3 rightHoldPointOffset;

    private Collider baseCollider;

    [SerializeField] private bool drawGuiGrabPoints = true;

    private void Awake() {
        baseCollider = GetAttachedCollider();
    }

    private void OnDrawGizmos() {
        if (leftHoldPoint != Vector3.zero && rightHoldPoint != Vector3.zero) {
            Gizmos.color = gizmoColor;
            Gizmos.DrawSphere(leftHoldPoint, 0.07f);
            Gizmos.DrawSphere(rightHoldPoint, 0.07f);
        }
    }

    private void FixedUpdate() {
        if (baseCollider != null && playerTransform != null) {
            CalculateHoldPoints();
        }
    }

    private void CalculateHoldPoints() {
        Vector3 objectCenter = baseCollider.bounds.center;
        Vector3 directionToPlayer = (playerTransform.position - objectCenter).normalized;

        // Calculate the right and left directions relative to the player
        Vector3 rightDirection = Vector3.Cross(Vector3.up, directionToPlayer).normalized;
        Vector3 leftDirection = -rightDirection;

        // Right grip point
        RaycastForHoldPoint(objectCenter, rightDirection, ref leftHoldPoint, leftHoldPointOffset);

        // Left grip point
        RaycastForHoldPoint(objectCenter, leftDirection, ref rightHoldPoint, rightHoldPointOffset);
    }

    private void RaycastForHoldPoint(Vector3 objectCenter, Vector3 direction, ref Vector3 holdPoint, Vector3 holdPointOffset) {
        Vector3 rayOrigin = objectCenter + direction + rayOffset * rayLength;
        if (Physics.Raycast(rayOrigin, -direction, out RaycastHit hit, rayLength, layerMask)) {
            if (hit.collider == baseCollider) {
                holdPoint = hit.point + holdPointOffset;
                if (showRayCast) Debug.DrawRay(rayOrigin, -direction * rayLength, rayCastColor);
            } else {
                holdPoint = objectCenter + direction + holdPointOffset * rayLength;
            }
        } else {
            holdPoint = objectCenter + direction + holdPointOffset * rayLength;
        }
    }

    public Vector3 GetLeftHoldPoint() => leftHoldPoint;
    public Vector3 GetRightHoldPoint() => rightHoldPoint;

    private Collider GetAttachedCollider() {
        if (TryGetComponent(out Collider collider)) {
            return collider;
        } else {
            collider = GetComponentInParent<Collider>();
            if (collider != null) {
                return collider;
            } else {
                Debug.LogWarning("Object has no attached collider.");
                return null;
            }
        }
    }

    private void OnGUI() {
        if ((leftHoldPoint == Vector3.zero || rightHoldPoint == Vector3.zero) && !drawGuiGrabPoints) return;

        // Create a style for the label
        GUIStyle style = new GUIStyle {
            fontSize = 10,
            normal = { textColor = Color.green }
        };

        // Convert world positions to screen positions
        Vector3 leftScreenPoint = Camera.main.WorldToScreenPoint(leftHoldPoint);
        Vector3 rightScreenPoint = Camera.main.WorldToScreenPoint(rightHoldPoint);

        // Draw labels at screen positions
        GUI.Label(new Rect(leftScreenPoint.x, Screen.height - leftScreenPoint.y, 100, 20), "Left", style);
        GUI.Label(new Rect(rightScreenPoint.x, Screen.height - rightScreenPoint.y, 100, 20), "Right", style);
    }
}
