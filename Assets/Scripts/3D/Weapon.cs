using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Mesh settings")]
    [Tooltip("Position where the gun barrel ends")]
    [SerializeField] Transform muzzlePosition;

    [Header("Weapon settings")]
    [SerializeField] float weaponRange = 15f;
    [SerializeField] float weaponDamage = 1f;
    [SerializeField] float delayBetweenShots = 0.5f;
    [Tooltip("If gun will shoot continuously when shoot button is pressed vs one shot per click.")]
    [SerializeField] public bool automaticWeapon = false;
    [SerializeField] LayerMask layersThatShouldBeHit;

    private float shotCooldown = 0f;

    void Start()
    {
        
    }

    void Update()
    {
        Cooldown();
    }

    private void Cooldown()
    {
        if (OnCoolDown())
        {
            shotCooldown += Time.deltaTime;
        }
    }

    private bool OnCoolDown()
    {
        return shotCooldown < delayBetweenShots;
    }

    public void Shoot()
    {
        if(!OnCoolDown())
        {
            if(Physics.Raycast(Camera.main.ViewportPointToRay(Vector3.zero),
                out RaycastHit hitInfo, weaponRange, layersThatShouldBeHit))
            {
                if (hitInfo.transform.gameObject.CompareTag("Enemy"))
                {
                    //hitInfo.transform.GetComponent<AIMovement>().
                }
            }
        }
    }
}
