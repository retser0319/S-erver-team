using UnityEngine;

public class Boss_2 : MonoBehaviour
{
    [SerializeField] private GameObject beee;

    private void Start()
    {
        InvokeRepeating("CreateBee", 5, 3);
    }

    private void CreateBee()
    {
        Instantiate(beee, (Vector2)transform.position + new Vector2(2, 2), Quaternion.identity);
        Instantiate(beee, (Vector2)transform.position + new Vector2(2, -2), Quaternion.identity);
    }
}
