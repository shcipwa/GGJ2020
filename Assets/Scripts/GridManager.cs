using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class GridManager : Singleton<GridManager>
{
    public GameObject CubePrefab;

    public int GridSize;
    public float GridSpacing;
    public AnimateRenderEmissive[][] GridCells;

    public bool Respawn;
    
    // Start is called before the first frame update
    void Start()
    {
        RespawnCells();
    }

    private void RespawnCells()
    {
        if (GridCells != null)
        {
            foreach (var animateRenderEmissive in GridCells)
            {
                foreach (var renderEmissive in animateRenderEmissive)
                {
                    Destroy(renderEmissive.gameObject);
                }
            }

            GridCells = null;
        }
        Vector3 startPoint =
            transform.position - new Vector3((GridSize * GridSpacing) / 2f, 0, (GridSize * GridSpacing) / 2f);

        GridCells = new AnimateRenderEmissive[GridSize][];
        for (var i = 0; i < GridSize; i++)
        {
            GridCells[i] = new AnimateRenderEmissive[GridSize];

            for (var j = 0; j < GridSize; j++)
            {
                var newCube = Instantiate(CubePrefab,
                    startPoint + Vector3.forward * i * GridSpacing + Vector3.right * j * GridSpacing, Quaternion.identity);
                newCube.transform.SetParent(transform);
                var emissive = newCube.GetComponent<AnimateRenderEmissive>();
                emissive.GridCoord = new Vector2Int(i, j);
                GridCells[i][j] = emissive;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.F4))
        {
            TriggerCell(new Vector2Int((int)(Random.value * GridSize),(int)(Random.value * GridSize)),GridEventType.WhiteFlash);
        }
        
        if (Input.GetKey(KeyCode.F5))
        {
            TriggerCell(new Vector2Int((int)(Random.value * GridSize),(int)(Random.value * GridSize)),GridEventType.DestroyGrid);
        }
        
        if (Input.GetKey(KeyCode.F6))
        {
            TriggerCell(new Vector2Int((int)(Random.value * GridSize),(int)(Random.value * GridSize)),GridEventType.RepairGrid);
        }

        if (Respawn)
        {
            RespawnCells();
            Respawn = false;
        }
    }

    public void TriggerAdjacent(Vector2Int gridCoord, GridEventType type)
    {
        TriggerCell(gridCoord + Vector2Int.down,type);
        TriggerCell(gridCoord + Vector2Int.down + Vector2Int.left,type);
        TriggerCell(gridCoord + Vector2Int.left,type);
        TriggerCell(gridCoord + Vector2Int.down + Vector2Int.right,type);
        TriggerCell(gridCoord + Vector2Int.right,type);
        TriggerCell(gridCoord + Vector2Int.up + Vector2Int.left,type);
        TriggerCell(gridCoord + Vector2Int.up,type);
        TriggerCell(gridCoord + Vector2Int.up + Vector2Int.right,type);
    }

    private void TriggerCell(Vector2Int gridCoord, GridEventType type)
    {
        //Debug.Log("Attempt: " + gridCoord);
        if (gridCoord.x >= GridSize || gridCoord.y >= GridSize || gridCoord.x < 0 || gridCoord.y < 0)
        {
            return;
        }
        //Debug.Log("Success: " + gridCoord);
        GridCells[gridCoord.x][gridCoord.y].TriggerCell(type);
    }

    public void TriggerNearest(Vector3 transformPosition, GridEventType type)
    {
        var coordVec = GetNearestCell(transformPosition);

        GridCells[coordVec.x][coordVec.y].TriggerCell(type);
    }

    public bool IsNearestActive(Vector3 position)
    {
        var coordVec = GetNearestCell(position);

        return GridCells[coordVec.x][coordVec.y].IsActive();
    }

    private Vector2Int GetNearestCell(Vector3 transformPosition)
    {
        Vector3 startPoint =
            transform.position - new Vector3((GridSize * GridSpacing) / 2f, 0, (GridSize * GridSpacing) / 2f);

        var delta = transformPosition - startPoint;

        int x = (int) (delta.x / GridSpacing);
        int y = (int) (delta.z / GridSpacing);

        x = Math.Clamp(x, 0, GridSize);
        y = Math.Clamp(y, 0, GridSize);
        
        var coordVec = new Vector2Int(y, x);
        //Debug.Log("Got coord " + coordVec + " from pos " + transformPosition + " start point was " + startPoint + " delta " + delta);
        return coordVec;
    }
}
