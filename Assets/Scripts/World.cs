using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Game;
using AnttiStarterKit.Managers;
using AnttiStarterKit.ScriptableObjects;
using AnttiStarterKit.Utils;
using AnttiStarterKit.Visuals;
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
    [SerializeField] private TMP_Text moneyDisplay;
    [SerializeField] private Pulsater moneyPulsater;
    [SerializeField] private EffectCamera effectCamera;
    [SerializeField] private TMP_Text timeDisplay;
    [SerializeField] private GameObject endCam;
    [SerializeField] private SoundComposition buySound;
    [SerializeField] private Transform lootContainer;
    [SerializeField] private ButtonStyle sellButtonPrefab;
    [SerializeField] private GameObject stopSellingButton;
    [SerializeField] private GameObject sellCam;
    [SerializeField] private ScoreDisplay score;
    [SerializeField] private GameObject gameOver;

    private Country current;
    private bool flying;
    private bool isMenuFlipped;
    private bool inCapital;
    private int level;
    private bool wasBookShown;
    private bool started;

    private int fuel;
    private int tank;

    private int money = 500;
    private TimeSpan time = new TimeSpan(0, 6, 0, 0);
    private List<HuntTarget> targetList;
    private bool warnedAboutFuel;
    private bool toldAboutHunt;
    private readonly List<HuntReward> loot = new();
    private bool selling;

    public int CityRange { get; set; } = 1;

    private const int TankSize = 250;

    private void Start()
    {
        targetList = new List<HuntTarget>
        {
            new() { Name = "Wolbachia", Short = "Wolbachia", Description = "This tiny parasitic creature needs to be eradicated. Be careful not to become the host." },
            new() { Name = "Yersinia", Short = "Yersinia", Description = "Filthy disease spreader this one! Get rid of it quick or we might soon have another plague in our hands." },
            new() { Name = "Vibrio Vulnificus", Short = "V.Vulnificus", Description = "The increased sea temperatures have increased the population of these tiny pests. Time to thin it out a bit!" },
            new() { Name = "Spirillum Volutans", Short = "S.Volutans", Description = "This little nuisance likes to dwell in freshwater. Get rid of it before it infects our drink supply." },
            new() { Name = "Rickettsia Quintana", Short = "R.Quintana", Description = "They lurk in cooties! Time to hunt it down, we don't want another trench fever outbreak!" },
            new() { Name = "Proteus Vulgaris", Short = "P.Vulgaris", Description = "Common and dangerous! Often causes wounds to become infected. Might be closer than you think." },
            new() { Name = "Nocardia Asteroides", Short = "N.Asteroides", Description = "Not that dangerous to most people but can cause some major breathing issues to some." },
            new() { Name = "Mycoplasma Hominis", Short = "M.Hominis", Description = "A nasty one this little bugger! It can literally phase inside you and even cause infertility." },
            new() { Name = "Leptospira Interrogans", Short = "L.Interrogans", Description = "Don't let the cute corkscrew look fool you! Often a threat to our pets but can be dangerous to humans too." },
            new() { Name = "Kingella Kingae", Short = "K.Kingae", Description = "What a faux royalty! This tiny pest is a major nuisance and a cause of several different kinds of infections and diseases." },
            new() { Name = "Gardnerella Vaginalis", Short = "G.Vaginalis", Description = "The stinky one! Just follow the fish-like scent and you'll find it in no time. It's still quite dangerous." },
            new() { Name = "Ehrlichia Chaffeensis", Short = "E.Chaffeensis", Description = "It is hosting in ticks and leeches and can cause various different issues even affecting the mind." },
            new() { Name = "Campylobacter Fetus", Short = "C.Fetus", Description = "This tiny critter prays on cattle. And by extension, it's also a threat to us humans. Protect the cows and get rid of it!" },
            new() { Name = "Borrelia Burgdorferi", Short = "B.Burgdorferi", Description = "Cute name but a nasty little bastard! It spreads lyme disease and other other equally severe diseases." }
        }.RandomOrder().ToList();

        AudioManager.Instance.TargetPitch = 1f;
        
        tank = fuel = TankSize;
        UpdateFuel();
        
        UpdateMoney();
        UpdateTime();
        
        current = countries.Random();
        current.Show();
        hunter.transform.position = current.CapitalPosition;
        hunter.Hop();
        this.StartCoroutine(() => physicsDecorations.SetParent(null), 0.25f);
        
        book.Init(countries, targetList, level);

        this.StartCoroutine(() =>
        {
            hunter.Bubble.Show("Time to do some (hunting)! Where did I put that (notebook) of mine...");
        }, 1f);
    }

    public void SellMode()
    {
        selling = true;
        menu.Hide();
        hunter.Bubble.Show("Hmm... time to turn (these pelts) of mine to some (hard currency). Which pelts should I (sell) here?");
        sellCam.SetActive(true);
        stopSellingButton.SetActive(true);
        hunter.Read(true);
    }

    public void StopSelling()
    {
        selling = false;
        sellCam.SetActive(false);
        stopSellingButton.SetActive(false);
        ShowMenuAgain();
        hunter.Read(false);
    }

    public void Sell(HuntReward reward)
    {
        if (!selling) return;
        PlayBuySound();
        UpdateMoney(reward.GetSellPrice(current));
        loot.Remove(reward);
        Destroy(reward.Button.gameObject);
        if(!loot.Any()) StopSelling();
    }

    private void UpdateTime()
    {
        timeDisplay.text = $"Day {time.Days + 1}, <size=20>{time.Hours}:{time.Minutes:D2}</size>";
    }

    private void PassTime(TimeSpan amount, float duration)
    {
        StartCoroutine(PassTimeCoroutine(amount, duration));
    }

    private IEnumerator PassTimeCoroutine(TimeSpan amount, float duration)
    {
        var steps = duration * 20;
        for (var i = 0; i < steps; i++)
        {
            time += amount / steps;
            UpdateTime();
            yield return new WaitForSeconds(0.05f);
        }
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

    private void UpdateMoney(int change = 0)
    {
        money += change;
        moneyDisplay.text = $"{money} <size=35>€</size>";
        if(change != 0) moneyPulsater.Pulsate();
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

    private Vector3 AdjustDistance(Vector3 dir)
    {
        var consumption = GetFuelConsumption(dir.magnitude);
        return fuel >= consumption ? dir : dir * fuel / consumption;
    }

    private void NextLevel()
    {
        level++;
        book.Init(countries, targetList, level);
        book.Show();
        ShowMenu(hunter.transform.position);
    }

    private void Update()
    {
        UpdateRoute();
        if (Input.GetMouseButtonDown(0)) SetTarget();
        if (DevKey.Down(KeyCode.F)) FlyMode();
        if (DevKey.Down(KeyCode.N))
        {
            AddLoot(book.Reward);
            NextLevel();
        }
        if (DevKey.Down(KeyCode.T)) PassTime(TimeSpan.FromHours(12), 0.2f);
        if (DevKey.Down(KeyCode.Z)) UpdateMoney(100);
        if (DevKey.Down(KeyCode.S)) score.Add(100);

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
        if (money < current.FuelPrice)
        {
            effectCamera.BaseEffect(0.2f);
            hunter.Bubble.Show("I don't have (enough funds) to do that...");
            AudioManager.Instance.TargetPitch = 0.8f;
            PlayNoAfford();
            return;
        }
        
        PlayBuySound();
        AudioManager.Instance.TargetPitch = 1f;
        UpdateMoney(-current.FuelPrice);
        
        menu.HideButton(1);
        fuel = tank;
        UpdateFuel();
    }

    private void PlayNoAfford()
    {
        AudioManager.Instance.PlayEffectAt(7, transform.position, 2f);
    }

    public void CancelFlyMode()
    {
        AudioManager.Instance.Highpass(false);
        info.Hide();
        cancelButton.Reset();
        cancelButton.gameObject.SetActive(false);
        flying = false;
        if(wasBookShown) book.Show();
        route.gameObject.SetActive(false);
        ShowMenuAgain();
        zoomCam.SetActive(true);
        UpdateFuel(0.2f);
    }

    private void ShowMenuAgain()
    {
        menu.Show(book, current, inCapital, isMenuFlipped, loot.Any());
    }

    public void FlyMode()
    {
        AudioManager.Instance.Highpass();
        hunter.Read(false);
        cancelButton.gameObject.SetActive(true);
        wasBookShown = book.IsShown;
        if(wasBookShown) book.Hide();
        ShowInfo("Pick (flight) target!");
        route.gameObject.SetActive(true);
        menu.Hide();
        zoomCam.SetActive(false);
        flying = true;
    }
    
    public void Bribe()
    {
        if (money < current.BribePrice)
        {
            effectCamera.BaseEffect(0.2f);
            hunter.Bubble.Show("I don't have (enough funds) to do that...");
            PlayNoAfford();
            return;
        }
        
        PlayBuySound();
        UpdateMoney(-current.BribePrice);
        current.Bribe(this);
        menu.HideButton(2);
        ShowMenuAgain();
    }

    public void BuyTrap()
    {
        if (money < current.TrapPrice)
        {
            effectCamera.BaseEffect(0.2f);
            hunter.Bubble.Show("I don't have (enough funds) to do that...");
            PlayNoAfford();
            return;
        }
        
        PlayBuySound();
        UpdateMoney(-current.TrapPrice);
        TryComplete(TaskType.Trap, 3);
    }

    private void PlayBuySound()
    {
        buySound.Play(transform.position, 2f);
    }

    public void FindTrack()
    {
        PassTime(TimeSpan.FromMinutes(30), 3f);
        hunter.Read(true);
        
        huntCam.SetActive(true);
        menu.Hide();
        info.Hide();

        var success = TryComplete(TaskType.Track, 5, 2.75f);
        hunter.HopAround(4, success);
        
        this.StartCoroutine(() =>
        {
            if (success)
            {
                ShowInfo($"Found the track of ({book.TargetName})!", 0.5f);
            }
            
            this.StartCoroutine(() =>
            {
                info.Hide();
                menu.Show(book, current, inCapital, isMenuFlipped, loot.Any());
                huntCam.SetActive(false);

                if (success && !toldAboutHunt)
                {
                    toldAboutHunt = true;
                    hunter.Bubble.Show($"Right, now I gotta go after the (main target) of the hunt, ({book.TargetName})!");
                }
                
            }, 1.5f);
        }, 2f);
    }

    private void ShowInfo(string message, float delay = 0)
    {
        var sb = new StringBuilder(message);
        sb.Replace("(", "<color=#FFFD98>");
        sb.Replace(")", "</color>");
        info.ShowWithText(sb.ToString(), delay);
    }

    private bool TryComplete(TaskType type, int buttonIndex, float delay = 0f)
    {
        if (book.CanComplete(type, current))
        {
            this.StartCoroutine(() =>
            {
                score.Add((level + 1) * money);
                book.Complete(type);
                menu.HideButton(buttonIndex);
            }, delay);
            return true;
        }

        return false;
    }

    public void Hunt()
    {
        PassTime(TimeSpan.FromHours(4), 3f);
        
        hunter.Read(false);
        huntCam.SetActive(true);
        menu.Hide();
        info.Hide();
        
        var success = book.CanHunt(current);
        hunter.HopAround(4, success);
        
        this.StartCoroutine(() =>
        {
            var reward = book.Reward;
            
            if (success)
            {
                score.Add((level + 1) * 5 * money);
                book.Complete(TaskType.Hunt);
                this.StartCoroutine(() => book.Hide(), 0.7f);
                this.StartCoroutine(NextLevel, 1.2f);
            }
            ShowInfo(success ? $"Successfully hunted ({book.TargetName})!" : "You didn't find anything...", 0.5f);

            if (success)
            {
                this.StartCoroutine(() =>
                {
                    hunter.Bubble.Show("Here we go, (next target)! I should probably also (sell) this (pelt) somewhere...");
                    AddLoot(reward);
                }, 1f);
            }
            
            this.StartCoroutine(() =>
            {
                info.Hide();
                menu.Show(book, current, inCapital, isMenuFlipped, loot.Any());
                huntCam.SetActive(false);
            }, 1.5f);
        }, 2f);
    }

    private void AddLoot(HuntReward reward)
    {
        reward.Button = Instantiate(sellButtonPrefab, lootContainer);
        reward.Button.onClick += () => Sell(reward);
        reward.Button.onHoverIn += () => hunter.Bubble.Show(reward.Description, true);
        loot.Add(reward);
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
        var dir = AdjustDistance(mp - hunter.transform.position);
        var distance = dir.magnitude;
        var delay = Mathf.Max(1.2f, 0.3f * distance);
        var pos = hunter.transform.position + dir;
        
        var hit = Physics2D.OverlapCircleAll(pos, 0.1f, countryMask);
        if (!hit.Any()) return;
        flying = false;
        
        PassTime(TimeSpan.FromHours(distance), delay);
        
        ConsumeFuel(distance, delay);
        Tweener.MoveToQuad(hunter.transform, pos, delay);
        var landed = hit.Select(h => h.GetComponent<Country>()).Where(c => c != null).ToList();
        var closest = landed.OrderBy(c => Vector3.Distance(pos, c.CapitalPosition)).First();
        
        zoomCam.SetActive(true);
        hunter.Lift(pos);
        
        hunter.ScaleUp(delay * 0.3f);
        this.StartCoroutine(() => hunter.ScaleDown(delay * 0.3f), delay * 0.7f);
        
        if (current)
        {
            current.Hide();
        }

        inCapital = Vector3.Distance(pos, closest.CapitalPosition) < 0.5f * CityRange;
        var flipped = inCapital && pos.x - closest.CapitalPosition.x > 0;
        this.StartCoroutine(() =>
        {
            if(wasBookShown) book.Show();
            hunter.Land();
            
            route.gameObject.SetActive(false);
            this.StartCoroutine(() => PostLanding(closest, flipped), 0.5f);
        }, delay);
        
        if (inCapital)
        {
            this.StartCoroutine(() => closest.Show(), delay);
        }
        
        current = closest;
    }

    private void PostLanding(Country country, bool flipped)
    {
        if ((!inCapital || money < country.FuelPrice) && fuel <= 0)
        {
            hunter.Bubble.Show("Oh no, I've run (out of fuel)! I think my (hunting) holiday (week) must be (cut short)...");
            AudioManager.Instance.TargetPitch = 0f;
            Invoke(nameof(GameOver), 2f);
            return;
        }
        
        if (time > new TimeSpan(7, 0, 0, 0))
        {
            hunter.Bubble.Show("How the (time flies)! It's already (been a week), and I need to get (back to work) from my (hunting holiday)...");
            AudioManager.Instance.TargetPitch = 0f;
            Invoke(nameof(GameOver), 2f);
            return;
        }
        
        if (fuel < tank * 0.5f)
        {
            if (!warnedAboutFuel)
            {
                warnedAboutFuel = true;
                hunter.Bubble.Show("I should probably consider (refueling) the (plane) pretty soon. Don't want to end up (stranded)...");   
            }
            AudioManager.Instance.TargetPitch = 0.9f;
        }
        
        ShowInfo(inCapital || country.Visited ? $"Landed in ({country.name.ToUpper()})!" : "Landed in some (UNKNOWN LAND)...\n<size=50>No (cities) nearby...</size>");
        ShowMenu(hunter.transform.position, country, flipped);
    }

    private void GameOver()
    {
        book.Hide();
        endCam.SetActive(true);
        gameOver.SetActive(true);
    }

    private void ShowMenu(Vector3 pos, Country country = null, bool flip = false)
    {
        isMenuFlipped = flip;
        menu.transform.position = pos;
        menu.Show(book, country, inCapital, flip, loot.Any());
    }

    public void RevealCitiesAround(Country country, float radius)
    {
        countries.Where(c => Vector3.Distance(c.CapitalPosition, country.CapitalPosition) < radius).ToList().ForEach(c => c.RevealCapital());
    }

    public void IncreaseTank(int amount)
    {
        tank += amount;
        UpdateFuel(0.2f);
    }

    public void IncreaseMultiplier()
    {
        score.AddMulti();
    }
}