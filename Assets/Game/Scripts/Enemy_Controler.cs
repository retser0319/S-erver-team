using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Nav nav;

    private void Awake()
    {
        nav = GetComponent<Nav>();
    }

    private void Update()
    {
        // 마우스 왼쪽 버튼을 눌렀을 때
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                nav.MoveTo(hit.point);
            }
        }
    }
}