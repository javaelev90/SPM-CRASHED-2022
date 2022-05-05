using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Weapon))]
public class SoldierCharacter : Controller3D
{
    [SerializeField] private LayerMask fireLayer;
    [SerializeField] private float interactionDistance = 1f;
    [SerializeField] private Weapon weapon;

    [Header("Punch settings")]
    [SerializeField] float punchRange = 2f;
    [SerializeField] int punchDamage = 1;
    [SerializeField] float delayBetweenShots = 0.3f;
    [SerializeField] LayerMask layersThatShouldBeHit;

    private float shotCooldown = 0f;

    protected override void Awake()
    {
        base.Awake();
        weapon = GetComponent<Weapon>();
    }

    protected override void Update()
    {
        base.Update();
        WeaponRotation();
        Cooldown();
    }

    public void Shoot()
    {
        weapon.Shoot();
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

    public void Punch()
    {
        if (OnCoolDown() == false)
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward,
                out RaycastHit hitInfo, punchRange, layersThatShouldBeHit))
            {
                HealthHandler healthHandler = hitInfo.transform.GetComponent<HealthHandler>();
                Debug.Log("Hit the enemy?");
                if (healthHandler)
                {
                    Debug.Log("Hit the enemy.");
                    healthHandler.TakeDamage(punchDamage);
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

}
