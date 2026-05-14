using UnityEngine;

public class GridCursor : MonoBehaviour
{
    public Vector2Int currentPos = new Vector2Int(0, 0);
    public int width = 5;
    public int height = 5;
    public float step = 1.0f;

    // Referencja do danych z minigierki (możesz to ustawić przez statyczną klasę)
    public static PlacedBuilding pendingBuilding;

    void Update()
    {
        HandleMovement();

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            PlaceBuilding();
        }
    }

    void HandleMovement()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && currentPos.y < height - 1) currentPos.y++;
        else if (Input.GetKeyDown(KeyCode.DownArrow) && currentPos.y > 0) currentPos.y--;
        else if (Input.GetKeyDown(KeyCode.RightArrow) && currentPos.x < width - 1) currentPos.x++;
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && currentPos.x > 0) currentPos.x--;

        // Aktualizacja pozycji wizualnej kursora
        // Pamiętaj o offsetach Twojego Gridu!
        transform.localPosition = new Vector3(currentPos.x * step, currentPos.y * step, -1f);
    }

    void PlaceBuilding()
    {
        // Tutaj logika sprawdzająca czy pole jest wolne
        Debug.Log($"Budowanie na: {currentPos.x}, {currentPos.y}");
        
        // Wywołaj tutaj metodę z CityGrid, która postawi odpowiedni prefab/ikonkę
        // FindObjectOfType<CityGrid>().SetBuildingAt(currentPos, pendingBuilding);
    }
}