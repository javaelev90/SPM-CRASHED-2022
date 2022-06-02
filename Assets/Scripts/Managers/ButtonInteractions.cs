using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using EventCallbacksSystem;
using Photon.Pun;

public class ButtonInteractions : MonoBehaviourPunCallbacks
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
    [SerializeField] private GameObject pausedText;



    private bool isSaved;

    public ShowPauseMenu pause;
    private GameStateManager gameStateManager;

      private void Start()
    {
        source = GetComponent<AudioSource>();   
    }

    private void OnEnable() {
        if (pause == null)
        {
            Cursor.lockState = CursorLockMode.None;
        }

        gameStateManager = FindObjectOfType<GameStateManager>();
    }

    private void OnDisable() {
        if (pause == null)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }


    private void Update()
    {
        if (pause != null)
        {
            if (GameManager.gameIsPaused)
            {
                //Game is paused and menu is not showing
                pausedText.SetActive(MenuIsOpen() == false);
            }
            else if (GameManager.gameIsPaused == false && pausedText.activeSelf == true)
            {
                //Game is not paused
                pausedText.SetActive(false);
            }

            //if (MenuIsOpen() == false)
            //{
            //    Cursor.lockState = CursorLockMode.Locked;
            //}
        }
        
    }

    public void Show(InputAction.CallbackContext ctx){
        if(ctx.performed){
            pause.gameObject.SetActive(!pause.gameObject.activeSelf);

            if (pause.gameObject.activeSelf)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            EventSystem.Instance.FireEvent(new LockControlsEvent(pause.gameObject.activeSelf));
        }
    }

    private bool MenuIsOpen()
    {
        return settingsPanel.activeSelf || mainPanel.activeSelf || popUpExit.activeSelf || 
            popUpPause.activeSelf || popUpSave.activeSelf || pause.gameObject.activeSelf ||
            popUpResume.activeSelf || popUpSound.activeSelf || popUpImage.activeSelf;
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
        PhotonNetwork.LoadLevel("GameLobbyScene");
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
        EventSystem.Instance.FireEvent(new LockControlsEvent(pause.gameObject.activeSelf));
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
        GameManager.Instance.PauseGame(!GameManager.gameIsPaused);
        EventSystem.Instance.FireEvent(new LockControlsEvent(pause.gameObject.activeSelf));
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
        EventSystem.Instance.FireEvent(new LockControlsEvent(pause.gameObject.activeSelf));
        gameStateManager.SaveGame();
    }

    public void SaveInfo()
    {
      if(isSaved == true){
            //popUpSaveInfo.SetActive(true);
            StartCoroutine(SaveInfoTime());
        }
    }

    IEnumerator SaveInfoTime(){

      popUpSaveInfo.SetActive(true);
      yield return new WaitForSeconds(2);
      popUpSaveInfo.SetActive(false);
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
