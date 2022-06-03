using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ShipUpgradePanel : MonoBehaviour
{
    [SerializeField] private GameObject Panel;
    [SerializeField] private GameObject ErrorMsg;
    [SerializeField] private GameObject CostInfo;

    // Start is called before the first frame update

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        GameManager.player.GetComponent<PlayerInput>().enabled = true;
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
        ErrorMsg.SetActive(false);
        gameObject.SetActive(false);
        GameManager.player.GetComponent<PlayerInput>().enabled = true;
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
