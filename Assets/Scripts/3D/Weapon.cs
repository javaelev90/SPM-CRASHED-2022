using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using EventCallbacksSystem;
using Photon.Pun;

public class Weapon : MonoBehaviourPunCallbacks
{
    [Header("Mesh settings")]
    [Tooltip("Position where the gun barrel ends")]
    [SerializeField] VisualEffect muzzlePosition;

    [SerializeField] GameObject hitPosition;

    [Header("Weapon settings")]
    [SerializeField] float weaponRange = 15f;
    [SerializeField] int weaponDamage = 1;
    [SerializeField] float delayBetweenShots = 0.5f;
    [SerializeField] LayerMask layersThatShouldBeHit;
    [SerializeField] private int maxAmmo;
    [SerializeField] public int currentAmmo;
    private float shotCooldown = 0f;
    public bool IsShooting { get; set; }
    private WeaponAmmunitionUpdateEvent ammunitionUpdateEvent;

    [Header("Weapon effects")]
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource sourceOne;
    public AudioClip[] shot;

    [Header("Inventory")]
    [SerializeField] private bool useInventory = false;
    [SerializeField] private InventorySystem inventory;
    [SerializeField] private int greenGooCost = 1;

    private  static bool textShown; 

    public GameObject uiObject;
    private bool initialized = false;

    void Update()
    {
        Cooldown();
        Shoot();
        sourceOne.volume = Random.Range(0.8f, 2);
        sourceOne.pitch = Random.Range(0.8f, 1.4f);
    }

    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if(initialized == false)
        {
            sourceOne = GetComponent<AudioSource>();
            EventSystem.Instance.RegisterListener<GunDamageUpgradeEvent>(UpgradeDamage);
            EventSystem.Instance.RegisterListener<GunDamageUpgradeEvent>(UpgradeDamage); // behövs två rader av samma?
            sourceOne.volume = Random.Range(1.8f, 2.5f);
            sourceOne.pitch = Random.Range(0.8f, 1.2f);

            currentAmmo = maxAmmo;
            ammunitionUpdateEvent = new WeaponAmmunitionUpdateEvent(currentAmmo, maxAmmo);
            EventSystem.Instance.FireEvent(ammunitionUpdateEvent);
            initialized = true;
        }
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

    private void ShowAmmo(){
        InventorySystem inventorySystem = gameObject.GetComponent<InventorySystem>();
        if(inventorySystem.Amount<GreenGoo>() == 0 && !textShown){
            uiObject.SetActive(true);
            textShown = true;
        }
        
        if(IsShooting == true){
            uiObject.SetActive(false);
            textShown = true;
        }
    }

    public void Shoot()
    {
        if (OnCoolDown() == false && IsShooting == true && currentAmmo > 0)
        {
            muzzlePosition.Play();
            animator.CrossFadeInFixedTime("Shooting", 0.1f);

            currentAmmo--;
            SetAmmo(currentAmmo);

            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward,
                out RaycastHit hitInfo, weaponRange, layersThatShouldBeHit))
            {
                HealthHandler healthHandler = hitInfo.transform.GetComponent<HealthHandler>();
                if (healthHandler != null)
                {
                    healthHandler.TakeDamage(weaponDamage);
                }
                // Plays VFX where the bullet hits
                //Instantiate(hitPosition, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                Destroy(Instantiate(hitPosition, hitInfo.point, Quaternion.LookRotation(hitInfo.normal)), 10f);

                AIBaseLogic ai = hitInfo.transform.GetComponent<AIBaseLogic>();
                if (ai)
                {
                    ai.FindAttackingTarget(transform);
                }
            }
            // Add cooldown time
            shotCooldown = delayBetweenShots;
            PlayShotEffects();
        }
        else
        {
            if (currentAmmo == 0 && inventory.Amount<GreenGoo>() > 0)
            {
                currentAmmo = maxAmmo;
                inventory.Remove<GreenGoo>();
                SetAmmo(currentAmmo);
            }
            else if (currentAmmo == 0 && inventory.Amount<GreenGoo>() == 0)
            {
                SetAmmo(currentAmmo);
            }
        }

    }

    public void SetAmmo(int ammo)
    {
        Initialize();
        ammunitionUpdateEvent.AmmunitionAmount = ammo;
        EventSystem.Instance.FireEvent(ammunitionUpdateEvent);
    }

    private void PlayShotEffects()
    {
        photonView.RPC(nameof(PlayShotEffectsRPC), RpcTarget.All);
    }

    [PunRPC]
    private void PlayShotEffectsRPC()
    {
        sourceOne.PlayOneShot(GetAudioClip());
    }

    private AudioClip GetAudioClip()
    {
        int index = Random.Range(0, shot.Length - 1);
        sourceOne.volume = Random.Range(1.8f, 2.5f);
        sourceOne.pitch = Random.Range(0.4f, 1.6f);
        return shot[index];

    }

    public void UpgradeDamage(GunDamageUpgradeEvent damageUpgradeEvent)
    {
        weaponDamage += damageUpgradeEvent.UpgradeAmount;
    }

    public void FireRateUpgrade(GunFireRateUpgradeEvent gunRateUpgradeEvent)
    {
        delayBetweenShots *= (1 - gunRateUpgradeEvent.UpgradePercent);
    }

}
