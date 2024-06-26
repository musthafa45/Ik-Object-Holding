using UnityEngine;

public interface IHoldable
{
    Vector3 GetLeftHandIkTargetPosition();

    Vector3 GetRightHandIkTargetPosition();

    Transform GetTransform();

    Collider GetCollider();
    void SetKinematic(bool active);
    void SetParent(Transform target, bool resetLocalPos = true);
    void Throw(Transform objectHoldTransform);
}
