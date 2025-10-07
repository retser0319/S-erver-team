using UnityEngine;

public class Player_Control : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] Transform head;
    private Rigidbody rb;

    // 임시
    public float speed = 10;
    public float rotationSpeed = 100f;

    public float jumpForce = 5f;
    public bool jump = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump = true;
        }
    }
    void FixedUpdate()
    {
        if (jump)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jump = false;
        }

        float v = Input.GetAxis("Vertical");   // W/S, ↑/↓
        float h = Input.GetAxis("Horizontal"); // A/D, ←/→

        Move(v);
        Turn(h);

        HeadRot();
        CameraPosition();
    }
    private void CameraPosition()
    {
        cam.transform.position = new Vector3(transform.position.x, transform.position.y + 16, transform.position.z - 8);
    }
    private void Move(float v)
    {
        Vector3 move = transform.forward * v;
        rb.MovePosition(rb.position + move * speed * Time.fixedDeltaTime);
    }
    private void Turn(float h)
    {
        Quaternion rot = Quaternion.Euler(0, h * rotationSpeed * Time.fixedDeltaTime, 0);
        rb.MoveRotation(rb.rotation * rot);
    }
    private void HeadRot()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        // y=height 평면 정의
        float height = transform.position.y;
        Plane plane = new Plane(Vector3.up, new Vector3(0, height, 0));

        float distance;
        if (plane.Raycast(ray, out distance))
        {
            Vector3 lookPos = ray.GetPoint(distance);

            lookPos.y = head.position.y; // 높이 차이 제거
            head.LookAt(lookPos);
        }
    }
}
