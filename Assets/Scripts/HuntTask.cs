using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

public class HuntTask
{
    private readonly string country;
    private readonly string city;
    private readonly TaskType type;
    
    public bool IsDone { get; private set; }
    public TaskField Field { get; set; }

    public string Title => $"{GetTaskName()} in {GetTarget()}";
    
    public HuntTask(TaskType taskType, List<Country> countries, int countryLetters, int cityLetters)
    {
        type = taskType;
        var c = countries.Random();
        Debug.Log($"From {c.name} / {c.CapitalName}");
        country = GetLetters(countryLetters, c.name).ToUpper();
        city = GetLetters(cityLetters, c.CapitalName).ToUpper();
    }

    private string GetTarget()
    {
        if (country.Length > 0 && city.Length > 0) return $"country/city {country}/{city}";
        return city.Length > 0 ? $"city {city}" : $"country {country}";
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
        return string.Join(string.Empty, name.Distinct().Where(l => l.ToString() != " ").OrderBy(_ => Random.value).Take(amount));
    }

    public bool FilledBy(Country candidate)
    {
        return country.All(letter => candidate.name.ToUpper().Contains(letter)) && city.All(letter => candidate.CapitalName.ToUpper().Contains(letter));
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