namespace web.Models;

public record RequestDataModel
{
    public string RequestType { get; set; }
    public string RequestName { get; set; }
    public ArgumentsModel Arguments { get; set; }
    public Dictionary<string, dynamic?> Result { get; set; }
    public int StatucCode { get; set; }
}