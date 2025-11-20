using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    float speed = 20f;

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
            collision.collider.GetComponent<Enemy>().health -= 3;
            Destroy(gameObject);  // 자기 자신 삭제
        }
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }
}
