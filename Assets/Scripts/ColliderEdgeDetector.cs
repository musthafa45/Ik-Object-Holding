using UnityEngine;

public class ColliderEdgeDetector : MonoBehaviour {
    public Color gizmoColor = Color.red;
    public Color rayCastColor = Color.red;

    public bool showRayCast = true;

    public Transform playerTransform; // Reference to the player

    public float rayLength = 2f; // Length of the raycasts
    public LayerMask layerMask;  // LayerMask to filter which layers to consider
    public Vector3 rayOffset = Vector3.zero; // Offset of Ray Origin

    private Transform leftHoldPointTransform;
    private Transform rightHoldPointTransform;

    [Space]

    [SerializeField] private Vector3 leftHoldPointOffset;
    [SerializeField] private Vector3 rightHoldPointOffset;

    private Collider baseCollider;

    [SerializeField] private bool drawGuiGrabPoints = true;

    private void Awake() {

        baseCollider = GetAttachedCollider();

        leftHoldPointTransform = new GameObject("left_Point").transform;
        rightHoldPointTransform = new GameObject("right_point").transform;
        leftHoldPointTransform.parent = transform;
        rightHoldPointTransform.parent = transform;
    }

    private void OnDrawGizmos() {
        if (leftHoldPointTransform != null && rightHoldPointTransform != null) {
            Gizmos.color = gizmoColor;
            Gizmos.DrawSphere(leftHoldPointTransform.position, 0.07f);
            Gizmos.DrawSphere(rightHoldPointTransform.position, 0.07f);
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
        RaycastForHoldPoint(objectCenter, rightDirection, ref leftHoldPointTransform, leftHoldPointOffset);

        // Left grip point
        RaycastForHoldPoint(objectCenter, leftDirection, ref rightHoldPointTransform, rightHoldPointOffset);
    }

    private void RaycastForHoldPoint(Vector3 objectCenter, Vector3 direction, ref Transform holdPointTransform, Vector3 holdPointOffset) {
        Vector3 rayOrigin = objectCenter + direction + rayOffset * rayLength;
        if (Physics.Raycast(rayOrigin, -direction, out RaycastHit hit, rayLength, layerMask)) {
            if (hit.collider == baseCollider) {
                holdPointTransform.position = hit.point + holdPointOffset;
                if (showRayCast) Debug.DrawRay(rayOrigin, -direction * rayLength, rayCastColor);
            } else {
                holdPointTransform.position = objectCenter + direction + holdPointOffset * rayLength;
            }
        } else {
            holdPointTransform.position = objectCenter + direction + holdPointOffset * rayLength;
        }
    }

    public Transform GetLeftHoldPoint() => leftHoldPointTransform;

    public Transform GetRightHoldPoint() => rightHoldPointTransform;

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

        if (leftHoldPointTransform == null || rightHoldPointTransform == null && !drawGuiGrabPoints) return;

        // Create a style for the label
        GUIStyle style = new GUIStyle {
            fontSize = 10,
            normal = { textColor = Color.green }
        };

        // Convert world positions to screen positions
        Vector3 leftScreenPoint = Camera.main.WorldToScreenPoint(leftHoldPointTransform.position);
        Vector3 rightScreenPoint = Camera.main.WorldToScreenPoint(rightHoldPointTransform.position);

        // Draw labels at screen positions
        GUI.Label(new Rect(leftScreenPoint.x, Screen.height - leftScreenPoint.y, 100, 20), "Left", style);
        GUI.Label(new Rect(rightScreenPoint.x, Screen.height - rightScreenPoint.y, 100, 20), "Right", style);
    }
}
