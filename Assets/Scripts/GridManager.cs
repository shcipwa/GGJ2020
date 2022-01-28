using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GridManager : Singleton<GridManager>
{
    public GameObject CubePrefab;

    public int GridSize;
    public float GridSpacing;
    public AnimateRenderEmissive[][] GridCells;
    
    // Start is called before the first frame update
    void Start()
    {
        Vector3 startPoint = transform.position - new Vector3((GridSize * GridSpacing) / 2f,0,(GridSize * GridSpacing) / 2f);
        
        GridCells = new AnimateRenderEmissive[GridSize][];
        for (var i = 0; i < GridSize; i++)
        {
            GridCells[i] = new AnimateRenderEmissive[GridSize];
            
            for (var j = 0; j < GridSize; j++)
            {
                var newCube = Instantiate(CubePrefab, startPoint + Vector3.forward * i* GridSpacing + Vector3.right * j * GridSpacing, Quaternion.identity);
                newCube.transform.SetParent(transform);
                var emissive = newCube.GetComponent<AnimateRenderEmissive>();
                emissive.GridCoord = new Vector2Int(i,j);
                GridCells[i][j] = emissive;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            TriggerCell(new Vector2Int((int)(Random.value * GridSize),(int)(Random.value * GridSize)));
        }
    }

    public void TriggerAdjacent(Vector2Int gridCoord)
    {
        TriggerCell(gridCoord + Vector2Int.down);
        TriggerCell(gridCoord + Vector2Int.down + Vector2Int.left);
        TriggerCell(gridCoord + Vector2Int.left);
        TriggerCell(gridCoord + Vector2Int.down + Vector2Int.right);
        TriggerCell(gridCoord + Vector2Int.right);
        TriggerCell(gridCoord + Vector2Int.up + Vector2Int.left);
        TriggerCell(gridCoord + Vector2Int.up);
        TriggerCell(gridCoord + Vector2Int.up + Vector2Int.right);
    }

    private void TriggerCell(Vector2Int gridCoord)
    {
        //Debug.Log("Attempt: " + gridCoord);
        if (gridCoord.x >= GridSize || gridCoord.y >= GridSize || gridCoord.x < 0 || gridCoord.y < 0)
        {
            return;
        }
        //Debug.Log("Success: " + gridCoord);
        GridCells[gridCoord.x][gridCoord.y].TriggerCell();
    }
}
