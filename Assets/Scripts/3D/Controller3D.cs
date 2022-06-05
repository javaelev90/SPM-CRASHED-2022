using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;
using EventCallbacksSystem;
public class Controller3D : MonoBehaviourPunCallbacks
{
    [Header("Multiplayer")]
    public bool isMine;

    [Header("Weapon")]
    [SerializeField] public GameObject weaponRotation;
    [SerializeField] public GameObject muzzlePoint;
    [SerializeField] private PhotonView bullet;
    [SerializeField] private float bulletSpeed;
    private string pathBullet = "Prefab/Player/Bullet";

    [Header("Player")]
    [SerializeField] private float skinWidth = 0.5f;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] public GameObject playerGUI;
    [SerializeField] public GameObject pauseMenu;
    private HealthHandler healthHandler;

    [Header("Engineer")]
    public Turret turretObjRef;
    public Engineer engineerRef;
    public Transform turretBodyTransform;
    [SerializeField] private LayerMask enemyLayer;
    public bool isShootingTurret { get; set; }


    [Header("Camera settings")]
    [SerializeField] private bool isFPS;
    [SerializeField] protected GameObject camPositionFPS;
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
    [SerializeField] private float maxClimbableAngel = 50f;
    private float minYOnUnitCircle;
    private Vector3 input;

    [Header("States")]
    [SerializeField] private StateMachine stateMachine;
    [SerializeField] private State[] states;
    [SerializeField] public Animator animator;

    [Header("PhysicsBody")]
    private PhysicsBody body;
    public PhysicsBody Body => body;

    [Header("Inventory")]
    [SerializeField] protected Inventory inventory;
    
    [Header("InputSystem")]
    public PlayerInputActions playerActions;
    public PlayerInput playerInput;
    private string currentControlScheme; // The controlls we are currently using (Keyboard&Mouse or Gamepad)
    Vector2 cameraLooking; // The input from mouse/gamepad that is used to move camera

    AudioSource source;
    public AudioClip clip;

    [Header("FPS Visuals")]
    [SerializeField] GameObject weaponVisuals;
    [SerializeField] GameObject bodyMesh;

    protected bool PlayerPaused { get; set; }
    protected bool ControlsAreLocked { get; set; }

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

       
        //weaponPrefab.transform.SetParent(mainCam.transform);
        isMine = photonView.IsMine;
        Cursor.lockState = CursorLockMode.Locked;
        healthHandler = GetComponent<HealthHandler>();
        source = GetComponent<AudioSource>();
        playerGUI.SetActive(isMine);
        pauseMenu.SetActive(isMine);

        if (isMine)
        {
            mainCam = Camera.main;
            mainCam.transform.position = camPositionFPS.transform.position;
            mainCam.transform.SetParent(camPositionFPS.transform);
            camPositionFPS.transform.rotation = Quaternion.LookRotation(bodyMesh.transform.position, Vector3.up);
            mainCam.transform.rotation = camPositionFPS.transform.rotation;
            EventSystem.Instance.RegisterListener<LockControlsEvent>(LockControlsEventHandler);
        }

        bodyMesh.SetActive(isMine == false);
        weaponVisuals.SetActive(isMine);

        minYOnUnitCircle = Mathf.Sin(maxClimbableAngel / 180 * Mathf.PI) ;
    }

    private void OnEnable()
    {
        playerActions.Player.Enable();
    
    }

    private void OnDisable()
    {
        playerActions.Player.Disable();
    }

    private void LockControlsEventHandler(LockControlsEvent lockEvent)
    {
        ControlsAreLocked = lockEvent.AreControlsLocked;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (isMine)
        {
            if (PlayerControlsAreOn() == false) return;

            switch (ChangeControlls.ControlType)
            {
                case 1:
                    InputHandling();
                    PlayerRotation();
                    stateMachine.UpdateStates();
                    RoatateCamera();
                    break;
                case 2:
                    RoatateCamera();
                    //EngineerUseTurretHandling();
                    break;
            }
        }
        
        /*
        if (isMine)
        {
            InputHandling();
            PlayerRotation();
            
            stateMachine.UpdateStates();

            RoatateCamera();
        }
        */
    }

    public void Jump(InputAction.CallbackContext ctx)
    {
        float jumpForce = 5f;

        if (ctx.performed && Body.Grounded && Body.GroundHit.normal.normalized.y > minYOnUnitCircle)
        {
            Vector3 jumpMovement = Vector3.up * jumpForce;
            source.PlayOneShot(clip);
            Body.Velocity += jumpMovement;
        }
    }

    public void InputHandling() //InputAction.CallbackContext value
    {
        //groundHit = IsGrounded();
        if (isMine)
        {
            Vector3 movementInput = playerActions.Player.Move.ReadValue<Vector2>();
            input = new Vector3(movementInput.x, 0, movementInput.y);
            input = /*mainCam.*/transform.rotation * input;
        
            
            if (Body.GroundHit.normal.normalized.y > minYOnUnitCircle)
            {
                input = Vector3.ProjectOnPlane(input, Body.GroundHit.normal).normalized;
            }
            else
            {
                input = Vector3.ProjectOnPlane(input, Body.GroundHit.normal).normalized;
                if (input.y + Body.GroundHit.point.y > 0)
                {
                    //Debug.Log(input);
                    input.y = 0;
                }
                input = Vector3.ProjectOnPlane(Body.GroundHit.normal, Body.GroundHit.normal).normalized + input;

            }



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

    public void EngineerUseTurretHandling(InputAction.CallbackContext ctx)
    {
        RoatateCamera();
        PlayerRotation();

        if (engineerRef.GetComponent<Engineer>().isUsingTurret == true)
        {
            if (ctx.performed) 
            {
                isShootingTurret = true;
            }
            if (ctx.canceled)
            {
                isShootingTurret = false;
            }
            //Debug.Log("isShootingTurret " + isShootingTurret);
        }
    }

    public void EngineerUseTurretLooking()
    {
        if (engineerRef.GetComponent<Engineer>().isUsingTurret == true)
        {
            // Rotate the turret towards where the player is looking
            Physics.Raycast(muzzlePoint.transform.position, muzzlePoint.transform.forward, out RaycastHit hit, 20f, enemyLayer);
            // skapa nytt obj framf�r muzzle som direction, origin �r muzzlepoint
            Vector3 lookDirection = Camera.main.transform.forward;//(muzzlePoint.transform.position - camPositionFPS.transform.position).normalized;
            Quaternion rotateTo = Quaternion.LookRotation(lookDirection, turretBodyTransform.transform.up);

            //turretBodyTransform = turretBodyTransform.transform;
            //turretBodyTransform.LookAt(lookDirection, Vector3.up);
            turretBodyTransform.transform.rotation = Quaternion.Slerp(turretBodyTransform.transform.rotation, rotateTo, 1f);
            ClampRotBody();
            //ClampRotBodyPlayer();
            //Body.enabled = false;

        }
    }

    public void ClampRotBody()
    {
        // Clamp rotation so it doesn't go all over the place and end up upside down
        Vector3 pivotRotation = turretBodyTransform.transform.eulerAngles;
        //pivotRotation.x = Mathf.Clamp(pivotRotation.x, -30f, 30f);
        
        // Need to clamp rotation in a different way around 0
        if (pivotRotation.x > 180f)
        {
            pivotRotation.x -= 360f;
        }
        if (pivotRotation.x < -180f)
        {
            pivotRotation.x += 360f;
        }

        if (pivotRotation.x > 30f)
            pivotRotation.x = 30f;
        if (pivotRotation.x < -30f)
            pivotRotation.x = -30f;

        pivotRotation.z = Mathf.Clamp(pivotRotation.z, 0f, 0f);
        turretBodyTransform.transform.eulerAngles = pivotRotation;
    }

    public void ClampRotBodyPlayer()
    {
        // Clamp rotation so it doesn't go all over the place and end up upside down
        Vector3 pivotRotation = camPositionFPS.transform.eulerAngles;
        pivotRotation.x = Mathf.Clamp(pivotRotation.x, -60f, 60f);
        pivotRotation.z = Mathf.Clamp(pivotRotation.z, 0f, 0f);
        camPositionFPS.transform.eulerAngles = pivotRotation;
    }

    public void MoveCamera(InputAction.CallbackContext obj)
    {
        cameraLooking = obj.ReadValue<Vector2>();
        //Debug.Log(cameraLooking);
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

        //mainCam.transform.localRotation = Quaternion.Euler(cameraRotation.x, cameraRotation.y, 0f);
        camPositionFPS.transform.localRotation = Quaternion.Euler(cameraRotation.x, cameraRotation.y, 0f);
        transform.rotation = Quaternion.Euler(0f, cameraRotation.y, 0f);
    }

    private void PlayerRotation()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.x, cameraRotation.y, transform.rotation.z);
    }

    protected void WeaponRotation()
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

    

    private void UpdateCamera()
    {
        //mainCam.transform.rotation = Quaternion.Euler(cameraRotation.x, cameraRotation.y, 0.0f);
        camPositionFPS.transform.rotation = Quaternion.Euler(cameraRotation.x, cameraRotation.y, 0.0f);

        if (isFPS)
        {
            //mainCam.transform.position = camPositionFPS.transform.position;
            camPositionFPS.transform.position = camPositionFPS.transform.position;

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

    private void OnTriggerEnter(Collider other)
    {
        Projectile projectile;
        if((projectile = other.gameObject.GetComponent<Projectile>()) != null)
        {
            healthHandler.TakeDamage(projectile.DamageDealer);
        }
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

    protected bool PlayerControlsAreOn()
    {
        return (GameManager.gameIsPaused == false) && (ControlsAreLocked == false);
    }

    private void OnDrawGizmos()
    {
        if (mainCam)
            Gizmos.DrawWireSphere(mainCam.transform.position, radius);
    }
#if (UNITY_EDITOR)
    public void Immortal()
    {
        EventSystem.Instance.FireEvent(new ImmortalEvent());
    }
#endif
}
