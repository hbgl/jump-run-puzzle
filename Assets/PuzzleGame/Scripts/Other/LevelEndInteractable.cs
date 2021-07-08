using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEndInteractable : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        GameObject.Find("Game Manager").GetComponent<GameManager>().TriggerLevelEnd();
    }
}
