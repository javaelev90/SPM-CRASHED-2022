using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventStarter : MonoBehaviour
{
    public float eventTime = 30f;
    public float minTimeLeftAfter = 120f;
    public GameObject dome;
    public TeleportToShip teleportToShip;
    public Ship ship;
    public Timer timeDisplay;
    public LightingManager light;
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
        timeDisplay.DisplayingTime(false);
        light.SetCycleOngoing(false);
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
        timeDisplay.DisplayingTime(true);
        light.SetCycleOngoing(true);
        light.SetMinTimeUntilDawn(minTimeLeftAfter);
        teleportToShip.TP();
        ship.newPartObtained();
        Destroy(gameObject);
    }
}
