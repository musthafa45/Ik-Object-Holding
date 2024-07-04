using System;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    private int pickUp_Hash;

    private void Awake() {
        animator = GetComponent<Animator>();
        pickUp_Hash = Animator.StringToHash("PickUp");
    }

    void Start()
    {
        PlayerItemHolder.Instance.OnReachedHoldableItemPosition += PlayerItemHolder_Instance_OnReachedHoldableItemPosition;

        InputManager.Instance.OnThrowKeyPerformed += InputManager_Instance_OnThrowKeyPerformed;
        InputManager.Instance.OnDropItemKeyPerformed += InputManager_Instance_OnDropItemKeyPerformed;
    }

    private void PlayerItemHolder_Instance_OnReachedHoldableItemPosition() {
        animator.SetTrigger(pickUp_Hash);
    }

    public void OnLeanMiddleComplete() { // Animation Event

        PlayerIK.Instance.SetLeftHandFollowTarget(PlayerItemHolder.Instance.GetHoldable().GetLeftHandIkTargetTransform());

        PlayerIK.Instance.SetRightHandFollowTarget(PlayerItemHolder.Instance.GetHoldable().GetRightHandIkTargetTransform());

        PlayerIK.Instance.SetHeadAimTarget(PlayerItemHolder.Instance.GetHoldable().GetTransform());
    }

    public void OnLeanFloorComplete() { // Animation Event
        if(!PlayerItemHolder.Instance.HasPhysicalHoldingObject()) {

            PlayerItemHolder.Instance.PickUp();
        }
        else {
            PlayerItemHolder.Instance.Drop();
        }
        
    }


    private void InputManager_Instance_OnThrowKeyPerformed(object sender, EventArgs e) {
        //Add Throw Anim And Use Anmation Event To throw 
        //here Im ByPassing throw Anim.
        PlayerItemHolder.Instance.Throw();
    }

    private void InputManager_Instance_OnDropItemKeyPerformed(object sender, EventArgs e) {
        if(PlayerItemHolder.Instance.HasHoldingObject())
        animator.SetTrigger(pickUp_Hash);
    }
}
