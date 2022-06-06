using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventCallbacksSystem;
using TMPro;
using UnityEngine.UI;

public class WeaponUIListener : MonoBehaviour
{
    [Header("Rifle settings")]
    [SerializeField] private GameObject gunSlot;
    [SerializeField] private TMP_Text ammunitionText;
    [SerializeField] private TMP_Text reloadText;
    [SerializeField] private float pingPongValue;
    [SerializeField] private float pingPongMultplier;
    private int ammunitionAmount;
    private bool hasAmmunition;
    private bool hasGreenGoo;
    private Color reloadTextColor;

    [Header("Stungun settings")]
    [SerializeField] private GameObject stunGunSlot;
    [SerializeField] private Image reloadBar;
    private float coolDownTime = 0.5f;
    private bool isCoolinDown;


    private void OnEnable()
    {
        EventSystem.Instance.RegisterListener<WeaponAmmunitionUpdateEvent>(UpdateAmmo);
        EventSystem.Instance.RegisterListener<UpdateUIAmountsEvent>(UpdateGreenGooAmount);
        EventSystem.Instance.RegisterListener<StungunCoolDownEvent>(UpdateStunGun);
    }

    private void OnDisable()
    {
        EventSystem.Instance.UnregisterListener<WeaponAmmunitionUpdateEvent>(UpdateAmmo);
        EventSystem.Instance.UnregisterListener<StungunCoolDownEvent>(UpdateStunGun);
    }

    private void Start()
    {
        if (GameManager.player.GetComponent<SoldierCharacter>() == true) // maybe the player refences should be cached at the gamemanager class instead
        {
            gunSlot.SetActive(true);
        }
        else
        {
            stunGunSlot.SetActive(true);
        }

        reloadTextColor = reloadText.color;
        reloadBar.fillAmount = 1f;
    }


    void Update()
    {
        if (gunSlot.activeSelf)
        {
            if (hasAmmunition == false && hasGreenGoo == false)
            {
                if (reloadText.gameObject.activeSelf == false)
                    reloadText.gameObject.SetActive(true);

                reloadTextColor.a = Mathf.Clamp(Mathf.PingPong(Time.time * pingPongMultplier, pingPongValue), 0, pingPongValue);
                reloadText.color = reloadTextColor;
            }
        }

        if (stunGunSlot.activeSelf)
        {
            if (isCoolinDown)
            {
                reloadBar.fillAmount = 0f;
                isCoolinDown = false;
            }
            else
            {
                reloadBar.fillAmount += (Time.deltaTime * (1f/ coolDownTime));
                reloadBar.fillAmount = Mathf.Clamp(reloadBar.fillAmount, 0f, 1f);
            }
        }

    }

    public void UpdateAmmo(WeaponAmmunitionUpdateEvent ev)
    {
        ammunitionText.text = ev.AmmunitionAmount.ToString();
        ammunitionAmount = ev.AmmunitionAmount;

        if (ammunitionAmount == 0)
        {
            hasAmmunition = false;
        }
        if (ammunitionAmount > 0)
        {
            hasAmmunition = true;
        }

        ResetReloadText();
    }

    public void UpdateGreenGooAmount(UpdateUIAmountsEvent ev)
    {

        if (ev.Amounts[typeof(GreenGoo)] > 0)
        {
            hasGreenGoo = true;
        }

        if (ev.Amounts[typeof(GreenGoo)] == 0)
        {
            hasGreenGoo = false;
        }

        ResetReloadText();
    }

    private void ResetReloadText()
    {
        if (hasAmmunition == true || hasGreenGoo == true)
        {
            reloadText.gameObject.SetActive(false);
        }
    }

    public void UpdateStunGun(StungunCoolDownEvent ev)
    {
        if(coolDownTime == 0)
        {
            coolDownTime = ev.CoolDownTime;
        }

        isCoolinDown = true;
    }
}
