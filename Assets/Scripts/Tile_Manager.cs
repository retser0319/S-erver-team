using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;
using static UnityEngine.UI.Image;

public class Tile
{
    public bool obstacle = false;
}
public class Tile_Manager : MonoBehaviour
{
    [SerializeField] private GameObject[] bluePrint;
    public GameObject[,] map;
    public Tile[,] tiles;
    public int ySize, xSize;

    public GameObject selectedTile;

    void Awake()
    {
        MapSetting();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0)) Click();
    }

    private void Click()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        // 태그로 벽만
        if (selectedTile != null) selectedTile.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
        if (hit.collider != null) selectedTile = hit.transform.gameObject;
        selectedTile.GetComponent<SpriteRenderer>().color = new Color(0.5f,0.5f,0.5f);
    }
    private void MapSetting()
    {
        map = null;
        tiles = null;

        int xMax = 0;
        int yMax = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform tile = transform.GetChild(i);
            if (tile.position.x > xMax) xMax = (int)tile.position.x;
            if (tile.position.y > yMax) yMax = (int)tile.position.y;
        }

        xSize = xMax + 1;
        ySize = yMax + 1;

        map = new GameObject[ySize, xSize];
        tiles = new Tile[ySize, xSize];

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform tile = transform.GetChild(i);
            map[(int)tile.position.y, (int)tile.position.x] = tile.gameObject;
            tiles[(int)tile.position.y, (int)tile.position.x] = new Tile();
            tiles[(int)tile.position.y, (int)tile.position.x].obstacle = false;
        }
    }
    public void TileChange(int x, int y, int id) // 0 : 잔디 , 1 : 벽
    {
        Destroy(map[y,x]);
        map[y, x] = Instantiate(bluePrint[id],new Vector2(x, y),Quaternion.identity, transform);

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // 예: 각 오브젝트 위치 출력
        foreach (GameObject enemy in enemies)
        {
            enemy.GetComponent<Nav_AI>().Find();
        }
    }
}
