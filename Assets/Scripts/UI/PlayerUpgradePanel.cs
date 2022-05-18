using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EventCallbacksSystem;

public class PlayerUpgradePanel : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject damage;
    [SerializeField] private GameObject gunRateTurretHealth;

    [SerializeField] private int upgradeAmount;
    [SerializeField] int gunDamageUpgradeAmount = 1;
    [SerializeField] float fireRateUpgradePercent = 0.2f;
    [SerializeField] int turretDamageUpgradeAmount = 1;
    [SerializeField] int turretHealthUpgradeAmount = 1;

    private Character player;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        player = GameManager.character;
        if(player == Character.ENGINEER)
        {
            damage.GetComponent<Text>().text = "Upgrade Turret Damage";
            gunRateTurretHealth.GetComponent<Text>().text = "Upgade Turret Health";
        }
        else if (player == Character.SOLDIER)
        {
            damage.GetComponent<Text>().text = "Upgrade Weapon Damage";
            gunRateTurretHealth.GetComponent<Text>().text = "Upgade Fire Rate";
        }
    }

    public void HealthUpgrade()
    {
        EventSystem.Instance.FireEvent(new HealthUpgradeEvent(upgradeAmount));
    }
    public void DamageUpgrade()
    {
        if (player == Character.SOLDIER)
        {
            EventSystem.Instance.FireEvent(new GunDamageUpgradeEvent(gunDamageUpgradeAmount));
        }
        else if (player == Character.ENGINEER)
        {
            EventSystem.Instance.FireEvent(new TurretDamageUpgradeEvent(turretDamageUpgradeAmount));
        }
    }
    public void GunRateTurretHealthUpgrade()
    {
        if (player == Character.SOLDIER)
        {
            EventSystem.Instance.FireEvent(new GunFireRateUpgradeEvent(fireRateUpgradePercent));
        }
        else if (player == Character.ENGINEER)
        {
            EventSystem.Instance.FireEvent(new TurretHealthUpgradeEvent(turretHealthUpgradeAmount));
        }
    }

}