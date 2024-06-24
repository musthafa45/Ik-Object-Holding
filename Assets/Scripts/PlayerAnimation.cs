using StarterAssets;
using System;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    private int hold_Hash;
    private ThirdPersonController controller;

    private void Awake() {
        animator = GetComponent<Animator>();
        hold_Hash = Animator.StringToHash("Hold");
        controller = GetComponent<ThirdPersonController>();
    }

    void Start()
    {
        Interactor.Instance.OnHoldableInteracted += Interactor_Instance_OnHoldableInteracted;
    }

    private void Interactor_Instance_OnHoldableInteracted(IHoldable holdable) {

        Debug.Log(holdable.GetTransform().name);

        Transform standTargetTransform = holdable.GetStandTargetTransform();

        SetTargetToMove(standTargetTransform, () => {
            animator.SetTrigger(hold_Hash);
        });

    }

    private void SetTargetToMove(Transform standTargetTransform, Action OnTargetReached) {
        controller.SetTargetPosition(standTargetTransform, OnTargetReached);
    }
}
