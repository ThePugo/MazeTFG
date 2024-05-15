public static class EventManager
{
    public delegate void KeyEventHandler();
    public static event KeyEventHandler OnKeyCollected;

    public delegate void DeathHandler();
    public static event DeathHandler OnDeath;

    public static void KeyCollected()
    {
        if (OnKeyCollected != null)
            OnKeyCollected();
    }
}
