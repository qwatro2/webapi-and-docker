using System.Text.Json;

namespace web.Models;

public class RequestDataModel
{
    public long Id { get; set; }
    public string RequestType { get; set; }
    public string RequestName { get; set; }
    public ArgumentsModel? Arguments { get; set; }
    public long? ArgumentsId { get; set; }
    public Dictionary<string, dynamic?> Result { get; set; }
    public int StatusCode { get; set; }
    public DateTime Date { get; set; }

    private RequestDataModel(long Id, string RequestType, string RequestName, ArgumentsModel? Arguments,
        long? ArgumentsId, Dictionary<string, dynamic?> Result, int StatusCode, DateTime Date)
    {
        this.Id = Id;
        this.RequestType = RequestType;
        this.RequestName = RequestName;
        this.Arguments = Arguments;
        this.ArgumentsId = ArgumentsId;
        this.Result = Result;
        this.StatusCode = StatusCode;
        this.Date = Date;
    }

    public static RequestDataModel CreateRequestDataModel(IDictionary<string, object?> data)
    {
        dynamic argumentsId = data["arguments_id"] is null ? null : data["arguments_id"];
        
        return new RequestDataModel(
            (long) data["id"],
            (string) data["request_type"],
            (string) data["request_name"],
            null,
            argumentsId,
            JsonSerializer.Deserialize<Dictionary<string, dynamic?>>((string) data["result"]),
            (int) data["status_code"],
            (DateTime) data["date"]
        );
    }
}