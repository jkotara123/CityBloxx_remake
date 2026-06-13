using UnityEngine;
using UnityEngine.SceneManagement;

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

    public GameObject cancelButton;
    public GameObject[] createButtons;

    [Header("UI Statistics")]
    public TMPro.TextMeshProUGUI currentTileScoreText;
    public TMPro.TextMeshProUGUI totalCityScoreText;

    public TMPro.TextMeshProUGUI newBuildingScore;
    public UnityEngine.UI.Image newBuildingPreviewImage;
    public TMPro.TextMeshProUGUI newBuildingScoreText;

    void Start()
    {
        CityManager.LoadGame();
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
        // UpdateCancelButtonVisibility();
        
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
        UpdateStatisticsUI();
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

    void PlaceBuilding()    {
        if (LevelManager.LastBuilding == null)
            return;

        if (!CanPlaceBuilding(currentX, currentY, LevelManager.LastBuilding.color))
        {
            return;
        }

        CityManager.grid[currentX, currentY] = LevelManager.LastBuilding;
        LevelManager.LastBuilding = null;
        
        RefreshVisuals();
        CityManager.Print();
        
        CityManager.SaveGame();
    }

    private bool CanPlaceBuilding(int targetX, int targetY, BuildingColor color)
    {
        // Niebieski budynek można postawić zawsze
        if (color == BuildingColor.Blue) 
            return true;

        bool hasBlueNeighbor = false;
        bool hasRedNeighbor = false;
        bool hasGreenNeighbor = false;

        // Tablice przesunięć dla 4 sąsiadów: prawo, lewo, góra, dół
        int[] dx = { 1, -1, 0, 0 };
        int[] dy = { 0, 0, 1, -1 };

        // Sprawdzamy wszystkich 4 sąsiadów wokół pozycji docelowej
        for (int i = 0; i < 4; i++)
        {
            int neighborX = targetX + dx[i];
            int neighborY = targetY + dy[i];

            // Upewniamy się, że sąsiad mieści się w granicach gridu
            if (neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY < height)
            {
                PlacedBuilding neighbor = CityManager.grid[neighborX, neighborY];
                if (neighbor != null)
                {
                    if (neighbor.color == BuildingColor.Blue) hasBlueNeighbor = true;
                    if (neighbor.color == BuildingColor.Red) hasRedNeighbor = true;
                    if (neighbor.color == BuildingColor.Green) hasGreenNeighbor = true;
                }
            }
        }

        // Warunki logiczne dla poszczególnych kolorów:
        switch (color)
        {
            case BuildingColor.Red:
                // Musi sąsiadować z niebieskim
                return hasBlueNeighbor;

            case BuildingColor.Green:
                // Musi sąsiadować z niebieskim ORAZ czerwonym
                return hasBlueNeighbor && hasRedNeighbor;

            case BuildingColor.Yellow:
                // Musi sąsiadować z niebieskim, czerwonym ORAZ zielonym
                return hasBlueNeighbor && hasRedNeighbor && hasGreenNeighbor;

            default:
                return false;
        }
    }
    public void DestroyBuilding(){
        LevelManager.LastBuilding = null;
        UpdateStatisticsUI();
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

        UpdateStatisticsUI();
    }

    public void UpdateStatisticsUI(){
        // === 1. AKTUALIZACJA STANU MIASTA (Zawsze aktywne) ===
        int totalScore = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (CityManager.grid[x, y] != null)
                {
                    totalScore += CityManager.grid[x, y].score;
                }
            }
        }

        if (totalCityScoreText != null) 
            totalCityScoreText.text = $"{totalScore}";

        // === 2. AKTUALIZACJA CURRENT TILE (Zawsze aktywne) ===
        PlacedBuilding buildingOnTile = CityManager.grid[currentX, currentY];
        if (currentTileScoreText != null)
        {
            if (buildingOnTile != null)
                currentTileScoreText.text = $"{buildingOnTile.score}";
            else
                currentTileScoreText.text = "0";
        }

        // === 3. ZARZĄDZANIE WIDOCZNOŚCIĄ I LOGIKĄ NA PODSTAWIE LASTBUILDING ===
        bool hasNewBuilding = (LevelManager.LastBuilding != null);

        // Przełączanie przycisków kreacji (aktywne tylko gdy brak nowego budynku)
        foreach (GameObject button in createButtons)
        {
            if (button != null)
            {
                button.SetActive(!hasNewBuilding);
            }
        }

        // Przełączanie przycisku kasowania (aktywne gdy jest nowy budynek)
        if (cancelButton != null)
        {
            cancelButton.SetActive(hasNewBuilding);
        }

        // Przełączanie widoczności statystyk nowego budynku
        if (newBuildingScore != null)
        {
            newBuildingScore.gameObject.SetActive(hasNewBuilding);
        }

        if (newBuildingScoreText != null){
            newBuildingScoreText.gameObject.SetActive(hasNewBuilding);
        }
        
        if (newBuildingPreviewImage != null)
        {
            newBuildingPreviewImage.gameObject.SetActive(hasNewBuilding);
        }

        // Aktualizacja danych nowego budynku, jeśli istnieje
        if (hasNewBuilding)
        {
            if (newBuildingScore != null)
                newBuildingScore.text = $"{LevelManager.LastBuilding.score}";

            if (newBuildingPreviewImage != null)
            {   
                newBuildingPreviewImage.sprite = GetSpriteForBuilding(LevelManager.LastBuilding);
            }
        }
    }

    private Sprite GetSpriteForBuilding(PlacedBuilding b){
        if (b == null) return sprites.empty;

        switch (b.color)
        {
            case BuildingColor.Red: return b.hasRoof ? sprites.redRoof : sprites.redNoRoof;
            case BuildingColor.Blue: return b.hasRoof ? sprites.blueRoof : sprites.blueNoRoof;
            case BuildingColor.Green: return b.hasRoof ? sprites.greenRoof : sprites.greenNoRoof;
            case BuildingColor.Yellow: return b.hasRoof ? sprites.yellowRoof : sprites.yellowNoRoof;
            default: return sprites.empty;
        }
    }

    public void NewGame(){
        // 1. Usuwamy zapis z pamięci urządzenia
        if (PlayerPrefs.HasKey("CityBloxxSave"))
        {
            PlayerPrefs.DeleteKey("CityBloxxSave");
        }

        // 2. Czyścimy tablicę w pamięci RAM
        CityManager.grid = new PlacedBuilding[5, 5];
        if (LevelManager.LastBuilding != null) 
            LevelManager.LastBuilding = null;

        // 3. Przeładowujemy aktualną scenę, aby zresetować grid i UI
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
        
        Debug.Log("Rozpoczęto nową grę. Stary zapis został usunięty.");
    }

    public void QuitGame(){
        #if UNITY_EDITOR
            // Jeśli testujesz grę w edytorze Unity, to polecenie wyłączy tryb Play
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // Jeśli grasz w zbuildowaną grę (.exe), to polecenie zamknie program
            Application.Quit();
        #endif
        
        Debug.Log("Wyjście z gry zostało wywołane.");
    }
}