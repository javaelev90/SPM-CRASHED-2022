using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventCallbacksSystem;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject lobbyMenu;
    private AudioSource source;

 
    // Start is called before the first frame update
    void Start()
    {
        EventSystem.Instance.RegisterListener<EnterLobbyEvent>(EnterLobby);
        EventSystem.Instance.RegisterListener<JoinedLobbyEvent>(JoinedLobby);
        EventSystem.Instance.RegisterListener<LeaveLobbyEvent>(LeaveLobby);
        source = GetComponent<AudioSource>();
        source.Play();
    }

    public void EnterLobby(EnterLobbyEvent lobbyEvent)
    {
        if (lobbyEvent.IsNameLongEnough == false)
        {
            Debug.Log("Server name is too short.");
        }
        else
        {
            Debug.Log("Trying to enter lobby.");
        }
    }

    public void JoinedLobby(JoinedLobbyEvent lobbyEvent)
    {
        mainMenu.SetActive(false);
        lobbyMenu.SetActive(true);
    }

    public void LeaveLobby(LeaveLobbyEvent lobbyEvent)
    {
        mainMenu.SetActive(true);
        lobbyMenu.SetActive(false);
    }
}
