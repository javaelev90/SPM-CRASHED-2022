using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Mesh settings")]
    [Tooltip("Position where the gun barrel ends")]
    [SerializeField] Transform muzzlePosition;

    [Header("Weapon settings")]
    [SerializeField] float weaponRange = 15f;
    [SerializeField] int weaponDamage = 1;
    [SerializeField] float delayBetweenShots = 0.5f;
    [Tooltip("If gun will shoot continuously when shoot button is pressed vs one shot per click.")]
    [SerializeField] public bool automaticWeapon = false;
    [SerializeField] LayerMask layersThatShouldBeHit;

    private float shotCooldown = 0f;

    AudioSource source;
    public AudioClip[] shot;

    void Update()
    {
        Cooldown();
    }

    void Start()
    {
        source = GetComponent<AudioSource>();
     
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
        if(OnCoolDown() == false)
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward,
                out RaycastHit hitInfo, weaponRange, layersThatShouldBeHit))
            {
                HealthHandler healthHandler = hitInfo.transform.GetComponent<HealthHandler>();
                Debug.Log("Hit the enemy?");
                if (healthHandler)
                {
                    Debug.Log("Hit the enemy.");
                    AudioClip clip = GetAudioClip();
                    source.PlayOneShot(clip);
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
        }
    }

    private AudioClip GetAudioClip()
    {
        int index = Random.Range(0, shot.Length - 1);
        return shot[index];
    }
}
