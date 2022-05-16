using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using EventCallbacksSystem;

public class Turret : MonoBehaviourPunCallbacks
{
    [Header("Turret Properties")]
    [SerializeField] private GameObject turretMuzzlePoint;
    [SerializeField] private GameObject turretBody;
    [SerializeField] private Transform currentTarget;
    [SerializeField] private Transform newTarget;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Vector3 rotation;
    [SerializeField] private float radius = 5f;
    [SerializeField] private float closestDistance = 5f;
    [SerializeField] private PhotonView bullet;
    [SerializeField] private float fireTimer = 1f;
    [SerializeField] private int turretDamage;
    [SerializeField] private int turretDamageIncreaseAtUpgrade;
    [SerializeField] private int turretHealthIncreaseAtUpgrade;

    private GameObject emptyTarget;
    [SerializeField] public Transform useTurretPosition;
    public bool IsPlaced { get; set; }
    private float counter;
    private bool isMine;

    private AudioSource source;

    public AudioClip clip;

    // Start is called before the first frame update
    void Awake()
    {
        counter = fireTimer;
        isMine = photonView.IsMine;
        emptyTarget = new GameObject();
        emptyTarget.transform.position = transform.forward * 3f;
        EventSystem.Instance.RegisterListener<TurretDamageUpgradeEvent>(DamageUpgrade);
        EventSystem.Instance.RegisterListener<TurretHealthUpgradeEvent>(HealthUpgrade);
        source = GetComponent<AudioSource>();

    }

    public void DamageUpgrade(TurretDamageUpgradeEvent turretDamageUpgrade)
    {
        turretDamage += turretDamageIncreaseAtUpgrade;
    }

    public void HealthUpgrade(TurretHealthUpgradeEvent turretDamageUpgrade)
    {
        //... += turretHealthIncreaseAtUpgrade;
    }



    private void FindTargets()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, enemyLayer);
        if (colliders.Length > 0)
        {
            currentTarget = colliders[0].transform;
            Debug.Log(currentTarget);

            foreach (Collider col in colliders)
            {
                newTarget = col.transform;
                if (Vector3.Distance(newTarget.position, transform.position) < closestDistance)
                {
                    currentTarget = newTarget;
                }
            }


            Vector3 direction = (currentTarget.position - transform.position).normalized;
            Quaternion rotateTo = Quaternion.LookRotation(direction, turretBody.transform.up);
            turretBody.transform.rotation = Quaternion.Slerp(transform.rotation, rotateTo, 1f);

            // Clamp rotation so it doesn't go all over the place and end up uppside down
            rotateTo.x = ClampAngle(rotateTo.x, -90f, 90f);
            rotateTo.z = ClampAngle(rotateTo.z, -90f, 90f);

        }

        if (colliders.Length == 0)
        {
            currentTarget = emptyTarget.transform;
        }
    }

    float ClampAngle(float angle, float from, float to)
    {
        // accepts e.g. -80, 80
        if (angle < 0f) angle = 360 + angle;
        if (angle > 180f) return Mathf.Max(angle, 360 + from);
        return Mathf.Min(angle, to);
    }

    // Update is called once per frame
    void Update()
    {

        FindTargets();

        if (currentTarget.position != emptyTarget.transform.position)
        {
            //if (Vector3.Distance(currentTarget.transform.position, transform.position) > radius)
            //{
            //    currentTarget = transform;
            //}

            counter -= Time.deltaTime;
            if (counter <= 0f)
            {
                GameObject bullet = PhotonNetwork.Instantiate(GlobalSettings.MiscPath + "Bullet", turretMuzzlePoint.transform.position, turretBody.transform.rotation);
                Projectile projectile = bullet.GetComponent<Projectile>();
                projectile.Velocity = turretBody.transform.rotation * Vector3.forward * 100f;
                projectile.DamageDealer = turretDamage;
                projectile.IsShot = true;
                counter = fireTimer;
                source.PlayOneShot(clip);
                //Debug.Log("Is shooting");
            }
        }
        else
        {
            //turretBody.transform.LookAt(Vector3.forward, Vector3.up);
        }



        Debug.DrawRay(turretMuzzlePoint.transform.position, turretBody.transform.rotation * Vector3.forward * 8f);
        //if (IsPlaced)
        //{
        //}

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
