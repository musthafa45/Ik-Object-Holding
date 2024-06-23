using UnityEngine;

public class Item : MonoBehaviour, IHoldable {

    [SerializeField] private Transform leftHandTargetIk;
    [SerializeField] private Transform rightHandTargetIk;

    public Transform GetLeftHandIkTarget() => leftHandTargetIk;

    public Transform GetRightHandIkTarget() => rightHandTargetIk;
}
