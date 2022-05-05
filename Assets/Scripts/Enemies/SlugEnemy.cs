using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlugEnemy : AIBaseLogic
{
    [SerializeField] private float minBlowUpRadius;
    [SerializeField] private float maxBlowUpRadius;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float timeToWayPoint;
    [SerializeField] private float timeToExplosion;
    [SerializeField] private int explosionDamage;

    private float timeCounterWaypoint;
    private float timeCounterExplosion;
    private Transform wayPoint;

    // Start is called before the first frame update
    void Start()
    {
        minBlowUpRadius = viewRadius / 4f;
        maxBlowUpRadius = viewRadius / 1.5f;
        wayPoint = wayPointSystem.RandomPosition;
        timeCounterExplosion = timeToExplosion;
        Debug.Log("root " + root.name);
    }

    // Update is called once per frame
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

    private void MoveToWayPoint()
    {
        timeCounterWaypoint -= Time.deltaTime;
        if (timeCounterWaypoint <= 0f)
        {
            wayPoint = wayPointSystem.NewRandomPosition;
            timeCounterWaypoint = timeToWayPoint;
        }

        if (agent.isOnNavMesh)
            agent.destination = wayPoint.position;
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

    private void Move()
    {

        if (distanceToTarget < maxBlowUpRadius && minBlowUpRadius < distanceToTarget)
        {
            agent.isStopped = true;
            BlowUp();
        }
        else
        {
            agent.isStopped = false;
        }

        if (agent.isOnNavMesh)
        {
            agent.destination = target.position;
            Rotate();
        }
    }

    public void BlowUp()
    {
        timeCounterExplosion -= Time.deltaTime;
        if (timeCounterExplosion <= 0f)
        {
            Collider[] targets = Physics.OverlapSphere(transform.position, maxBlowUpRadius, targetMask);
            if (targets.Length > 0)
            {
                foreach (Collider coll in targets)
                {
                    coll.transform.GetComponent<HealthHandler>().TakeDamage(explosionDamage);
                    Debug.Log("Blow player up");
                }
            }
            root.DeSpawn();
            timeCounterExplosion = timeToExplosion;
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
        Gizmos.DrawWireSphere(transform.position, minBlowUpRadius);
        Gizmos.DrawWireSphere(transform.position, maxBlowUpRadius);
        Gizmos.color = Color.red;

        Gizmos.DrawSphere(-(directionToTarget * viewRadius - transform.position), 0.1f);
        Debug.DrawLine(transform.position, -(directionToTarget * viewRadius - transform.position), Color.cyan);
    }
}
