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
    protected Transform target;
    protected float distanceToTarget;
    protected Vector3 directionToTarget;

    [Header("Navigation")]
    [SerializeField] private WayPointSystem wayPointSystem;
    protected NavMeshAgent agent;

    [Header("Aggro parameters")]
    [SerializeField] private float timeToAggro = 1f;
    private float timeCounterAggro;

    [Header("Stun parameters")]
    [SerializeField] private float timeStunned;
    private float timeStunnedCounter;
    public bool IsStunned { get; set; }

    public float TimeToAggro
    { get { return timeToAggro; } }
    public bool IsWithinSight { get; set; }
    public bool IsAggresive { get; set; }

    private void Awake()
    {
        StartCoroutine("FindTargetsWithDelay", delayToNewTarget);
        agent = GetComponent<NavMeshAgent>();
        timeStunnedCounter = timeStunned;
    }

    protected virtual void Update()
    {
        if (IsStunned)
        {
            timeStunnedCounter -= Time.deltaTime;
            if (timeStunnedCounter <= 0f)
            {
                IsStunned = false;
                timeStunnedCounter = timeStunned;
            }
        }
        else
        {
            if (IsWithinSight)
            {
                timeCounterAggro -= Time.deltaTime;

                if (timeCounterAggro <= 0f)
                {
                    IsAggresive = true;
                    timeCounterAggro = timeToAggro;
                }
            }
            else if (!IsWithinSight && IsAggresive)
            {
                timeCounterAggro -= Time.deltaTime;

                if (timeCounterAggro <= 0f)
                {
                    IsAggresive = false;
                    timeCounterAggro = timeToAggro;
                }
            }
        }
    }

    public void StunnedBy(Transform target)
    {
        this.target = target;
        IsStunned = true;
    }

    public void FindAttackingTarget(Transform target)
    {
        this.target = target;
        Debug.Log("target is set: " + target.name);
        IsAggresive = true;
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
        Collider[] targetInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
        IsWithinSight = targetInViewRadius.Length > 0;

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

        if (targetInViewRadius.Length == 0)
        {
            visibleTargets.Clear();
        }
    }

    public Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
            angleInDegrees += transform.eulerAngles.y;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0f, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

}