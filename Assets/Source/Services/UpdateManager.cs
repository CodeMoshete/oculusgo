using System;
using System.Collections.Generic;
using UnityEngine;

public class UpdateManager : MonoBehaviour
{
    private HashSet<Action<float>> updateObservers;

    public void Start()
    {
        Service.UpdateManager = this;
    }

    public UpdateManager()
    {
        updateObservers = new HashSet<Action<float>>();
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
