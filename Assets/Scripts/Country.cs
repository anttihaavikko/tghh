using System;
using AnttiStarterKit.Animations;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class Country : MonoBehaviour
{
    [SerializeField] private string capitalName;
    [SerializeField] private Transform capital;
    [SerializeField] private TMP_Text capitalNameField, capitalNameOutline;
    [SerializeField] private Appearer capitalText, capitalTap;
    [SerializeField] private float priceModifier = 1f;
    
    public string CapitalName => capitalName;
    public Vector3 CapitalPosition => capital.position;

    public bool Visited { get; private set; }
    public float PriceModifier => priceModifier;

    private void Start()
    {
        capitalNameField.text = capitalNameOutline.text = capitalName;
    }

    public void Show()
    {
        Visited = true;
        capitalText.ShowAfter(0.2f);
        capitalTap.Show();
    }

    public void Hide()
    {
        capitalText.Hide();
        capitalTap.HideWithDelay();
    }
}