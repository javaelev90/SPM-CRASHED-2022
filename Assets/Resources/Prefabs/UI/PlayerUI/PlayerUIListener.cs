using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIListener : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private Button cookedMeatButton;
    [SerializeField] private Button greenGooButton;
    private void OnEnable()
    {
        cookedMeatButton.onClick.AddListener(OnSelectCookedMeat);
    }

    private void OnDisable()
    {
        cookedMeatButton.onClick.RemoveAllListeners();
    }

    public void OnSelectCookedMeat()
    {
        Debug.Log("SelectedCookedMeat");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
