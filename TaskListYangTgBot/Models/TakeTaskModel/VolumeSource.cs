using System.Collections.Generic;

public class VolumeSource
{
    public DefaultEnvironmentDistribution defaultEnvironmentDistribution { get; set; }
    public string type { get; set; }
    public List<string> values { get; set; }
}
