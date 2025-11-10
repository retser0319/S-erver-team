using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private NavMove nav;

    private void Awake()
    {
        nav = GetComponent<NavMove>();
    }
}