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
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            collision.collider.GetComponent<Enemy>().health -= damage;
            Destroy(gameObject);  // 자기 자신 삭제
        }
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }
}
