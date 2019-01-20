using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMonobehaviour<S> : MonoBehaviour where S : SingletonMonobehaviour<S>, new() {

    private static bool IsInstanceCreated = false;
    private static S instance;

    protected SingletonMonobehaviour()
    {
        if (IsInstanceCreated)
        {
            throw new InvalidOperationException("Constructing a " + typeof(S).Name +
                " manually is not allowed, use the Instance property.");
        }
    }

    public static S Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject(typeof(S).Name).AddComponent<S>();
                IsInstanceCreated = true;
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = (S)this;
            if (Persistent) DontDestroyOnLoad(gameObject);
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }
    }

    protected virtual bool Persistent
    {
        get { return true; }
    }
}
