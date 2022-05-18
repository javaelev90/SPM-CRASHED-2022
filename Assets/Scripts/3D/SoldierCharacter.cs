using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;

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

    private void Start()
    {
        if (photonView.IsMine)
            Minimap.Instance.Player = gameObject;

        StartCoroutine(SearchOtherPlayer());
    }


    IEnumerator SearchOtherPlayer()
    {
        while (true)
        {
            Minimap.Instance.OtherPlayer = FindObjectOfType<Engineer>()?.gameObject;
            if (Minimap.Instance.OtherPlayer != null)
            {
                break;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        //weapon = GetComponent<Weapon>();
       
    }

    protected override void Update()
    {
        base.Update();
        WeaponRotation();
        Cooldown();
    }

    public void Shoot(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            weapon.IsShooting = true;
        }
        else if (context.canceled)
        {
            weapon.IsShooting = false;
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

    public void Punch()
    {
        if (OnCoolDown() == false)
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward,
                out RaycastHit hitInfo, punchRange, layersThatShouldBeHit))
            {
                HealthHandler healthHandler = hitInfo.transform.GetComponent<HealthHandler>();
                if (healthHandler)
                {
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
