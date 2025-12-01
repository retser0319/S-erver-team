using UnityEngine;
using UnityEngine.UIElements;

public class Coin : MonoBehaviour
{
    bool is_grounded = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        Invoke("IsGrounded", 2);
    }

    // Update is called once per frame
    void Update()
    {
        if (!is_grounded)
        {
            transform.Rotate(new Vector3(15f,15f,0));
        }
    }

    private void IsGrounded()
    {
        is_grounded = true;
    }
}
