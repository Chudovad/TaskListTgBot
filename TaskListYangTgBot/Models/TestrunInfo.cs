using System.Collections.Generic;

public class TestrunInfo
{
    public string environment { get; set; }
    public string id { get; set; }
    public string title { get; set; }
    public string final_requester_code { get; set; }
    public string env_descr { get; set; }
    public List<string> env_requester_code_explanation { get; set; }
    public List<string> required_envs { get; set; }
}