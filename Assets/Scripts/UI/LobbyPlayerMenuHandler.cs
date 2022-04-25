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
    LobbyManager lobbyManager;

    public string LobbyPlayerId { get; set; }
    public bool IsReady { get; private set; }
    private bool hasReadyChanged;

    private void Start()
    {
        lobbyManager = FindObjectOfType<LobbyManager>();
        IsReady = IsPlayerReady();
    }

    private void Update()
    {
        if(PhotonNetwork.LocalPlayer.UserId == LobbyPlayerId)
        {
            IsReady = IsPlayerReady();

            if (hasReadyChanged != IsReady)
            {
                hasReadyChanged = IsReady;
                photonView.RPC("UpdateReadyCheck", RpcTarget.AllBuffered, IsReady);
            }
        }
        
    }

    private bool IsPlayerReady()
    {
        return lobbyManager.PlayerChoice != Character.NONE;
    }

    public void SetPlayerName(string name)
    {
        playerName.text = name;
    }

    [PunRPC]
    public void UpdateReadyCheck(bool isReady)
    {
        IsReady = isReady;
        ToggleReadyCheck();
    }

    private void ToggleReadyCheck()
    {
        readyCheck.gameObject.SetActive(IsReady);
    }
}
