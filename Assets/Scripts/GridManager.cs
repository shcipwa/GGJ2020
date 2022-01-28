using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : Singleton<GridManager>
{
    public GameObject CubePrefab;

    public int GridSize;
    public float GridSpacing;
    public AnimateRenderEmissive[][] GridCells;
    
    // Start is called before the first frame update
    void Start()
    {
        Vector3 startPoint = transform.position;
        
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
        
    }
}
