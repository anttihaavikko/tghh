using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Book : MonoBehaviour
{
    [SerializeField] private List<TaskField> tasks;
    
    private Hunt hunt;

    public void Init(List<Country> countries)
    {
        hunt = new Hunt(countries);
        tasks.ForEach(t =>
        {
            var index = tasks.IndexOf(t);
            var exists = hunt.Tasks.Count > index;
            t.gameObject.SetActive(exists);
            if (exists)
            {
                t.Init(hunt.Tasks[index]);
            }
        });
    }

    public bool CanHunt(Country country)
    {
        return hunt.Tasks.All(t => t.IsDone || t.Is(TaskType.Hunt) && t.FilledBy(country));
    }

    public void Complete(TaskType taskType)
    {
        hunt.Tasks.Where(t => t.Is(taskType)).ToList().ForEach(t => t.Complete());
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
}