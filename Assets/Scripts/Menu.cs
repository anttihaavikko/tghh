using AnttiStarterKit.Animations;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] private Appearer appearer;
    [SerializeField] private Transform flipper, panel;

    public void Show(bool flip)
    {
        flipper.localScale = panel.localScale = new Vector3(flip ? -1 : 1, 1, 1);
        appearer.Show();
    }

    public void Hide()
    {
        appearer.Hide();
    }
}