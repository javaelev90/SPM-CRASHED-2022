using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventCallbacksSystem;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject lobbyMenu;

    // Start is called before the first frame update
    void Start()
    {
        EventSystem.Instance.RegisterListener<StartLobbyEvent>(EnterLobby);
        EventSystem.Instance.RegisterListener<LeaveLobbyEvent>(LeaveLobby);
    }

    public void EnterLobby(StartLobbyEvent lobbyEvent)
    {
        if (lobbyEvent.IsNameLongEnough)
        {
            mainMenu.SetActive(false);
            lobbyMenu.SetActive(true);
        } 
        else
        {
            Debug.Log("Server name is too short.");
        }
    }

    public void LeaveLobby(LeaveLobbyEvent lobbyEvent)
    {
        mainMenu.SetActive(true);
        lobbyMenu.SetActive(false);
    }
}
