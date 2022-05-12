using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace EventCallbacksSystem
{
    public abstract class Event
    {
        public string Description { get; set; }
    }

    public class DebugEvent : Event
    {
        public int VerbosityLevel { get; set; }
        public string DebugInfo { get; set; }
    }

    public class DeathEvent : Event
    {
        public GameObject GameObjectUnit { get; set; }
    }

    public class TriggerEvent : Event
    {
        private bool isTriggered = true;
        public bool IsTriggered { get { return isTriggered; } }
    }

    public class StartLobbyEvent : Event
    {
        public bool IsNameLongEnough { get; set; }
        public StartLobbyEvent(bool isNameLongEnough)
        {
            IsNameLongEnough = isNameLongEnough;
        }
    }

    public class LeaveLobbyEvent : Event
    {
    }

    public class EventEvent : Event
    {
        public bool Start { get; set; }

        public EventEvent(bool start)
        {
            Start = start;
        }
    }

    public class ShipPartEvent : Event
    {
        public float TimeUntilDawn { get; set; }

        public ShipPartEvent(float timeUntilDawn)
        {
            TimeUntilDawn = timeUntilDawn;
        }
    }

    public class PlayerHealthUpgradeEvent : Event { }

    public class GunDamageUpgradeEvent : Event { }

    public class GunReloadUpgradeEvent : Event { }

    public class TurretHealthUpgradeEvent : Event { }

    public class TurretDamageUpgradeEvent : Event { }


    public class TypeToInventoryEvent : Event
    {
        private Pickup_Typs.Pickup type;
        public Pickup_Typs.Pickup Type { get { return type; } }

        public TypeToInventoryEvent(Pickup_Typs.Pickup type)
        {
            this.type = type;
        }
    }

    public class UpdateUIAmountsEvent : Event
    {
        private Dictionary<System.Type, int> amounts;
        private int amount;
        private Pickup_Typs.Pickup type;
        public Dictionary<System.Type, int> Amounts { get { return amounts; } set { amounts = value; } }

        public UpdateUIAmountsEvent(Dictionary<System.Type, int> amounts)
        {
            this.amounts = amounts;
        }

        public UpdateUIAmountsEvent() { }
    }
}