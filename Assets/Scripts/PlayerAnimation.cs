using StarterAssets;
using System;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    private int pickUp_Hash;
    private ThirdPersonController controller;

    private void Awake() {
        animator = GetComponent<Animator>();
        pickUp_Hash = Animator.StringToHash("PickUp");
        controller = GetComponent<ThirdPersonController>();
    }

    void Start()
    {
        Interactor.Instance.OnHoldableInteracted += Interactor_Instance_OnHoldableInteracted;

        InputManager.Instance.OnThrowKeyPerformed += InputManager_Instance_OnThrowKeyPerformed;
        InputManager.Instance.OnDropItemKeyPerformed += InputManager_Instance_OnDropItemKeyPerformed;
    }


    private void Interactor_Instance_OnHoldableInteracted(IHoldable holdable) {

        //Debug.Log(holdable.GetTransform().name);

        if (PlayerIK.Instance.HasHoldingObject()) return;

        Transform standTargetTransform = holdable.GetTransform();

        SetTargetToMove(standTargetTransform, () => {
            animator.SetTrigger(pickUp_Hash);
        });

    }

    private void InputManager_Instance_OnThrowKeyPerformed(object sender, EventArgs e) {
        //Add Throw Anim And Use Anmation Event To throw 
        //here Im ByPassing throw Anim.
        PlayerIK.Instance.Throw();
    }

    private void InputManager_Instance_OnDropItemKeyPerformed(object sender, EventArgs e) {
        if(PlayerIK.Instance.HasHoldingObject())
        animator.SetTrigger(pickUp_Hash);
    }

    private void SetTargetToMove(Transform standTargetTransform, Action OnTargetReached) {
        controller.SetTargetPosition(standTargetTransform, OnTargetReached);
    }
}
