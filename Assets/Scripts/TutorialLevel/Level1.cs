using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1 : MonoBehaviour
{

    [SerializeField] GameObject[] colliders;
    public bool turretPlaced;

    public PhotonObjectPool pool;
    private float timer = 0f;
    private float delayBetweenChecks = 0.5f;

    public Ship ship;

    public GameObject uiObject;
    
    public GameObject panel;

    public GameObject found;

    public Timer time;

    private bool tutorialIsDone = false;
    
    void Start()
    {
        uiObject.SetActive(false);
        panel.SetActive(true);
        found.SetActive(false);
       
        //time.DisplayingTime(false);
        //time.Show(false);

    }

    public void TutorialOver(){
        if((ship.nextUpgrade>0))
        {
            DisableColliders();
            uiObject.SetActive(true);
            panel.SetActive(false);
            time.DisplayingTime(true);
            time.Show(true);
            found.SetActive(true);
        }
    }

    private void DisableColliders()
    {
        foreach (GameObject c in colliders)
        {
            c.SetActive(false);
        }
    }

    private bool PlacedTurret(){
        return FindObjectOfType<Turret>() != null;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > delayBetweenChecks)
        {
            TutorialOver();
            timer = 0f;
        }
    }
}
