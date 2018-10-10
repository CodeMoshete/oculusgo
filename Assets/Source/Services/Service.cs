public static class Service
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
}
