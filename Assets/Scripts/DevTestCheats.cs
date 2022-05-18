using UnityEngine;
using EventCallbacksSystem;

public class DevTestCheats : MonoBehaviour
{
    [SerializeField] private GameObject devCanvas;

    // Start is called before the first frame update
    void Start()
    {
        EventSystem.Instance.RegisterListener<ImmortalEvent>(DevToolOn);
    }

    // Update is called once per frame



    public void DevToolOn(ImmortalEvent immortal)
    {
        devCanvas.SetActive(!devCanvas.activeSelf); ;
    }
  
}
