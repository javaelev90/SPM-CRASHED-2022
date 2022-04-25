using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharacterChoice : MonoBehaviourPunCallbacks
{
    [SerializeField] Character character;
    LobbyManager lobbyManager;

    private Toggle choiceButton;

    private void Start()
    {
        choiceButton = GetComponent<Toggle>();
        lobbyManager = FindObjectOfType<LobbyManager>();
    }

    [PunRPC]
    public void SendPlayerChoice()
    {
        ToggleButton();
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
        photonView.RPC("SendPlayerChoice", RpcTarget.OthersBuffered);

    }

    private void ToggleButton()
    {
        choiceButton.interactable = !choiceButton.interactable;
    }
}
