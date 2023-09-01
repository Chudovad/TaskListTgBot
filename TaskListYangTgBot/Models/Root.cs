using System.Collections.Generic;

public class Root
{
    public string refUuid { get; set; }
    public string groupUuid { get; set; }
    public int projectId { get; set; }
    public bool mayContainAdultContent { get; set; }
    public string title { get; set; }
    public string description { get; set; }
    public bool hasInstructions { get; set; }
    public int snapshotMajorVersion { get; set; }
    public int snapshotMinorVersion { get; set; }
    public bool snapshotMajorVersionActual { get; set; }
    public string assignmentIssuingType { get; set; }
    public RequesterInfo requesterInfo { get; set; }
    public Availability availability { get; set; }
    public bool postAccept { get; set; }
    public string iframeSubdomain { get; set; }
    public TrainingDetails trainingDetails { get; set; }
    public ProjectStats projectStats { get; set; }
    public List<Pool> pools { get; set; }
    public ProjectMetaInfo projectMetaInfo { get; set; }
    public string request_id { get; set; }
    public string requestId { get; set; }
    public string message { get; set; }
    public string code { get; set; }
}
