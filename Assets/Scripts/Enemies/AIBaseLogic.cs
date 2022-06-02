using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using System.Linq;

public class AIBaseLogic : MonoBehaviourPunCallbacks
{
    [Header("Vision parameters")]
    [SerializeField] protected float viewRadius;
    [Range(0, 360)]
    [SerializeField] protected float viewAngle;
    [SerializeField] private float delayToNewTarget = 1f;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] protected LayerMask targetMask;
    [SerializeField] protected PooledObject root;
    protected List<Transform> visibleTargets = new List<Transform>();
    protected Transform target;
    [SerializeField] protected Transform eventTarget;
    protected float distanceToTarget;
    protected Vector3 directionToTarget;

    [Header("Navigation")]
    [SerializeField] protected WayPointSystem wayPointSystem;
    protected NavMeshAgent agent;

    [Header("Aggro parameters")]
    [SerializeField] protected float timeToAggro = 1f;
    [SerializeField] protected float timeToCoolWhenAttacked;
    protected float timeCounterAggro;
    protected float timeCounterAttacked;
    public bool IsAttacked { get; set; }
    public bool IsStunned { get; set; }

    [Header("Stun parameters")]
    [SerializeField] protected float timeStunned;
    protected float timeStunnedCounter;

    protected bool IsMasterClient { get; set; }
    private Coroutine findTargets;
    public float TimeToAggro { get { return timeToAggro; } }
    public bool IsWithinSight { get; set; }
    public bool IsAggresive { get; set; }

    [Header("Animator")]
    [SerializeField] protected Animator animator;

    protected void OnEnable()
    {
        root.CustomInitializeFunction = Initialize;
        findTargets = StartCoroutine("FindTargetsWithDelay", delayToNewTarget);
        agent = GetComponent<NavMeshAgent>();
        IsMasterClient = PhotonNetwork.IsMasterClient;
        eventTarget = PhotonView.Find(root.photonViewTargetId)?.transform;
    }

    private void OnDisable()
    {
        if (findTargets != null)
            StopCoroutine(findTargets);
        target = null;
    }

    protected virtual void Update() { }

    public void StunnedBy(int photonViewId, Vector3 hitPoint, Vector3 hitNormal)
    {
        photonView.RPC(nameof(StunnedByRPC), RpcTarget.All, photonViewId, hitPoint, hitNormal);
    }

    [PunRPC]
    public void StunnedByRPC(int photonViewId, Vector3 hitPoint, Vector3 hitNormal)
    {
        this.target = PhotonView.Find(photonViewId).transform;
        IsStunned = true;
        timeStunnedCounter = timeStunned;
        directionToTarget = (target.position - transform.position).normalized;
        distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (GameManager.PrefabManager.stunEffectPrefab != null)
        {
            Destroy(Instantiate(GameManager.PrefabManager.stunEffectPrefab, hitPoint, Quaternion.LookRotation(hitNormal)), timeStunned);
        }
    }

    public void FindAttackingTarget(Transform target)
    {
        this.target = target;
        IsAttacked = true;
        timeCounterAttacked = timeToCoolWhenAttacked;
        directionToTarget = (target.position - transform.position).normalized;
        distanceToTarget = Vector3.Distance(transform.position, target.position);
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

        for (int i = 0; i < targetInViewRadius.Length; i++)
        {
            Transform tempTarget = targetInViewRadius[i].transform;
            directionToTarget = (tempTarget.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2)
            {
                distanceToTarget = Vector3.Distance(transform.position, tempTarget.position);
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask))
                {
                    visibleTargets.Add(tempTarget);
                    target = tempTarget;
                    IsWithinSight = targetInViewRadius.Length > 0;
                }
            }
        }

        if (targetInViewRadius.Length == 0)
        {
            visibleTargets.Clear();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Projectile projectile;
        if ((projectile = other.transform.GetComponent<Projectile>()) != null)
        {
            GetComponent<HealthHandler>().TakeDamage(projectile.DamageDealer);
        }
    }

    public Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
            angleInDegrees += transform.eulerAngles.y;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0f, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public void Initialize(object[] parameters)
    {
        if (parameters.Length > 0)
        {
            wayPointSystem.spreadRadius = (float)parameters[0];
            wayPointSystem.PositionWayPoints(wayPointSystem.AssignLocalPosition);
        }
        if (parameters.Length > 1)
        {
            root.shouldBeCulled = (bool)parameters[1];
        }
        if (parameters.Length > 2)
        {
            wayPointSystem.AssignWayPoints(((Vector3[])parameters[2]).ToList());
        }
    }
}