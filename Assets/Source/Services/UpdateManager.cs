﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class UpdateManager : MonoBehaviour
{
    private HashSet<Action<float>> updateObservers;

    private static UpdateManager instance;
    public static UpdateManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject EngineObj = GameObject.Find("Engine");
                if (EngineObj == null)
                {
                    EngineObj = GameObject.Instantiate(new GameObject("Engine"));
                }
                instance = EngineObj.AddComponent<UpdateManager>();
            }

            return instance;
        }
    }

    public UpdateManager()
    {
        updateObservers = new HashSet<Action<float>>();
    }

    public void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void AddObserver(Action<float> observer)
    {
        updateObservers.Add(observer);
    }

    public void RemoveObserver(Action<float> observer)
    {
        if (updateObservers.Contains(observer))
        {
            updateObservers.Remove(observer);
        }
    }

    public void Update()
    {
        float dt = Time.deltaTime;
        foreach(Action<float> observer in updateObservers)
        {
            observer.Invoke(dt);
        }
    }
}
