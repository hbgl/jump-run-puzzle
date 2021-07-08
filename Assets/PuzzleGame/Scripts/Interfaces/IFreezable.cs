using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFreezable
{
    public void SetFrozenState(GameObject iceblock);
    public void RemoveFrozenState();
    public bool GetFrozenState();
}
