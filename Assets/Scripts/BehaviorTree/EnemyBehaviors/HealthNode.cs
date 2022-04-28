using UnityEngine;
using BehaviorTree;

public class HealthNode : Node
{
    private EnemyAI ai;
    private float threshHold;

    public HealthNode(EnemyAI ai, float threshHold)
    {
        this.ai = ai;
        this.threshHold = threshHold;
    }

    public override NodeStates Evaluate()
    {
        return ai.Health <= threshHold ? NodeStates.SUCCESS : NodeStates.FAILURE;
    }
}