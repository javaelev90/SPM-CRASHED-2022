using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup_ShipPart : MonoBehaviour
{
    public EventStarter eventStarter;

    public void PickUpPart()
    {
        eventStarter.StartEvent();
    }
}
