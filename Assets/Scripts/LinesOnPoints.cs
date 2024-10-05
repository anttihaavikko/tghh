using System;
using System.Collections.Generic;
using UnityEngine;

public class LinesOnPoints : MonoBehaviour
{
    [SerializeField] private List<Transform> nodes;
    
    private LineRenderer line;

    private void Start()
    {
        line = GetComponent<LineRenderer> ();
    }

    private void Update () {
        for (var i = 0; i < nodes.Count; i++) {
            line.SetPosition (i, new Vector3(nodes[i].position.x, nodes[i].position.y, 0));
        }
    }
}