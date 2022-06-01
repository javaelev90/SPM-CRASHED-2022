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

    public class EnterLobbyEvent : Event
    {
        public bool IsNameLongEnough { get; set; }
        public EnterLobbyEvent(bool isNameLongEnough)
        {
            IsNameLongEnough = isNameLongEnough;
        }
    }

    public class JoinedLobbyEvent : Event
    {
    }

    public class LeaveLobbyEvent : Event
    {
    }

    public class EventEvent : Event
    {
        public bool Start { get; set; }
        public float EventTime { get; set; }

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

    public class TeleportToShipEvent : Event
    {
    }

    public class GameOverEvent : Event
    {
        public string Reason { get; set; }

        public GameOverEvent(string reason)
        {
            Reason = reason;
        }
    }

    public class AttachPartEvent : Event
    {
        public GameObject AttachedPart { get; set; }
        public GameObject MissingPart { get; set; }

        public AttachPartEvent(GameObject attachedPart, GameObject missingPart)
        {
            AttachedPart = attachedPart;
            MissingPart = missingPart;
        }
    }


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
        public System.Type type;
        public Dictionary<System.Type, int> Amounts { get { return amounts; } set { amounts = value; } }

        public UpdateUIAmountsEvent(Dictionary<System.Type, int> amounts)
        {
            this.amounts = amounts;
        }

        public UpdateUIAmountsEvent() { }
    }

    public class ImmortalEvent : Event
    {
    }

    public class ShipUppgradPanelEvent : Event
    {
    }

    public class OpenPlayerUpgradePanelEvent : Event
    {
    }
    public class HealthUpgradeEvent : Event
    {
        private int upgradeAmount;
        public int UpgradeAmount { get { return upgradeAmount; } }

        public HealthUpgradeEvent(int upgradeAmount)
        {
            this.upgradeAmount = upgradeAmount;
        }
    }
    public class GunDamageUpgradeEvent : Event
    {
        private int upgradeAmount;
        public int UpgradeAmount { get { return upgradeAmount; } }

        public GunDamageUpgradeEvent(int upgradeAmount)
        {
            this.upgradeAmount = upgradeAmount;
        }
    }
    public class GunFireRateUpgradeEvent : Event
    {
        private float upgradePercent;
        public float UpgradePercent { get { return upgradePercent; } }

        public GunFireRateUpgradeEvent(float upgradePercent)
        {
            this.upgradePercent = upgradePercent;
        }
    }

    public class TurretDamageUpgradeEvent : Event
    {
        private int upgradeAmount;
        public int UpgradeAmount { get { return upgradeAmount; } }

        public TurretDamageUpgradeEvent(int upgradeAmount)
        {
            this.upgradeAmount = upgradeAmount;
        }
    }

    public class TurretHealthUpgradeEvent : Event
    {
        private int upgradeAmount;
        public int UpgradeAmount { get { return upgradeAmount; } }

        public TurretHealthUpgradeEvent(int upgradeAmount)
        {
            this.upgradeAmount = upgradeAmount;
        }
    }

    public class ShipUpgradeProgressionEvent : Event
    {
        public int UpgradeNumber { get; set; }
        public int TotalNumberOfParts { get; private set; }

        public ShipUpgradeProgressionEvent(int upgradeNumber, int totalParts)
        {
            UpgradeNumber = upgradeNumber;
            TotalNumberOfParts = totalParts;
        }

        public ShipUpgradeProgressionEvent(int upgradeNumber)
        {
            UpgradeNumber = upgradeNumber;
        }
    }

    public class ObjectiveUpdateEvent : Event
    {
        public bool IsNight { get; set; }
        public bool IsShipPartEvent { get; set; }

        public string ObjectiveDescription { get; set; }
        
    }

    public class StungunCoolDownEvent : Event
    {
        public float CoolDownTime { get; private set; }

        public StungunCoolDownEvent(float coolDownTime)
        {
            CoolDownTime = coolDownTime;
        }
    }

    public class WeaponAmmunitionUpdateEvent : Event
    {
        public int AmmunitionAmount { get; set; }
        public int MaxAmmunitionAmount { get; private set; }
        public int GreenGooAmount { get; set; }

        public WeaponAmmunitionUpdateEvent(int ammunitionAmount, int maxAmmunitionAmount)
        {
            AmmunitionAmount = ammunitionAmount;
            MaxAmmunitionAmount = maxAmmunitionAmount;
        }
    }

    public class LockControlsEvent : Event
    {
        public bool AreControlsLocked { get; private set; }

        public LockControlsEvent(bool lockControls)
        {
            AreControlsLocked = lockControls;
        }
    }
}