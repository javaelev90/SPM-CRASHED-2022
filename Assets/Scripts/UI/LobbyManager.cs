using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System;
using EventCallbacksSystem;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField lobbyNameInput;
    [SerializeField] GameObject playerList;
    [SerializeField] GameObject lobbyPlayerPrefab;
    [SerializeField] List<CharacterChoice> choices;
    [Header("Master Client UI elements")]
    [SerializeField] GameObject startGameButton;
    [SerializeField] Toggle loadSaveFile;

    private AudioSource source;

    public AudioClip clip;
   

    private ArrayList currentRoomList = new ArrayList();
    public Character PlayerChoice { get; set; }
    public bool IsMaster { get { return PhotonNetwork.IsMasterClient; } }
    private bool isStarted = false;
    private bool connectedToMaster = false;
    // Joined server
    // Click character
    // Send RPC, choose character
    //     receiving - disable character
    //     sending   - highlight selection


    void Start()
    {
        PhotonNetwork.GameVersion = "0.1";
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
        source = GetComponent<AudioSource>();
        if (PhotonNetwork.LocalPlayer.NickName == "")
        {
            PhotonNetwork.LocalPlayer.NickName = "Player"+GetInstanceID();
        }
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master successfully!");
        connectedToMaster = true;
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public void JoinOrCreateRoom()
    {
        if (connectedToMaster == false)
            return;

        bool inputIsLongEnough = false;
        if (lobbyNameInput.text.Trim().Length > 2)
        {
            RoomOptions roomOptions = new RoomOptions
            {
                MaxPlayers = 2,
                PlayerTtl = 50,
                PublishUserId = true
            };

            PhotonNetwork.JoinOrCreateRoom(lobbyNameInput.text, roomOptions, TypedLobby.Default, null);
            inputIsLongEnough = true;
            source.PlayOneShot(clip);
        }
        EventSystem.Instance.FireEvent(new EnterLobbyEvent(inputIsLongEnough));
    }

    public void SetPlayerReady(bool ready, string userId)
    {
        foreach (GameObject player in currentRoomList)
        {
            LobbyPlayerMenuHandler playerInfo = player.GetComponent<LobbyPlayerMenuHandler>();
            if (playerInfo.LobbyPlayerId == userId)
            {
                playerInfo.IsReady = ready;
            }
        }
    }
      
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a room successfully!");
        EventSystem.Instance.FireEvent(new JoinedLobbyEvent());
        if (IsMaster)
        {
            AddPlayer(PhotonNetwork.LocalPlayer);
        }
        else
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                AddPlayer(player);
            }
            SyncOptions();
        }
        UpdateLobbyUI();
    }

    public override void OnLeftRoom()
    {
        foreach (CharacterChoice choice in choices)
        {
            if (choice.choiceButton != null)
            {
                choice.choiceButton.interactable = true;
                PlayerChoice = Character.NONE;
            }

        }
    }

    private void UpdateLobbyUI()
    {
        if (IsMaster)
        {
            loadSaveFile.gameObject.SetActive(SaveAndLoadHelper.SaveFileExists());
            loadSaveFile.isOn = false;
            startGameButton.SetActive(true);
        }
        else
        {
            startGameButton.SetActive(false);
            loadSaveFile.gameObject.SetActive(false);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddPlayer(newPlayer);
    }

    public void SyncOptions()
    {
        if (IsMaster)
        {
            foreach (CharacterChoice choice in choices)
            {
                if (choice.character == PlayerChoice)
                {
                    choice.photonView.RPC("SyncChoice", RpcTarget.All, !choice.choiceButton.interactable);
                }
            }
        }
    }

    private void AddPlayer(Player newPlayer)
    {
        Debug.Log($"User {newPlayer.UserId} connected");
        GameObject playerInfo = PhotonNetwork.Instantiate(GlobalSettings.UIPath + "MenuLobby/" + lobbyPlayerPrefab.name, playerList.transform.position, Quaternion.identity);
        playerInfo.transform.SetParent(playerList.transform, false);
        playerInfo.GetComponent<LobbyPlayerMenuHandler>().SetPlayerName(newPlayer.NickName);
        playerInfo.GetComponent<LobbyPlayerMenuHandler>().LobbyPlayerId = newPlayer.UserId;
        currentRoomList.Add(playerInfo);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemovePlayer(otherPlayer);
        UpdateLobbyUI();
    }

    private void RemovePlayer(Player otherPlayer)
    {
        GameObject playerToRemove = default;
        lock (currentRoomList)
        {
            foreach (GameObject item in currentRoomList)
            {
                if (item.GetComponent<LobbyPlayerMenuHandler>().LobbyPlayerId == otherPlayer.UserId)
                {
                    playerToRemove = item;
                    break;
                }
            }
            Destroy(playerToRemove);
            currentRoomList.Remove(playerToRemove);
        }

        RemovePlayerChoice();
    }

    private void RemovePlayerChoice()
    {
        foreach (CharacterChoice choice in choices)
        {
            if (choice.choiceButton.interactable == false)
            {
                choice.choiceButton.interactable = true;
            }
        }
    }

    private bool AreAllPlayersReady()
    {
        foreach (GameObject player in currentRoomList)
        {
            LobbyPlayerMenuHandler playerInfo = player.GetComponent<LobbyPlayerMenuHandler>();
            Debug.Log($"User {playerInfo.LobbyPlayerId} is ready {playerInfo.IsReady}");
            if (!playerInfo.IsReady)
            {
                return false;
            }
        }
        return true;
    }

    public void StartGame()
    {
        if (IsMaster && AreAllPlayersReady() && isStarted == false)
        {
            PlayerPrefs.SetInt(GlobalSettings.LoadSaveFileSettingName, loadSaveFile.isOn ? 1 : 0);

            isStarted = true;
            PhotonNetwork.LoadLevel(GlobalSettings.GameSettings.GameSceneName);
            source.PlayOneShot(clip);
           
        }
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        ResetLobbySettings();
        EventSystem.Instance.FireEvent(new LeaveLobbyEvent());
        source.PlayOneShot(clip);
        source.Stop();
    }

    private void ResetLobbySettings()
    {
        PlayerChoice = Character.NONE;
        PlayerPrefs.SetInt(GlobalSettings.GameSettings.CharacterChoicePropertyName, (int)Character.NONE);
        currentRoomList.Clear();
        UpdateLobbyUI();
    }

}
