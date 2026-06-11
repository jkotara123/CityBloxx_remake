using UnityEngine;

public static class CityManager
{
    public static PlacedBuilding[,] grid = new PlacedBuilding[5,5];

    public static void SaveGame(){
        SaveData data = new SaveData();

        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                if (grid[x, y] != null)
                {
                    // Zapamiętujemy pozycję wewnątrz obiektu budynku przed zapisem
                    grid[x, y].gridPos = new Vector2Int(x, y);
                    data.savedBuildings.Add(grid[x, y]);
                }
            }
        }

        // Zamiana obiektu na tekst JSON
        string json = JsonUtility.ToJson(data);
        // Zapis do pamięci urządzenia
        PlayerPrefs.SetString("CityBloxxSave", json);
        PlayerPrefs.Save();
        
        UnityEngine.Debug.Log("Gra została zapisana!");
    }

    public static void LoadGame(){
        grid = new PlacedBuilding[5, 5];

        if (!PlayerPrefs.HasKey("CityBloxxSave"))
        {
            UnityEngine.Debug.Log("Brak zapisanego stanu gry. Zaczynamy od nowa.");
            return;
        }

        string json = PlayerPrefs.GetString("CityBloxxSave");
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        foreach (PlacedBuilding b in data.savedBuildings)
        {
            grid[b.gridPos.x, b.gridPos.y] = b;
        }

        UnityEngine.Debug.Log("Gra została wczytana!");
    }
        
    public static void Print(){
        string output = "City Grid:\n";
        for (int y = 0; y < grid.GetLength(1); y++)
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                PlacedBuilding building = grid[x, y];
                if (building != null)
                {
                    output += $"[{building.color}|S:{building.score}|R:{building.hasRoof}] ";
                }
                else
                {
                    output += "[Empty] ";
                }
            }
            output += "\n";
        }
        UnityEngine.Debug.Log(output);
    }
}

[System.Serializable]
public class SaveData
{
    public System.Collections.Generic.List<PlacedBuilding> savedBuildings = new System.Collections.Generic.List<PlacedBuilding>();
}

