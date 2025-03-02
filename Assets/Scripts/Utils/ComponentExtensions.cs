using UnityEngine;

public static class ComponentExtensions
{
    public static T GetInterface<T>(this Component component) where T : class
    {
        return component.GetComponent<MonoBehaviour>() as T;
    }
}
