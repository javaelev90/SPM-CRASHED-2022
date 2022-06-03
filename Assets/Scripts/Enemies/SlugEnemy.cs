using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SlugEnemy : AIBaseLogic
{
    [SerializeField] private float minBlowUpRadius;
    [SerializeField] private float maxBlowUpRadius;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float timeToWayPoint;
    [SerializeField] private float timeToExplosion;
    [SerializeField] private int explosionDamage;
    [SerializeField] private GameObject explosionEffects;
    [SerializeField] private LayerMask AttackableTargets;
    private G_SnailExplosion snailEffects;

    private float timeCounterWaypoint;
    private float timeCounterExplosion;
    private float timeCounterChainReaction;
    private Vector3 wayPoint;
    private bool isBlowingUp;
    private bool canBlowUp;

    AudioSource source;
    public AudioClip walk;
    public AudioClip explode;
    public AudioClip attack;

    // Start is called before the first frame update
    void Start()
    {
        wayPoint = wayPointSystem.GetNewPosition;
        timeCounterExplosion = timeToExplosion;
        source = GetComponent<AudioSource>();
    }

    private void OnDisable()
    {
        base.OnDisable();

        animator.SetBool("IsWalking", false);
        animator.SetBool("IsBlowingUp", false);
        isBlowingUp = false;
        canBlowUp = false;
        distanceToTarget = maxBlowUpRadius + 5;
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

        if (agent.isOnNavMesh == true)
        {
            animator.SetBool("IsWalking", (Mathf.Abs(agent.velocity.x) > 0f || Mathf.Abs(agent.velocity.z) > 0f));
        }


        if (timeCounterExplosion > 0f && canBlowUp == true)
        {
            timeCounterExplosion -= Time.deltaTime;
            animator.SetBool("IsBlowingUp", canBlowUp);
            if (timeCounterExplosion <= 0f)
            {
                BlowUp(true);
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
            source.Play();
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
        if (canBlowUp == false)
        {
            if (distanceToTarget < maxBlowUpRadius)
            {
                if (agent.isOnNavMesh) agent.isStopped = true;
                canBlowUp = true;
            }
            else
            {
                if (agent.isOnNavMesh) agent.isStopped = false;
                source.Play();
            }

            if (agent.isOnNavMesh && target != null && isBlowingUp == false)
            {
                agent.destination = target.position;
                Rotate();
            }
        }
    }


    private void BlowUp(bool isTriggeredToBlowUp)
    {
        if (isTriggeredToBlowUp && canBlowUp)
        {
            canBlowUp = false;
            animator.SetBool("IsBlowingUp", canBlowUp);
            timeCounterExplosion = timeToExplosion;
            
            Collider[] targets = Physics.OverlapSphere(transform.position, maxBlowUpRadius, AttackableTargets);
            if (targets.Length > 0)
            {
                foreach (Collider coll in targets)
                {
                    HealthHandler healthHandler = coll.transform.GetComponent<HealthHandler>();
                    SlugEnemy enemy;
                    if (healthHandler != null)
                    {
                        enemy = coll.GetComponent<SlugEnemy>(); // reset after debug
                        if (enemy && !(enemy.gameObject.GetInstanceID() == gameObject.GetInstanceID()))
                        {
                            enemy.BlowUp(true);
                        }

                        if (!enemy)
                        {
                            healthHandler.TakeDamage(explosionDamage);
                        }
                    }
                }
            }
            photonView.RPC(nameof(Explode), RpcTarget.All); // reset after debug
            source.PlayOneShot(explode);
            root.DeSpawn();
        }
    }

    // Blows up immediately, called on death. Deals damage to other slugs. Despawn happens in EnemyHealthHandler.Die()
    public void BlowUp()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, maxBlowUpRadius, AttackableTargets);
        isBlowingUp = true;
        if (targets.Length > 0)
        {
            foreach (Collider coll in targets)
            {
                HealthHandler healthHandler = coll.transform.GetComponent<HealthHandler>();
                if (coll.gameObject.GetInstanceID() == gameObject.GetInstanceID())
                {
                    continue;
                }
                if (healthHandler != null)
                {
                    healthHandler.TakeDamage(explosionDamage);
                }
            }
        }
        photonView.RPC(nameof(Explode), RpcTarget.All);
        source.PlayOneShot(explode);
        animator.SetBool("IsBlowingUp", false);
        isBlowingUp = false;
    }

    [PunRPC]
    private void Explode()
    {
        GameObject explosion = Instantiate(explosionEffects, transform.position, Quaternion.identity);
        snailEffects = explosion.GetComponent<G_SnailExplosion>();
        source.PlayOneShot(explode);
        snailEffects.snail = gameObject;
        snailEffects.Explode();

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
