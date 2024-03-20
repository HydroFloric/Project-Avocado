using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeEnity : EntityBase
{
    [SerializeField]
    Pipe p;
    public void init()
    {
        p = GetComponent<Pipe>();
    }
    private void OnDestroy()
    {
        if (p != null)
        {
            p.setActive(false);
            foreach (var c in p.Children)
            {
                c.Parent = null;
            }
        }
    }
}
