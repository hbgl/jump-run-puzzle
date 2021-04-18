using JumpRunPuzzle.Multiplayer.TestLab.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeesawTrigger : MonoBehaviour
{
    public Seesaw seesaw;   // Das gesamte Brett
    public bool isLeft;     // Prüft ob Character auf linker oder rechter Hälfte steht

    private void OnTriggerEnter(Collider other)
    {
        seesaw.SetDirection(isLeft ? -1 : 1);
    }
}
