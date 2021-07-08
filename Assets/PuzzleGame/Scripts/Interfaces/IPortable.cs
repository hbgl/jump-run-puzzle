using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPortable
{
    public void SetPortableState(GameObject hand);
    public void RemovePortableState();
    public bool GetPortableState();
}
