using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIBaseLogic : MonoBehaviour
{
    [Header("Vision parameters")]
    [SerializeField] protected float viewRadius;
    [Range(0, 360)]
    [SerializeField] protected float viewAngle;
    [SerializeField] private float delayToNewTarget = 1f;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private LayerMask targetMask;
    protected List<Transform> visibleTargets = new List<Transform>();
    protected float distanceToTarget;
    protected Vector3 directionToTarget;

    [Header("Navigation")]
    [SerializeField] private WayPointSystem wayPointSystem;
    protected NavMeshAgent agent;
    [SerializeField] private float timeToAggro = 1f;

    public float TimeToAggro
    { get { return timeToAggro; } }
    public bool IsWithinRange { get; set; }
    public bool IsAggresive { get; set; }

    private void Awake()
    {
        StartCoroutine("FindTargetsWithDelay", delayToNewTarget);
        agent = GetComponent<NavMeshAgent>();
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

        //if(targetInViewRadius.Length == 0)
        //{
        //    visibleTargets.Clear();
        //}
    }

    public Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
            angleInDegrees += transform.eulerAngles.y;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0f, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

}