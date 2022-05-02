using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
public class WalkNode : Node
{
    private WayPointSystem wayPointSystem;
    private AIBaseLogic ai;
    private float rotationSpeed = 5f;
    private float movementSpeed = 5f;
    Vector3 direction = Vector3.zero;
    Transform oldTarget;

    private float timer = 1f;
    private float timeCounter;
    private bool isWaiting;
    private float distanceDiff = 0.1f;
    public WalkNode(AIBaseLogic ai, WayPointSystem wayPointSystem)
    {
        this.ai = ai;
        this.wayPointSystem = wayPointSystem;
        timeCounter = timer;
    }

    public override NodeStates Evaluate()
    {
        Transform newTarget = wayPointSystem.RandomPosition;
        if (newTarget == oldTarget)
            direction = (wayPointSystem.NewRandomPosition.position - ai.transform.position).normalized;

        if (isWaiting)
        {
            timeCounter -= Time.deltaTime;
            if (timeCounter <= 0f)
            {
                isWaiting = false;
            }
        }
        else
        {
            if (Vector3.Dot(ai.transform.forward, direction) < 0.7f)
            {
                Quaternion rotateTo = Quaternion.LookRotation(direction, Vector3.up);
                ai.transform.rotation = Quaternion.Slerp(ai.transform.rotation, rotateTo, rotationSpeed * Time.deltaTime);
            }

            if (Vector3.Distance(ai.transform.position, newTarget.position) < distanceDiff)
            {
                ai.transform.position = newTarget.position;
                timeCounter = timer;
                isWaiting = true;
            }
            else
            {
                ai.transform.Translate(0, 0, movementSpeed * Time.deltaTime);
            }
        }

        oldTarget = newTarget;

        nodeState = NodeStates.RUNNING;
        return nodeState;
    }


}
