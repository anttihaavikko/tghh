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

    public bool Visited { get; private set; }
    public bool IsBribed { get; private set; }
    

    public int FuelPrice => Mathf.CeilToInt(priceModifier * 100);
    public int TrapPrice => Mathf.CeilToInt(priceModifier * 20);
    public int BribePrice => Mathf.RoundToInt(priceModifier * 250);
    public int PriceModifier => Mathf.CeilToInt(priceModifier * 100);

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
        if (IsBribed) return;
        capitalText.Hide();
        capitalTap.HideWithDelay();
    }
}