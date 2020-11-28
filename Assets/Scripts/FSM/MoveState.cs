﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : BaseState, IState
{
    public MoveState(GameObject gameObject) : base(gameObject){}
    public void OnEnter(){
        PlayerDetector.Instance.Move((int)Input.GetAxisRaw("Horizontal"));
        PlayerAnimator.AnimatorInstance.SetBool("Moving", true);
    }
    public void OnExit()
    {
        PlayerAnimator.AnimatorInstance.SetBool("Moving", false);
    }
    public override void HandleInput(){
        if(!PlayerInput.isLeftHold() && !PlayerInput.isRightHold()){
            _stateMachine.ChangeToState( new IdleState(_gameObject));
        }else if( PlayerInput.isJumpPressed()){
            _stateMachine.ChangeToState( new JumpState(_gameObject));
        }
    }
    public override void ProcessGraphics(){
        PlayerAnimator.Instance.UpdateSide((int)Input.GetAxisRaw("Horizontal"));
    }
    public override void ProcessPhysics(){
        PlayerDetector.Instance.Move((int)Input.GetAxisRaw("Horizontal"));

    }
}
