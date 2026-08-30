using TMPro;
using UnityEngine;

public class TownTooltip : MonoBehaviour
{
    public RectTransform panel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;

    public void Show(string title, string description, Vector2 mousePosition)
    {
        panel.gameObject.SetActive(true);

        titleText.text = title;
        descriptionText.text = description;

        panel.position = mousePosition + new Vector2(20f, -20f);
    }

    public void Hide()
    {
        panel.gameObject.SetActive(false);
    }
}