using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePanel : MonoBehaviour
{
    [SerializeField] private GameObject Panel;
    [SerializeField] private GameObject ErrorMsg;
    [SerializeField] private GameObject CostInfo;

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
        Debug.Log(1);
        ErrorMsg.SetActive(false);
        Panel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ToggleErrorMessage(bool active)
    {
        ErrorMsg.SetActive(active);
    }

    public void SetErrorMessage(string errorMessage)
    {
        if(ErrorMsg)
            ErrorMsg.GetComponent<Text>().text = errorMessage;
    }

    public void SetCostInfo(string costInfo)
    {
        CostInfo.GetComponent<Text>().text = costInfo;
    }
}
