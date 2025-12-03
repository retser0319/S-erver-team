using UnityEngine;

public class AI_Fly : MonoBehaviour
{
    public float speed;
    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(-speed * Time.deltaTime, 0, 0);
    }
}
