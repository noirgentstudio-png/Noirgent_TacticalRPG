using System.Collections;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TextMeshProUGUI messageText;

    public GameObject townPanel;
    public TextMeshProUGUI townNameText;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowMessage(string message)
    {
        messageText.text = message;

        StopAllCoroutines();
        StartCoroutine(ClearMessage());
    }

    private IEnumerator ClearMessage()
    {
        yield return new WaitForSeconds(3f);

        messageText.text = "";
    }

    public void ShowTownPanel(string townName)
    {
        townPanel.SetActive(true);
        townNameText.text = townName;
    }

    public void HideTownPanel()
    {
        townPanel.SetActive(false);
    }
}