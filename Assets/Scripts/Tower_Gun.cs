using System.Drawing;
using UnityEngine;

public class Tower_Gun : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    /*
    ±ÇÃÑ
    ±â°üÃÑ
    ¼¦°Ç
    ´ëÆ÷
    Àú°ÝÃÑ
    */
    [SerializeField] GameObject bullet;
    public int attackCount;
    public float attackSpeed;
    public float accuracy;
    float attackDelay;
    float interval;

    public GameObject head;
    public Range range;

    void Start()
    {
        interval = 1f / attackSpeed;
        attackDelay = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - attackDelay > interval && range.targets.Count > 0)
        {
            Rotate();
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
        head.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    private void Attack()
    {
        for (int i = 0; i < attackCount; i++)
        {
            Quaternion rotation = head.transform.rotation * Quaternion.Euler(0f, 0f, Random.Range(-accuracy, accuracy));

            Instantiate(bullet, transform.position, rotation);
        }
    }
}
