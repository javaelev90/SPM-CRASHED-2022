using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterTeleporter : MonoBehaviour
{
    public int playerOnTeleport = 0;
    public EventStarter eventStarter;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
            playerOnTeleport++;
        if((playerOnTeleport > 0 && !(GameManager.otherPlayer != null && GameManager.otherPlayer.activeSelf)) || playerOnTeleport > 1))
        {
            eventStarter.Teleport(true);
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerOnTeleport--;
            eventStarter.Teleport(false);
        }
    }
}
