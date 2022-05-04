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
    private float timer;
    private GameObject fleePos;

    private bool isFleeing;
    private void Start()
    {
        minThrowRange = viewRadius / 2f;
        maxThrowRange = viewRadius / 1.5f;
        deadZoneRange = viewRadius / 2.01f;
        fleePos = new GameObject();
        fleePos.name = "Fleeposition";
    }

    protected override void Update()
    {
        base.Update();

        if (IsStunned)
        {
            return;
        }
        else
        {
            // om spelare syns ska den räkna ut distansen och agera därefter
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

            if (isFleeing)
            {
                FleeToPosition();
            }
        }
    }

    private void Throw()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            timer = timeToThrow;
            GameObject bull = Instantiate(bullet, transform.position, Quaternion.identity);
            Projectile proj = bull.GetComponent<Projectile>();
            proj.Velocity += directionToTarget * 10f;
            proj.IsShot = true;
        }
    }

    private void FleeToPosition()
    {
        agent.destination = fleePos.transform.position;
    }

    private float DistanceToTarget(Vector3 position)
    {
        return Vector3.Distance(transform.position, position);
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
        }
        agent.destination = visibleTargets[0].position;
        Rotate();
    }

    private void Rotate()
    {
        float dot = Vector3.Dot(transform.forward, directionToTarget);
        if (dot < 0.6f)
        {
            Quaternion rotateTo = Quaternion.LookRotation(directionToTarget, transform.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotateTo, rotationSpeed * Time.deltaTime);
        }
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

    void OnGUI()
    {
        float dot = Vector3.Dot(transform.forward, directionToTarget);

        GUI.Label(new Rect(10, 10, 100, 20), "Dot: " + dot);
    }
}
