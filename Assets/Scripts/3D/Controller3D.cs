using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;

public class Controller3D : MonoBehaviourPunCallbacks
{
    [Header("Multiplayer")]
    private bool isMine;

    [Header("Weapon")]
    [SerializeField] public GameObject weaponRotation;
    [SerializeField] public GameObject muzzlePoint;
    [SerializeField] private PhotonView bullet;
    [SerializeField] private float bulletSpeed;
    private string pathBullet = "Prefab/Player/Bullet";

    [Header("Player")]
    [SerializeField] private float skinWidth = 0.5f;
    [SerializeField] private float groundCheckDistance;

    [Header("Camera settings")]
    [SerializeField] private bool isFPS;
    [SerializeField] private GameObject camPositionFPS;
    [SerializeField] Vector3 cameraOffsetTPS;
    [SerializeField] Vector3 cameraOffsetFPS;
    [SerializeField] float smoothFactorQuick = 0.23f;
    [SerializeField] float smoothFactorSlow = 0.05f;
    [SerializeField] protected float radius = 1.0f;
    private Vector3 smoothedPos;
    private Vector3 topPos;
    private Vector2 cameraRotation;
    private Camera mainCam;
    private Vector3 offset;

    [Header("Physics")]
    [SerializeField] public LayerMask obstacleLayer;
    [SerializeField] RaycastHit groundHit;
    private CapsuleCollider capsuleCollider;
    private Vector3 upperPoint;
    private Vector3 lowerPoint;

    [Header("Input")]
    [SerializeField] private float mouseSensitivity = 1f;
    [SerializeField] private float gamepadSensitivity = 2f;
    [SerializeField] private float minXrotation = 1f;
    [SerializeField] private float maxXrotation = 1f;
    private Vector3 input;

    [Header("States")]
    [SerializeField] private StateMachine stateMachine;
    [SerializeField] private State[] states;

    [Header("PhysicsBody")]
    private PhysicsBody body;
    public PhysicsBody Body => body;


    
    [Header("InputSystem")]
    public PlayerInputActions playerActions;
    public PlayerInput playerInput;
    private string currentControlScheme; // The controlls we are currently using (Keyboard&Mouse or Gamepad)
    Vector2 cameraLooking; // The input from mouse/gamepad that is used to move camera


    protected virtual void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerActions = new PlayerInputActions();
        currentControlScheme = playerInput.currentControlScheme;

        //inventory = GetComponent<Inventory>();
        stateMachine = new StateMachine(states, this);
        groundCheckDistance = 10 * skinWidth;

        capsuleCollider = GetComponent<CapsuleCollider>();
        body = GetComponent<PhysicsBody>();

        mainCam = Camera.main;
        isMine = photonView.IsMine;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        playerActions.Player.Enable();
    }

    private void OnDisable()
    {
        playerActions.Player.Disable();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (isMine)
        {
            InputHandling();
            PlayerRotation();
            
            stateMachine.UpdateStates();

            RoatateCamera();


        }
    }

    public void Jump()
    {
        float jumpForce = 5f;
        
        
        //groundHit = IsGrounded();
        if (playerActions.Player.Jump.IsPressed() && Body.Grounded)
        {
            Vector3 jumpMovement = Vector3.up * jumpForce;

            Body.Velocity += jumpMovement;
        }
    }

    public void InputHandling() //InputAction.CallbackContext value
    {
        //groundHit = IsGrounded();
        Debug.Log("FUCK");
        if (isMine)
        {
            Debug.Log("FUCK YOU");
            Vector3 movementInput = playerActions.Player.Move.ReadValue<Vector2>();
            Debug.Log(movementInput);
            input = new Vector3(movementInput.x, 0, movementInput.y);

            input = mainCam.transform.rotation * input;
            input = Vector3.ProjectOnPlane(input, Body.GroundHit.normal).normalized;

            //Body.Velocity += input;

            /*
            input = Vector3.right * Input.GetAxisRaw("Horizontal") + Vector3.forward * Input.GetAxisRaw("Vertical");
            cameraRotation.y += Input.GetAxisRaw("MouseX") * mouseSensitivity;
            cameraRotation.x -= Input.GetAxisRaw("MouseY") * mouseSensitivity;
            
            input = mainCam.transform.rotation * input;
            input = Vector3.ProjectOnPlane(input, Body.GroundHit.normal).normalized;

            if (Input.GetKeyDown(KeyCode.Q))
            {
                isFPS = !isFPS;
            }
            */
        }
    }

    public void MoveCamera(InputAction.CallbackContext obj)
    {
        cameraLooking = obj.ReadValue<Vector2>();
        Debug.Log(cameraLooking);
    }

    private void RoatateCamera()
    {
        if (currentControlScheme == "Gamepad")
        {
            cameraRotation.x -= cameraLooking.y * gamepadSensitivity * Time.deltaTime;
            cameraRotation.y += cameraLooking.x * gamepadSensitivity * Time.deltaTime;
        }

        if (currentControlScheme == "KeyboardMouse")
        {
            cameraRotation.x -= cameraLooking.y * mouseSensitivity * Time.deltaTime;
            cameraRotation.y += cameraLooking.x * mouseSensitivity * Time.deltaTime;
        }

        cameraRotation.x = Mathf.Clamp(cameraRotation.x, minXrotation, maxXrotation);

        mainCam.transform.localRotation = Quaternion.Euler(cameraRotation.x, cameraRotation.y, 0f);

        transform.rotation = Quaternion.Euler(0f, cameraRotation.y, 0f);
    }

    private void PlayerRotation()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.x, cameraRotation.y, transform.rotation.z);
    }

    private void WeaponRotation()
    {
        weaponRotation.transform.rotation = Quaternion.Euler(cameraRotation.x, cameraRotation.y, 0f);

        Debug.DrawRay(muzzlePoint.transform.position, weaponRotation.transform.rotation * Vector3.forward * 100f, Color.red);

        /*
        if (!canPutDownTurret)
        {
            
            if (Input.GetMouseButtonDown(0))
            {
                GameObject bull = PhotonNetwork.Instantiate(pathBullet, muzzlePoint.transform.position, weaponRotation.transform.rotation);
                Projectile projectile = bull.GetComponent<Projectile>();
                projectile.Velocity = weaponRotation.transform.rotation * Vector3.forward * bulletSpeed;
                projectile.IsShot = true;
            }
            
        }
        */


    }

    /*
    public void TurretHandling()
    {
        Physics.Raycast(muzzlePoint.transform.position, weaponRotation.transform.rotation * Vector3.forward * 10f, out RaycastHit hit, obstacleLayer);
        
        if (turretCount < maxTurretToSpawn && playerActions.Player.PlaceTurret.IsPressed()) //&& (inventory.GreenGoo >= gooCostTurret && inventory.Metal >= metalCostTurret))
        {
            GameObject turretObject;

            
            //
            if (canPutDownTurret && turretObject != null && playerActions.Player.PlaceTurret.IsPressed()) //Input.GetMouseButton(1)
            {
                turretObject.transform.position = turretPos.position;
                turretObject.transform.rotation = Quaternion.FromToRotation(turretObject.transform.up, Vector3.up) * turretObject.transform.rotation;
            }
            //
            

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
*/

    /*
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
    */

    private void UpdateCamera()
    {
        mainCam.transform.rotation = Quaternion.Euler(cameraRotation.x, cameraRotation.y, 0.0f);

        if (isFPS)
        {
            mainCam.transform.position = camPositionFPS.transform.position;
        }
        else
        {
            topPos = transform.position + upperPoint;
            offset = mainCam.transform.rotation * cameraOffsetTPS;
            Vector3 direction = offset - topPos;
            Debug.DrawLine(
                transform.position,
                transform.position + offset,
                Color.red);

            Vector3 newPos;
            if (Physics.SphereCast(
                transform.position + upperPoint,
                radius,
                direction.normalized,
                out RaycastHit hit,
                direction.magnitude))
            {
                newPos = cameraOffsetTPS.normalized * hit.distance;
            }
            else
            {
                newPos = cameraOffsetTPS;
            }
            smoothedPos = Vector3.Lerp(smoothedPos, newPos, Time.deltaTime * (hit.collider ? smoothFactorQuick : smoothFactorSlow));
            mainCam.transform.position = topPos + mainCam.transform.rotation * smoothedPos;
        }
    }

    private void LateUpdate()
    {
        if (isMine)
            UpdateCamera();
    }

    private RaycastHit IsGrounded()
    {
        Physics.CapsuleCast(
            upperPoint,
            lowerPoint,
            capsuleCollider.radius,
            Vector3.down,
            out RaycastHit hit,
            groundCheckDistance + skinWidth,
            obstacleLayer);
        return hit;
    }

    public Vector3 GetInput() { return input; }


    //INPUT SYSTEM AUTOMATIC CALLBACKS --------------

    //This is automatically called from PlayerInput, when the input device has changed
    //(IE: Keyboard -> Xbox Controller)
    public void OnControlsChanged()
    {

        if (playerInput.currentControlScheme != currentControlScheme)
        {
            currentControlScheme = playerInput.currentControlScheme;

            RemoveAllBindingOverrides();
        }
    }

    //This is automatically called from PlayerInput, when the input device has been disconnected and can not be identified
    //IE: Device unplugged or has run out of batteries
    public void OnDeviceLost()
    {

    }


    public void OnDeviceRegained()
    {
        StartCoroutine(WaitForDeviceToBeRegained());
    }

    IEnumerator WaitForDeviceToBeRegained()
    {
        yield return new WaitForSeconds(0.1f);
    }

    void RemoveAllBindingOverrides()
    {
        InputActionRebindingExtensions.RemoveAllBindingOverrides(playerInput.currentActionMap);
    }

    private void OnDrawGizmos()
    {
        if (mainCam)
            Gizmos.DrawWireSphere(mainCam.transform.position, radius);
    }
}
