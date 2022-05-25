using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPersistable
{
    public void SaveState();
    public void LoadState();
}
