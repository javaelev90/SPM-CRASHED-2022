using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBaseLogic : MonoBehaviour
{
    public float viewRadius;

    [Range(0, 360)]
    public float viewAngle;

    public float delayToNewTarget = 1f;
    public LayerMask obstacleMask;
    public LayerMask targetMask;
    public List<Transform> visibleTargets = new List<Transform>();
    public float distanceToTarget;
    public Vector3 directionToTarget;

    [Header("BehaviorTree")]
    private Sequence rootNode;
    private AggroNode aggroNode;
    private ChaseNode chaseNode;
    private Material material;
    [SerializeField] private float timeToAggro = 1f;

    public float TimeToAggro
    { get { return timeToAggro; } }
    public bool IsWithinRange { get; set; }
    public bool IsAggresive { get; set; }

    private void Start()
    {
        StartCoroutine("FindTargetsWithDelay", delayToNewTarget);
        material = GetComponent<MeshRenderer>().material;

        chaseNode = new ChaseNode(this);
        aggroNode = new AggroNode(this, material);
        List<Node> nodes = new List<Node>();
        nodes.Add(aggroNode);
        nodes.Add(chaseNode);

        //rootNode = new Selector(nodes);
        rootNode = new Sequence(nodes);
    }

    private void Update()
    {
        rootNode.Evaluate();
    }

    private IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    private void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider[] targetInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
        IsWithinRange = targetInViewRadius.Length > 0;

        for (int i = 0; i < targetInViewRadius.Length; i++)
        {
            Transform target = targetInViewRadius[i].transform;
            directionToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2)
            {
                distanceToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);
                }
            }
        }
    }

    public Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
            angleInDegrees += transform.eulerAngles.y;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0f, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}