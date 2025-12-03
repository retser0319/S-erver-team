using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour
{
    public RectTransform rect;
    private bool invoke_on = false;

    void Update()
    {
        var p = rect.anchoredPosition;

        if (p.y > -290) //특정 위치에서 원점으로 이동
        {
            p.y -= 2;
            rect.anchoredPosition = p;
        }
        else if (!invoke_on)
        {
            rect.anchoredPosition = new Vector2(0, -290);
        }
    }

}
