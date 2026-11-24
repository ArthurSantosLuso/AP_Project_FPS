using TMPro;
using UnityEngine;

public class UIHandler : MonoBehaviour
{
    [Header("Interaction Hint Panel")]
    [SerializeField] private GameObject interactionHintPanel;
    [SerializeField] private TextMeshProUGUI interactionName;

    #region Singleton
    private static UIHandler _instance;

    public static UIHandler Instance
    {
        get
        {
            if (_instance == null)
                FindFirstObjectByType<UIHandler>().Init();

            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
            Init();
        else if (_instance != this)
            Destroy(gameObject);
    }

    private void Init()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    public void ShowInteractionHint(string interactionName)
    {
        this.interactionName.text = interactionName;
        interactionHintPanel.SetActive(true);
    }
    
    public void HideInteractionHint()
    {
        interactionHintPanel.SetActive(false);
        interactionName.text = "";
    }
}
