using UnityEngine;

public static class GameState
{
    public static bool IsPaused { get; private set; }
    public static event System.Action<bool> OnPauseChanged;

    public static void SetPaused(bool paused) => Apply(paused, force: false);

    // Use for initialization: Applies side effects even if the value is unchanged
    public static void Initialize(bool paused) => Apply(paused, force: true);

    private static void Apply(bool paused, bool force)
    {
        if (!force && IsPaused == paused) return;
        IsPaused = paused;
        Time.timeScale = paused ? 0f : 1f;
        AudioListener.pause = paused;

        Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Confined;
        Cursor.visible = paused;

        OnPauseChanged?.Invoke(paused);
    }
}