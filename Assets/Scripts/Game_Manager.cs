using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Manager : MonoBehaviour
{
    [SerializeField] private Tile_Manager tileManager;
    [SerializeField] private Round_Manager roundManager;
    [SerializeField] public GameObject[] Towers;
<<<<<<< HEAD

=======
>>>>>>> Test
    public void CreateRandomTower()
    {
        Transform wall = tileManager.selectedTile.transform;
        int num = Random.Range(0, Towers.Length);
        Instantiate(Towers[num], wall.position, Quaternion.identity, wall);
        tileManager.ResetSelectedTile();
    }
    public void RemoveTower()
    {
        if (tileManager.selectedTile.transform.GetChild(0) != null)
            Destroy(tileManager.selectedTile.transform.GetChild(0).gameObject);
        tileManager.ResetSelectedTile();
    }
}
