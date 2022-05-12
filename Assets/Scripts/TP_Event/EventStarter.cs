using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventCallbacksSystem;

public class EventStarter : MonoBehaviour
{
    public float eventTime = 30f;
    public float minTimeLeftAfter = 120f;
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

    public void StartEvent()
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

    private IEnumerator TeleportIn(float eventTime)
    {
        yield return new WaitForSeconds(eventTime);
        //timeDisplay.DisplayingTime(true);
        //light.SetCycleOngoing(true);
        //light.SetMinTimeUntilDawn(minTimeLeftAfter);
        EventSystem.Instance.FireEvent(new EventEvent(false));
        EventSystem.Instance.FireEvent(new AttachPartEvent());
        teleportToShip.TP();
        Destroy(gameObject);
    }
}
