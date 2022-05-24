   using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class Objective : MonoBehaviour
{

    public TextMeshProUGUI ShipPart;
    public int part;
    public Ship ship;
    // Start is called before the first frame update
    void Start()
    {
    }
    IEnumerator FindInvetory(){
        while(!GameObject.FindGameObjectWithTag("Player") && !GameObject.FindGameObjectWithTag("Player").GetComponent<PhotonView>().IsMine){
            yield return new WaitForSeconds(2);
        }
        ship = GameObject.FindGameObjectWithTag("Player").GetComponent<Ship>();
        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        if(ship){
           ShipPart.text = " Ship Parts found: " + ship.shipUpgradeCost.Count;
        }

    }
}