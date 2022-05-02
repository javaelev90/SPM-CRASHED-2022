using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

public class LobbyPlayerMenuHandler : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text playerName;
    [SerializeField] Image readyCheck;

    public string LobbyPlayerId { get; set; }
    public bool IsReady { get; set; }

    private void Update()
    {
        ToggleReadyCheck();
    }

    public void SetPlayerName(string name)
    {
        playerName.text = name;
    }

    private void ToggleReadyCheck()
    {
        readyCheck.gameObject.SetActive(IsReady);
    }
}
