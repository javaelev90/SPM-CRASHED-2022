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

}
