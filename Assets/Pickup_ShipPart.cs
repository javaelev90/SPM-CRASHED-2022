using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup_ShipPart : MonoBehaviour
{
    //[SerializeField] GameObject textColliderObject;
    public EventStarter eventStarter;

    public void PickUpPart()
    {
       // textColliderObject.SetActive(false);
        eventStarter.StartEvent();
    }
}
