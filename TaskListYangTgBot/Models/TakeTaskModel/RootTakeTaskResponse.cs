using System.Collections.Generic;

public class RootTakeTaskResponse
{
    public string id { get; set; }
    public List<Task> tasks { get; set; }
    public string taskSuiteId { get; set; }
    public int poolId { get; set; }
    public string status { get; set; }
    public int secondsLeft { get; set; }
    public string reward { get; set; }
    public int projectId { get; set; }
    public int projectAssignmentsQuotaLeft { get; set; }
    public TaskSuite taskSuite { get; set; }
    public string request_id { get; set; }
    public Payload payload { get; set; }
    public string requestId { get; set; }
    public string message { get; set; }
    public string code { get; set; }
    public int statusCode { get; set; }
    public string title { get; set; }
}
