using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventStarter : MonoBehaviour
{
    public float eventTime = 30f;
    public GameObject dome;
    public TeleportToShip teleportToShip;
    public Ship ship;
    public List<ObjectSpawner> eventSpawners;
    
    
    // Start is called before the first frame update
    private void Start()
    {
        foreach (Transform child in transform)
        {
            if (child.GetComponent<ObjectSpawner>())
            {
                eventSpawners.Add(child.GetComponent<ObjectSpawner>());
            }
        }
        
    }

    public void StartEvent()
    {
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
        teleportToShip.TP();
        ship.newPartObtained();
        Destroy(gameObject);
    }
}
