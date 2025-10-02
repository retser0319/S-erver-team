using UnityEngine;

public class Tank_Move : MonoBehaviour
{
    private Rigidbody rb;
    public float speed = 10;
    public float rotationSpeed = 100f;
    public float jumpForce = 5f;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        float v = Input.GetAxis("Vertical");   // W/S, ก่/ก้
        float h = Input.GetAxis("Horizontal"); // A/D, ก็/กๆ

        Vector3 move = transform.forward * v;
        Quaternion rotation = Quaternion.Euler(0, h * rotationSpeed * Time.fixedDeltaTime, 0);

        rb.MoveRotation(rotation);
        rb.MovePosition(rb.position + move * speed * Time.fixedDeltaTime);

    }
}
