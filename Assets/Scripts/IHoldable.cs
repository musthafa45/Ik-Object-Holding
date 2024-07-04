using UnityEngine;

public interface IHoldable
{
    Transform GetLeftHandIkTargetTransform();

    Transform GetRightHandIkTargetTransform();

    Transform GetTransform();

    Collider GetCollider();
    void SetKinematic(bool active);
    void SetParent(Transform target);
    void Throw(Transform objectHoldTransform);
}
