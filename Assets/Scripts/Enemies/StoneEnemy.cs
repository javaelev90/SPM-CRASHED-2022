using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneEnemy : AIBaseLogic
{
    [SerializeField] private float minThrowRange;
    [SerializeField] private float maxThrowRange;
    [SerializeField] private float deadZoneRange;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float stoppingDistance;
    [SerializeField] private GameObject stone;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform throwTransform;
    [SerializeField] private float timeToThrow;
    [SerializeField] private int stoneDamage;
    [SerializeField] private float timeToWayPoint;
    [SerializeField] private float stoneProjectileAngle = 45f;
    [SerializeField] private float throwingMultiplier = 20f;
    private float timeCounterWaypoint;
    private Vector3 wayPoint;
    private float timer;
    private GameObject fleePos;
    private StoneProjectile stoneProjectile;
    private bool isThrowing;

    private bool isFleeing;

    AudioSource source;
    public AudioClip walk;
    public AudioClip angry;
    public AudioClip attack;
    private void Start()
    {
        fleePos = new GameObject();
        fleePos.name = "Fleeposition";
        wayPoint = wayPointSystem.GetNewPosition;
        source = GetComponent<AudioSource>();
    }

    protected override void Update()
    {
        if (IsStunned)
        {
            agent.isStopped = true;
            timeStunnedCounter -= Time.deltaTime;
            if (timeStunnedCounter <= 0f)
            {
                IsAttacked = true;
                timeStunnedCounter = timeStunned;
                IsStunned = false;
                agent.isStopped = false;
            }
        }
        else
        {
            if (!IsAttacked)
            {
                AggroBasedOnSight();
                AttackBasedOnSight();
            }

            if (IsAttacked)
            {
                AggroBasedOnAttack();
            }

            if (!IsAggresive && !IsAttacked)
            {
                MoveToWayPoint();
            }
        }

        if (agent.isOnNavMesh)
            animator.SetBool("IsWalking", (Mathf.Abs(agent.velocity.x) > 0f || Mathf.Abs(agent.velocity.z) > 0f));


        if (stoneProjectile != null && stoneProjectile.IsThrown == false)
        {
            stoneProjectile.transform.position = throwTransform.position;
            stoneProjectile.transform.forward = directionToTarget.normalized;
        }

    }

    private void MoveToWayPoint()
    {
        if (eventTarget)
        {
            agent.SetDestination(eventTarget.position);
        }
        else
        {
            timeCounterWaypoint -= Time.deltaTime;
            if (timeCounterWaypoint <= 0f)
            {
                wayPoint = wayPointSystem.GetNewPosition;
                timeCounterWaypoint = timeToWayPoint;
            }

            if (agent.isOnNavMesh)
                agent.destination = wayPoint;
        }
    }

    private void AggroBasedOnAttack()
    {
        if (IsAttacked)
        {
            timeCounterAttacked -= Time.deltaTime;
            if (timeCounterAttacked <= 0f)
            {
                IsAttacked = false;
            }

            Move();
        }
    }

    private void AggroBasedOnSight()
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

    private void AttackBasedOnSight()
    {
        if (IsWithinSight)
        {
            if (IsAggresive)
            {
                Move();
            }
        }

    }

    private void Throw()
    {
        if (animator.GetBool("IsThrowing") == false)
        {
            animator.SetBool("IsThrowing", true);
            isThrowing = true;
        }

        Rotate();
    }

    private void ThrowStone()
    {
        animator.SetBool("IsThrowing", false);
        isThrowing = false;

        if (target != null && stoneProjectile != null)
        {
            Vector3 directionOfProjectile = target.transform.position - transform.position;
            //float height = directionOfProjectile.y;
            directionOfProjectile.y = 0f;

            float angleToRadians = stoneProjectileAngle * Mathf.Deg2Rad;
            directionOfProjectile.y = throwingMultiplier * Mathf.Tan(angleToRadians);
            //throwingMultiplier += height / Mathf.Tan(angleToRadians);
            float velocity = Mathf.Sqrt(throwingMultiplier * Physics.gravity.magnitude / Mathf.Sin(2 * angleToRadians));

            if (IsMasterClient)
            {
                stoneProjectile.IsThrown = true;
                stoneProjectile.transform.forward = directionOfProjectile.normalized;
                stoneProjectile.DamageDealer = stoneDamage;
                stoneProjectile.GetComponent<Rigidbody>().velocity = velocity * directionToTarget.normalized;
            }
        }

    }

    private void PickupStone()
    {
        if (IsMasterClient)
        {
            if (stoneProjectile == null || stoneProjectile.IsThrown == true)
            {
                GameObject stoneObject = PhotonNetwork.Instantiate(GlobalSettings.MiscPath + stone.name, throwTransform.position, Quaternion.identity);
                stoneProjectile = stoneObject.GetComponent<StoneProjectile>();
            }
        }
    }

    private void Move()
    {
        if (isThrowing == false)
        {
            if (distanceToTarget < maxThrowRange /*&& minThrowRange < distanceToTarget*/)
            {
                isFleeing = false;
                if (agent.isOnNavMesh) agent.isStopped = true;
                Throw();
            }
            else
            {
                if (agent.isOnNavMesh) agent.isStopped = false;
                source.Play();
            }

            if (agent.isOnNavMesh && target != null)
            {
                agent.destination = target.position;
                Rotate();
            }
        }
    }

    private void Rotate()
    {
        float dot = Vector3.Dot(transform.forward, directionToTarget);
        Quaternion rotateTo = Quaternion.LookRotation(directionToTarget, transform.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotateTo, rotationSpeed * Time.deltaTime);
    }
    private void OnDrawGizmos()
    {
        Debug.DrawLine(transform.position, transform.position + transform.forward * 5f, Color.blue);
        Debug.DrawLine(transform.position, transform.position + directionToTarget * 5f, Color.red);
        Gizmos.DrawWireSphere(transform.position, viewRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, minThrowRange);
        Gizmos.DrawWireSphere(transform.position, maxThrowRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, deadZoneRange);

        Gizmos.DrawSphere(-(directionToTarget * viewRadius - transform.position), 0.1f);
        Debug.DrawLine(transform.position, -(directionToTarget * viewRadius - transform.position), Color.cyan);
    }

}
