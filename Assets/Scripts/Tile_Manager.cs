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
    [SerializeField] private Game_Manager gameManager;
    [SerializeField] private GameObject[] bluePrint;
    [SerializeField] private RectTransform UI_tower_info;
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
        if (Input.GetMouseButtonDown(0) && !gameManager.round_in_progress) Click();
    }

    private void Click()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null && hit.collider.CompareTag("Wall"))
        {
            if (selectedTile != null) 
                selectedTile.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);

            selectedTile = hit.transform.gameObject;
            selectedTile.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
            UI_tower_info.transform.position = selectedTile.transform.position;
            UI_tower_info.gameObject.SetActive(true);
        }
        else
        {
            ResetSelectedTile();
        }
    }
    public void ResetSelectedTile()
    {
        if (selectedTile != null)
            selectedTile.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);

        selectedTile = null;
        UI_tower_info.gameObject.SetActive(false);
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
    public void TileChange(int x, int y, int id) // 0 : ÀÜµð , 1 : º®
    {
        Destroy(map[y,x]);
        map[y, x] = Instantiate(bluePrint[id],new Vector2(x, y),Quaternion.identity, transform);
    }
}
