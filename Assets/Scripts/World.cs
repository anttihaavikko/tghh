using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Utils;
using TMPro;
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
    [SerializeField] private Transform physicsDecorations;
    [SerializeField] private ButtonStyle cancelButton;
    [SerializeField] private TMP_Text fuelDisplay;
    [SerializeField] private Transform fuelBar, fuelPreview;

    private Country current;
    private bool flying;
    private bool isMenuFlipped;
    private bool inCapital;
    private int level;
    private bool wasBookShown;
    private bool started;

    private int fuel;
    private int tank;

    private const int TankSize = 250;

    private void Start()
    {
        tank = fuel = TankSize;
        UpdateFuel();
        
        current = countries.Random();
        current.Show();
        hunter.transform.position = current.CapitalPosition;
        hunter.Hop();
        this.StartCoroutine(() => physicsDecorations.SetParent(null), 0.25f);
        
        book.Init(countries, level);
        
        hunter.Bubble.Show("Time to do some (hunting)! Where did I put that (notebook) of mine...");
    }

    private void UpdateFuel(float duration = 0.3f)
    {
        var ratio = 1f * fuel / tank;
        var size = new Vector3(ratio, 1, 1);
        Tweener.ScaleToQuad(fuelPreview, size, duration);
        Tweener.ScaleToQuad(fuelBar, size, duration);
        // this.StartCoroutine(() => Tweener.ScaleToQuad(fuelBar, size, 0.3f), duration);
        fuelDisplay.text = $"Fuel: {fuel}/{tank} liters";
    }

    private void PreviewFuelLoss(float distance)
    {
        var preview = Mathf.Max(0, fuel - GetFuelConsumption(distance));
        var ratio = 1f * preview / tank;
        fuelPreview.localScale = new Vector3(ratio, 1, 1);
    }

    private int GetFuelConsumption(float distance)
    {
        return Mathf.RoundToInt(distance * 10) + 20;
    }

    private void ConsumeFuel(float distance, float duration)
    {
        fuel = Mathf.Max(0, fuel - GetFuelConsumption(distance));
        UpdateFuel(duration);
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

        if (Input.anyKeyDown && !started)
        {
            started = true;
            var letters = book.FirstTaskLetters;
            hunter.Read(true);
            hunter.Bubble.Show($"I should start with that (tracking task) first. Gotta fly to a (country) with ({letters[0]} and {letters[1]}) in the (name) to do it.");
            this.StartCoroutine(() => book.Show(), 0.4f);
            this.StartCoroutine(() => ShowMenu(hunter.transform.position), 0.8f);
        }
    }

    public void Refuel()
    {
        menu.HideButton(1);
        fuel = tank;
        UpdateFuel();
    }

    public void CancelFlyMode()
    {
        info.Hide();
        cancelButton.Reset();
        cancelButton.gameObject.SetActive(false);
        flying = false;
        if(wasBookShown) book.Show();
        route.gameObject.SetActive(false);
        menu.Show(book, current, inCapital, isMenuFlipped);
        zoomCam.SetActive(true);
    }

    public void FlyMode()
    {
        hunter.Read(false);
        cancelButton.gameObject.SetActive(true);
        wasBookShown = book.IsShown;
        if(wasBookShown) book.Hide();
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
        hunter.Read(true);
        
        huntCam.SetActive(true);
        menu.Hide();
        info.Hide();

        var success = TryComplete(TaskType.Track, 5);
        hunter.HopAround(4, success);
        
        this.StartCoroutine(() =>
        {
            if (success)
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
        hunter.Read(false);
        huntCam.SetActive(true);
        menu.Hide();
        info.Hide();
        
        var success = book.CanHunt(current);
        hunter.HopAround(4, success);
        
        this.StartCoroutine(() =>
        {
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
        var distance = Vector3.Distance(mp, hunter.transform.position);
        PreviewFuelLoss(distance);
    }
    
    private void SetTarget()
    {
        if (!flying || cancelButton.IsHovered) return;
        cancelButton.gameObject.SetActive(false);
        info.Hide();
        var mp = cam.ScreenToWorldPoint(Input.mousePosition).WhereZ(0);
        var hit = Physics2D.OverlapCircleAll(mp, 0.1f, countryMask);
        if (!hit.Any()) return;
        flying = false;
        var distance = Vector3.Distance(mp, hunter.transform.position);
        var delay = Mathf.Max(1.2f, 0.3f * distance);
        ConsumeFuel(distance, delay);
        Tweener.MoveToQuad(hunter.transform, mp, delay);
        var landed = hit.Select(h => h.GetComponent<Country>()).Where(c => c != null).ToList();
        var closest = landed.OrderBy(c => Vector3.Distance(mp, c.CapitalPosition)).First();
        
        zoomCam.SetActive(true);
        hunter.Lift(mp);
        
        hunter.ScaleUp(delay * 0.3f);
        this.StartCoroutine(() => hunter.ScaleDown(delay * 0.3f), delay * 0.7f);
        
        if (current)
        {
            current.Hide();
        }

        inCapital = Vector3.Distance(mp, closest.CapitalPosition) < 0.5f;
        var flipped = inCapital && mp.x - closest.CapitalPosition.x > 0;
        this.StartCoroutine(() =>
        {
            if(wasBookShown) book.Show();
            hunter.Land();
            
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