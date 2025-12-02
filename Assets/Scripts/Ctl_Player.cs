using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Ctl_Player : MonoBehaviour
{
    [SerializeField] public int P;

    [SerializeField] Game_Manager gameManager;
    [SerializeField] Round_Manager roundManager;
    [SerializeField] Tile_Manager tileManager;
    [SerializeField] GameObject BF_wall;
    [SerializeField] GameObject bullet;

    float speed = 3f;
    float xSpeed = 0;
    float ySpeed = 0;

    bool wallMode = false;
    GameObject bluePrint;

    private void Awake()
    {
        if (gameManager == null) gameManager = FindObjectOfType<Game_Manager>();
        if (roundManager == null) roundManager = FindObjectOfType<Round_Manager>();
        if (tileManager == null) tileManager = FindObjectOfType<Tile_Manager>();
    }

    private void LateUpdate()
    {
        if (P != GameClient.LocalPlayerId) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (wallMode && !roundManager.round_in_progress)
            {
                if (bluePrint != null && GameClient.Instance != null)
                {
                    Vector3 pos = bluePrint.transform.position;
                    int tx = (int)pos.x;
                    int ty = (int)pos.y;
                    GameClient.Instance.SendTileChange(P, tx, ty, 1);
                }
            }
            else if (roundManager.round_in_progress)
            {
                if (GameClient.Instance != null)
                {
                    float angleZ = transform.eulerAngles.z;
                    GameClient.Instance.SendFire(P, transform.position, angleZ);
                }
            }

            if (wallMode)
                ChangeWallMode();
        }

        if (Input.GetKeyDown(KeyCode.E) && !roundManager.round_in_progress)
            ChangeWallMode();
    }

    private void FixedUpdate()
    {
        if (GameClient.LocalPlayerId > 0 && P != GameClient.LocalPlayerId)
            return;

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Move(x, y);
        LookMouse();

        if (wallMode)
            BF_Follow();

        if (GameClient.Instance != null)
        {
            float angleZ = transform.eulerAngles.z;
            GameClient.Instance.SendPosition(P, transform.position, angleZ);
        }
    }

    private void ChangeWallMode()
    {
        wallMode = !wallMode;
        if (wallMode)
        {
            bluePrint = Instantiate(BF_wall);
            BF_Follow();
        }
        else
        {
            if (bluePrint != null)
                Destroy(bluePrint);
        }
    }

    private void Move(float x, float y)
    {
        xSpeed += x * Time.deltaTime;
        ySpeed += y * Time.deltaTime;

        xSpeed *= 0.9f;
        ySpeed *= 0.9f;

        transform.position += new Vector3(xSpeed, ySpeed, 0f);
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
        mousePos.z = 0f;
        if (bluePrint != null)
            bluePrint.transform.position = mousePos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Coin"))
        {
            Destroy(collision.gameObject);
            gameManager.AddCoin(1);
        }
    }
}