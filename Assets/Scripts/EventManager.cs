public static class EventManager
{
    public delegate void KeyEventHandler();
    public static event KeyEventHandler OnKeyCollected;

    public static void KeyCollected()
    {
        if (OnKeyCollected != null)
            OnKeyCollected();
    }
}
