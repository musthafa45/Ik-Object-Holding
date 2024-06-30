using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerIK : MonoBehaviour {
    public static PlayerIK Instance { get; private set; }

    public event Action<IHoldable> OntemPicked;
    public event Action<IHoldable> OnPlayerPlaceDownKeyPerformed;

    [SerializeField] private Rig handPositionRotationRig;
    [SerializeField] private Rig rightLegPositionRig;
    [SerializeField] private Rig headAimRig;

    private float targetWeight = 0f;

    [SerializeField] private TwoBoneIKConstraint leftBoneIKConstraint;
    [SerializeField] private TwoBoneIKConstraint rightBoneIKConstraint;

    [SerializeField] private MultiAimConstraint leftHandAimConstraint;
    [SerializeField] private MultiAimConstraint rightHandAimConstraint;

    [SerializeField] private MultiAimConstraint headAimConstraint;

    [SerializeField] private Transform objectHoldTransform;

    private IHoldable holdable = null;

    private bool isHolding = false;
    private bool canPullHoldable = false;
    private bool isPickedItemStabled = false;

    [SerializeField] private float lerpSpeed = 5f; // Speed of the lerp transition

    private void Awake() {
        Instance = this;
    }

    private void Start() {

        Interactor.Instance.OnHoldableInteracted += Interactor_Instance_OnHoldableInteracted;

        SetActiveIks(new List<Rig> { handPositionRotationRig, rightLegPositionRig, headAimRig }, false);
    }

    private void Interactor_Instance_OnHoldableInteracted(IHoldable holdable) {
        if (this.holdable == null) {
            // Grabbing Data Holdable That Player interacted
            this.holdable = holdable;
            // To Avoid Accidental Kicks 
            holdable.SetKinematic(true);
        }
    }

    private void Update() {
     
        // Update Hands IK weights smoothly
        handPositionRotationRig.weight = Mathf.Lerp(handPositionRotationRig.weight, targetWeight, Time.deltaTime * lerpSpeed / 2);

        // Update Leg IK weights smoothly
        rightLegPositionRig.weight = Mathf.Lerp(rightLegPositionRig.weight, targetWeight, Time.deltaTime * lerpSpeed / 2);

        // Update Head IK weights smoothly
        headAimRig.weight = Mathf.Lerp(headAimRig.weight, targetWeight, Time.deltaTime * lerpSpeed / 2);

        // Update IK targets positions if holdable is not null
        if (holdable != null && targetWeight > 0f) {
            // Update positions with Lerp
            leftBoneIKConstraint.data.target.position = Vector3.Lerp(leftBoneIKConstraint.data.target.position, holdable.GetLeftHandIkTargetPosition(), Time.deltaTime * lerpSpeed);
            rightBoneIKConstraint.data.target.position = Vector3.Lerp(rightBoneIKConstraint.data.target.position, holdable.GetRightHandIkTargetPosition(), Time.deltaTime * lerpSpeed);

            // To update target Aim Source Position
            leftHandAimConstraint.data.sourceObjects[0].transform.position = Vector3.Lerp(leftHandAimConstraint.data.sourceObjects[0].transform.position, holdable.GetTransform().position, Time.deltaTime * lerpSpeed);
            rightHandAimConstraint.data.sourceObjects[0].transform.position = Vector3.Lerp(rightHandAimConstraint.data.sourceObjects[0].transform.position, holdable.GetTransform().position, Time.deltaTime * lerpSpeed);
        }

        // Lerp the child object to the reset position if CanPullHoldable is true
        if (canPullHoldable && holdable != null && objectHoldTransform.childCount > 0) {
            Transform childTransform = objectHoldTransform.GetChild(0);
            childTransform.localPosition = Vector3.Lerp(childTransform.localPosition, Vector3.zero, Time.deltaTime * lerpSpeed / 2);

            if (childTransform.localPosition == Vector3.zero) {
                //Debug.Log("Item Stabled");
                isPickedItemStabled = true;
            }

            if (holdable.GetCollider() is CapsuleCollider /* || holdableColliderType is MeshCollider */) {
                childTransform.localRotation = Quaternion.Lerp(childTransform.localRotation, Quaternion.identity, Time.deltaTime * lerpSpeed / 2);
            }
        }
    }

    public void OnLeanMiddle() { //Animation Event
        ToggleWeight();
    }

    private void ToggleWeight() {
        isHolding = !isHolding;

        targetWeight = isHolding ? 1 : 0;
    }

    public void OnLeanFloor() { //Animation Event

        if (isHolding && holdable != null) {

            Debug.Log("Picking Up Saved Data Item");

            holdable.SetParent(objectHoldTransform, false);

            holdable.GetTransform().GetComponent<Collider>().isTrigger = true;

            OntemPicked?.Invoke(holdable); // It Invokes Lean Up Anim

            canPullHoldable = true;
        }
        else if (!isHolding && holdable != null) {

            Debug.Log("Dropping Down Item");

            holdable.SetParent(null, false);

            holdable.GetTransform().GetComponent<Collider>().isTrigger = false;

            holdable.SetKinematic(false);

            OntemPicked?.Invoke(null);

            holdable = null;
        }

    }

    public void OnLeanStand() { //Animation Event

    }



    private void SetActiveIks(List<Rig> rigs, bool active) {
        foreach (Rig rig in rigs)
            rig.weight = active ? 1f : 0f;
    }

    public bool HasHoldingObject() => holdable != null;

    public bool IsHoldItemStabled() => HasHoldingObject() && isPickedItemStabled;

    public IHoldable GetHoldItem() => holdable;

    public void Throw() {
        if ((holdable != null))
        {
            holdable?.Throw(objectHoldTransform);
            holdable = null;
            isPickedItemStabled = false;
            ToggleWeight();
        }
    }
}
