using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovement : MonoBehaviour
{
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;
    public float delayToNewTarget = 1f;
    public float rotationSpeed = 5f;
    public float movementSpeed = 5f;

    public LayerMask obstacleMask;
    public LayerMask targetMask;

    public List<Transform> visibleTargets = new List<Transform>();
    public float distanceToTarget;
    public WayPointSystem wayPointSystem;
    private Vector3 directionToTarget;
    private float enemyBlowUpDistance;
    private bool isBlowingUp;

    private void Start()
    {
        StartCoroutine("FindTargetsWithDelay");
        Debug.Log(wayPointSystem != null ? "Not null" : "null");
        enemyBlowUpDistance = viewRadius / 2;
    }

    IEnumerator FindTargetsWithDelay()
    {
        while (true)
        {
            yield return new WaitForSeconds(delayToNewTarget);
            FindVisibleTargets();
        }
    }

    private void Update()
    {
        if (!isBlowingUp)
        {
            RotateToTarget();
            transform.Translate(0, 0, movementSpeed * Time.deltaTime);
        }

        if(visibleTargets.Count > 0 && distanceToTarget < enemyBlowUpDistance)
        {
            isBlowingUp = true;
        }
    }

    void BlowUp()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, enemyBlowUpDistance, targetMask);
        if(targets.Length > 0)
        {

        }
        isBlowingUp = false;
    }

    void FindVisibleTargets()
    {
        //visibleTargets.Clear();
        Collider[] targetInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        #region
        //for (int i = 0; i < targetInViewRadius.Length; i++)
        //{
        //    Transform target = targetInViewRadius[i].transform;
        //    directionToTarget = (target.position - transform.position).normalized;
        //    if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2)
        //    {
        //        distanceToTarget = Vector3.Distance(transform.position, target.position);
        //        if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask))
        //        {
        //            visibleTargets.Add(target);
        //        }
        //    }
        //}
        #endregion

        if (targetInViewRadius.Length > 0)
        {
            Transform target = targetInViewRadius[0].transform;
            directionToTarget = (target.position - transform.position).normalized;
            directionToTarget.y = 0f;
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
            visibleTargets.Clear();
    }

    void RotateToTarget()
    {
        if (visibleTargets.Count == 0 && wayPointSystem != null)
        {
            directionToTarget = (wayPointSystem.RandomPosition.position - transform.position).normalized;
        }

        Quaternion rotateTo = Quaternion.LookRotation(directionToTarget, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotateTo, rotationSpeed * Time.deltaTime);
    }

    public Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
            angleInDegrees += transform.eulerAngles.y;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0f, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
