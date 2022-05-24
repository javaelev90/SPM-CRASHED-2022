using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUIListener : MonoBehaviour
{
    [SerializeField] private GameObject gunSlot;
    [SerializeField] private GameObject stunGunSlot;

    private float stunGunCooldown;
    private int ammunition;

    void Start()
    {
        if(Minimap.Instance.Player.GetComponent<SoldierCharacter>() == true)
        {
            gunSlot.SetActive(true);
        }
        else
        {
            stunGunSlot.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
