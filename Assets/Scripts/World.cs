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

    private Country current;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) SetTarget();
    }
    
    private void SetTarget()
    {
        var mp = cam.ScreenToWorldPoint(Input.mousePosition).WhereZ(0);
        var hit = Physics2D.OverlapCircleAll(mp, 0.1f, countryMask);
        if (!hit.Any()) return;
        var distance = Vector3.Distance(mp, hunter.transform.position);
        var delay = 0.3f * distance;
        Tweener.MoveToQuad(hunter.transform, mp, delay);
        var landed = hit.Select(h => h.GetComponent<Country>()).Where(c => c != null).ToList();
        var closest = landed.OrderBy(c => Vector3.Distance(mp, c.CapitalPosition)).First();
        
        if (current)
        {
            current.Hide();
        }
        
        if (Vector3.Distance(mp, closest.CapitalPosition) < 0.5f)
        {
            this.StartCoroutine(() => closest.Show(), delay);
            current = closest;
        }
    }
}