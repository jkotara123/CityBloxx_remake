using UnityEngine;

[CreateAssetMenu(fileName = "NewBuildingType", menuName = "CityBloxx/BuildingType")]
public class BuildingType : ScriptableObject
{
    public string buildingName;
    public Sprite blockSprite;
    public Sprite roofSprite;
    public int totalBlocks;
}