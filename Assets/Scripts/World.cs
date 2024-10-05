using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Utils;
using UnityEngine;

public class World : MonoBehaviour
{
    [SerializeField] private Hunter hunter;
    [SerializeField] private List<Country> countries;
    [SerializeField] private LayerMask countryMask;
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject zoomCam;
    [SerializeField] private Menu menu;
    [SerializeField] private GameObject huntCam;
    [SerializeField] private LineRenderer route;

    private Country current;
    private bool flying;
    private bool isMenuFlipped;
    private bool inCapital;

    private void Start()
    {
        ShowMenu(hunter.transform.position);
    }

    private void Update()
    {
        UpdateRoute();
        if (Input.GetMouseButtonDown(0)) SetTarget();
        if (DevKey.Down(KeyCode.F)) FlyMode();
    }

    public void FlyMode()
    {
        route.gameObject.SetActive(true);
        menu.Hide();
        zoomCam.SetActive(false);
        flying = true;
    }

    public void Hunt()
    {
        huntCam.SetActive(true);
        menu.Hide();
        this.StartCoroutine(() =>
        {
            menu.Show(current, inCapital, isMenuFlipped);
            huntCam.SetActive(false);
        }, 2f);
    }

    private void UpdateRoute()
    {
        if (!flying) return;
        var mp = cam.ScreenToWorldPoint(Input.mousePosition).WhereZ(0);
        route.SetPosition(0, hunter.transform.position);
        route.SetPosition(1, mp);
    }
    
    private void SetTarget()
    {
        if (!flying) return;
        var mp = cam.ScreenToWorldPoint(Input.mousePosition).WhereZ(0);
        var hit = Physics2D.OverlapCircleAll(mp, 0.1f, countryMask);
        if (!hit.Any()) return;
        flying = false;
        var distance = Vector3.Distance(mp, hunter.transform.position);
        var delay = 0.3f * distance;
        Tweener.MoveToQuad(hunter.transform, mp, delay);
        var landed = hit.Select(h => h.GetComponent<Country>()).Where(c => c != null).ToList();
        var closest = landed.OrderBy(c => Vector3.Distance(mp, c.CapitalPosition)).First();
        
        zoomCam.SetActive(true);
        
        if (current)
        {
            current.Hide();
        }

        inCapital = Vector3.Distance(mp, closest.CapitalPosition) < 0.5f;
        var flipped = inCapital && mp.x - closest.CapitalPosition.x > 0;
        this.StartCoroutine(() =>
        {
            ShowMenu(inCapital ? closest.CapitalPosition : hunter.transform.position, closest, flipped);
            route.gameObject.SetActive(false);
        }, delay);
        
        if (inCapital)
        {
            this.StartCoroutine(() => closest.Show(), delay);
            current = closest;
        }
    }

    private void ShowMenu(Vector3 pos, Country country = null, bool flip = false)
    {
        isMenuFlipped = flip;
        menu.transform.position = pos;
        menu.Show(country, inCapital, flip);
    }
}