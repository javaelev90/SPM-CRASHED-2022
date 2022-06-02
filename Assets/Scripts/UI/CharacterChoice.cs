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

      private AudioSource source;

    public AudioClip clip;

    public Toggle choiceButton;

    private void Awake()
    {
        choiceButton = GetComponent<Toggle>();
        lobbyManager = FindObjectOfType<LobbyManager>();
        source = GetComponent<AudioSource>();
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
             source.PlayOneShot(clip);
        } 
        else if (lobbyManager.PlayerChoice == character)
        {
            lobbyManager.PlayerChoice = Character.NONE;
        }

        PlayerPrefs.SetInt(GlobalSettings.GameSettings.CharacterChoicePropertyName, (int)lobbyManager.PlayerChoice);
        photonView.RPC(nameof(SendPlayerChoice), RpcTarget.OthersBuffered, lobbyManager.PlayerChoice != Character.NONE, PhotonNetwork.LocalPlayer.UserId);
        lobbyManager.SetPlayerReady(lobbyManager.PlayerChoice != Character.NONE, PhotonNetwork.LocalPlayer.UserId);
    }

    private void ToggleButton()
    {
        choiceButton.interactable = !choiceButton.interactable;
    }
}
