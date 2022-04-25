using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;


public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField lobbyNameInput;
    [SerializeField] GameObject playerList;
    [SerializeField] GameObject lobbyPlayerPrefab;
    [SerializeField] List<CharacterChoice> choices;

    private ArrayList currentRoomList = new ArrayList();
    public Character PlayerChoice { get; set; }
    public bool IsMaster { get { return PhotonNetwork.IsMasterClient; } }
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
        if (PhotonNetwork.LocalPlayer.NickName == "")
        {
            PhotonNetwork.LocalPlayer.NickName = "Player";
        }
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public void JoinRoom()
    {
        if (lobbyNameInput.text.Length > 1)
        {
            PhotonNetwork.JoinRoom(lobbyNameInput.text);
        }
    }

    public void SetPlayerReady(bool ready, string userId)
    {
        foreach (GameObject player in currentRoomList)
        {
            LobbyPlayerMenuHandler playerInfo = player.GetComponent<LobbyPlayerMenuHandler>();
            if (playerInfo.LobbyPlayerId == userId)
            {
                playerInfo.photonView.RPC("UpdateReadyCheck", RpcTarget.AllBuffered, ready);
            }
        }
    }

    public void CreateRoom()
    {
        if (lobbyNameInput.text.Length > 1)
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 2;
            roomOptions.PlayerTtl = 50;
            roomOptions.PublishUserId = true;

            PhotonNetwork.CreateRoom(lobbyNameInput.text, roomOptions, null, null);
        }
    }
      
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a room successfully!");
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
        
    }

    public override void OnLeftRoom()
    {
        foreach (CharacterChoice choice in choices)
        {
            choice.choiceButton.interactable = true;
            PlayerChoice = Character.NONE;
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
        GameObject playerInfo = PhotonNetwork.Instantiate("Prefabs/UI/MenuLobby/" + lobbyPlayerPrefab.name, playerList.transform.position, Quaternion.identity);
        playerInfo.transform.SetParent(playerList.transform, false);
        playerInfo.GetComponent<LobbyPlayerMenuHandler>().SetPlayerName(newPlayer.NickName);
        playerInfo.GetComponent<LobbyPlayerMenuHandler>().LobbyPlayerId = newPlayer.UserId;
        currentRoomList.Add(playerInfo);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemovePlayer(otherPlayer);
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
        if (IsMaster && AreAllPlayersReady())
        {
            Debug.Log("Everyone is ready, starting game.");
            PhotonNetwork.LoadLevel(GlobalSettings.GameSettings.GameSceneName);
        }
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

}
