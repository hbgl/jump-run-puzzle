using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomTags : MonoBehaviour
{

    public List<string> tags = new List<string>();

    public bool HasTag(string tag)
    {
        return tags.Contains(tag);
    }

    public void AddTag(string tag)
    {
        tags.Add(tag);
    }
}
