using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using EventCallbacksSystem;
using UnityEngine.VFX;

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
    [SerializeField] private int turretDamageWhenUsing;
    [SerializeField] private int turretHealthIncreaseAtUpgrade;
    [SerializeField] public Transform useTurretPosition;
    [SerializeField] public Transform useTurretBody;
    [SerializeField] private float rangeWithEngineer;
    [SerializeField] private float rangeWhenAutomatic;

    public AudioSource source;
    public AudioClip clip;

    private GameObject emptyTarget;
    private Engineer engineerRef;

    public bool IsPlaced { get; set; }
    private float counter;
    //private bool isMine;
    public bool isCurrent;

    // Start is called before the first frame update
    void Awake()
    {
        counter = fireTimer;
        //isMine = photonView.IsMine;
        emptyTarget = new GameObject();
        isCurrent = false;
        emptyTarget.transform.position = transform.forward * 3f;
        engineerRef = FindObjectOfType<Engineer>();
        source = GetComponent<AudioSource>();
        source.volume = Random.Range(0.8f, 2);
        source.pitch = Random.Range(0.8f, 1.4f);
    }

    private void Start()
    {
        EventSystem.Instance.RegisterListener<TurretDamageUpgradeEvent>(DamageUpgrade);
    }

    public void DamageUpgrade(TurretDamageUpgradeEvent turretDamageUpgrade)
    {
        turretDamage += turretDamageUpgrade.UpgradeAmount;
        turretDamageWhenUsing += turretDamageUpgrade.UpgradeAmount;
    }

    private void FindTargets()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, enemyLayer);
        if (colliders.Length > 0)
        {
            currentTarget = colliders[0].transform;
            //Debug.Log("Turret current target: "currentTarget);

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
            Transform turretBodyTransform = turretBody.transform;
            turretBodyTransform.LookAt(currentTarget, Vector3.up);
            //turretBody.transform.rotation = Quaternion.Slerp(transform.rotation, rotateTo, 1f);

            ClampRotBody();
        }

        if (colliders.Length == 0)
        {
            currentTarget = emptyTarget.transform;
        }
    }

    public void ClampRotBody()
    {
        // Clamp rotation so it doesn't go all over the place and end up upside down
        Vector3 pivotRotation = turretBody.transform.eulerAngles;
        pivotRotation.x = Mathf.Clamp(pivotRotation.x, -60f, 80f);
        pivotRotation.z = Mathf.Clamp(pivotRotation.z, 0f, 0f);
        turretBody.transform.eulerAngles = pivotRotation;
    }



    void Update()
    {
        if (transform.GetComponent<HealthHandler>().isAlive == true)
        {
            // If the turret is in auto-mode (targets enemies automatically)
            if (engineerRef.isUsingTurret == false)
            {
                FindTargets();

                if (currentTarget.position != emptyTarget.transform.position)
                {
                    counter -= Time.deltaTime;
                    TurretShoot();
                }
            }

            // If the Engineer is using the turret and shooting
            else
            {
                counter -= Time.deltaTime;
                if (engineerRef.isShootingTurret == true)
                {
                    TurretShoot();
                }
            }
        }


        Debug.DrawRay(turretMuzzlePoint.transform.position, turretBody.transform.rotation * Vector3.forward * 8f);
    }


    [SerializeField] VisualEffect muzzlePosition;
    public void TurretShoot()
    {
        if (counter <= 0f)
        {
            /*
            // Shoot projectile
            GameObject bullet = PhotonNetwork.Instantiate(GlobalSettings.MiscPath + "Bullet", turretMuzzlePoint.transform.position, turretBody.transform.rotation);
            Projectile projectile = bullet.GetComponent<Projectile>();
            projectile.Velocity = turretBody.transform.rotation * Vector3.forward * 100f;
            projectile.DamageDealer = turretDamage;
            projectile.IsShot = true;
            counter = fireTimer;
            source.PlayOneShot(clip);
            source.volume = Random.Range(0.8f, 2);
            source.pitch = Random.Range(0.8f, 1.4f);
            Debug.Log("Is shooting");
            */
            
            if(Physics.Raycast(turretMuzzlePoint.transform.position, turretBody.transform.rotation * Vector3.forward, out RaycastHit hitInfo, engineerRef.isUsingTurret ? rangeWithEngineer : rangeWhenAutomatic, enemyLayer))
            {
                HealthHandler healthHandler = hitInfo.transform.GetComponent<HealthHandler>();
                if (healthHandler)
                {
                    if (engineerRef.isUsingTurret)
                    {
                        healthHandler.TakeDamage(turretDamageWhenUsing);
                    }
                    else
                    {
                        healthHandler.TakeDamage(turretDamage);
                    }
                    
                }

                AIBaseLogic ai = hitInfo.transform.GetComponent<AIBaseLogic>();
                if (ai)
                {
                    ai.FindAttackingTarget(transform);
                }
            }

            muzzlePosition.Play();
            counter = fireTimer;
            source.PlayOneShot(clip);
            source.volume = Random.Range(0.8f, 2);
            source.pitch = Random.Range(0.8f, 1.4f);
            Debug.Log("Is shooting");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}

