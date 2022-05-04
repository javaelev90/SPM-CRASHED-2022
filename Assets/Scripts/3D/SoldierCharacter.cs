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

    protected override void Awake()
    {
        base.Awake();
        weapon = GetComponent<Weapon>();
    }

    protected override void Update()
    {
        base.Update();
        WeaponRotation();
    }

    public void Shoot()
    {
        weapon.Shoot();
    }

    public void CookFood()
    {
        if (Physics.Raycast(Camera.main.ViewportPointToRay(Vector3.zero),
            out RaycastHit hitInfo,
            interactionDistance,
            fireLayer))
        {
            //Cook
        }
    }

    public void ConsumeFood()
    {
        if (Input.GetKey(KeyCode.X))
        {
            if (inventory.CookedAlienMeat > 0)
            {
                inventory.eat();
                photonView.RPC("AddHealth", RpcTarget.All, 1);
            }
        }
    }
}
