using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBaseLogic : MonoBehaviour
{
    [Header("Vision parameters")]
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;
    public float delayToNewTarget = 1f;
    public LayerMask obstacleMask;
    public LayerMask targetMask;
    public List<Transform> visibleTargets = new List<Transform>();
    public float distanceToTarget;
    public Vector3 directionToTarget;

    [Header("Navigation")]
    [SerializeField] private WayPointSystem wayPointSystem;

    [Header("BehaviorTree")]
    private Selector rootNode;
    private AggroNode aggroNode;
    private ChaseNode chaseNode;
    private WalkNode walkNode;
    private Sequence attackSequence;
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
        attackSequence = new Sequence(new List<Node> { aggroNode, chaseNode }); 
        walkNode = new WalkNode(this, wayPointSystem);
        
        rootNode = new Selector(new List<Node> { attackSequence, walkNode});
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