using UnityEngine;
using UnityEngine.UIElements;

public class Coin : MonoBehaviour
{
    bool is_grounded = false;
    Vector2 pos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pos = new Vector2(transform.position.x + Random.Range(-1,1), transform.position.y + Random.Range(-1,1));
        Invoke("IsGrounded", 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (!is_grounded)
        {
            transform.Rotate(new Vector3(3f,3f,0));
            transform.position = Vector2.MoveTowards(transform.position, pos, Time.deltaTime);
        }
        
    }
    private void IsGrounded()
    {
        is_grounded = true;
        transform.rotation = Quaternion.Euler(0,0,0);
    }
}
