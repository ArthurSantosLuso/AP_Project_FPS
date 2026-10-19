using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RevolverCylinderUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RevolverShot revolver;

    [Header("UI (index 0 = Slot 1, going clockwise)")]
    [SerializeField] private Image[] slotImages = new Image[6];
    [SerializeField] private TMP_Text slot1Number;

    [Header("Options")]
    [SerializeField] private bool invertScroll = false;
    [SerializeField] private Sprite emptySprite;

    private void OnEnable()
    {
        revolver.OnCylinderChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        revolver.OnCylinderChanged -= Refresh;
    }

    private void Update()
    {
        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Approximately(scroll, 0f)) return;

        bool down = scroll < 0f;
        if (invertScroll) down = !down;

        revolver.RotateCylinder(down ? 1 : -1);
    }

    private void Refresh()
    {
        for (int i = 0; i < slotImages.Length; i++)
        {
            BulletData data = revolver.GetBulletAt(i);
            Sprite icon = data != null ? data.bulletIcon : emptySprite;

            slotImages[i].sprite = icon;
            slotImages[i].enabled = icon != null;
        }

        if (slot1Number != null)
            slot1Number.text = revolver.GetAmmoAt(0).ToString();
    }
}
