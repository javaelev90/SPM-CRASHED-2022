using System.Collections.Generic;

namespace BehaviorTree
{
    public class Selector : Node
    {
        protected List<Node> nodes = new List<Node>();

        public Selector(List<Node> passedNodes)
        {
            nodes = passedNodes;
        }

        public override NodeStates Evaluate()
        {
            foreach (Node node in nodes)
            {
                switch (node.Evaluate())
                {
                    case NodeStates.FAILURE:
                        continue;
                    case NodeStates.SUCCESS:
                        nodeState = NodeStates.SUCCESS;
                        return nodeState;
                    case NodeStates.RUNNING:
                        nodeState = NodeStates.RUNNING;
                        return nodeState;
                    default:
                        continue;
                }
            }
            nodeState = NodeStates.FAILURE;
            return nodeState;
        }
    }
}