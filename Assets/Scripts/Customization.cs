using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customization : MonoBehaviour
{
    public GameObject bootsHelmet;
    public GameObject backpackWeaponSoldier;
    public GameObject backpackWeaponEngineer;
    public GameObject bodySoldier;
    public GameObject bodyEngineer;

    public void ChangeMaterialBootsHelmet(Material material)
    {
        bootsHelmet.GetComponent<MeshRenderer>().material = material;
    }

    public void ChangeMaterialBackpackWeaponSoldiert(Material material)
    {
        backpackWeaponSoldier.GetComponent<MeshRenderer>().material = material;
    }
    public void ChangeMaterialBackpackWeaponEngineer(Material material)
    {
        backpackWeaponEngineer.GetComponent<MeshRenderer>().material = material;
    }
    public void ChangeMaterialBodySoldier(Material material)
    {
        bodySoldier.GetComponent<MeshRenderer>().material = material;
    }
    public void ChangeMaterialBodyEngineer(Material material)
    {
        bodyEngineer.GetComponent<MeshRenderer>().material = material;
    }

}
