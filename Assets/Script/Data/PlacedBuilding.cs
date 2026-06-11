using UnityEngine;
    
[System.Serializable]
public class PlacedBuilding {
    public BuildingColor color;
    public int score;
    public bool hasRoof;
    public Vector2Int gridPos;

    public void Print() {
        UnityEngine.Debug.Log($"Color: {color}, Score: {score}, Has Roof: {hasRoof}");
    }
}