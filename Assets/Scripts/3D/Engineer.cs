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

    [Header("Carry Ship Part")]
    /// <summary>
    /// PLAYER NEED A CHILD OBJECT CALLED CarryPos
    /// </summary>
    [SerializeField] private Transform destination;
    private Transform player;
    private GameObject shipPart;

    // Start is called before the first frame update
    void Start()
    {
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
        targetTime -= Time.deltaTime;
        //Debug.Log(targetTime);
    }

    public void TurretHandling()
    {
        Physics.Raycast(muzzlePoint.transform.position, weaponRotation.transform.rotation * Vector3.forward * 10f, out RaycastHit hit, obstacleLayer);

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
                    turretObject.transform.position = turretPos.position;
                    turretObject.GetComponent<Turret>().IsPlaced = true;
                    turretCount++;
                    canPutDownTurret = false;
                    //inventory.removeMetalAndGreenGoo(metalCostTurret,gooCostTurret);

                }

                targetTime = 1f;
            }
        }

    }

    public void PickUpShipPart()
    {
        Collider[] colliderHits = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider col in colliderHits)
        {
            if (col.tag == ("ShipPart") && playerActions.Player.PickUp.IsPressed())
            {
                Debug.Log("Inside");
                destination = player.transform.Find("CarryPos");
                //GetComponent<Rigidbody>().useGravity = false;
                this.transform.position = destination.position;
                this.transform.parent = GameObject.Find("CarryPos").transform;
            }
            if (playerActions.Player.DropShitPart.IsPressed())
            {
                this.transform.parent = null;
                //GetComponent<Rigidbody>().useGravity = true;
            }
            else
            {
                //Debug.Log("Outside");
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
}
