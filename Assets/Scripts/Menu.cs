using System;
using System.Collections.Generic;
using AnttiStarterKit.Animations;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] private Appearer appearer;
    [SerializeField] private Transform flipper, panel;
    [SerializeField] private List<ActionButton> buttons;
    
    public void Show(Book book, Country country, bool inCapital, bool flip, bool canSell)
    {
        if (country != null)
        {
            buttons[1].SetPrice(country.FuelPrice);
            buttons[3].SetPrice(country.TrapPrice);
            buttons[4].SetSecondaryLine($"{country.FuelPrice}% sell value");
        }
        buttons[1].gameObject.SetActive(inCapital);
        buttons[2].gameObject.SetActive(false); // bribe
        buttons[3].gameObject.SetActive(inCapital && book.HasUncompleted(TaskType.Trap) && country != null && book.CanComplete(TaskType.Trap, country));
        buttons[4].gameObject.SetActive(canSell); // sell
        buttons[5].gameObject.SetActive(book.HasUncompleted(TaskType.Track));
        
        // flipper.localScale = panel.localScale = new Vector3(flip ? -1 : 1, 1, 1);
        appearer.Show();
    }

    public void HideButton(int index)
    {
        buttons[index].Reset();
        buttons[index].gameObject.SetActive(false);
    }

    public void Hide()
    {
        appearer.Hide();
    }
}