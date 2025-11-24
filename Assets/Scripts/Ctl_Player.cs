using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Ctl_Player : MonoBehaviour
{
    [SerializeField] Game_Manager gameManager;
    [SerializeField] Round_Manager roundManager;
    [SerializeField] Tile_Manager tileManager;
    [SerializeField] GameObject BF_wall;
    [SerializeField] GameObject bullet;
    // Update is called once per frame
    float speed = 3f;
    float xSpeed = 0;
    float ySpeed = 0;

    bool wallMode = false;
    GameObject bluePrint;
    private void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 벽 설치
            if (wallMode && !roundManager.round_in_progress)
            {
                tileManager.TileChange((int)bluePrint.transform.position.x, (int)bluePrint.transform.position.y, 1);
            }
            // 라운드 시작시 공격가능
            else if (roundManager.round_in_progress)
            {
                Instantiate(bullet, transform.position, transform.rotation);
            }

            if (wallMode)
            {
                ChangeWallMode();
            }
        }

        if (Input.GetKeyDown(KeyCode.E) && !roundManager.round_in_progress)
        {
            ChangeWallMode();
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

    private void ChangeWallMode() {
        wallMode = !wallMode;
        if (wallMode)
        {
            bluePrint = Instantiate(BF_wall);
            BF_Follow();
        }
        else
        {
            Destroy(bluePrint);
        }
    }
    private void Move(float x, float y)
    {
        if (xSpeed < speed) xSpeed += x * Time.deltaTime;
        if (ySpeed < speed) ySpeed += y * Time.deltaTime;

        xSpeed *= 0.9f;
        ySpeed *= 0.9f;

        transform.position += new Vector3(xSpeed, ySpeed, 0f);
    }
    private void LookMouse()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePos - (Vector2)transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, angle);
    }
    private void BF_Follow()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.x = (int)(mousePos.x + 0.5f);
        mousePos.y = (int)(mousePos.y + 0.5f);
        mousePos.z = 0f; // 2D용
        bluePrint.transform.position = mousePos;
    }
}
