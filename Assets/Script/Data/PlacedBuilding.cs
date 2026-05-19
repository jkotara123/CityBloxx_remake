[System.Serializable]
public class PlacedBuilding {
    public BuildingColor color;
    public int score;
    public bool hasRoof;

    public void Print() {
        UnityEngine.Debug.Log($"Color: {color}, Score: {score}, Has Roof: {hasRoof}");
    }
}