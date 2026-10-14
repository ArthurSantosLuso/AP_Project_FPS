using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ScreenFeedback : MonoBehaviour
{
    public static ScreenFeedback Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Image _flashImage;
    [SerializeField] private Volume _impactVolume;
    [SerializeField] private Volume _dashVolume;
    [Space]
    [Header("Fade")]
    [SerializeField] private float _flashFadeSpeed = 12f;
    [SerializeField] private float _impactFadeSpeed = 6f;
    [SerializeField] private float _dashFadeSpeed = 10f;

    private bool _isDashing;

    private void Awake()
    {
        Instance = this;
    }

    public void Flash(Color color, float intensity)
    {
        color.a = intensity;
        _flashImage.color = color;
        _impactVolume.weight = 1f;
    }

    public void SetDashing(bool isDashing)
    {
        _isDashing = isDashing;
    }

    private void Update()
    {
        float deltaTime = Time.unscaledDeltaTime;
        Color flashColor = _flashImage.color;
        flashColor.a = Mathf.MoveTowards(flashColor.a, 0f, _flashFadeSpeed * deltaTime * Mathf.Max(flashColor.a, 0.1f));
        _flashImage.color = flashColor;

        _impactVolume.weight = Mathf.MoveTowards(_impactVolume.weight, 0f, _impactFadeSpeed * deltaTime);
        _dashVolume.weight = Mathf.MoveTowards(_dashVolume.weight, _isDashing ? 1f : 0f, _dashFadeSpeed * deltaTime);
    }
}
