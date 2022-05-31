using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class ButtonInteractions : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject mainPanel;

    [SerializeField] private GameObject popUpExit;

    [SerializeField] private GameObject popUpPause;

    [SerializeField] private GameObject popUpSave;

    [SerializeField] private GameObject popUpSaveInfo;

    [SerializeField] private GameObject popUpResume;

    [SerializeField] private GameObject popUpSound;

    [SerializeField] private GameObject popUpImage;

    [SerializeField] private Image controls;

    private AudioSource source;

    public AudioClip clip;
    private bool isSaved;

    public ShowPauseMenu pause;

      private void Start()
    {
        source = GetComponent<AudioSource>();   
    }

    private void OnEnable() {
        Cursor.lockState = CursorLockMode.None;
    }

    private void OnDisable() {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Show(InputAction.CallbackContext ctx){
        if(ctx.performed){
            Debug.Log("bajs");
            pause.gameObject.SetActive(!pause.gameObject.activeSelf);
            Cursor.lockState = CursorLockMode.None;
        }
        Debug.Log("korv");
    }

    public void doExitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit");
    }

    public void doOpenSettings()
    { 
        popUpImage.SetActive(false);
        settingsPanel.SetActive(true);
        mainPanel.SetActive(false);
        source.PlayOneShot(clip);
        
    }

    public void doOpenMain()
    {
        settingsPanel.SetActive(false);
        mainPanel.SetActive(true);
        popUpSound.SetActive(false);
    }

     public void MainMenuOpen()
    {
        SceneManager.LoadScene("GameLobbyScene");
    }

     public void CloseExitWindow()
    {
        popUpExit.SetActive(false);
        pause.gameObject.SetActive(true);
    }
    public void OpenExitWindow()
    {
        popUpExit.SetActive(true);
        pause.gameObject.SetActive(false);
        Debug.Log("quit");
    
    }

     public void OpenResumeWindow()
    {
        popUpResume.SetActive(true);
        pause.gameObject.SetActive(false);
    }
    public void CloseResumeWindow()
    {
        popUpResume.SetActive(false);
        pause.gameObject.SetActive(true);

    }

      public void Resume()
    {
       popUpResume.SetActive(false);
       pause.gameObject.SetActive(false);
       Cursor.lockState = CursorLockMode.Locked;

    }

    public void OpenPauseWindow()
    {
      popUpPause.SetActive(true);
      pause.gameObject.SetActive(false);
    }

     public void ClosePauseWindow()
    {
      popUpPause.SetActive(false);
      pause.gameObject.SetActive(true);
      //Cursor.lockState = CursorLockMode.Locked;
    }

     public void Pause()
    {
      popUpPause.SetActive(false);
      //länka till pausgrej som kommer
      Debug.Log("Väntar på att göras klart");
    }

    public void OpenSaveWindow()
    {
      popUpSave.SetActive(true);
      pause.gameObject.SetActive(false);
    }

     public void CloseSaveWindow()
    {
      popUpSave.SetActive(false);
      pause.gameObject.SetActive(true);
      isSaved = false;
    }

     public void Save()
    {
       popUpSave.SetActive(false);
      isSaved = true;
      //länka till spargrej som kommer
      Debug.Log("Väntar på att göras klart");
    }

     public void SaveInfo()
    {
      if(isSaved == true){
      popUpSaveInfo.SetActive(true);
      Destroy(popUpSaveInfo, 3f);
     
    }
    }
    public void OpenSoundWindow()
    {
      popUpSound.SetActive(true);
      settingsPanel.SetActive(false);
    }

     public void CloseSoundWindow()
    {
      popUpSound.SetActive(false);
    }
      public void ViewImage()
    {
      popUpImage.SetActive(true);
      settingsPanel.SetActive(false);
    }

     public void CloseImage()
    {
      popUpImage.SetActive(false);
      controls.gameObject.SetActive(false);
    }
    
}
