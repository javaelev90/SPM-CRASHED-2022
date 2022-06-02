using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventCallbacksSystem;
using UnityEngine.UI;

public class DeathPopUp : MonoBehaviour
{
    [SerializeField] private Text deathReasonText;
    public GameObject GameOverPanel;

    void Start()
    {
        EventSystem.Instance.RegisterListener<GameOverEvent>(WriteText);
    }

    void WriteText(GameOverEvent gameOverEvent)
    {
        GameOverPanel.SetActive(true);
        EventSystem.Instance.FireEvent(new LockControlsEvent(GameOverPanel.activeSelf));
        deathReasonText.text = gameOverEvent.Reason;
    }
}
