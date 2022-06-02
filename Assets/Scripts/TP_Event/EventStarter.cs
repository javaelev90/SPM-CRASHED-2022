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

    public AudioSource source;

    public AudioSource audioSource;
  
    public AudioClip triggerSound;

    public AudioClip teleporterSound;
    

    //public GameObject uiObject;


    public List<ObjectSpawner> eventSpawners;
    private bool eventStarted = false;
    

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
        source = GetComponent<AudioSource>();
        audioSource = GetComponent<AudioSource>();
        //uiObject.SetActive(false);
        
    }

    [PunRPC]
    private void StartEventRPC()
    {
        //timeDisplay.DisplayingTime(false);
        //light.SetCycleOngoing(false);
        if (eventStarted == false)
        {
            InitiatEvent();

            StartCoroutine(TeleportIn(GameManager.otherPlayer.transform));
        }

    }
    [ContextMenu("Start Event")]
    public void StartEvent()
    {
        if (eventStarted == false)
        {
            photonView.RPC(nameof(StartEventRPC), RpcTarget.Others);

            InitiatEvent();

            StartCoroutine(TeleportIn(GameManager.player.transform));
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

    private void InitiatEvent()
    {
        eventStarted = true;
        EventEvent eventEvent = new EventEvent(true);
        eventEvent.EventTime = eventTime;
        EventSystem.Instance.FireEvent(eventEvent);

        dome.SetActive(true);
         source.Play();
        teleporter.SetActive(true);

        foreach (ObjectSpawner objectSpawner in eventSpawners)
        {
            objectSpawner.TriggerSpawner();
        }
    }

    private IEnumerator TeleportIn(Transform targetPlayer)
    {
        float timer = 0;
        Vector3 startPosition = shipPart.position;
        Vector3 startSize = shipPart.localScale;
        shipPart.gameObject.GetComponent<MeshCollider>().enabled = false;

        while (timer < eventTime)
        {
            timer += Time.deltaTime;
            emissionFill.SetFloat("EmissionFill", timer / eventTime);
            if (timer < pickUpTime)
            {
                shipPart.position = Vector3.Lerp(startPosition, targetPlayer.position + (Vector3.up * 1.35f), Mathf.Pow(timer, 2)/ Mathf.Pow(pickUpTime, 2));
                shipPart.localScale = Vector3.Lerp(startSize, Vector3.zero, Mathf.Log((timer / pickUpTime) + 1, 2)/* (-Mathf.Pow(timer - pickUpTime, 2) / Mathf.Pow(pickUpTime, 2)) + 1*/) ;
            }else if(shipPart != null)
            {
                Destroy(shipPart.gameObject);
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
        audioSource.PlayOneShot(teleporterSound);
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
