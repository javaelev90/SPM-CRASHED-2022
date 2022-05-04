namespace BehaviorTree
{
    public enum NodeStates
    {
        FAILURE,
        RUNNING,
        SUCCESS
    }

    [System.Serializable]
    public abstract class Node
    {
        /* delegate that return the state of the node*/

        public delegate NodeStates NodeReturn();

        /* The current state of the node */
        protected NodeStates nodeState;

        public NodeStates NodeState
        { get { return nodeState; } }

        public Node()
        { }

        /* Implementing classes us this method evaluate the desired set of conditions */

        public abstract NodeStates Evaluate();
    }
}