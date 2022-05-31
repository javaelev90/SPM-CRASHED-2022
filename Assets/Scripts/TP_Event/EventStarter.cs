using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventCallbacksSystem;
using Photon.Pun;
using System;

public class EventStarter : MonoBehaviourPunCallbacks
{
    [Header("Teleporter (Don't touch)")]
    public GameObject teleporter;
    public Material emissionFill;
    public ParticleSystem swirl;
    public ParticleSystem beam;
    public bool teleportTimeDone;
    public bool teleporPositionRight;

    [Header("Dome (Don't touch)")]
    public GameObject dome;

    [Header("Event")]
    public float eventTime = 30f;
    public Transform shipPart;
    public float pickUpTime = 2f;

    [Header("Ship Part")]
    public GameObject missingPart;
    public GameObject attachedPart;
    


    public List<ObjectSpawner> eventSpawners;
    private bool eventStarted = false;
    private Transform player;
    private Transform otherPlayer;

    // Start is called before the first frame update
    private void Start()
    {
        foreach (Transform child in transform)
        {
            if (child.GetComponent<ObjectSpawner>())
            {
                child.GetComponent<ObjectSpawner>().ResetSpawner();
                eventSpawners.Add(child.GetComponent<ObjectSpawner>());
            }
        }
        player = GameManager.player.transform;
        otherPlayer = GameManager.otherPlayer.transform;
    }

    [PunRPC]
    private void StartEventRPC()
    {
        //timeDisplay.DisplayingTime(false);
        //light.SetCycleOngoing(false);
        if (eventStarted == false)
        {
            eventStarted = true;
            EventEvent eventEvent = new EventEvent(true);
            eventEvent.EventTime = eventTime;
            EventSystem.Instance.FireEvent(eventEvent);

            dome.SetActive(true);
            teleporter.SetActive(true);

            foreach (ObjectSpawner objectSpawner in eventSpawners)
            {
                objectSpawner.TriggerSpawner();
            }

            StartCoroutine(TeleportIn(otherPlayer));
        }

    }
    [ContextMenu("Start Event")]
    public void StartEvent()
    {
        if (eventStarted == false)
        {
            photonView.RPC(nameof(StartEventRPC), RpcTarget.Others);

            eventStarted = true;
            EventEvent eventEvent = new EventEvent(true);
            eventEvent.EventTime = eventTime;
            EventSystem.Instance.FireEvent(eventEvent);

            dome.SetActive(true);
            teleporter.SetActive(true);

            foreach (ObjectSpawner objectSpawner in eventSpawners)
            {
                objectSpawner.TriggerSpawner();
            }

            StartCoroutine(TeleportIn(player));
        }
        //timeDisplay.DisplayingTime(false);
        ////light.SetCycleOngoing(false);
        //EventSystem.Instance.FireEvent(new EventEvent(true));

        //dome.SetActive(true);

        //foreach (ObjectSpawner objectSpawner in eventSpawners)
        //{
        //    objectSpawner.TriggerSpawner();
        //}

        //StartCoroutine(TeleportIn(eventTime));
    }

    private IEnumerator TeleportIn(Transform targetPlayer)
    {
        float timer = 0;
        Vector3 startPosition = shipPart.transform.position;
        Vector3 startSize = shipPart.transform.localScale;

        while (timer < eventTime)
        {
            timer += Time.deltaTime;
            emissionFill.SetFloat("EmissionFill", timer / eventTime);
            if (timer < pickUpTime)
            {
                shipPart.position = Vector3.Lerp(startPosition, targetPlayer.position, timer / pickUpTime);
                shipPart.localScale = Vector3.Lerp(startSize, Vector3.zero, timer / pickUpTime);
            }else if(shipPart != null)
            {
                Destroy(shipPart);
            }
            yield return null;
        }
        ActivatTeleport();
        
        //EndEvent();  
    }

    private void ActivatTeleport()
    {
        swirl.Play();
        beam.Play();
        if (PhotonNetwork.IsMasterClient)
        {
            teleportTimeDone = true;
            Teleport();
        }
    }

    public void Teleport(bool teleporPositionRight)
    {
        this.teleporPositionRight = teleporPositionRight;
        Teleport();
    }

    private void Teleport()
    {
        if (teleportTimeDone && teleporPositionRight)
            EndEvent();
    }

    private void EndEvent()
    {
        photonView.RPC(nameof(EndEventRPC), RpcTarget.All);
    }

    [PunRPC]
    public void EndEventRPC()
    {
        EventSystem.Instance.FireEvent(new EventEvent(false));
        EventSystem.Instance.FireEvent(new AttachPartEvent(attachedPart, missingPart));
        EventSystem.Instance.FireEvent(new TeleportToShipEvent());
        Destroy(gameObject);
    }
}
