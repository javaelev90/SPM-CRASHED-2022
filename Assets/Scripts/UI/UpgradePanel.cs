using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePanel : MonoBehaviour
{
    [SerializeField] private GameObject Panel;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    // Update is called once per frame
    void Update()
    {
        
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
        Panel.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
    }
}
