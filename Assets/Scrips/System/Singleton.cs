using UnityEngine;

// Non persistent singleton
public abstract class Singleton<T> : MonoBehaviour where T : Component
{
    public static T s_instance { get; private set; }

    protected virtual void Awake()
    {
        // If instance exists, we delete this (avoid duplicates)
        if (s_instance != null && s_instance != this)
        {
            Destroy(gameObject);
            return;
        }
        s_instance = this as T;
    }
}

// Sugestion: PersistentSingleton duplica casi todo el codigo de Singleton; podria heredar de Singleton<T> y solo agregar DontDestroyOnLoad para no repetir logica.
// Persistent singleton
public abstract class PersistentSingleton<T> : MonoBehaviour where T : Component
{
    public static T s_instance { get; private set; }

    protected virtual void Awake()
    {
        // If instance exists, we delete this (avoid duplicates)
        if (s_instance != null && s_instance != this)
        {
            Destroy(gameObject);
            return;
        }
        s_instance = this as T;
        // Set this object as persistent
        DontDestroyOnLoad(gameObject);
    }
}