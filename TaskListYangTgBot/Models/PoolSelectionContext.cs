using System.Collections.Generic;

public class PoolSelectionContext
{
    public List<ActiveFilter> activeFilters { get; set; }
    public List<ActiveSort> activeSorts { get; set; }
    public List<string> visibleGroupsUuids { get; set; }
    public bool favorite { get; set; }
    public List<VisibleGroupsMetum> visibleGroupsMeta { get; set; }
}