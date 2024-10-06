using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Managers;
using TMPro;
using UnityEngine;

public class Book : MonoBehaviour
{
    [SerializeField] private List<TaskField> tasks;
    [SerializeField] private Transform onPosition, offPosition;
    [SerializeField] private TMP_Text reminder;
    [SerializeField] private Transform arrow;
    [SerializeField] private TMP_Text title, description;
    [SerializeField] private Hunter hunter;
    
    private Hunt hunt;

    public bool IsShown { get; private set; }
    public string FirstTaskLetters => hunt.Tasks[0].CountryLetters;
    public string TargetName => hunt.Target.Short.ToUpper();

    public HuntReward Reward => hunt.Reward;

    private void Start()
    {
        transform.position = offPosition.position;
    }

    public void Init(List<Country> countries, List<HuntTarget> targetList, int level)
    {
        hunt = new Hunt(countries, targetList, level);
        tasks.ForEach(t =>
        {
            t.Reset();
            var index = tasks.IndexOf(t);
            var exists = hunt.Tasks.Count > index;
            t.gameObject.SetActive(exists);
            if (exists)
            {
                t.Init(hunt.Tasks[index]);
            }
        });

        title.text = hunt.Target.Name;
        description.text = hunt.Target.Description;
        
        UpdateReminder();
    }

    public bool CanHunt(Country country)
    {
        return hunt.Tasks.All(t => t.IsDone || t.Is(TaskType.Hunt) && t.FilledBy(country));
    }

    public void Complete(TaskType taskType)
    {
        hunt.Tasks.Where(t => t.Is(taskType)).ToList().ForEach(t => t.Complete());
        UpdateReminder();
    }

    public bool CanComplete(TaskType type, Country country)
    {
        var match = hunt.Tasks.Find(t => t.Is(type));
        return match != default && match.FilledBy(country);
    }

    public bool HasUncompleted(TaskType type)
    {
        return hunt.Tasks.Any(t => t.Is(type) && !t.IsDone);
    }

    public void Show()
    {
        ToggleSound();
        IsShown = true;
        Tweener.MoveToBounceOut(transform, onPosition.position, 0.2f);
        reminder.gameObject.SetActive(false);
        arrow.localScale = new Vector3(1, 1, 1);
    }

    public void Hide()
    {
        ToggleSound();
        IsShown = false;
        Tweener.MoveToBounceOut(transform, offPosition.position, 0.2f);
        this.StartCoroutine(() => reminder.gameObject.SetActive(true), 0.15f);
        arrow.localScale = new Vector3(1, -1, 1);
    }

    private void ToggleSound()
    {
        AudioManager.Instance.PlayEffectAt(9, hunter.transform.position, 1.5f);
    }

    public void Toggle()
    {
        if (IsShown)
        {
            Hide();
            return;
        }
        
        Show();
    }

    private void UpdateReminder()
    {
        var first = hunt.Tasks.FirstOrDefault(t => !t.IsDone);
        reminder.text = first != default ? first.Title : "";
    }
}