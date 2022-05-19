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
    private G_SnailExplosion snailEffects;

    private float timeCounterWaypoint;
    private float timeCounterExplosion;
    private Vector3 wayPoint;

    AudioSource source;
    public AudioClip walk;
    public AudioClip explode;
    public AudioClip attack;

    // Start is called before the first frame update
    void Start()
    {
        minBlowUpRadius = viewRadius / 4f;
        maxBlowUpRadius = viewRadius / 1.5f;
        wayPoint = wayPointSystem.GetNewPosition;
        timeCounterExplosion = timeToExplosion;
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

        if (distanceToTarget < maxBlowUpRadius && minBlowUpRadius < distanceToTarget)
        {
            if (agent.isOnNavMesh) agent.isStopped = true;
            BlowUp();
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

    public void BlowUp()
    {
        GameObject explosion = Instantiate(explosionEffects, transform.position, Quaternion.identity);
        snailEffects = explosion.GetComponent<G_SnailExplosion>();
        timeCounterExplosion -= Time.deltaTime;
        photonView.RPC(nameof(ReadyToExplode), RpcTarget.All);
        if (timeCounterExplosion <= 0f)
        {
            Collider[] targets = Physics.OverlapSphere(transform.position, maxBlowUpRadius, targetMask);
            if (targets.Length > 0)
            {
                foreach (Collider coll in targets)
                {
                    HealthHandler healthHandler = coll.transform.GetComponent<HealthHandler>();
                    SlugEnemy enemy;
                    if (healthHandler != null)
                    {
                        enemy = coll.GetComponent<SlugEnemy>();
                        if (enemy)
                        {
                            enemy.BlowUp();
                            return;
                        }
                        healthHandler.TakeDamage(explosionDamage);
                    }
                }
            }
            photonView.RPC(nameof(Explode), RpcTarget.All);
            root.DeSpawn();
            source.PlayOneShot(explode);
            timeCounterExplosion = timeToExplosion;
        }
    }

    [PunRPC]
    private void Explode()
    {
        snailEffects.Explode();
    }

    [PunRPC]
    private void ReadyToExplode()
    {
        snailEffects.snail = gameObject;
        snailEffects.ReadyToExplode();
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
