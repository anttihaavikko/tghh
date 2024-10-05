using AnttiStarterKit.Animations;
using TMPro;
using UnityEngine;

public class TaskField : MonoBehaviour
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private Appearer check;
    
    public void Init(HuntTask huntTask)
    {
        title.text = huntTask.Title;
        huntTask.Field = this;
    }

    public void Mark()
    {
        check.Show();
    }

    public void Reset()
    {
        check.Hide();
    }
}