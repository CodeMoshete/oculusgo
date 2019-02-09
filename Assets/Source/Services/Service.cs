﻿public static class Service
{
    private static EventManager eventManager;
    public static EventManager EventManager
    {
        get
        {
            if (eventManager == null)
            {
                eventManager = new EventManager();
            }

            return eventManager;
        }
    }

    private static UpdateManager updateManager;
    public static UpdateManager UpdateManager
    {
        get
        {
            return updateManager;
        }
        set
        {
            if (updateManager == null)
            {
                updateManager = value;
            }
        }
    }

    private static TimerManager timerMananager;
    public static TimerManager TimerManager
    {
        get
        {
            if (timerMananager == null)
            {
                timerMananager = new TimerManager();
            }

            return timerMananager;
        }
    }

    // Manually set services
    public static PlayerData PlayerData
    {
        get
        {
            return PlayerData.Instance;
        }
    }

    public static ControlsManager Controls
    {
        get
        {
            return ControlsManager.Instance;
        }
    }

}
