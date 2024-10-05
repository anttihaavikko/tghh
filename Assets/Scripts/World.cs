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
    [SerializeField] private Book book;
    [SerializeField] private Appearer info;

    private Country current;
    private bool flying;
    private bool isMenuFlipped;
    private bool inCapital;
    private int level;

    private void Start()
    {
        current = countries.Random();
        current.Show();
        hunter.transform.position = current.CapitalPosition;
        
        book.Init(countries, level);
        ShowMenu(hunter.transform.position);
        book.Show();
    }

    private void NextLevel()
    {
        level++;
        book.Init(countries, level);
        book.Show();
        ShowMenu(hunter.transform.position);
    }

    private void Update()
    {
        UpdateRoute();
        if (Input.GetMouseButtonDown(0)) SetTarget();
        if (DevKey.Down(KeyCode.F)) FlyMode();
        if (DevKey.Down(KeyCode.N)) NextLevel();
    }

    public void FlyMode()
    {
        info.ShowWithText("Pick flight target!", 0);
        route.gameObject.SetActive(true);
        menu.Hide();
        zoomCam.SetActive(false);
        flying = true;
    }

    public void BuyTrap()
    {
        TryComplete(TaskType.Trap, 3);
    }

    public void FindTrack()
    {
        huntCam.SetActive(true);
        menu.Hide();
        info.Hide();
        
        this.StartCoroutine(() =>
        {
            if (TryComplete(TaskType.Track, 5))
            {
                info.ShowWithText("Found the track!", 0);    
            }
            
            this.StartCoroutine(() =>
            {
                info.Hide();
                menu.Show(book, current, inCapital, isMenuFlipped);
                huntCam.SetActive(false);
            }, 1.5f);
        }, 2f);
    }

    private bool TryComplete(TaskType type, int buttonIndex)
    {
        if (book.CanComplete(type, current))
        {
            book.Complete(type);
            menu.HideButton(buttonIndex);
            return true;
        }

        return false;
    }

    public void Hunt()
    {
        huntCam.SetActive(true);
        menu.Hide();
        info.Hide();
        
        this.StartCoroutine(() =>
        {
            var success = book.CanHunt(current);
            if (success)
            {
                book.Complete(TaskType.Hunt);
                this.StartCoroutine(() => book.Hide(), 0.7f);
                this.StartCoroutine(NextLevel, 1.2f);
            }
            info.ShowWithText(success ? "Successfully hunted X!" : "You didn't find anything...", 0);
            this.StartCoroutine(() =>
            {
                info.Hide();
                menu.Show(book, current, inCapital, isMenuFlipped);
                huntCam.SetActive(false);
            }, 1.5f);
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
        info.Hide();
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
            route.gameObject.SetActive(false);
            info.ShowWithText(inCapital || closest.Visited ? $"Landed in {closest.name.ToUpper()}!" : "Landed in some UNKNOWN LAND...\n<size=50>No cities nearby...</size>", 0.3f);
            this.StartCoroutine(() => ShowMenu(inCapital ? closest.CapitalPosition : hunter.transform.position, closest, flipped), 0.5f);
        }, delay);
        
        if (inCapital)
        {
            this.StartCoroutine(() => closest.Show(), delay);
        }
        
        current = closest;
    }

    private void ShowMenu(Vector3 pos, Country country = null, bool flip = false)
    {
        isMenuFlipped = flip;
        menu.transform.position = pos;
        menu.Show(book, country, inCapital, flip);
    }
}