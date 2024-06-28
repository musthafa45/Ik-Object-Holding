using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerIK : MonoBehaviour {
    public static PlayerIK Instance {  get; private set; } 

    public event Action<IHoldable> OnObjectParentChanged;
    public event Action<IHoldable> OnPlayerStabled;

    [SerializeField] private Rig holdingIkRig;

    private float targetWeight = 0f;

    [SerializeField] private TwoBoneIKConstraint leftBoneIKConstraint;
    [SerializeField] private TwoBoneIKConstraint rightBoneIKConstraint;

    [SerializeField] private MultiAimConstraint leftHandAimConstraint;
    [SerializeField] private MultiAimConstraint rightHandAimConstraint;

    [SerializeField] private MultiAimConstraint headAimConstraint;


    [SerializeField] private Transform objectHoldTransform;

    private IHoldable holdable = null;
    private Collider holdableColliderType;

    private bool canPullHoldable = false;
    private bool isStableEventInvoked = false;

    [SerializeField] private float lerpSpeed = 5f; // Speed of the lerp transition

    private void Awake() {
        Instance = this;
    }
    private void Start() {
        Interactor.Instance.OnHoldableInteracted += Interactor_Instance_OnHoldableInteracted;
    }

    private void Interactor_Instance_OnHoldableInteracted(IHoldable holdable) {
        if (this.holdable == null) {

            this.holdable = holdable;
            holdableColliderType = holdable.GetCollider();
            holdable.SetKinematic(true);

        }
      
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.G)) {
            ThrowObject();
            DisableHandIk();
        }

        // Update IK weights smoothly
        float currentWeight = holdingIkRig.weight;
        float newWeight = Mathf.Lerp(currentWeight, targetWeight, Time.deltaTime * lerpSpeed);
        holdingIkRig.weight = newWeight;

        // Update IK targets positions if holdable is not null
        if (holdable != null && targetWeight > 0f) {
            // Update positions with Lerp
            leftBoneIKConstraint.data.target.position = Vector3.Lerp(leftBoneIKConstraint.data.target.position, holdable.GetLeftHandIkTargetPosition(), Time.deltaTime * lerpSpeed);
            rightBoneIKConstraint.data.target.position = Vector3.Lerp(rightBoneIKConstraint.data.target.position, holdable.GetRightHandIkTargetPosition(), Time.deltaTime * lerpSpeed);

            //To update target Aim Source Position
            leftHandAimConstraint.data.sourceObjects[0].transform.position = Vector3.Lerp(leftHandAimConstraint.data.sourceObjects[0].transform.position, holdable.GetTransform().position,Time.deltaTime * lerpSpeed);
            rightHandAimConstraint.data.sourceObjects[0].transform.position = Vector3.Lerp(rightHandAimConstraint.data.sourceObjects[0].transform.position, holdable.GetTransform().position, Time.deltaTime * lerpSpeed);
        }

        // Lerp the child object to the reset position if CanPullHoldable is true
        if (canPullHoldable && holdable != null && objectHoldTransform.childCount > 0) {
            Transform childTransform = objectHoldTransform.GetChild(0);
            childTransform.localPosition = Vector3.Lerp(childTransform.localPosition, Vector3.zero, Time.deltaTime * lerpSpeed / 2);

            if(childTransform.localPosition ==  Vector3.zero && !isStableEventInvoked) {
                Debug.Log("raise event Here OnPlayer get Stabled");
                OnPlayerStabled?.Invoke(holdable);
                isStableEventInvoked = true;
            }

            if(holdableColliderType is CapsuleCollider /* || holdableColliderType is MeshCollider*/) {
                childTransform.localRotation = Quaternion.Lerp(childTransform.localRotation, Quaternion.identity, Time.deltaTime * lerpSpeed / 2);
            }
        }
    }

    private void ThrowObject() {
        holdable?.Throw(objectHoldTransform);
        SetHoldableNull();
        isStableEventInvoked = false;
    }

    private void SetHoldableNull() {
        holdable = null;
        holdableColliderType = null;
        OnObjectParentChanged?.Invoke(null);
    }

    public void EnableHandIk() {
        if (holdable != null) {
            targetWeight = 1f;
        }
    }

    public void DisableHandIk() {
        targetWeight = 0f;
    }

    public void OnLeanDownAnimComplete() {
        if (holdable != null) {
            holdable.SetParent(objectHoldTransform, false);
            holdable.GetTransform().GetComponent<Collider>().isTrigger = true;
            OnObjectParentChanged?.Invoke(holdable);
        }
    }

    public void OnLeanUpAnimMiddle() {
        Debug.Log("On Lean Up Middle");
        canPullHoldable = true;
    }

    public bool HasHoldingObject() {
        return holdable != null;
    }
}
