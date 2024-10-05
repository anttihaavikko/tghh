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
    
    public string CapitalName => capitalName;
    public Vector3 CapitalPosition => capital.position;

    private void Start()
    {
        capitalNameField.text = capitalNameOutline.text = capitalName;
    }

    public void Show()
    {
        capitalText.ShowAfter(0.2f);
        capitalTap.Show();
    }

    public void Hide()
    {
        capitalText.Hide();
        capitalTap.HideWithDelay();
    }
}