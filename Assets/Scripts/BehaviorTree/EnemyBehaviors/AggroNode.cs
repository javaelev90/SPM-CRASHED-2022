using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;
public class AggroNode : Node
{
    private Material material;
    private AIBaseLogic ai;
    private float timeCounter;

    public AggroNode(AIBaseLogic ai, Material material)
    {
        this.ai = ai;
        this.material = material;
    }

    public override NodeStates Evaluate()
    {
        if (ai.IsWithinSight)
        {
            timeCounter -= Time.deltaTime;

            if (timeCounter <= 0f)
            {
                ai.IsAggresive = true;
                timeCounter = ai.TimeToAggro;
            }
        }
        else
        {
            timeCounter -= Time.deltaTime;

            if (timeCounter <= 0f)
            {
                ai.IsAggresive = false;
                timeCounter = ai.TimeToAggro;
            }
        }

        if (ai.IsAggresive)
        {
            material.color = Color.red;
            return NodeStates.SUCCESS;
        }
        else
        {
            material.color = Color.white;
            return NodeStates.FAILURE;
        }
    }
}