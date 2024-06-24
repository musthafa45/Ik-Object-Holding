using UnityEngine;

public interface IHoldable
{
    Transform GetLeftHandIkTarget();

    Transform GetRightHandIkTarget();

    Transform GetTransform();

    Transform GetStandTargetTransform();
}
