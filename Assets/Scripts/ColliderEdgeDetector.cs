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

    [SerializeField] private bool DRAW_GUI_GRABPOINTS = true;

    private void Awake() {
        baseCollider = GetCollider();
    }

    private void OnDrawGizmos() {
        if (leftHoldPoint == null || rightHoldPoint == null) return;

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

                if (showRayCast)
                    Debug.DrawRay(objectCenter + rightDirection + rayOffset * rayLength, -rightDirection * rayLength, rayCastColor);
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

                if (showRayCast)
                    Debug.DrawRay(objectCenter + leftDirection + rayOffset * rayLength, -leftDirection * rayLength, rayCastColor);
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

    private Collider GetCollider() {
        if (TryGetComponent(out Collider collider)) {
            return collider;
        }
        else {
            collider = GetComponentInParent<Collider>();
            if (collider != null) {
                return collider;
            }
            else {
                Debug.Log("Object Has not Any Attached Collider");
                return null;
            }
        }
    }

    private void OnGUI() {
        if (leftHoldPoint == null || rightHoldPoint == null && !DRAW_GUI_GRABPOINTS) return;

        // Create a style for the label
        GUIStyle style = new GUIStyle();
        style.fontSize = 10;
        style.normal.textColor = Color.green;

        // Convert world positions to screen positions
        Vector3 leftScreenPoint = Camera.main.WorldToScreenPoint(leftHoldPoint);
        Vector3 rightScreenPoint = Camera.main.WorldToScreenPoint(rightHoldPoint);

        // Draw labels at screen positions
        GUI.Label(new Rect(leftScreenPoint.x, Screen.height - leftScreenPoint.y, 100, 20), "Left", style);
        GUI.Label(new Rect(rightScreenPoint.x, Screen.height - rightScreenPoint.y, 100, 20), "Right", style);
    }

}
