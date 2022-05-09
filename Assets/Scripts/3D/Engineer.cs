using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;

public class Engineer : Controller3D
{
    private float targetTime = 1f;

    [Header("Turrets")]
    [SerializeField] private PhotonView turret;
    [SerializeField] private Transform turretPos;
    [SerializeField] private int maxTurretToSpawn = 3;
    [SerializeField] private int turretCount;
    [SerializeField] private Vector3 turretOffset;
    [SerializeField] private int gooCostTurret = 1;
    [SerializeField] private int metalCostTurret = 1;
    //[SerializeField] private GameObject turretObject;
    private bool canPutDownTurret;
    //private string pathTurret = "Resources/Prefabs/TurretAssembly";
    [SerializeField] private GameObject turretPrefab;
    [SerializeField] private GameObject greenGooPrefab;
    [SerializeField] private GameObject metalPrefab;
    [SerializeField] public LayerMask turretLayer;

    [Header("Carry Ship Part")]
    /// <summary>
    /// PLAYER NEED A CHILD OBJECT CALLED CarryPos
    /// </summary>
    [SerializeField] private Transform destination;
    [SerializeField] private Transform player;
    private GameObject[] shipPart;
    private float checkRadius = 5f;
    RaycastHit hit;

    [Header("Stun")]
    [SerializeField] Transform muzzlePosition;
    [SerializeField] float weaponRange = 15f;
    [SerializeField] float delayBetweenShots = 0.5f;
    [SerializeField] public LayerMask stunLayer;
    private float shotCooldown = 0f;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Wait(5));
        //playerActions = new PlayerInputActions();
    }
    protected override void Awake()
    {
        base.Awake();


    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        Cooldown();
        targetTime -= Time.deltaTime;
        PickUpShipPart();
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
        if (OnCoolDown() == false)
        {
            if (Physics.Raycast(muzzlePoint.transform.position, weaponRotation.transform.rotation * Vector3.forward * 10f, out hit, weaponRange, stunLayer))
            {
                AIBaseLogic aIBaseLogic = hit.transform.GetComponent<AIBaseLogic>();
                if (aIBaseLogic)
                {
                    Debug.Log("Enemy stunned");
                    aIBaseLogic.StunnedBy(transform);
                }
            }
            // Add cooldown time
            shotCooldown = delayBetweenShots;
        }

    }

    public void OnPlaceTurret(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            Debug.Log("show outline of turret where to place");
        }

        if (ctx.canceled)
        {
            Debug.Log("released button should result in placing turret");
        }
    }
 

    public void TurretHandling()
    {
        Physics.Raycast(muzzlePoint.transform.position, weaponRotation.transform.rotation * Vector3.forward * 10f, out RaycastHit hit, turretLayer);

        if (playerActions.Player.DeleteTurret.IsPressed())
        {
            Transform greenGooDropPos = turretPrefab.transform.Find("DropMetal");
            Transform metalDropPos = turretPrefab.transform.Find("DropGoo");

            if (hit.collider.gameObject.tag == "Turret")
            {
                GameObject greenGooDrop = PhotonNetwork.Instantiate("Prefabs/Pickups/" + greenGooPrefab.name, greenGooDropPos.transform.position, Quaternion.identity);
                greenGooDrop.name = "Green Goo";
                GameObject metalDrop = PhotonNetwork.Instantiate("Prefabs/Pickups/" + metalPrefab.name, metalDropPos.transform.position, Quaternion.identity);
                metalDrop.name = "Metal";
                Debug.Log("Hit that lil turret bitch");

                Destroy(hit.collider.gameObject);
            }
        }


        if (turretCount < maxTurretToSpawn && playerActions.Player.PlaceTurret.IsPressed()) //&& (inventory.GreenGoo >= gooCostTurret && inventory.Metal >= metalCostTurret))
        {
            GameObject turretObject;


            /*
            if (canPutDownTurret && turretObject != null && playerActions.Player.PlaceTurret.IsPressed()) //Input.GetMouseButton(1)
            {
                turretObject.transform.position = turretPos.position;
                turretObject.transform.rotation = Quaternion.FromToRotation(turretObject.transform.up, Vector3.up) * turretObject.transform.rotation;
            }
            */


            if (targetTime < 0.0f)
            {

                canPutDownTurret = true;
                turretObject = PhotonNetwork.Instantiate("Prefabs/" + turretPrefab.name, turretPos.position, Quaternion.identity);//(pathTurret, turretPos.position, Quaternion.identity);



                if (canPutDownTurret && turretObject != null && playerActions.Player.PlaceTurret.IsPressed())//Input.GetMouseButtonUp(1))
                {
                    turretObject.transform.rotation = Quaternion.FromToRotation(turretObject.transform.up, Vector3.up) * turretObject.transform.rotation;
                    if (hit.collider != null && hit.distance < 3f)
                    {
                        Debug.Log("turret should be placed");
                        turretObject.transform.position = hit.transform.position;
                        turretObject.GetComponent<Turret>().IsPlaced = true;
                        turretCount++;
                        canPutDownTurret = false;
                    }
                    //inventory.removeMetalAndGreenGoo(metalCostTurret,gooCostTurret);

                }

                targetTime = 1f;
            }
        }

    }

    public void PickUpShipPart()
    {
        if (player != null)
        {
            Collider[] colliderHits = Physics.OverlapSphere(transform.position, checkRadius);
            //Debug.Log(colliderHits);

            foreach (Collider col in colliderHits)
            {
                if (col.tag == ("ShipPart") && playerActions.Player.ShipPickUp.IsPressed())
                {
                    //destination = player.transform.Find("CarryPos");
                    //shipPart = GameObject.FindGameObjectsWithTag("ShipPart");
                    //GetComponent<Rigidbody>().useGravity = false;
                    //col.transform.position = destination.position;
                    //col.transform.parent = GameObject.Find("CarryPos").transform;
                    col.GetComponent<EventStarter>().StartEvent();

                }
                if (playerActions.Player.DropShitPart.IsPressed())
                {
                    col.transform.parent = null;
                    //GetComponent<Rigidbody>().useGravity = true;
                }
                else
                {
                    //Debug.Log("Outside");
                }
            }
        }

    }

    IEnumerator Wait(float sec)
    {
        while (player == null)
        {
            yield return new WaitForSeconds(sec);
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, checkRadius);
        //Gizmos.DrawRay(transform.position, hit.transform.position);
    }
}
