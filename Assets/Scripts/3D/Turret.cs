using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Turret : MonoBehaviourPunCallbacks
{
    [Header("Turret Properties")]
    [SerializeField] private GameObject muzzlePoint;
    [SerializeField] private GameObject turretBody;
    [SerializeField] private Transform currentTarget;
    [SerializeField] private Transform newTarget;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Vector3 rotation;
    [SerializeField] private float radius = 5f;
    [SerializeField] private float closestDistance = 5f;
    [SerializeField] private PhotonView bullet;
    [SerializeField] private float fireTimer = 1f;
    private string pathBullet = "Prefab/Player/Bullet";
    public bool IsPlaced { get; set; }
    private float counter;
    private bool isMine;

    // Start is called before the first frame update
    void Awake()
    {
        counter = fireTimer;
        isMine = photonView.IsMine;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMine)
        {
            if (IsPlaced)
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, radius, enemyLayer);
                if (colliders.Length > 0)
                {
                    currentTarget = colliders[0].transform;

                    foreach (Collider col in colliders)
                    {
                        newTarget = col.transform;
                        if (Vector3.Distance(newTarget.position, transform.position) < closestDistance)
                        {
                            currentTarget = newTarget;
                        }
                    }
                }

                if (currentTarget != null)
                {

                    if (Vector3.Distance(currentTarget.transform.position, transform.position) > radius)
                    {
                        currentTarget = transform;
                    }

                    turretBody.transform.LookAt(currentTarget);

                    counter -= Time.deltaTime;
                    if (counter <= 0f)
                    {
                        GameObject bullet = PhotonNetwork.Instantiate(pathBullet, muzzlePoint.transform.position, turretBody.transform.rotation);
                        Projectile projectile = bullet.GetComponent<Projectile>();
                        projectile.Velocity = turretBody.transform.rotation * Vector3.forward * 100f;
                        projectile.IsShot = true;
                        counter = fireTimer;
                    }
                }
                else
                {
                    turretBody.transform.LookAt(Vector3.forward, Vector3.up);
                }

                Debug.DrawRay(muzzlePoint.transform.position, turretBody.transform.rotation * Vector3.forward * 8f);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
