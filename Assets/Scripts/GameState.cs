using UnityEngine;

public static class GameState
{
    public static bool IsPaused { get; private set; }

    public static event System.Action<bool> OnPauseChanged;
    
    public static void SetPaused(bool paused)
    {
        if (IsPaused == paused) return;
        IsPaused = paused;
        Time.timeScale = paused ? 0f : 1f;
        AudioListener.pause = paused;
        OnPauseChanged?.Invoke(paused);
    }
}
