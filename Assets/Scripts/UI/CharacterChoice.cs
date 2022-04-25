using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharacterChoice : MonoBehaviourPunCallbacks
{
    [SerializeField] public Character character;
    LobbyManager lobbyManager;

    public Toggle choiceButton;

    private void Start()
    {
        choiceButton = GetComponent<Toggle>();
        lobbyManager = FindObjectOfType<LobbyManager>();
    }

    [PunRPC]
    public void SendPlayerChoice(bool selected, string userId)
    {
        ToggleButton();
        lobbyManager.SetPlayerReady(selected, userId);
    }

    [PunRPC]
    public void SyncChoice(bool isActive)
    {
        choiceButton.interactable = isActive;
    }

    public void ChoosePlayer()
    {
        if (lobbyManager.PlayerChoice != character && choiceButton.interactable)
        {
            lobbyManager.PlayerChoice = character;
        } 
        else if (lobbyManager.PlayerChoice == character)
        {
            lobbyManager.PlayerChoice = Character.NONE;
        }
        PlayerPrefs.SetInt("CharacterChoice", (int)lobbyManager.PlayerChoice);
        photonView.RPC("SendPlayerChoice", RpcTarget.OthersBuffered, lobbyManager.PlayerChoice != Character.NONE, PhotonNetwork.LocalPlayer.UserId);
        lobbyManager.SetPlayerReady(lobbyManager.PlayerChoice != Character.NONE, PhotonNetwork.LocalPlayer.UserId);
    }

    private void ToggleButton()
    {
        choiceButton.interactable = !choiceButton.interactable;
    }
}
