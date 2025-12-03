using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public float speed;

    private void Awake()
    {
        Invoke("Destroy", 3f);
    }

    void Update()
    {
        transform.position += transform.right * speed * Time.deltaTime;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            var e = collision.GetComponent<Enemy>();
            float d = damage - e.defense;
            if (d < 0) d = 0;
            e.health -= d;
            Destroy(gameObject);  // 자기 자신 삭제
        }
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }
}
