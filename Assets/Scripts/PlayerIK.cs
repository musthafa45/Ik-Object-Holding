using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerIK : MonoBehaviour {
    public static PlayerIK Instance { get; private set; }

    [SerializeField] private Rig handPositionRotationRig;
    [SerializeField] private Rig rightLegPositionRig;
    [SerializeField] private Rig headAimRig;

    [SerializeField] private TwoBoneIKConstraint leftBoneIKConstraint;
    [SerializeField] private TwoBoneIKConstraint rightBoneIKConstraint;

    [SerializeField] private MultiAimConstraint leftHandAimConstraint;
    [SerializeField] private MultiAimConstraint rightHandAimConstraint;

    [SerializeField] private MultiAimConstraint headAimConstraint;

    [SerializeField] private float lerpSpeed = 5f; // Speed of the lerp transition


    private Transform leftHandTargetFollowTransform = null;
    private Transform rightHandTargetFollowTransform = null;

    private Transform rightLegTargetFollowTransform = null;

    private Transform headAimTargetFollowTransform = null;

    private void Awake() {
        Instance = this;
    }

    private void Start() {

        PlayerItemHolder.Instance.OnObjectThrown += PlayerItemHolder_Instance_OnObjectThrown;
        PlayerItemHolder.Instance.OnHoldableDropped += PlayerItemHolder_Instance_OnHoldableDropped;

        handPositionRotationRig.weight = 0f;
        rightLegPositionRig.weight = 0f;
        headAimRig.weight = 0f;  
    }

    private void PlayerItemHolder_Instance_OnHoldableDropped() {
        SetAllTargetConstrainsNull();
    }

    private void PlayerItemHolder_Instance_OnObjectThrown() {
        SetAllTargetConstrainsNull();
    }

    private void SetAllTargetConstrainsNull() {
        leftHandTargetFollowTransform = null;
        rightHandTargetFollowTransform = null;
        rightLegTargetFollowTransform = null;
        headAimTargetFollowTransform = null;
    }

    private void Update() {
       HandleIk();
    }

    private void HandleIk() {

        if (leftHandTargetFollowTransform != null && rightHandTargetFollowTransform != null) {
            // Update Hands IK weights smoothly to 1
            handPositionRotationRig.weight = Mathf.Lerp(handPositionRotationRig.weight, 1f, Time.deltaTime * lerpSpeed / 2);

            // Updating Ik Data Object Positions
            leftBoneIKConstraint.data.target.position = Vector3.Lerp(leftBoneIKConstraint.data.target.position, leftHandTargetFollowTransform.position, Time.deltaTime * lerpSpeed);
            rightBoneIKConstraint.data.target.position = Vector3.Lerp(rightBoneIKConstraint.data.target.position, rightHandTargetFollowTransform.position, Time.deltaTime * lerpSpeed);

            // To update Hand target Aim Source Position
            leftHandAimConstraint.data.sourceObjects[0].transform.position = Vector3.Lerp(leftHandAimConstraint.data.sourceObjects[0].transform.position, rightHandTargetFollowTransform.position, Time.deltaTime * lerpSpeed);
            rightHandAimConstraint.data.sourceObjects[0].transform.position = Vector3.Lerp(rightHandAimConstraint.data.sourceObjects[0].transform.position, leftHandTargetFollowTransform.position, Time.deltaTime * lerpSpeed);
        }
        else {
            // Update Hands IK weights smoothly to 0
            handPositionRotationRig.weight = Mathf.Lerp(handPositionRotationRig.weight, 0f, Time.deltaTime * lerpSpeed / 2);
        }


        if (headAimTargetFollowTransform != null) {
            // Update Head IK weights smoothly to 1
            headAimRig.weight = Mathf.Lerp(headAimRig.weight, 1f, Time.deltaTime * lerpSpeed / 2);
        }
        else {
            // Update Head IK weights smoothly to 0
            headAimRig.weight = Mathf.Lerp(headAimRig.weight, 0f, Time.deltaTime * lerpSpeed / 2);
        }

        if (rightLegTargetFollowTransform != null) {
            // Update Leg IK weights smoothly to 1
            rightLegPositionRig.weight = Mathf.Lerp(rightLegPositionRig.weight, 1f, Time.deltaTime * lerpSpeed / 2);
        }
        else {
            // Update Leg IK weights smoothly to 0
            rightLegPositionRig.weight = Mathf.Lerp(rightLegPositionRig.weight, 0f, Time.deltaTime * lerpSpeed / 2);
        }
    }

    public void SetLeftHandFollowTarget(Transform leftHandTarget) {
        leftHandTargetFollowTransform = leftHandTarget;
    }

    public void SetRightHandFollowTarget(Transform rightHandTarget) {
       rightHandTargetFollowTransform = rightHandTarget;
    }

    public void SetRightLegFollowTarget(Transform rightLegTarget) {
        rightLegTargetFollowTransform = rightLegTarget;
    }

    public void SetHeadAimTarget(Transform headAimTarget) {
        headAimTargetFollowTransform = headAimTarget;
    }
}
