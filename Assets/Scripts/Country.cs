using System;
using System.Collections.Generic;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class Bribe
{
    public string Description { get; set; }
    public Action<World, Country> Effect { get; set; }
}

public class Country : MonoBehaviour
{
    [SerializeField] private string capitalName;
    [SerializeField] private string nationality;
    [SerializeField] private Transform capital;
    [SerializeField] private TMP_Text capitalNameField, capitalNameOutline;
    [SerializeField] private Appearer capitalText, capitalTap;
    [SerializeField] private float priceModifier = 1f;
    
    public string CapitalName => capitalName;
    public Vector3 CapitalPosition => capital.position;
    public string BribeEffect => bribe.Description;

    private Bribe bribe;
    private int fuelPriceMod = 1;
    private bool revealed;
    private int sellPriceMod = 1;

    public bool Visited { get; private set; }
    public bool IsBribed { get; private set; }
    public bool HasTraps { get; set; }

    public int FuelPrice => Mathf.CeilToInt(priceModifier * 100) * fuelPriceMod;
    public int TrapPrice => Mathf.CeilToInt(priceModifier * 20);
    public int BribePrice => Mathf.RoundToInt(priceModifier * 250);
    public int PriceModifier => Mathf.CeilToInt(priceModifier * 100) * sellPriceMod;

    private void Start()
    {
        bribe = GetRandomBribe();
        capitalNameField.text = capitalNameOutline.text = capitalName;
    }

    private Bribe GetRandomBribe()
    {
        return new List<Bribe>
        {
            new()
            {
                Description = $"(The {nationality} Embassy) increases the (range) where you can access (cities) after landing close by.",
                Effect = (world, country) => world.CityRange++
            },
            new()
            {
                Description = $"(The {nationality} Embassy) allows you to (refuel) in ({CapitalName}) for (free) of charge.",
                Effect = (world, country) => country.fuelPriceMod = 0
            },
            new()
            {
                Description = $"Bribing (The {nationality} Embassy) reveals all other cities in a (1000 km) radius.",
                Effect = (world, country) => world.RevealCitiesAround(country, 2f)
            },
            new()
            {
                Description = $"(The {nationality} Embassy) has a great (fence). They will (buy) all your (pelts) for (double) the value.",
                Effect = (world, country) => country.sellPriceMod = 2
            },
            new()
            {
                Description = $"Bribing (The {nationality} Embassy) will increase your (plane tank) size with additional (150 liters).",
                Effect = (world, country) => world.IncreaseTank(150)
            },
            new()
            {
                Description = $"(The {nationality} Embassy) will (double) all your future (score gains).",
                Effect = (world, country) => world.IncreaseMultiplier()
            },
            new()
            {
                Description = $"(The {nationality} Embassy) can always sell you a (correct trap), no matter the (task) in hand.",
                Effect = (world, country) => country.HasTraps = true
            }
        }.Random();
    }

    public void Bribe(World word)
    {
        if (IsBribed) return;
        IsBribed = true;
        bribe.Effect(word, this);
    }

    public void Show()
    {
        Visited = true;
        capitalText.ShowAfter(0.2f);
        capitalTap.Show();
    }

    public void Hide()
    {
        if (IsBribed || revealed) return;
        capitalText.Hide();
        capitalTap.HideWithDelay();
    }

    public void RevealCapital()
    {
        revealed = true;
        Show();
    }
}