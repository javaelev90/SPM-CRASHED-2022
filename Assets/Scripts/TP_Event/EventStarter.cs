using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventCallbacksSystem;
using Photon.Pun;

public class EventStarter : MonoBehaviourPunCallbacks
{
    public float eventTime = 30f;
    public GameObject dome;
    public TeleportToShip teleportToShip;
    public GameObject missingPart;
    public GameObject attachedPart;


    public List<ObjectSpawner> eventSpawners;
    
    
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
        
    }

    [PunRPC]
    private void StartEventRPC()
    {
        //timeDisplay.DisplayingTime(false);
        //light.SetCycleOngoing(false);
        EventSystem.Instance.FireEvent(new EventEvent(true));

        dome.SetActive(true);

        foreach (ObjectSpawner objectSpawner in eventSpawners)
        {
            objectSpawner.TriggerSpawner();
        }

        StartCoroutine(TeleportIn(eventTime));
    }

    public void StartEvent()
    {
        photonView.RPC(nameof(StartEventRPC), RpcTarget.All);
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

    private IEnumerator TeleportIn(float eventTime)
    {
        yield return new WaitForSeconds(eventTime);
        EventSystem.Instance.FireEvent(new EventEvent(false));
        EventSystem.Instance.FireEvent(new AttachPartEvent(attachedPart, missingPart));
        EventSystem.Instance.FireEvent(new TeleportToShipEvent());
        Destroy(gameObject);
    }
}
