using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LobbyPlayerMenuHandler : MonoBehaviour
{
    [SerializeField] TMP_Text playerName;
    [SerializeField] Image readyCheck;
    public int LobbyPlayerId { get; set; }

    public void SetPlayerName(string name)
    {
        playerName.text = name;
    }

    public void ToggleReadyCheck()
    {
        readyCheck.gameObject.SetActive(!readyCheck.gameObject.activeSelf);
    }
}
