using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Reflection;
using System;

public class RecycleUtils
{
    public static void ResetGameObjectComponents(Transform prefab, Transform instantiation)
    {
        ResetComponents(prefab.gameObject, instantiation.gameObject);
        for (int childIndex = 0; childIndex < prefab.childCount; childIndex++)
        {
            ResetGameObjectComponents(prefab.GetChild(childIndex), instantiation.GetChild(childIndex));
        }
    }

    public static void ResetComponents(GameObject prefab, GameObject instantiation)
    {
        Component[] prefabComponents = prefab.GetComponents<Component>();
        Component[] destinationComponents = instantiation.GetComponents<Component>();
        for (int componentIndex = 0; componentIndex < prefabComponents.Length; componentIndex++)
        {
            if (prefabComponents[componentIndex] is PhotonView)
            {
                continue;
            }
            else if (prefabComponents[componentIndex] is PhotonTransformViewClassic
                || prefabComponents[componentIndex] is PhotonTransformView)
            {
                instantiation.transform.SetPositionAndRotation(prefab.transform.position, prefab.transform.rotation);
                instantiation.transform.localScale = prefab.transform.localScale;
                continue;
            }
            else if (prefabComponents[componentIndex] is ParticleSystem)
            {
                ((ParticleSystem)destinationComponents[componentIndex]).Stop();
                ((ParticleSystem)destinationComponents[componentIndex]).Clear();
            }
            ResetPublicValues(
                    prefabComponents[componentIndex],
                    destinationComponents[componentIndex]
            );

        }
    }

    public static void ResetPublicValues(Component source, Component destination)
    {
        Type type = source.GetType();
        FieldInfo[] fields = type.GetFields();
        foreach (FieldInfo field in fields)
        {
            if (field.IsPublic && field.FieldType.IsValueType)
            {
                field.SetValue(destination, field.GetValue(source));
            }
        }
    }
}
