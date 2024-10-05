using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using UnityEngine;

public class World : MonoBehaviour
{
    [SerializeField] private Hunter hunter;
    [SerializeField] private List<Country> countries;
    [SerializeField] private LayerMask countryMask;
    [SerializeField] private Camera cam;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) SetTarget();
    }
    
    private void SetTarget()
    {
        var mp = cam.ScreenToWorldPoint(Input.mousePosition).WhereZ(0);
        var hit = Physics2D.OverlapCircleAll(mp, 0.2f, countryMask);
        if (!hit.Any()) return;
        var distance = Vector3.Distance(mp, hunter.transform.position);
        Tweener.MoveToQuad(hunter.transform, mp, 0.3f * distance);
        var landed = hit.Select(h => h.GetComponent<Country>()).Where(c => c != null).ToList();
        Debug.Log($"Hit in {string.Join(", ", landed.Select(c => c.name))}");
    }
}