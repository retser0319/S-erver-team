using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Ctl_Player : MonoBehaviour
{
    [SerializeField] Game_Manager gameManager;
    [SerializeField] Tile_Manager tileManager;
    [SerializeField] GameObject BF_wall;
    [SerializeField] GameObject bullet;
    // Update is called once per frame
    float speed = 3f;
    float xSpeed = 0;
    float ySpeed = 0;

    bool wallMode = false;
    GameObject bluePrint;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (wallMode && gameManager.round_in_progress)
            {
                wallMode = false;
                Destroy(bluePrint);
            }
            else if (wallMode && !gameManager.round_in_progress)
                tileManager.TileChange((int)bluePrint.transform.position.x, (int)bluePrint.transform.position.y, 1);
            else
                Instantiate(bullet, transform.position, transform.rotation);
        }

        if (Input.GetKeyDown(KeyCode.E) && !gameManager.round_in_progress)
        {
            wallMode = !wallMode;
            if (wallMode)
            {
                bluePrint = Instantiate(BF_wall);
            }
            else
            {
                Destroy(bluePrint);
            }
        }
    }
    void FixedUpdate()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Move(x, y);
        LookMouse();

        if (wallMode)
        {
            BF_Follow();
        }
    }

    private void Move(float x, float y)
    {
        if (xSpeed < speed) xSpeed += x * Time.deltaTime;
        if (ySpeed < speed) ySpeed += y * Time.deltaTime;

        xSpeed *= 0.9f;
        ySpeed *= 0.9f;

        transform.position = new Vector2(transform.position.x + xSpeed, transform.position.y + ySpeed);
    }
    private void LookMouse()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePos - (Vector2)transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    private void BF_Follow()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.x = (int)(mousePos.x + 0.5f);
        mousePos.y = (int)(mousePos.y + 0.5f);
        mousePos.z = 0f; // 2D¿ë
        bluePrint.transform.position = mousePos;
    }
}
