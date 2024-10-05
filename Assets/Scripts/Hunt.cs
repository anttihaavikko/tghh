using System;
using System.Collections.Generic;

public class Hunt
{
    public List<HuntTask> Tasks { get; }

    public Hunt(List<Country> countries, int level)
    {
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