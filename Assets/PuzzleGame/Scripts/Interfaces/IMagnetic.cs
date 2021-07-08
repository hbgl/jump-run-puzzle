using Assets.PuzzleGame.Support;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMagnetic
{
    public MoveConstraints MoveConstraints { get; }
    public bool CanStandOn { get; }

    public void SetMagneticState(GameObject magnet);
    public void RemoveMagneticState();
    public bool GetMagneticState();
    public bool GetEnabledState();
    public void Cooldown(double secs);
    public double GetCooldown();
}
