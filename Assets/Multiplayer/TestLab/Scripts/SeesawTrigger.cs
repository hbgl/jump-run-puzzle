using JumpRunPuzzle.Multiplayer.TestLab.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeesawTrigger : MonoBehaviour
{
    public Seesaw seesaw;   // Das gesamte Brett
    public bool isLeft;     // Pr�ft ob Character auf linker oder rechter H�lfte steht

    private void OnTriggerEnter(Collider other)
    {
        seesaw.SetDirection(isLeft ? -1 : 1);
    }
}
