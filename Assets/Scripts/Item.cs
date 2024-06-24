using UnityEngine;

public class Item : MonoBehaviour, IHoldable {

    [SerializeField] private Transform leftHandTargetIk;
    [SerializeField] private Transform rightHandTargetIk;
    [SerializeField] private Transform targetStandPosition;

    public Transform GetLeftHandIkTarget() => leftHandTargetIk;

    public Transform GetRightHandIkTarget() => rightHandTargetIk;

    public Transform GetTransform() => transform;
    public Transform GetStandTargetTransform() => targetStandPosition;
}
