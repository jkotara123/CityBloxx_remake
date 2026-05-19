using UnityEngine;


[System.Serializable]
public class BuildingSprites
{
    public Sprite redNoRoof;
    public Sprite redRoof;

    public Sprite blueNoRoof;
    public Sprite blueRoof;

    public Sprite greenNoRoof;
    public Sprite greenRoof;

    public Sprite yellowNoRoof;
    public Sprite yellowRoof;

    public Sprite empty;
}

public class CityGrid : MonoBehaviour
{
    public int width = 5;
    public int height = 5;
    public float stepX = 1.031f; 
    public float stepY = 1.069f; 

    public GameObject slotPrefab;
    public BuildingSprites sprites;

    public GameObject cursorPrefab;
    private GameObject cursorInstance;
    private float cursorOffsetX = 4.75f;
    private float cursorOffsetY = 4.75f;
    private int currentX = 0;
    private int currentY = 0;
    
    private GameObject[,] visuals = new GameObject[5,5];

    void Start()
    {
        GenerateGrid();

        cursorInstance = Instantiate(cursorPrefab);

        UpdateCursor();

        RefreshVisuals();

        if (LevelManager.LastBuilding != null)
        {
            LevelManager.LastBuilding.Print();
        }
    }

    void Update(){
        if (Input.GetKeyDown(KeyCode.Return)){
            PlaceBuilding();
        }
        HandleMovement();
    }

    void GenerateGrid(){
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 pos = new Vector3(x * stepX, y * stepY, -1);

                GameObject slot = Instantiate(slotPrefab, pos, Quaternion.identity, transform);
                slot.name = $"Slot_{x}_{y}";

                visuals[x, y] = slot;

                SetEmptyVisual(x, y);
            }
        }

        CenterGrid();
    }


    void CenterGrid(){
        transform.position = new Vector3(-4.85f,-8f, 0);
        transform.localScale = new Vector3(1.5f, 1.5f, 1f);
    }

    void HandleMovement(){
        if (Input.GetKeyDown(KeyCode.RightArrow))
            currentX = Mathf.Min(currentX + 1, width - 1);

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            currentX = Mathf.Max(currentX - 1, 0);

        if (Input.GetKeyDown(KeyCode.UpArrow))
            currentY = Mathf.Min(currentY + 1, height - 1);

        if (Input.GetKeyDown(KeyCode.DownArrow))
            currentY = Mathf.Max(currentY - 1, 0);

        UpdateCursor();
    }

    void UpdateCursor(){
        if (cursorInstance == null)
            return;

        cursorInstance.transform.position =
            new Vector3(
                currentX * stepX * 1.5f + cursorOffsetX,
                currentY * stepY * 1.5f + cursorOffsetY,
                -1
            ) + transform.position;
    }

    void PlaceBuilding(){
        if (LevelManager.LastBuilding == null)
            return;

        CityManager.grid[currentX, currentY] = LevelManager.LastBuilding;

        LevelManager.LastBuilding = null;

        RefreshVisuals();
        CityManager.Print();
    }

    void SetEmptyVisual(int x, int y){
        var sr = visuals[x, y].GetComponent<SpriteRenderer>();
        sr.sprite = sprites.empty;
    }

    void SetBuildingVisual(int x, int y, PlacedBuilding b){
        var sr = visuals[x, y].GetComponent<SpriteRenderer>();

        if (b == null)
        {
            sr.sprite = sprites.empty;
            return;
        }

        switch (b.color)
        {
            case BuildingColor.Red:
                sr.sprite = b.hasRoof ? sprites.redRoof : sprites.redNoRoof;
                break;

            case BuildingColor.Blue:
                sr.sprite = b.hasRoof ? sprites.blueRoof : sprites.blueNoRoof;
                break;

            case BuildingColor.Green:
                sr.sprite = b.hasRoof ? sprites.greenRoof : sprites.greenNoRoof;
                break;

            case BuildingColor.Yellow:
                sr.sprite = b.hasRoof ? sprites.yellowRoof : sprites.yellowNoRoof;
                break;
        }
    }

    void RefreshVisuals(){
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                SetBuildingVisual(x, y, CityManager.grid[x, y]);
            }
        }
    }
}