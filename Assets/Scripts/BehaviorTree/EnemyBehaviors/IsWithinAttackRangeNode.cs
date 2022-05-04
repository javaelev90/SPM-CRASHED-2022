using BehaviorTree;

public class IsWithinAttackRangeNode : Node
{
    private AIBaseLogic ai;

    public IsWithinAttackRangeNode(AIBaseLogic ai)
    {
        this.ai = ai;
    }

    public override NodeStates Evaluate()
    {
        if (ai.IsWithinSight)
        {
            return NodeStates.SUCCESS;
        }
        else
        {
            return NodeStates.FAILURE;
        }
    }
}