using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class klickaforwin : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WinTime());
    }

    private IEnumerator WinTime()
    {
        yield return new WaitForSeconds(5);
        Application.Quit();
    }
}
