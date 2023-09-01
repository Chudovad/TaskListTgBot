using System.Collections.Generic;

public class Data
{
    public List<Case> cases { get; set; }
    public CasesPackInfo cases_pack_info { get; set; }
    public CoherenceInfo coherence_info { get; set; }
    public DeadlineInfo deadline_info { get; set; }
    public TestrunInfo testrun_info { get; set; }
    public VersionInfo version_info { get; set; }
}
