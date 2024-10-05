using System.Collections.Generic;

public class Hunt
{
    public List<HuntTask> Tasks { get; }

    public Hunt(List<Country> countries)
    {
        Tasks = new List<HuntTask>
        {
            new(TaskType.Track, countries, 2),
            new(TaskType.Hunt, countries, 2)
        };
    }
}