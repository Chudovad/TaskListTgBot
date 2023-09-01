using System;
using System.Collections.Generic;

public class Pool
{
    public int id { get; set; }
    public DateTime startedAt { get; set; }
    public string reward { get; set; }
    public int assignmentMaxDurationSeconds { get; set; }
    public int acceptancePeriodDays { get; set; }
    public List<ActiveAssignment> activeAssignments { get; set; }
}
