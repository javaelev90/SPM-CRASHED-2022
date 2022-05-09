using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportToShip : MonoBehaviour
{
    private GameObject playerObject;

    public GameObject soldierTPPosition;
    public GameObject engenerTPPosition;


    public void TP()
    {
        //soldier.transform.position = soldierTPPosition.transform.position;
        playerObject = GameManager.playerObject;

        if (playerObject.GetComponent<Engineer>())
        {
            playerObject.transform.position = engenerTPPosition.transform.position;
        }
        else
        {
            playerObject.transform.position = soldierTPPosition.transform.position;
        }
    }
}
