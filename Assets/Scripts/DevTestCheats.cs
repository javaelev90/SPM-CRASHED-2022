using UnityEngine;
using EventCallbacksSystem;

public class DevTestCheats : MonoBehaviour
{
    
    private bool cheatMenyActive;
    [SerializeField] private GameObject cheatCanvas;

    // Start is called before the first frame update
    void Start()
    {
        cheatMenyActive = false;
    }

    // Update is called once per frame
   

    public void BecomeImmortal(bool immortal)
    {
        EventSystem.Instance.FireEvent(new ImmortalEvent(immortal));
    }
}
