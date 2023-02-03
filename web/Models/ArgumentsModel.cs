namespace web.Models;

public record ArgumentsModel
{
    public double FirstArgument { get; set; }
    public double SecondArgument { get; set; }
    public char Operation { get; set; }
}