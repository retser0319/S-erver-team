using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] GameObject ch;
    float delay = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - delay > 0.5f)
        {
            Instantiate(ch, transform.position, transform.rotation);
            delay = Time.time;
        }
    }
}
