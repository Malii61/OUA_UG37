public static class CanvasManager
{
    private static bool isOpen = false;
    public static bool IsOpen()
    {
        return isOpen;
    }
    public static void SetActivation( bool _isOpen)
    {
        isOpen = _isOpen;
    }

}
