using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Extensions;
using Random = UnityEngine.Random;

public class HuntTask
{
    private readonly string country;
    private string city;
    private readonly TaskType type;
    
    public bool IsDone { get; private set; }
    public TaskField Field { get; set; }

    public string Title => $"{GetTaskName()} in country {country}";
    
    public HuntTask(TaskType taskType, List<Country> countries, int letters)
    {
        type = taskType;
        country = GetLetters(letters, countries.Random().name).ToUpper();
    }

    private string GetTaskName()
    {
        return type switch
        {
            TaskType.Hunt => "Hunt",
            TaskType.Track => "Track",
            TaskType.Trap => "Buy trap",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private static string GetLetters(int amount, string name)
    {
        return string.Join(string.Empty, name.ToCharArray().Distinct().Where(l => l.ToString() != " ").OrderBy(_ => Random.value).Take(amount));
    }

    public bool FilledBy(Country candidate)
    {
        return country.ToCharArray().All(letter => candidate.name.ToUpper().Contains(letter));
    }

    public void Complete()
    {
        IsDone = true;
        Field.Mark();
    }

    public bool Is(TaskType taskType)
    {
        return type == taskType;
    }
}

public enum TaskType
{
    Hunt,
    Track,
    Trap
}