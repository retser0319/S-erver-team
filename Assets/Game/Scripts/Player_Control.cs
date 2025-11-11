using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Player_Control : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] public Transform head;
    private Rigidbody rb;

    // 임시
    public float speed = 10;
    public float rotationSpeed = 100f;

    public float jumpForce = 10f;
    public bool jump = false;
    public float jumpDelay = 1f;

    private bool hold = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if (jumpDelay > 0) jumpDelay -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space) && !hold && !jump)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpDelay = 1f;
            jump = true;
        }
    }
    void FixedUpdate()
    {
        float v = Input.GetAxis("Vertical");   // W/S, ↑/↓
        float h = Input.GetAxis("Horizontal"); // A/D, ←/→

        if (jumpDelay <= 0 && IsGrounded())
        {
            jump = false;
        }
        if (!hold)
        {
            Move(v);
            Turn(h);
        }

        if (cam != null)
        {
            HeadRot();
            CameraPosition();
        }
    }
    private void CameraPosition()
    {
        cam.transform.position = new Vector3(transform.position.x, transform.position.y + 24, transform.position.z - 12);
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
    public void Hold()
    {
        hold = !hold;
        Vector3 euler = transform.eulerAngles;
        euler.y = SnapAngle(euler.y);
        transform.eulerAngles = euler;
        var gpos = GroundedPosition();
        transform.position = new Vector3(gpos.x, transform.position.y, gpos.z);
    }
    bool IsGrounded()
    {
        LayerMask groundLayer = (1 << LayerMask.NameToLayer("Road")) | (1 << LayerMask.NameToLayer("Platform"));
        return Physics.Raycast(transform.position, Vector3.down, 1f, groundLayer);
    }
    Vector3 GroundedPosition()
    {
        LayerMask groundLayer = (1 << LayerMask.NameToLayer("Road")) | (1 << LayerMask.NameToLayer("Platform"));
        Ray ray = new Ray(transform.position + Vector3.up, Vector3.down);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, 100f, groundLayer);
        return hit.collider.transform.position;
    }

    float SnapAngle(float angle)
    {
        angle = Mathf.Repeat(angle, 360f); // 0~360으로 정규화
        float[] targets = { 0f, 90f, 180f, 270f };

        float closest = targets[0];
        float minDiff = Mathf.Abs(angle - closest);

        foreach (float t in targets)
        {
            float diff = Mathf.Abs(angle - t);
            if (diff < minDiff)
            {
                minDiff = diff;
                closest = t;
            }
        }

        return closest;
    }
}
