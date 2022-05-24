using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventCallbacksSystem;

public class TeleportToShip : MonoBehaviour
{
    public GameObject playerObject;

    public GameObject soldierTPPosition;
    public GameObject engenerTPPosition;

    private void Start()
    {
        EventSystem.Instance.RegisterListener<TeleportToShipEvent>(Teleport);
    }


    public void Teleport(TeleportToShipEvent teleport)
    {
        //soldier.transform.position = soldierTPPosition.transform.position;
        playerObject = GameManager.player;

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
