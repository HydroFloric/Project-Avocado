using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeEnity : EntityBase
{
    Pipe p;
    private void Start()
    {
        p = GetComponent<Pipe>();
    }
    private void OnDestroy()
    {
        p.setActive(false);
        foreach (var c in p.Children)
        {
            c.Parent = null;
        }
    }
}
