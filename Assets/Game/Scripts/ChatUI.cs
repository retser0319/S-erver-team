using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatUI : MonoBehaviour
{
    [Header("Refs")]
    public TMP_InputField input;
    public TextMeshProUGUI log;
    public ScrollRect scroll;

    [Header("Options")]
    public int maxLines = 200;

    private readonly StringBuilder sb = new StringBuilder();

    void Awake()
    {
        if (log != null) sb.Append(log.text);
    }

    IEnumerator Start()
    {
        yield return null;
        yield return new WaitForEndOfFrame();

        if (scroll != null)
        {
            Canvas.ForceUpdateCanvases();
            scroll.verticalNormalizedPosition = 1f;
        }
    }

    public void Append(string line)
    {
        if (string.IsNullOrEmpty(line) || log == null) return;

        sb.AppendLine(line);

        var text = sb.ToString();
        var lines = text.Split('\n');
        if (lines.Length > maxLines)
        {
            int start = lines.Length - maxLines;
            text = string.Join("\n", lines, start, maxLines);
            sb.Clear().Append(text);
        }

        bool wasAtBottom = false;
        if (scroll != null)
        {
            wasAtBottom = scroll.verticalNormalizedPosition <= 0.001f;
        }

        log.text = sb.ToString();

        if (scroll != null)
        {
            Canvas.ForceUpdateCanvases();
            if (wasAtBottom)
                scroll.verticalNormalizedPosition = 0f;
        }
    }
    public string ConsumeInput()
    {
        string msg = input != null ? input.text : string.Empty;
        if (input != null) input.text = string.Empty;
        return msg;
    }
}