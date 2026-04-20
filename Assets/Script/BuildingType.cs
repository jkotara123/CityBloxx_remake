using UnityEngine;

[CreateAssetMenu(fileName = "NewBuildingType", menuName = "CityBloxx/BuildingType")]
public class BuildingType : ScriptableObject
{
    public string buildingName;
    public Color blockColor;
    public Sprite blockSprite;  // Opcjonalnie, jeśli masz inne grafiki
    public Sprite roofSprite;   // Grafika dachu
    public int totalBlocksToBuild;
}