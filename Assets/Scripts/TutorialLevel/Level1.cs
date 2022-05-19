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

    public Timer time;
     void Start()
    {
        uiObject.SetActive(false);
        panel.SetActive(true);
        time.DisplayingTime(false);
        time.Show(false);

    }

    private void TutorialOver(){
        if(PlacedTurret() && pool.activeObjects.Count == 0 && ship.nextUpgrade>0){
            
            foreach (var c in colliders)
            {
                c.SetActive(false);
                uiObject.SetActive(true);
                panel.SetActive(false);
                time.DisplayingTime(true);
                time.Show(true);
            }
        }
    }

    private bool PlacedTurret(){
        return FindObjectOfType<Turret>() != null;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > delayBetweenChecks){
            TutorialOver();
            timer = 0f;
        }
    }
}
