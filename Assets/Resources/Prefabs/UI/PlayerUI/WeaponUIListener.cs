using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventCallbacksSystem;

public class WeaponUIListener : MonoBehaviour
{
    [SerializeField] private GameObject gunSlot;
    [SerializeField] private GameObject stunGunSlot;

    private float stunGunCooldown;
    private int ammunition;

    void Start()
    {
        if (Minimap.Instance.Player.GetComponent<SoldierCharacter>() == true)
        {
            gunSlot.SetActive(true);
            EventSystem.Instance.RegisterListener<WeaponAmmunitionUpdateEvent>(UpdateAmmo);
        }
        else
        {
            stunGunSlot.SetActive(true);
            EventSystem.Instance.RegisterListener<StungunCoolDownEvent>(UpdateStunGun);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateAmmo(WeaponAmmunitionUpdateEvent ev)
    {

    }

    public void UpdateStunGun(StungunCoolDownEvent ev)
    {

    }
}
