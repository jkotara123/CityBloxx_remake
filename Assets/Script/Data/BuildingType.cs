using UnityEngine;

[CreateAssetMenu(fileName = "NewBuildingType", menuName = "CityBloxx/BuildingType")]
public class BuildingType : ScriptableObject
{
    public Sprite blockSprite;
    public Sprite roofSprite;

    public BuildingColor color;
    public string buildingName;
    public int totalBlocks;
}