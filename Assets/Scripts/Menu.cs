using System;
using System.Collections.Generic;
using AnttiStarterKit.Animations;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] private Appearer appearer;
    [SerializeField] private Transform flipper, panel;
    [SerializeField] private List<ActionButton> buttons;
    [SerializeField] private Hunter hunter;
    
    public void Show(Book book, Country country, bool inCapital, bool flip, bool canSell, bool canFly, bool needsFuel)
    {
        if (country != null)
        {
            buttons[1].SetPrice(country.FuelPrice);
            buttons[2].SetPrice(country.BribePrice);
            buttons[2].Button.onHoverIn = () => hunter.Bubble.Show(country.BribeEffect, true);
            buttons[3].SetPrice(country.TrapPrice);
            buttons[4].SetSecondaryLine($"{country.PriceModifier}% sell value");
        }
        buttons[0].gameObject.SetActive(canFly);
        buttons[1].gameObject.SetActive(inCapital && needsFuel);
        buttons[2].gameObject.SetActive(inCapital && country != null && !country.IsBribed); // bribe
        buttons[3].gameObject.SetActive(inCapital && book.HasUncompleted(TaskType.Trap) && country != null && (country.HasTraps || book.CanComplete(TaskType.Trap, country)));
        buttons[4].gameObject.SetActive(inCapital && canSell); // sell
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