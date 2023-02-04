namespace web.Models;

public class ArgumentsModel
{
    public double FirstArgument { get; set; }
    public double SecondArgument { get; set; }
    public char Operation { get; set; }
    
    private ArgumentsModel(double FirstArgument, double SecondArgument, char Operation)
    {
        this.FirstArgument = FirstArgument;
        this.SecondArgument = SecondArgument;
        this.Operation = Operation;
    }
    
    public static ArgumentsModel CreateArgumentsModel(IDictionary<string, object?> data)
    {
        var operation = (string)data["operation"] switch
        {
            "+" => '+',
            "-" => '-',
            "*" => '*',
            "/" => '/',
            _ => throw new ArgumentException($"{(string)data["operation"]} is not operation symbol")
        };
        
        return new ArgumentsModel
        (
            (double) data["first_argument"],
            (double) data["second_argument"],
            operation
        );
    }
}