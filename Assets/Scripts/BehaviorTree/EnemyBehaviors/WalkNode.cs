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
    private Transform newTarget;

    public WalkNode(AIBaseLogic ai, NavMeshAgent agent, WayPointSystem wayPointSystem)
    {
        this.ai = ai;
        this.agent = agent;
        this.wayPointSystem = wayPointSystem;
        timeCounter = timer;
        newTarget = wayPointSystem.NewRandomPosition;
        agent.speed = movementSpeed;
    }

    public override NodeStates Evaluate()
    {
        if (!isWaiting)
        {
            if (Vector3.Distance(agent.transform.position, newTarget.position) < distanceDiff)
            {
                isWaiting = true;
                timeCounter = timer;
                newTarget = wayPointSystem.NewRandomPosition;
                agent.isStopped = false;
            }
            else
            {
                agent.SetDestination(newTarget.position);
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