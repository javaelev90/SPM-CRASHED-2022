using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Ship : MonoBehaviour
{
    [SerializeField] int upgradeCostGreenGoo = 3;
    [SerializeField] int upgradeCostMetal = 3;
    [SerializeField] private UpgradePanel Panel;
    [SerializeField] private Text errorMessage;
    [SerializeField] private float radius = 10f;
    private bool triggerActive = false;
    [SerializeField] Transform player;

    Controller3D playeriut;
    private Transform PartPickup;
    private Transform PartPickupDest;
    void Start()
    {
        StartCoroutine(Wait(1));
        //Wait(5);
        //playeriut = new PlayerInputActions();
        
        PartPickupDest = player.transform.Find("CarryPos");
        
        PartPickup = PartPickupDest.transform.Find("ShipPickup");
    }

    IEnumerator Wait(float sec)
    {
        while (player == null)
        {
            yield return new WaitForSeconds(sec);
            player = FindObjectOfType<Engineer>().transform;
        }

    }

    private void Update()
    {
        if(player != null)
        {


            Collider[] colliderHits = Physics.OverlapSphere(transform.position, radius);

            foreach (Collider col in colliderHits)
            {
                //  Debug.Log(playeriut.playerActions.Player.PickUp.IsPressed() );
                Engineer controller = col.transform.gameObject.GetComponent<Engineer>();
                if (controller && controller.playerActions.Player.PickUp.IsPressed() && Panel != null && PartPickup != null)
                {
                    Debug.Log("Inside");
                    OpenUpgradePanel();
                }
                else
                {
                    //Debug.Log("Outside");
                }
            }

            /*
            if (Physics.SphereCast(ray, radius, out hit, 10f))
            {
                if (hit.collider.tag == "Player")
                {
                    Debug.Log("Inside");
                    OpenUpgradePanel();
                }
            }
            */

            /*
            if (Physics.Raycast(ray, out hit, height))
            {

                if (hit.collider.tag == ("Player"))
                {
                    OpenUpgradePanel();
                }
            }
            */
        }

    }

    private bool TakeResources()
    {
        if (player != null)
        {
            Inventory inventory = player.gameObject.GetComponent<Inventory>();
            if (inventory.GreenGoo >= upgradeCostGreenGoo && inventory.Metal >= upgradeCostMetal)
            { 
                return inventory.removeMetalAndGreenGoo(upgradeCostMetal, upgradeCostGreenGoo);
            }
        }
        return false;
    }

    //private void hasResources(){

    //    Collider[] colliderHits = Physics.OverlapSphere(transform.position, radius);
    //    foreach (Collider col in colliderHits){

    //        Controller3D controller = col.transform.gameObject.GetComponent<Controller3D>();
    //        if(controller.playerActions.Player.PickUp.IsPressed() && gameObject.CompareTag("GreenGoo"))
    //        {

    //        }
    //        if(controller.playerActions.Player.PickUp.IsPressed() && gameObject.CompareTag("Metal"))
    //        {
                
    //        }
         
    //    }
    //}

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    public void OpenUpgradePanel()
    {
        //triggerActive = true;
        if(Panel != null)
        {
            bool isActive = Panel.gameObject.activeSelf;
            errorMessage.gameObject.SetActive(false);
            Panel.gameObject.SetActive(!isActive);


        }
    }

    public void TestUpgrade()
    {
        if (TakeResources() == false)
        {
            errorMessage.gameObject.SetActive(true);
            errorMessage.text = $"Too few resources to upgrade.\n Requires metal: {upgradeCostMetal}, green goo: {upgradeCostGreenGoo}";
        }
        else
        {
            Panel.ClosePanel();
            Destroy(PartPickup.gameObject);
        }
    }
}