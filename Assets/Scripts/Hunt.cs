using System;
using System.Collections.Generic;
using System.Linq;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using UnityEngine;

public class HuntTarget
{
    public string Name { get; set; }
    public string Short { get; set; }
    public string Description { get; set; }
}

public class HuntReward
{
    public string Name { get; set; }
    public string Country { get; set; }
    public string City { get; set; }
    public int BaseValue { get; set; }

    public string Description => $"Pelt of ({Name})!\nSold for double if the (country) name has ({Country}) and double if the (city) name has ({City}).\nBase value of ({BaseValue} €).";
    public ButtonStyle Button { get; set; }

    public HuntReward(string name, List<Country> countries, int level)
    {
        Name = name;
        BaseValue = (level + 1) * 100;
        var letters = Math.Min(level + 1, 4);
        Country = HuntTask.GetLetters(letters, countries.Random().name).ToUpper();
        City = HuntTask.GetLetters(letters, countries.Random().CapitalName).ToUpper();
    }

    public int GetSellPrice(Country current)
    {
        if (current == default) return 0;
        var multi = 1;
        if (Country.All(letter => current.name.ToUpper().Contains(letter))) multi *= 2;
        if (City.All(letter => current.CapitalName.ToUpper().Contains(letter))) multi *= 2;
        return Mathf.RoundToInt(BaseValue * multi * current.FuelPrice / 100f);
    }
}

public class Hunt
{
    public List<HuntTask> Tasks { get; }
    public HuntTarget Target { get; }
    
    public HuntReward Reward { get; }

    public Hunt(List<Country> countries, List<HuntTarget> targetList, int level)
    {
        Target = level < targetList.Count ? targetList[level] : targetList.Random();
        Reward = new HuntReward(Target.Short, countries, level);
        
        if (level == 0)
        {
            Tasks = new List<HuntTask>
            {
                new(TaskType.Track, countries, 2, 0),
                new(TaskType.Hunt, countries, 2, 0)
            };
            return;
        }
        
        if (level == 1)
        {
            Tasks = new List<HuntTask>
            {
                new(TaskType.Track, countries, 2, 0),
                new(TaskType.Trap, countries, 0, 2),
                new(TaskType.Hunt, countries, 3, 0)
            };
            return;
        }
        
        if (level == 2)
        {
            Tasks = new List<HuntTask>
            {
                new(TaskType.Track, countries, 3, 0),
                new(TaskType.Trap, countries, 0, 3),
                new(TaskType.Hunt, countries, 3, 1)
            };
            return;
        }
        
        if (level == 3)
        {
            Tasks = new List<HuntTask>
            {
                new(TaskType.Track, countries, 3, 1),
                new(TaskType.Trap, countries, 2, 2),
                new(TaskType.Hunt, countries, 2, 3)
            };
            return;
        }
        
        if (level == 4)
        {
            Tasks = new List<HuntTask>
            {
                new(TaskType.Track, countries, 4, 0),
                new(TaskType.Trap, countries, 0, 4),
                new(TaskType.Hunt, countries, 3, 3)
            };
            return;
        }

        Tasks = new List<HuntTask>
        {
            new(TaskType.Track, countries, 3, 2),
            new(TaskType.Trap, countries, 2, 3),
            new(TaskType.Hunt, countries, 3, 3)
        };
    }
}