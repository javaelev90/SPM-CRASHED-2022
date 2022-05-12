using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;
public class WalkNode : Node
{
    private WayPointSystem wayPointSystem;
    private AIBaseLogic ai;
    private NavMeshAgent agent;
    private float movementSpeed = 5f;

    private float timer = 2f;
    private float timeCounter;
    private bool isWaiting;
    private float distanceDiff = 2f;
    private Vector3 newTarget;

    public WalkNode(AIBaseLogic ai, NavMeshAgent agent, WayPointSystem wayPointSystem)
    {
        this.ai = ai;
        this.agent = agent;
        this.wayPointSystem = wayPointSystem;
        timeCounter = timer;
        newTarget = wayPointSystem.GetNewPosition;
        agent.speed = movementSpeed;
    }

    public override NodeStates Evaluate()
    {
        if (!isWaiting)
        {
            if (Vector3.Distance(agent.transform.position, newTarget) < distanceDiff)
            {
                isWaiting = true;
                timeCounter = timer;
                newTarget = wayPointSystem.GetNewPosition;
                agent.isStopped = false;
            }
            else
            {
                agent.SetDestination(newTarget);
            }
        }
        else
        {
            timeCounter -= Time.deltaTime;
            if (timeCounter <= 0f)
            {
                isWaiting = false;
            }
        }

        nodeState = NodeStates.RUNNING;
        return nodeState;
    }

}