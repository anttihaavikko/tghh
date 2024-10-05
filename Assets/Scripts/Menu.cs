using System.Collections.Generic;
using AnttiStarterKit.Animations;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] private Appearer appearer;
    [SerializeField] private Transform flipper, panel;
    [SerializeField] private List<ActionButton> buttons;
    
    public void Show(Book book, Country country, bool inCapital, bool flip)
    {
        buttons[2].gameObject.SetActive(false); // bribe
        buttons[3].gameObject.SetActive(book.HasUncompleted(TaskType.Trap));
        buttons[4].gameObject.SetActive(false); // sell
        buttons[5].gameObject.SetActive(book.HasUncompleted(TaskType.Track));
        
        // flipper.localScale = panel.localScale = new Vector3(flip ? -1 : 1, 1, 1);
        appearer.Show();
        // Debug.Log(inCapital ? $"In {country.CapitalName}, {country.name}" : $"In {country.name}");
    }

    public void Hide()
    {
        appearer.Hide();
    }
}