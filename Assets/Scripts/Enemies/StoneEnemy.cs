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
    [SerializeField] private GameObject bullet;
    [SerializeField] private float timeToThrow;
    [SerializeField] private int stoneDamage;
    [SerializeField] private float timeToWayPoint;
    private float timeCounterWaypoint;
    private Vector3 wayPoint;
    private float timer;
    private GameObject fleePos;

    private bool isFleeing;

    AudioSource source;
    public AudioClip walk;
    public AudioClip angry;
    public AudioClip attack;
    private void Start()
    {
        minThrowRange = viewRadius / 2f;
        maxThrowRange = viewRadius / 1.5f;
        deadZoneRange = viewRadius / 2.01f;
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
                isFleeing = false;
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

            if (isFleeing)
            {
                FleeToPosition();
            }

            if (!IsAggresive && !IsAttacked)
            {
                MoveToWayPoint();
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

    private void AggroBasedOnAttack()
    {
        if (IsAttacked)
        {
            timeCounterAttacked -= Time.deltaTime;
            if (timeCounterAttacked <= 0f)
            {
                IsAttacked = false;
            }

            if (distanceToTarget < deadZoneRange)
            {
                fleePos.transform.position = transform.position + -(directionToTarget * viewRadius);
                isFleeing = true;
                agent.isStopped = false;
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
            if (distanceToTarget < deadZoneRange)
            {
                fleePos.transform.position = transform.position + -(directionToTarget * viewRadius);
                isFleeing = true;
                agent.isStopped = false;
                IsAggresive = false;
            }

            if (IsAggresive)
            {
                Move();
            }
        }
        else
        {
            isFleeing = false;
        }
    }

    private void Throw()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            timer = timeToThrow;
            GameObject bull = PhotonNetwork.Instantiate("Prefabs/" + bullet.name, transform.position, Quaternion.identity);
            Projectile proj = bull.GetComponent<Projectile>();
            proj.DamageDealer = stoneDamage;
            proj.Velocity += directionToTarget * 10f;
            proj.IsShot = true;
            source.PlayOneShot(attack);
        }
    }

    private void FleeToPosition()
    {
        agent.destination = fleePos.transform.position;
    }

    private void Move()
    {
        if (distanceToTarget < maxThrowRange && minThrowRange < distanceToTarget)
        {
            isFleeing = false;
            agent.isStopped = true;
            Throw();
        }
        else
        {
            agent.isStopped = false;
            //source.Play();
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
