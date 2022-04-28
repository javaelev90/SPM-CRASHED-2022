using System.Collections.Generic;

namespace BehaviorTree
{
    public class Sequence : Node
    {
        /* Children nodes that belong to this sequence */
        private List<Node> nodes = new List<Node>();

        public Sequence(List<Node> passedNodes)
        {
            nodes = passedNodes;
        }

        public override NodeStates Evaluate()
        {
            bool anyChildRunning = false;
            foreach (Node node in nodes)
            {
                switch (node.Evaluate())
                {
                    case NodeStates.FAILURE:
                        nodeState = NodeStates.FAILURE;
                        return nodeState;
                    case NodeStates.SUCCESS:
                        continue;
                    case NodeStates.RUNNING:
                        anyChildRunning = true;
                        continue;
                    default:
                        nodeState = NodeStates.SUCCESS;
                        return nodeState;
                }
            }
            nodeState = anyChildRunning ? NodeStates.RUNNING : NodeStates.SUCCESS;
            return nodeState;
        }
    }
}