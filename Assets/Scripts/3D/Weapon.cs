using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Weapon : MonoBehaviour
{
    [Header("Mesh settings")]
    [Tooltip("Position where the gun barrel ends")]
    [SerializeField] VisualEffect muzzlePosition;

    [Header("Weapon settings")]
    [SerializeField] float weaponRange = 15f;
    [SerializeField] int weaponDamage = 1;
    [SerializeField] float delayBetweenShots = 0.5f;
    [Tooltip("If gun will shoot continuously when shoot button is pressed vs one shot per click.")]
    [SerializeField] public bool automaticWeapon = false;
    [SerializeField] LayerMask layersThatShouldBeHit;
    [SerializeField] private int maxAmmo;
    [SerializeField] private int currentAmmo;
    private AudioClip clip;

    private float shotCooldown = 0f;
    public bool IsShooting { get; set; }

    [SerializeField] private AudioSource sourceOne;
    public AudioClip[] shot;

    void Update()
    {
        Cooldown();
        Shoot();
    }

    void Start()
    {
        sourceOne = GetComponent<AudioSource>();
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

    public void Shoot()
    {
        if(OnCoolDown() == false && IsShooting == true)
        {
            muzzlePosition.Play();
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward,
                out RaycastHit hitInfo, weaponRange, layersThatShouldBeHit))
            {
                HealthHandler healthHandler = hitInfo.transform.GetComponent<HealthHandler>();
                if (healthHandler)
                {
                    healthHandler.TakeDamage(weaponDamage);
                }

                AIBaseLogic ai = hitInfo.transform.GetComponent<AIBaseLogic>();
                if (ai)
                {
                    Debug.Log(ai.transform.name);
                    ai.FindAttackingTarget(transform);
                }
                
            }
            // Add cooldown time
            shotCooldown = delayBetweenShots;
            clip = GetAudioClip();
            sourceOne.PlayOneShot(clip);
        }
    }

    private AudioClip GetAudioClip()
    {
        int index = Random.Range(0, shot.Length - 1);
        return shot[index];
    }
}
