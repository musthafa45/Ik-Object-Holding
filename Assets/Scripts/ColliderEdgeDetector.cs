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
    private Transform leftHoldPointTransform;
    private Transform rightHoldPointTransform;
    [Space]

    [SerializeField] private Vector3 leftHoldPointOffset;
    [SerializeField] private Vector3 rightHoldPointOffset;

    private IHoldable holdable = null;
    private Collider baseCollider;
    private bool canDoFixedHoldPoint = false;
    private void Awake() {
        baseCollider = GetCollider();
        holdable = GetComponent<IHoldable>();

        leftHoldPointTransform = new GameObject("left_Point").transform;
        rightHoldPointTransform = new GameObject("right_point").transform;
        leftHoldPointTransform.parent = transform;
        rightHoldPointTransform.parent = transform;
    }

    private void Start() {
        PlayerIK.Instance.OntemPicked += PlayerIk_Instance_OnObjectParentChanged;
    }

    private void PlayerIk_Instance_OnObjectParentChanged(IHoldable holdable) {
        if(holdable == null) {
            canDoFixedHoldPoint = false;
        }
    }

    private void PlayerIk_Instance_OnPlayerStabled(IHoldable holdable) {
        if (holdable != null && holdable == GetComponent<IHoldable>()) {
            canDoFixedHoldPoint = true;
        }
    }

    private void OnDrawGizmos() {
        if (leftHoldPointTransform == null || rightHoldPointTransform == null) return;

        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(leftHoldPointTransform.position, 0.07f);
        Gizmos.DrawSphere(rightHoldPointTransform.position, 0.07f);
    }

    private void Update() {
        canDoFixedHoldPoint = PlayerIK.Instance.IsHoldItemStabled() && PlayerIK.Instance.GetHoldItem() == holdable;
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

                if (!canDoFixedHoldPoint)
                    leftHoldPointTransform.position = hit.point + leftHoldPointOffset;


                if (showRayCast)
                    Debug.DrawRay(objectCenter + rightDirection + rayOffset * rayLength, -rightDirection * rayLength, rayCastColor);
            }
            else {
                if (!canDoFixedHoldPoint)
                    leftHoldPointTransform.position = objectCenter + rightDirection + leftHoldPointOffset * rayLength;


            }
        }
        else {
            if (!canDoFixedHoldPoint)
                leftHoldPointTransform.position = objectCenter + rightDirection + leftHoldPointOffset * rayLength;
        }

        // Left grip point
        if (Physics.Raycast(objectCenter + leftDirection + rayOffset * rayLength, -leftDirection, out hit, rayLength, layerMask)) {
            if (hit.collider == collider) {

                if (!canDoFixedHoldPoint)
                    rightHoldPointTransform.position = hit.point + rightHoldPointOffset;

                if (showRayCast)
                    Debug.DrawRay(objectCenter + leftDirection + rayOffset * rayLength, -leftDirection * rayLength, rayCastColor);
            }
            else {
                if (!canDoFixedHoldPoint)
                    rightHoldPointTransform.position = objectCenter + leftDirection + rightHoldPointOffset * rayLength;
            }
        }
        else {
            if (!canDoFixedHoldPoint)
                rightHoldPointTransform.position = objectCenter + leftDirection + rightHoldPointOffset * rayLength;
        }
    }

    public Vector3 GetLeftHoldPoint() => leftHoldPointTransform.position;

    public Vector3 GetRightHoldPoint() => rightHoldPointTransform.position;

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
        if(leftHoldPointTransform == null || rightHoldPointTransform == null) return;

        // Create a style for the label
        GUIStyle style = new GUIStyle();
        style.fontSize = 10;
        style.normal.textColor = Color.green;

        // Convert world positions to screen positions
        Vector3 leftScreenPoint = Camera.main.WorldToScreenPoint(leftHoldPointTransform.position);
        Vector3 rightScreenPoint = Camera.main.WorldToScreenPoint(rightHoldPointTransform.position);

        // Draw labels at screen positions
        GUI.Label(new Rect(leftScreenPoint.x, Screen.height - leftScreenPoint.y, 100, 20), "Left", style);
        GUI.Label(new Rect(rightScreenPoint.x, Screen.height - rightScreenPoint.y, 100, 20), "Right", style);
    }

}
