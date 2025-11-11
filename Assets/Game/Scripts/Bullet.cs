using UnityEngine;

public class Bullet : MonoBehaviour
{
    GameObject caster;
    float damage;
    float speed;
    float size;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    public void Setting(GameObject caster, float damage, float speed, float size)
    {
        this.caster = caster;
        this.damage = damage;
        this.speed = speed;
        this.size = size;
    }
}
