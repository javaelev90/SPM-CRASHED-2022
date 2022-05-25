using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Reflection;
using System;
using UnityEngine.UI;

public class RecycleUtils
{
    private static List<Type> types = new List<Type>()
    {
        typeof(Color)
    };

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
            ResetValues(
                    prefabComponents[componentIndex],
                    destinationComponents[componentIndex]
            );

        }
    }

    public static void ResetValues(Component prefab, Component instantiation)
    {
        Type type = prefab.GetType();
        FieldInfo[] fields;
        if (type == typeof(Image))
        {
            fields = type.BaseType.BaseType.GetFields(
                         BindingFlags.NonPublic |
                         BindingFlags.Instance);
            ResetSpecificFields(fields, prefab, instantiation);
        }
        else
        {
            fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            ResetPublicFields(fields, prefab, instantiation);
        }
    }

    private static void ResetSpecificFields(FieldInfo[] fields, Component prefab, Component instantiation)
    {
        foreach (FieldInfo field in fields)
        {
            if (types.Contains(field.FieldType))
            {
                field.SetValue(instantiation, field.GetValue(prefab));
            }
        }
    }

    private static void ResetPublicFields(FieldInfo[] fields, Component prefab, Component instantiation)
    {
        foreach (FieldInfo field in fields)
        {
            if (field.FieldType.IsPublic && field.FieldType.IsValueType)
            {
                field.SetValue(instantiation, field.GetValue(prefab));
            }
        }
    }
}
