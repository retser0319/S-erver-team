using UnityEngine;

public class Boss_1 : MonoBehaviour
{
    [SerializeField] private GameObject hp_green;
    float maxHP;
    Enemy stats;

    private void Start()
    {
        stats = GetComponent<Enemy>();
        maxHP = stats.health;
    }
    private void Update()
    {
        hp_green.transform.localScale = new Vector2(1.5f * (stats.health / maxHP), 0.1f);
        hp_green.transform.position = new Vector2(-0.75f * (stats.health / maxHP), 0.5f);
    }
}
