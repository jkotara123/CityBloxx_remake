using UnityEngine;

public class CityGrid : MonoBehaviour
{
    public GameObject slotPrefab;
    public int width = 5;
    public int height = 5;
    
    // Jeśli kwadrat ma 0.9, a przerwa 0.1, to suma (step) wynosi 1.0
    private float step = 1.0f; 

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 pos = new Vector3(x * step, y * step, 0);
                
                GameObject slot = Instantiate(slotPrefab, pos, Quaternion.identity, transform);
                slot.name = $"Slot_{x}_{y}";
            }
        }

        CenterGrid();
    }

    void CenterGrid()
    {
        transform.position = new Vector3(0f, -3f, 0);
        transform.localScale = new Vector3(1.5f, 1.5f, 1f);
    }
}