public class BookingInfo
{
    public int bookingId { get; set; }
    public string comment { get; set; }
    public Cost cost { get; set; }
    public long createdTs { get; set; }
    public Estimate estimate { get; set; }
    public string login { get; set; }
    public MetaData metaData { get; set; }
    public Params @params { get; set; }
    public string status { get; set; }
    public string title { get; set; }
    public string updatedBy { get; set; }
    public long updatedTs { get; set; }
}
