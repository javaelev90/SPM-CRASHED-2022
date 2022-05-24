using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonInteractions : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject mainPanel;
    public void doExitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit");
    }

    public void doOpenSettings()
    {
        settingsPanel.SetActive(true);
        mainPanel.SetActive(false);
    }

    public void doOpenMain()
    {
        settingsPanel.SetActive(false);
        mainPanel.SetActive(true);
    }
}
