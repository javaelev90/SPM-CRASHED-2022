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
    

    private void TutorialOver(){
        if(pool.activeObjects.Count == 0 && turretPlaced == true && ship.nextUpgrade>0){
            
            foreach (var c in colliders)
            {
                c.SetActive(false);
            }
        }
    }

    private void PlacedTurret(){
        turretPlaced = FindObjectOfType<Turret>() != null;
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
