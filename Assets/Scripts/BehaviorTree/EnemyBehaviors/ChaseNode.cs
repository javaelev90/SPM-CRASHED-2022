using BehaviorTree;
using UnityEngine;

public class ChaseNode : Node
{
    private AIBaseLogic ai;
    private float rotationSpeed = 5f;
    private float chaseSpeed = 7f;
    private float distanceToTarget;
    private bool withinRange;

    public ChaseNode(AIBaseLogic ai)
    {
        this.ai = ai;
        distanceToTarget = ai.viewRadius / 2;
    }

    public override NodeStates Evaluate()
    {
        if (ai.visibleTargets.Count == 0)
        {
            return NodeStates.FAILURE;
        }

        if (Vector3.Distance(ai.transform.position, ai.visibleTargets[0].position) < distanceToTarget)
        {
            withinRange = true;
            return NodeStates.SUCCESS;
        }
        else
        {
            withinRange = false;
            ChaseTarget();
            return NodeStates.RUNNING;
        }

    }

    private void ChaseTarget()
    {
        if (!withinRange)
        {
            Quaternion rotateTo = Quaternion.LookRotation(ai.directionToTarget, Vector3.up);
            ai.transform.rotation = Quaternion.Slerp(ai.transform.rotation, rotateTo, rotationSpeed * Time.deltaTime);
            ai.transform.Translate(0, 0, chaseSpeed * Time.deltaTime);
        }
    }

}