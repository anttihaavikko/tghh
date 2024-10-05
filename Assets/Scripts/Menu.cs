using AnttiStarterKit.Animations;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] private Appearer appearer;
    [SerializeField] private Transform flipper, panel;

    public void Show(Country country, bool inCapital, bool flip)
    {
        flipper.localScale = panel.localScale = new Vector3(flip ? -1 : 1, 1, 1);
        appearer.Show();
        Debug.Log(inCapital ? $"In {country.CapitalName}, {country.name}" : $"In {country.name}");
    }

    public void Hide()
    {
        appearer.Hide();
    }
}