using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventCallbacksSystem;
using TMPro;

public class WeaponUIListener : MonoBehaviour
{
    [Header("Rifle settings")]
    [SerializeField] private GameObject gunSlot;
    [SerializeField] private TMP_Text ammunitionText;

    [Header("Stungun settings")]
    [SerializeField] private GameObject stunGunSlot;
    private float coolDownTime;
    private bool isCoolinDown;

    private SoldierCharacter soldier;
    private Engineer engineer;

    private void Start()
    {
        if (GameManager.player.GetComponent<SoldierCharacter>() == true) // maybe the player refences should be cached at the gamemanager class instead
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
        ammunitionText.text = ev.AmmunitionAmount.ToString();
    }

    public void UpdateStunGun(StungunCoolDownEvent ev)
    {

    }
}
