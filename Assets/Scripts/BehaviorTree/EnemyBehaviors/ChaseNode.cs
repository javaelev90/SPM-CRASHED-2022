using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class ChaseNode : Node
{
    private AIBaseLogic ai;
    private NavMeshAgent agent;
    private float chaseSpeed = 10f;
    private float distanceToTarget;
    private bool withinRange;
    public ChaseNode(AIBaseLogic ai, NavMeshAgent agent)
    {
        this.ai = ai;
        this.agent = agent;
        //distanceToTarget = ai.viewRadius / 2;
        agent.speed = chaseSpeed;
    }

    public override NodeStates Evaluate()
    {
        return NodeStates.SUCCESS;
        //    if (ai.visibleTargets.Count == 0)
        //    {
        //        return NodeStates.FAILURE;
        //    }

        //    if (Vector3.Distance(agent.transform.position, ai.visibleTargets[0].position) < distanceToTarget)
        //    {
        //        withinRange = true;
        //        return NodeStates.SUCCESS;
        //    }
        //    else
        //    {
        //        withinRange = false;
        //        ChaseTarget();
        //        return NodeStates.RUNNING;
        //    }
        //}

        //private void ChaseTarget()
        //{
        //    if (!withinRange)
        //    {
        //        agent.SetDestination(ai.visibleTargets[0].position);
        //    }
    }
}