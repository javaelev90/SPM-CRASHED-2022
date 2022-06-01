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
    private bool isAttacking;
    private Vector3 wayPoint;

    AudioSource source;
    public AudioClip walk;
    public AudioClip angry;
    public AudioClip attack;

    // Start is called before the first frame update
    void Start()
    {
        timeCounterGas = timeToGas;
        timeCounterMelee = timeToMelee;
        timeCounterWaypoint = timeToWayPoint;
        wayPoint = wayPointSystem.GetNewPosition;
        source = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        base.OnEnable();
        if (IsMasterClient)
        {
            InvokeRepeating(nameof(PoisonGas), timeToGas, timeToGas);
        }
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

        if (agent.isOnNavMesh)
        {
            animator.SetBool("IsWalking", (Mathf.Abs(agent.velocity.x) > 0f || Mathf.Abs(agent.velocity.z) > 0f));
        }

        if(timeCounterMelee > 0f && !isAttacking)
        {
            timeCounterMelee -= Time.deltaTime;
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
        if (!isAttacking)
        {
            source.Play();

            if (distanceToTarget < maxMeleeRadius)
            {
                if (agent.isOnNavMesh)
                {
                    agent.isStopped = true;
                    if (timeCounterMelee <= 0f) Hit();
                }
            }
            else
            {
                if (agent.isOnNavMesh) agent.isStopped = false;
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

    private void Hit()
    {
        animator.SetBool("IsHitting", true);
        timeCounterMelee = timeToMelee;
        isAttacking = true;
        source.PlayOneShot(angry);
    }

    private void DealDamage()
    {
        animator.SetBool("IsHitting", false);
        

        if(IsMasterClient)
        {
            Vector3 hitPosition = transform.TransformPoint(Vector3.RotateTowards(new Vector3(0f, 1.6f, maxMeleeRadius / 2), new Vector3(maxMeleeRadius / 2, 1.6f, maxMeleeRadius / 2), 2f, 10f));
            Collider[] playersHitByMelee = Physics.OverlapSphere(hitPosition, maxMeleeRadius / 2, targetMask);

            foreach (Collider player in playersHitByMelee)
            {
                Debug.Log("Player hit");
                HealthHandler healthHandler = player.gameObject.GetComponent<HealthHandler>();
                if (healthHandler != null)
                {
                    healthHandler.TakeDamage(hitDamage);
                }
            }
        }

        isAttacking = false;
    }

    private void PoisonGas()
    {
        Collider[] playersHitByGas = Physics.OverlapSphere(transform.position, gasRadius, targetMask);

        foreach (Collider player in playersHitByGas)
        {
            HealthHandler healthHandler = player.gameObject.GetComponent<HealthHandler>();
            if (healthHandler != null)
            {
                healthHandler.TakeDamage(poisonDamage);
            }
        }
    }

    private void OnDrawGizmos()
    {
        //Debug.DrawLine(transform.position, transform.position + transform.forward * 5f, Color.blue);
        //Debug.DrawLine(transform.position, transform.position + directionToTarget * 5f, Color.red);
        //Gizmos.DrawWireSphere(transform.position, viewRadius);
        //Gizmos.color = Color.green;
        //Gizmos.DrawWireSphere(transform.position, minMeleeRadius);
        //Gizmos.DrawWireSphere(transform.position, maxMeleeRadius);
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(transform.position, gasRadius);

        //Gizmos.DrawSphere(-(directionToTarget * viewRadius - transform.position), 0.1f);
        //Debug.DrawLine(transform.position, -(directionToTarget * viewRadius - transform.position), Color.cyan);
    }
}
