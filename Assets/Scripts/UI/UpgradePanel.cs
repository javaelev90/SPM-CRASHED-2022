using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePanel : MonoBehaviour
{
    [SerializeField] private GameObject Panel;
    [SerializeField] private GameObject ErrorMsg;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    // Test Upgrade
    public void UpgradeTest()
    {
        Debug.Log("Level up!");
        Panel.SetActive(false);
    }

    // When hitting X
    public void ClosePanel()
    {
        ErrorMsg?.SetActive(false);
        Panel.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
    }

    public void ToggleErrorMessage(bool active)
    {
        ErrorMsg?.SetActive(active);
    }
}
