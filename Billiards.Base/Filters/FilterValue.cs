namespace Billiards.Base.Filters;

public class FilterValue
{
    public FilterValue(string name, double value)
    {
        Name = name;
        Value = value;
    }

    public FilterValue(string name, string? text)
    {
        Name = name;
        Text = text;
    }

    public FilterValue(string name, double value, string? text)
    {
        Name = name;
        Value = value;
        Text = text;
    }

    public string Name { get; set; }
    public double Value { get; set; }

    public string? Text { get; set; }

}