using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Weapon))]
public class SoldierCharacter : Controller3D
{
    [SerializeField] Weapon weapon;

    protected override void Awake()
    {
        base.Awake();
        weapon = GetComponent<Weapon>();
    }

    public void Shoot()
    {
        weapon.Shoot();
    }
}
