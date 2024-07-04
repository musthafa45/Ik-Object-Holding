using StarterAssets;
using System;
using UnityEngine;

[RequireComponent(typeof(ThirdPersonController))]
public class PlayerItemHolder : MonoBehaviour
{
    public static PlayerItemHolder Instance { get; private set; }

    public event Action OnReachedHoldableItemPosition;

    public event Action OnHoldableDropped;
    public event Action OnObjectThrown;

    [SerializeField] private Transform objectHoldTransform;
    private IHoldable holdable = null;

    [SerializeField] private float lerpSpeed = 5f;
    private bool canPullHoldable = false;

    private ThirdPersonController thirdPersonController;

    private void Awake() {
        Instance = this;
        thirdPersonController = GetComponent<ThirdPersonController>();
    }

    private void Start() {
        Interactor.Instance.OnHoldableInteracted += Interactor_Instance_OnHoldableInteracted;
    }

    private void Interactor_Instance_OnHoldableInteracted(IHoldable holdable) {
        if (this.holdable == null) {

            this.holdable = holdable;

            holdable.SetKinematic(true);

            thirdPersonController.SetTargetPosition(holdable.GetTransform(), () => {
                OnReachedHoldableItemPosition?.Invoke();
            });
        }
    }

    public void PickUp() {
        if (holdable != null) {
            holdable.SetParent(objectHoldTransform);
            holdable.GetCollider().isTrigger = true;
            canPullHoldable = true;
        }
    }

    public void Drop() {
        if (holdable != null) {
            holdable.SetParent(null);
            holdable.GetCollider().isTrigger = false;
            holdable.SetKinematic(false);
            OnHoldableDropped?.Invoke();
            holdable = null;
            canPullHoldable = false;
        }
    }

    public void Throw() {
        if (holdable != null) {
            holdable?.Throw(objectHoldTransform);
            OnObjectThrown?.Invoke();
            holdable = null;
        }
    }

    public bool HasHoldingObject() => holdable != null;

    public IHoldable GetHoldable() => holdable;

    private void Update() {
        if (canPullHoldable && holdable != null && objectHoldTransform.childCount > 0) {
            Transform childTransform = objectHoldTransform.GetChild(0);
            childTransform.localPosition = Vector3.Lerp(childTransform.localPosition, Vector3.zero, Time.deltaTime * lerpSpeed / 2);

            if (holdable.GetCollider() is CapsuleCollider /* || holdableColliderType is MeshCollider */) {
                childTransform.localRotation = Quaternion.Lerp(childTransform.localRotation, Quaternion.identity, Time.deltaTime * lerpSpeed / 2);
            }
        }
    }

    public bool HasPhysicalHoldingObject() => objectHoldTransform.childCount > 0;

}
