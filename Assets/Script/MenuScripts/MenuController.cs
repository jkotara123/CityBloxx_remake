using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{   
    public void SelectAndStart(BuildingType type)
    {
        LevelManager.SelectedBuilding = type;
        SceneManager.LoadScene("MainScene");
    }
}