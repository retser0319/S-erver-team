using System.Drawing;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Tower_Gun : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    /*
    ±ÇÃÑ
    ±â°üÃÑ
    ¼¦°Ç
    ´ëÆ÷

    */
    [SerializeField] GameObject bullet;
    public int attackDamage;
    public int attackCount;
    public float attackSpeed;
    float attackDelay;
    float interval;

    Range range;

    void Start()
    {
        interval = 1f / attackSpeed;
        attackDelay = Time.time;

        range = transform.Find("Range").GetComponent<Range>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - attackDelay > interval && range.targets.Count > 0)
        {
            Attack();
            attackDelay = Time.time;
        }
    }
    private void FixedUpdate()
    {
        if (range.targets.Count > 0)
            Rotate();

        
    }
    private void Rotate()
    {
        Vector2 direction = (Vector2)range.targets[0].transform.position - (Vector2)transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    private void Attack()
    {
        Instantiate(bullet, transform.position, transform.rotation);
    }
}
