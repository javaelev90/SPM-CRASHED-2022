using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using EventCallbacksSystem;

public class Engineer : Controller3D
{
    private float placeTimer = 1f;

    [Header("Turrets")]
    [SerializeField] private PhotonView turret;
    [SerializeField] private Transform turretPos;
    [SerializeField] private int maxTurretToSpawn = 3;
    [SerializeField] private int turretCount;
    [SerializeField] private Vector3 turretOffset;
    //[SerializeField] private int gooCostTurret;
    //[SerializeField] private int metalCostTurret;
    //[SerializeField] private GameObject turretObject;
    private bool canPutDownTurret;
    //private string pathTurret = "Resources/Prefabs/TurretAssembly";
    [SerializeField] private GameObject turretPrefab;
    [SerializeField] private GameObject greenGooPrefab;
    [SerializeField] private GameObject metalPrefab;
    [SerializeField] public LayerMask turretLayer;
    [SerializeField] private GameObject outlinedTurretPrefab;
    GameObject outlinedTurret;
    bool isPressed = false;
    public bool isUsingTurret { get; set; }
    [SerializeField] private Turret turretObj;
    Transform usePositionPos;
    // Queue for max turrets
    //Queue<GameObject> turrets = new Queue<GameObject>();
    List<GameObject> objects = new List<GameObject>();
    private bool isTryingToPlaceTurret;

    private  static bool textShown; 

    public GameObject uiObject;

    [Header("TurretRepair")]
    public TurretCost turretRepairCosts;
    [SerializeField] private int healthToAdd = 10;
    [SerializeField] private int healthToAddIfDead = 6;
    [SerializeField] private Animator atFullHealthText;
    [SerializeField] private Animator notEnoughResourcesToRepair;
    [SerializeField] private Animator notEnoughResourcesToBuild;

    [Header("TurretBuild")]
    public TurretCost turretBuildCosts;

    [Serializable]
    public class TurretCost
    {
        public int metalCost;
        public int gooCost;
    }


    [Header("Carry Ship Part")]
    /// <summary>
    /// PLAYER NEED A CHILD OBJECT CALLED CarryPos
    /// </summary>
    [SerializeField] private Transform destination;
    //[SerializeField] private Transform player;
    private GameObject[] shipPart;
    private float checkRadius = 5f;
    RaycastHit hit;

    [Header("Stun")]
    [SerializeField] Transform muzzlePosition;
    [SerializeField] float weaponRange = 15f;
    [SerializeField] float delayBetweenShots = 0.5f;
    [SerializeField] public LayerMask stunLayer;
    [SerializeField] private ParticleActivator shootingEffect;
    [SerializeField] GameObject hitPosition;
    [SerializeField] private AudioClip stunSound;
    [SerializeField] private AudioClip placeTurret;
    private StungunCoolDownEvent stunGunEvent;
    private AudioSource audioSource;

    private float shotCooldown = 0f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Wait(5));
        isUsingTurret = false;
        //playerActions = new PlayerInputActions();
        stunGunEvent = new StungunCoolDownEvent(delayBetweenShots);
        EventSystem.Instance.FireEvent(stunGunEvent);
        audioSource = GetComponent<AudioSource>();
        uiObject.gameObject.SetActive(false);
        //canvas.SetActive(false);
    }

    protected override void Awake()
    {
        base.Awake();
        textShown = false;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (PlayerControlsAreOn() == false) return;
        Cooldown();
        placeTimer -= Time.deltaTime;
        //PickUpShipPart();
        //OnTurretUse();
        if (isPressed)
        {
            OnPlacementStarted();
        }
        if (isUsingTurret)
        {
            transform.position = usePositionPos.transform.position;
        }
        ShowTurret();
        
        //TurretHandling();
        //Debug.Log(targetTime);
    }

    private void Cooldown()
    {
        if (OnCoolDown() == true)
        {
            shotCooldown -= Time.deltaTime;
        }
    }

    private bool OnCoolDown()
    {
        return shotCooldown >= 0;
    }

    public void StunEnemy()
    {
        if (PlayerControlsAreOn() == false) return;

        if (OnCoolDown() == false && isUsingTurret == false)
        {
            if (Physics.Raycast(muzzlePoint.transform.position, weaponRotation.transform.rotation * Vector3.forward * 10f, out hit, weaponRange, stunLayer))
            {
                AIBaseLogic aIBaseLogic = hit.transform.GetComponent<AIBaseLogic>();
                if (aIBaseLogic)
                {
                    //Debug.Log("Enemy stunned");
                    aIBaseLogic.StunnedBy(photonView.ViewID, hit.point, hit.normal);
                }
            }
            // Add cooldown time
            shotCooldown = delayBetweenShots;
            shootingEffect.PlayParticles();
            audioSource.PlayOneShot(stunSound);
            EventSystem.Instance.FireEvent(stunGunEvent);
        }

        if (OnCoolDown() == false)
        {
            EventSystem.Instance.FireEvent(stunGunEvent);
        }
    }

    public void OnPlacementStarted()
    {
        //Debug.Log("Show outline of turret where to place");

        canPutDownTurret = true;

        if (Physics.Raycast(turretPos.transform.position, Vector3.down, out hit, 3f, obstacleLayer))
        {
            if (isUsingTurret == false && canPutDownTurret && outlinedTurret != null)
            {
                //turretPos.position = hit.transform.position;
                //canvas.SetActive(true);
                Vector3 targetLocation = hit.point;
                //outlinedTurret.transform.position = targetLocation;
                //outlinedTurret.transform.position = hit.point;//turretPos.position;
                outlinedTurret.transform.SetPositionAndRotation(targetLocation, Quaternion.FromToRotation(Vector3.up, hit.normal));
            }
        }
    }

    public void OnPlaceTurret(InputAction.CallbackContext ctx)
    {
        if (PlayerControlsAreOn() == false) return;

        GameObject turretObject;
        isTryingToPlaceTurret = true;

        if (placeTimer <= 0.0f && isUsingTurret == false) // turretCount < maxTurretToSpawn && 
        {
            // Check if resources needed is in inventory
            InventorySystem inventorySystem = gameObject.GetComponent<InventorySystem>();
            if (inventorySystem.Amount<GreenGoo>() >= turretBuildCosts.gooCost && inventorySystem.Amount<Metal>() >= turretBuildCosts.metalCost)
            {
                if (ctx.started)
                {
                    //Debug.Log("Holding button should show turret outline");

                    outlinedTurret = PhotonNetwork.Instantiate("Prefabs/Equipment/" + outlinedTurretPrefab.name, turretPos.position, Quaternion.identity);//(pathTurret, turretPos.position, Quaternion.identity);
                    //canvas.SetActive(false);
                    isPressed = true;
                }

                if (ctx.canceled)
                {
                    //Debug.Log("released button should result in placing turret");

                    // If there are 3 placed turrets and a new one is placed, delete the first one
                    if (turretCount == maxTurretToSpawn && isTryingToPlaceTurret)
                    {
                        GameObject turretToDestroy = objects[0];
                        objects.Remove(turretToDestroy);
                        PhotonNetwork.Destroy(turretToDestroy);
                        turretCount--;
                    }


                    turretObject = CreateTurret(turretPos.position);

                    if (turretObject != null)
                    {
                        turretObject.transform.rotation = Quaternion.FromToRotation(turretObject.transform.up, Vector3.up) * turretObject.transform.rotation;
                        turretObject.transform.rotation = Quaternion.FromToRotation(turretObject.transform.forward, Vector3.forward) * turretObject.transform.rotation;
                        //if (hit.collider != null && hit.distance < 3f)
                        //Debug.Log("turret should be placed");

                        turretObject.transform.position = turretPos.transform.position;
                        turretObject.GetComponent<Turret>().IsPlaced = true;

                        // Remove resources
                        inventorySystem.Remove<Metal>(turretBuildCosts.metalCost);
                        inventorySystem.Remove<GreenGoo>(turretBuildCosts.gooCost);

                        // Add another turret to the queue and increase count of turrets
                        //turretCount++;
                        //objects.Add(turretObject);

                        // Destroy the outlined turret (for both players)
                        isPressed = false;
                        PhotonNetwork.Destroy(outlinedTurret);

                        // Reset targetTime
                        placeTimer = 1f;

                        // Is no longer trying to place a turret
                        isTryingToPlaceTurret = false;

                        audioSource.PlayOneShot(placeTurret);
                    }

                }
            }
            else
            {
                // Text to explain why placement not activating
                notEnoughResourcesToBuild.gameObject.SetActive(true);
                notEnoughResourcesToBuild.Play("FadeOut");
                StartCoroutine(ExecuteAfterTime(2f));
            }

        }
    }

    /// <summary>
    /// Destroys the turret the player is looking at and drops resources
    /// </summary>
    public void OnTurretDestroy()
    {
        if (PlayerControlsAreOn() == false) return;

        if (isUsingTurret == false && Physics.Raycast(camPositionFPS.transform.position, camPositionFPS.transform.forward, out hit, 5f) && hit.collider.gameObject.CompareTag("Turret"))
        {
            hit.collider.GetComponent<TurretHealthHandler>().SalvageDrop();
            GameObject GTG = hit.collider.gameObject;
            objects.Remove(GTG);
            PhotonNetwork.Destroy(GTG);
            turretCount--;
        }
    }

    public GameObject CreateTurret(Vector3 turretPosition)
    {
        GameObject turret = PhotonNetwork.Instantiate("Prefabs/Equipment/" + turretPrefab.name, turretPosition, Quaternion.identity);
        if (turret != null)
        {
            turretCount++;
            objects.Add(turret);
        }
        return turret;
    }

    /// <summary>
    /// Searches a GameObject for a specific child using "childName"
    /// </summary>
    private Transform GetChildWithName(GameObject objectToSearch, string childName)
    {
        Transform child = null;
        foreach (Transform t in objectToSearch.GetComponentsInChildren<Transform>())
        {
            if (t.name == childName)
            {
                child = t;
                break;
            }
        }
        return child;
    }

    public void OnTurretUse()
    {
        if (PlayerControlsAreOn() == false) return;

        if (isUsingTurret == false && Physics.Raycast(camPositionFPS.transform.position, camPositionFPS.transform.forward, out hit, 5f) && hit.collider.gameObject.CompareTag("Turret"))
        {
            if (hit.collider.GetComponent<HealthHandler>().isAlive)
            {
                //Debug.Log("You are using the turret");
                UpdateTurretControlSettings(2, false, true);
                // Put Engineer behind the turret that was hit
                foreach (Transform child in hit.transform)
                {
                    if (child.name == "UsePosition")
                    {
                        transform.position = child.transform.position;
                    }
                }

                hit.collider.GetComponent<Turret>().isCurrent = true;
                turretBodyTransform = GetChildWithName(hit.collider.gameObject, "TurretBodyRotationPoint");

                // Put Engineer behind the turret that was hit
                usePositionPos = GetChildWithName(hit.collider.gameObject, "UsePosition");
            }
        }

        else if (isUsingTurret == true)
        {
            //Debug.Log("You are no longer using turret");
            UpdateTurretControlSettings(1, true, false);
        }
    }

    public void UpdateTurretControlSettings(int controlType, bool isGravityOn, bool isUsingTurret)
    {
        this.isUsingTurret = isUsingTurret;
        ChangeControlls.ControlType = controlType;
        Body.IsGravityOn = isGravityOn;
    }

    public void OnTurretRepair(InputAction.CallbackContext ctx)
    {
        if (PlayerControlsAreOn() == false) return;

        if (Physics.Raycast(camPositionFPS.transform.position, camPositionFPS.transform.forward, out hit, 5f) && hit.collider.gameObject.CompareTag("Turret"))
        {
            if (ctx.performed)
            {
                // Make sure the turret doesn't alreday have full health
                TurretHealthHandler obj = hit.collider.gameObject.GetComponent<TurretHealthHandler>();

                if (obj.CurrentHealth == obj.MaxHealth)
                {
                    // Text to explain why repair not activating
                    atFullHealthText.gameObject.SetActive(true);
                    atFullHealthText.Play("FadeOut");
                    StartCoroutine(ExecuteAfterTime(2f));
                    return;
                }


                // Check if resources needed is in inventory
                InventorySystem inventorySystem = gameObject.GetComponent<InventorySystem>();
                if (inventorySystem.Amount<GreenGoo>() >= turretRepairCosts.gooCost && inventorySystem.Amount<Metal>() >= turretRepairCosts.metalCost)
                {
                    // Remove resources
                    inventorySystem.Remove<Metal>(turretRepairCosts.metalCost);
                    inventorySystem.Remove<GreenGoo>(turretRepairCosts.gooCost);

                    // Add health back to turret (MAKE SURE IT'S THE RIGHT TURRET)
                    TurretHealthHandler turretHealthHandler = hit.collider.GetComponent<TurretHealthHandler>();
                    if (turretHealthHandler.isAlive == true)
                    {
                        turretHealthHandler.AddTurretHealth(healthToAdd);
                    }
                    else
                    {
                        // If it's dead you gotta make it come back to life
                        turretHealthHandler.isAlive = true;
                        turretHealthHandler.Revived();
                        turretHealthHandler.AddTurretHealth(healthToAddIfDead);

                    }

                    // TODO Play sound
                }
                else
                {
                    // Text to explain why repair not activating
                    notEnoughResourcesToRepair.gameObject.SetActive(true);
                    notEnoughResourcesToRepair.Play("FadeOut");
                    StartCoroutine(ExecuteAfterTime(2f));
                }
            }

        }
    }

    IEnumerator ExecuteAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        // Code to execute after the delay
        atFullHealthText.gameObject.SetActive(false);
        notEnoughResourcesToRepair.gameObject.SetActive(false);
        notEnoughResourcesToBuild.gameObject.SetActive(false);
    }


    Engineer player;

    IEnumerator Wait(float sec)
    {
        while (player == null)
        {
            yield return new WaitForSeconds(sec);
            player = FindObjectOfType<Engineer>();//GameObject.FindGameObjectWithTag("Player").transform;
        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, checkRadius);
        //Gizmos.DrawRay(transform.position, hit.transform.position);
    }

    private void ShowTurret(){
        InventorySystem inventorySystem = gameObject.GetComponent<InventorySystem>();
        if(inventorySystem.Amount<GreenGoo>() >= 2 && inventorySystem.Amount<Metal>() >= 2 && !textShown && isMine){
            uiObject.gameObject.SetActive(true);
            textShown = true;
        }

        if(isTryingToPlaceTurret == true){
            uiObject.gameObject.SetActive(false);
            textShown = true;
        }
    }
}
