using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    /* A type of decorator node, one can think of it as a NOT modifier */
    public class Inverter : Node
    {
        /* Child node to evaluate */
        private Node node;

        public Node Node { get { return node; } }

        public Inverter(Node passedNode)
        {
            node = passedNode;
        }

        /* Reports a success if the child fails and a 
         and a failure if the child succeeds. Running will report as running */
        public override NodeStates Evaluate()
        {
            switch (node.Evaluate())
            {
                case NodeStates.FAILURE:
                    nodeState = NodeStates.SUCCESS;
                    return nodeState;
                case NodeStates.SUCCESS:
                    nodeState = NodeStates.FAILURE;
                    return nodeState;
                case NodeStates.RUNNING:
                    nodeState = NodeStates.RUNNING;
                    return nodeState;
            }
            nodeState = NodeStates.SUCCESS;
            return nodeState;
        }
    }
}