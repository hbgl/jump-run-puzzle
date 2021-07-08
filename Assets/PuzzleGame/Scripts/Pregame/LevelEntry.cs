using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

/**
 * Class that initializes the Level Button
 * Attach Script to RoomEntry Prefab
 */
public class LevelEntry : MonoBehaviour
{
    private Launcher launcher;
    private string levelName;

    public void Initialize(string name, Launcher launcher)
    {
        this.launcher = launcher;
        levelName = name;
        GetComponentInChildren<Text>().text = name;
    }

    public void ButtonLoadLevel()
    {
        launcher.ButtonCreateRoom(levelName);
    }
}