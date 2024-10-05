using System;
using AnttiStarterKit.Animations;
using TMPro;
using UnityEngine;

public class Country : MonoBehaviour
{
    [SerializeField] private string capitalName;
    [SerializeField] private Transform capital;
    [SerializeField] private TMP_Text capitalNameField, capitalNameOutline;
    [SerializeField] private Appearer capitalVisuals;
    
    public string CapitalName => capitalName;
    public Vector3 CapitalPosition => capital.position;

    private void Start()
    {
        capitalNameField.text = capitalNameOutline.text = capitalName;
    }

    public void Show()
    {
        capitalVisuals.Show();
    }

    public void Hide()
    {
        capitalVisuals.Hide();
    }
}