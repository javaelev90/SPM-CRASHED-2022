using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public class ActionNode : Node
    {
        public delegate NodeStates ActionNodeDelegate();

        /* The delegate that is called to evaluate this node */
        private ActionNodeDelegate action;

        public ActionNode(ActionNodeDelegate passedAction)
        {
            action = passedAction;
        }

        public override NodeStates Evaluate()
        {
            switch (action())
            {
                case NodeStates.SUCCESS:
                    nodeState = NodeStates.SUCCESS;
                    return nodeState;
                case NodeStates.FAILURE:
                    nodeState = NodeStates.FAILURE;
                    return nodeState;
                case NodeStates.RUNNING:
                    nodeState = NodeStates.RUNNING;
                    return nodeState;
                default:
                    nodeState = NodeStates.FAILURE; // can be set to running or success depending on implementation
                    return nodeState;
            }
        }
    }
}