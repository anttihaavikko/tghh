using AnttiStarterKit.Animations;
using TMPro;
using UnityEngine;

public class ActionButton : MonoBehaviour
{
    [SerializeField] private ButtonStyle buttonStyle;
    [SerializeField] private TMP_Text price;

    public void Reset()
    {
        buttonStyle.Reset();
    }

    public void SetPrice(int amount)
    {
        price.text = $"{amount} €";
    }

    public void SetSecondaryLine(string text)
    {
        price.text = text;
    }
}