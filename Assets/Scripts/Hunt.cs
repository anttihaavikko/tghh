using System.Collections.Generic;

public class Hunt
{
    public List<HuntTask> Tasks { get; }

    public Hunt(List<Country> countries)
    {
        Tasks = new List<HuntTask>
        {
            new(TaskType.Track, countries, 2, 0),
            new(TaskType.Trap, countries, 0, 3),
            new(TaskType.Hunt, countries, 2, 3)
        };
    }
}