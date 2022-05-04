using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    [SerializeField] private GameObject Panel;
    [SerializeField] private float radius = 10f;
    private bool triggerActive = false;
    [SerializeField] Transform player;

    void Start()
    {
        StartCoroutine(Wait(5));
        //Wait(5);
    }

    IEnumerator Wait(float sec)
    {
        while (player == null)
        {
            yield return new WaitForSeconds(sec);
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

    }

    private void Update()
    {
        if(player != null)
        {
            Transform PartPickupDest;
            PartPickupDest = player.transform.Find("CarryPos");
            Transform PartPickup;
            PartPickup = PartPickupDest.transform.Find("ShipPickup");

            Collider[] colliderHits = Physics.OverlapSphere(transform.position, radius);

            foreach (Collider col in colliderHits)
            {
                if (col.tag == ("Player") && Input.GetKeyDown(KeyCode.E) && Panel != null && PartPickup != null)
                {
                    Debug.Log("Inside");
                    Destroy(PartPickup.gameObject);
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    public void OpenUpgradePanel()
    {
        //triggerActive = true;
        if(Panel != null)
        {
            bool isActive = Panel.activeSelf;
            Panel.SetActive(!isActive);
        }
    }
}