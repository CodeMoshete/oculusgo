using System;
using System.Collections.Generic;

public class UpdateManager
{
    private HashSet<Action<float>> updateObservers;

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

    public void Update(float dt)
    {
        foreach(Action<float> observer in updateObservers)
        {
            observer.Invoke(dt);
        }
    }
}
