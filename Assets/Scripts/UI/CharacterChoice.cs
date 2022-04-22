using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class CharacterChoice : MonoBehaviourPunCallbacks
{
    [SerializeField] Character character;
    private Button choiceButton;

    private void Start()
    {
        choiceButton = GetComponent<Button>();
    }

    [PunRPC]
    public void SendPlayerChoice()
    {
        ToggleButton();
    }

    public void ChoosePlayer()
    {
        //if (GlobalSettings.GameSettings.CharacterChoice == character)
        //{
        photonView.RPC("SendPlayerChoice", RpcTarget.Others);
        //GlobalSettings.GameSettings.CharacterChoice = character;
        //Debug.Log(GlobalSettings.GameSettings.CharacterChoice);
        
        //}
    }

    private void ToggleButton()
    {
        choiceButton.enabled = !choiceButton.enabled;
    }
}
