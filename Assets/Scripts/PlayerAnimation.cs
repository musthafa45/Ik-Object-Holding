using StarterAssets;
using System;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    private int leanDown_Hash;
    private int leanUp_Hash;
    private ThirdPersonController controller;

    private void Awake() {
        animator = GetComponent<Animator>();
        leanDown_Hash = Animator.StringToHash("LeanDown");
        leanUp_Hash = Animator.StringToHash("LeanUp");
        controller = GetComponent<ThirdPersonController>();
    }

    void Start()
    {
        Interactor.Instance.OnHoldableInteracted += Interactor_Instance_OnHoldableInteracted;
        GetComponent<PlayerIK>().OnObjectParentChanged += PlayerAnimation_OnObjectParentChanged;
    }

    private void PlayerAnimation_OnObjectParentChanged(IHoldable holdable) {
        animator.SetTrigger(leanUp_Hash);
    }

    private void Interactor_Instance_OnHoldableInteracted(IHoldable holdable) {

        Debug.Log(holdable.GetTransform().name);

        if (PlayerIK.Instance.HasHoldingObject()) return;

        Transform standTargetTransform = holdable.GetTransform();

        SetTargetToMove(standTargetTransform, () => {
            animator.SetTrigger(leanDown_Hash);
        });

    }

    private void SetTargetToMove(Transform standTargetTransform, Action OnTargetReached) {
        controller.SetTargetPosition(standTargetTransform, OnTargetReached);
    }
}
