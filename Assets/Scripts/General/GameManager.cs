using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject player;

    public GameObject PlayerLocalInstance => player;

    #region Singleton
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
                FindFirstObjectByType<GameManager>().Init();

            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
            Init();
        else if (_instance != this)
            Destroy(gameObject);

        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Init()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Application.focusChanged += GameFocusChanged;
        GameState.SetPaused(false);
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Application.focusChanged -= GameFocusChanged;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindPlayer();
    }

    private void FindPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void GameFocusChanged(bool hasFocus)
    {
        if (!hasFocus) GameState.SetPaused(true);
    }
}
