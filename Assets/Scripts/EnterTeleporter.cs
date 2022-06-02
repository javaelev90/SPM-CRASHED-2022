using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnterTeleporter : MonoBehaviour
{
    public int playerOnTeleport = 0;
    public EventStarter eventStarter;
    [SerializeField] private GameObject playersOnTeleporterText;

    /*
    private void Update()
    {
        if (playerOnTeleport < 2)
        {
            playersOnTeleporterText.SetActive(true);
        }
        else 
        {
            playersOnTeleporterText.SetActive(false);
        }
    }
    */

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
            playerOnTeleport++;
        if (playerOnTeleport == 1)
        {
            playersOnTeleporterText.SetActive(true);
        }
            if ((playerOnTeleport > 0 && !(GameManager.otherPlayer != null && GameManager.otherPlayer.activeSelf)) || playerOnTeleport > 1)
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
        playersOnTeleporterText.SetActive(false);
    }
}
