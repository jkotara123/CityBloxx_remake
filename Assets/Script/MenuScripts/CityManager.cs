public static class CityManager
{
    public static PlacedBuilding[,] grid = new PlacedBuilding[5,5];

    public static void Print()
    {
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