using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<S> where S : Singleton<S>, new() {

    private static bool IsInstanceCreated = false;
    private static S instance;

    protected Singleton()
    {
        if (IsInstanceCreated)
        {
            throw new InvalidOperationException("Construction a " + typeof(S).Name + " manually is not allowed, use the Instance property");
        }
    }

    public static S Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new S();
                IsInstanceCreated = true;
            }
            return instance;
        }
    }
}
