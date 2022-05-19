using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasEnemy : AIBaseLogic
{
    [SerializeField] private float gasRadius;
    [SerializeField] private float minMeleeRadius;
    [SerializeField] private float maxMeleeRadius;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float timeToMelee;
    [SerializeField] private float timeToGas;
    [SerializeField] private int poisonDamage;
    [SerializeField] private int hitDamage;
    [SerializeField] private float timeToWayPoint;
    private float timeCounterWaypoint;
    private float timeCounterGas;
    private float timeCounterMelee;
    private Vector3 wayPoint;

    AudioSource source;
    public AudioClip walk;
    public AudioClip angry;
    public AudioClip attack;

    // Start is called before the first frame update
    void Start()
    {
        minMeleeRadius = viewRadius / 5f;
        maxMeleeRadius = viewRadius / 2.5f;
        timeCounterGas = timeToGas;
        timeCounterMelee = timeToMelee;
        timeCounterWaypoint = timeToWayPoint;
        wayPoint = wayPointSystem.GetNewPosition;
        source = GetComponent<AudioSource>();

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
        source.Play();

        if (distanceToTarget < gasRadius)
        {
            PoisonGas();
        }

        if (distanceToTarget < maxMeleeRadius && minMeleeRadius < distanceToTarget)
        {
            if (agent.isOnNavMesh) agent.isStopped = true;
            Hit();
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

    private void Rotate()
    {
        float dot = Vector3.Dot(transform.forward, directionToTarget);
        Quaternion rotateTo = Quaternion.LookRotation(directionToTarget, transform.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotateTo, rotationSpeed * Time.deltaTime);
    }

    private void Hit()
    {
        timeCounterMelee -= Time.deltaTime;
        if (timeCounterMelee <= 0f)
        {
            if (IsMasterClient)
                target.GetComponent<HealthHandler>().TakeDamage(hitDamage);

            timeCounterMelee = timeToMelee;
        }
    }

    private void PoisonGas()
    {
        timeCounterGas -= Time.deltaTime;
        if (timeCounterGas <= 0f)
        {
            if (IsMasterClient)
                target.GetComponent<HealthHandler>().TakeDamage(poisonDamage);

            timeCounterGas = timeToGas;
            source.PlayOneShot(angry);
        }
    }

    private void OnDrawGizmos()
    {
        Debug.DrawLine(transform.position, transform.position + transform.forward * 5f, Color.blue);
        Debug.DrawLine(transform.position, transform.position + directionToTarget * 5f, Color.red);
        Gizmos.DrawWireSphere(transform.position, viewRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, minMeleeRadius);
        Gizmos.DrawWireSphere(transform.position, maxMeleeRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, gasRadius);

        Gizmos.DrawSphere(-(directionToTarget * viewRadius - transform.position), 0.1f);
        Debug.DrawLine(transform.position, -(directionToTarget * viewRadius - transform.position), Color.cyan);
    }
}
