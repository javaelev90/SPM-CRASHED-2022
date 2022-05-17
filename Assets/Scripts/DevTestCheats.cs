using UnityEngine;
using EventCallbacksSystem;

public class DevTestCheats : MonoBehaviour
{
    [SerializeField] private GameObject devCanvas;

    // Start is called before the first frame update
    void Start()
    {
        EventSystem.Instance.RegisterListener<DevToolEvent>(DevToolOn);
    }

    // Update is called once per frame



    public void DevToolOn(DevToolEvent on)
    {
        devCanvas.SetActive(on.On);
    }
    public void BecomeImmortal(bool immortal)
    {
        EventSystem.Instance.FireEvent(new ImmortalEvent(immortal));
    }
}
