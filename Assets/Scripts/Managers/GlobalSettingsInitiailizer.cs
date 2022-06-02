using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSettingsInitiailizer : MonoBehaviour
{
    GlobalSettings globalSettings;
    private void Awake()
    {
        globalSettings = GlobalSettings.Instance;
        //DontDestroyOnLoad(this);
    }
}
