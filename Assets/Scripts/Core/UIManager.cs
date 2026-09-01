using System.Collections;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TextMeshProUGUI messageText;

    public GameObject townPanel;
    public TextMeshProUGUI townNameText;

    private Coroutine clearCoroutine;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (messageText != null && !string.IsNullOrEmpty(messageText.text))
        {
            ShowMessage(messageText.text, 10f);
        }
    }

    public void ShowMessage(string message, float duration = 10f)
    {
        if (messageText == null)
            return;

        messageText.text = message;
        messageText.gameObject.SetActive(true);

        if (clearCoroutine != null)
        {
            StopCoroutine(clearCoroutine);
        }

        clearCoroutine = StartCoroutine(ClearMessageRoutine(duration));
    }

    private IEnumerator ClearMessageRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (messageText != null)
        {
            messageText.text = "";
        }
    }

    public void ShowTownPanel(string townName)
    {
        if (townPanel != null)
        {
            townPanel.SetActive(true);
        }

        if (townNameText != null)
        {
            townNameText.text = townName;
        }
    }

    public void HideTownPanel()
    {
        if (townPanel != null)
        {
            townPanel.SetActive(false);
        }
    }
}