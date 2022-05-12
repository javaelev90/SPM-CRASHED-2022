using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeControlls : MonoBehaviour
{
    public static int ControlType = 1;
    public static string SetControlType_GetDesc(int type)
    {
        ControlType = type;
        switch (ControlType)
        {
            case 1:
                return "RegularControlls";
            case 2:
                return "TurretControlls";
            default:
                return ControlType.ToString();
        }
    }
}
