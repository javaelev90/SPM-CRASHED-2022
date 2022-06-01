using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EventCallbacksSystem;
using UnityEngine.InputSystem;

public class PlayerUpgradePanel : MonoBehaviour
{
    [SerializeField] private GameObject damageUpgradeButton;
    [SerializeField] private GameObject gunRateTurretHealthButton;

    [SerializeField] private int healthUpgradeAmount = 2;
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
            damageUpgradeButton.GetComponent<Text>().text = "Upgrade Turret Damage";
            gunRateTurretHealthButton.GetComponent<Text>().text = "Upgade Turret Health";
        }
        else if (player == Character.SOLDIER)
        {
            damageUpgradeButton.GetComponent<Text>().text = "Upgrade Weapon Damage";
            gunRateTurretHealthButton.GetComponent<Text>().text = "Upgade Fire Rate";
        }
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;
        GameManager.player.GetComponent<PlayerInput>().enabled = false;
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        GameManager.player.GetComponent<PlayerInput>().enabled = true;
    }

    public void HealthUpgrade()
    {
        EventSystem.Instance.FireEvent(new HealthUpgradeEvent(healthUpgradeAmount));
        Exit();
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
        Exit();
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
        Exit();
    }

    private void Exit()
    {
        Cursor.lockState = CursorLockMode.Locked;
        GameManager.player.GetComponent<PlayerInput>().enabled = true;
        gameObject.SetActive(false);
    }
}