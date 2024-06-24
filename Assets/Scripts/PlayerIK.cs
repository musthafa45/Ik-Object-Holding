using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerIK : MonoBehaviour {

    [SerializeField] private TwoBoneIKConstraint leftBoneIKConstraint;
    [SerializeField] private TwoBoneIKConstraint rightBoneIKConstraint;

    private float targetWeight = 0f;

    private IHoldable holdable = null;
    [SerializeField] private float lerpSpeed = 5f; // Speed of the lerp transition

    private void Start() {
        Interactor.Instance.OnHoldableInteracted += Interactor_Instance_OnHoldableInteracted;
    }

    private void Interactor_Instance_OnHoldableInteracted(IHoldable holdable) {
        this.holdable = holdable;
    }

    private void Update() {
        float currentWeightLeft = leftBoneIKConstraint.weight;
        float newWeightLeft = Mathf.Lerp(currentWeightLeft, targetWeight, Time.deltaTime * lerpSpeed);
        leftBoneIKConstraint.weight = newWeightLeft;

        float currentWeightRight = rightBoneIKConstraint.weight;
        float newWeightRight = Mathf.Lerp(currentWeightRight, targetWeight, Time.deltaTime * lerpSpeed);
        rightBoneIKConstraint.weight = newWeightRight;
    }

    public void EnableHandIk() {
        if (holdable != null) {
            leftBoneIKConstraint.data.target.transform.SetPositionAndRotation(holdable.GetLeftHandIkTarget().position, holdable.GetLeftHandIkTarget().rotation);
            rightBoneIKConstraint.data.target.transform.SetPositionAndRotation(holdable.GetRightHandIkTarget().position, holdable.GetRightHandIkTarget().rotation);
        }

        targetWeight = 1f;
    }
    public void DisableHandIk() {
        targetWeight = 0f;
    }
}
