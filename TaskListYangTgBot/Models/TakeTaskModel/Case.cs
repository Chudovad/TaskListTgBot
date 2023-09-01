using System.Collections.Generic;

public class Case
{
    public List<object> attachments { get; set; }
    public List<object> attributes_to_show { get; set; }
    public List<AttributesUpdated> attributes_updated { get; set; }
    public string automation { get; set; }
    public string automationFormatted { get; set; }
    public List<object> automations { get; set; }
    public int corrected_estimation { get; set; }
    public string createdBy { get; set; }
    public object createdTime { get; set; }
    public List<object> current_case_known_bugs { get; set; }
    public string description { get; set; }
    public int duration { get; set; }
    public List<object> environments { get; set; }
    public double estimatedTime { get; set; }
    public List<object> failedSteps { get; set; }
    public string finishedBy { get; set; }
    public string finishedTime { get; set; }
    public List<object> flags { get; set; }
    public List<object> hp_comments { get; set; }
    public int id { get; set; }
    public bool isAutotest { get; set; }
    public bool is_bad_timing { get; set; }
    public bool is_hp { get; set; }
    public bool is_main { get; set; }
    public List<KnownIncorrectBug> known_incorrect_bugs { get; set; }
    public List<object> known_verdicts { get; set; }
    public object lastModifiedTime { get; set; }
    public Meta meta { get; set; }
    public string modifiedBy { get; set; }
    public string name { get; set; }
    public int original_sort_order { get; set; }
    public List<object> parametersList { get; set; }
    public List<object> path { get; set; }
    public string preconditions { get; set; }
    public List<object> properties { get; set; }
    public List<object> properties_to_show { get; set; }
    public bool removed { get; set; }
    public List<string> scope { get; set; }
    public List<object> sharedPreconditions { get; set; }
    public string startedBy { get; set; }
    public string startedTime { get; set; }
    public Stats stats { get; set; }
    public string status { get; set; }
    public List<Step> steps { get; set; }
    public List<object> tags { get; set; }
    public List<object> tasks { get; set; }
    public string uuid { get; set; }
    public string version { get; set; }
}
