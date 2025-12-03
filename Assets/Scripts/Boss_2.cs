using UnityEngine;

public class Boss_2 : MonoBehaviour
{
    [SerializeField] private GameObject beee;
    [SerializeField] private GameObject hp_green;
    float maxHP;
    Enemy stats;

    private void Start()
    {
        stats = GetComponent<Enemy>();
        maxHP = stats.health;
        InvokeRepeating("CreateBee", 5, 4);
    }
    private void Update()
    {
        hp_green.transform.localScale = new Vector2(1.5f * (stats.health / maxHP), 0.1f);
        hp_green.transform.position = new Vector2(-0.75f * (stats.health / maxHP), 1.2f);
    }
    private void CreateBee()
    {
        Instantiate(beee, (Vector2)transform.position + new Vector2(2, 2), Quaternion.identity);
        Instantiate(beee, (Vector2)transform.position + new Vector2(2, -2), Quaternion.identity);
    }
}
