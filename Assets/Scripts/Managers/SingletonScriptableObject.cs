using UnityEngine;

public abstract class SingletonScriptableObject<T> : ScriptableObject where T : SingletonScriptableObject<T>
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if(instance == null)
            {
                Resources.LoadAll<T>("ScriptableObjects");
                T[] results = Resources.FindObjectsOfTypeAll<T>();
                if(results.Length == 0)
                {
                    Debug.LogError("No objects of " + typeof(T).ToString() + " found.");
                    return null;
                }
                if(results.Length > 1)
                {
                    Debug.LogError("There are more than 1 object for " + typeof(T).ToString() + " found.");
                    return null;
                }
                instance = results[0];
            }
            return instance;
        }
    }
}
