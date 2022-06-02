using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = ("PlayerWalkState"))]
public class PlayerWalkState : State
{
    [SerializeField] private float maxSpeed = 10f;
    private Vector3 input;



    public override void Enter()
    {
        base.Enter();
   
        player.animator.SetBool("isRunning", true);
        //player.animator.CrossFadeInFixedTime("Running", 0.1f);
    }

    public override void UpdateState()
    {
        base.UpdateState();

        input = player.GetInput();
        if (input.magnitude > float.Epsilon)
        {
            player.Body.Accelerate(input, maxSpeed);
            
        }
        else
        {
            player.Body.Decelerate();
        
        }

        //if (player.Body.Grounded && Input.GetKeyDown(KeyCode.Space))
        //{
        //    stateMachine.ChangeState<PlayerJumpState>();
        //}

        if (player.Body.Velocity.magnitude <= 0.0005f)
        {
            stateMachine.ChangeState<PlayerIdleState>();
        }
    }

  

    public override void Exit()
    {
        base.Exit();
        player.animator.SetBool("isRunning", false);
        player.animator.CrossFadeInFixedTime("Idle", 0.1f);
    }


}
