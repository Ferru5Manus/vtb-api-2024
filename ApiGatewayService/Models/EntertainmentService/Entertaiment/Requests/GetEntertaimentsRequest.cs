namespace EntertaimentService.Models.Tour.Requests;

// TODO: finish this
public class GetEntertaimentsRequest
{
    public int Page { get; set; } = 0;
    public List<long>? Categories { get; set; }
    public int MinimalRating { get; set; } = 0;
    public int MaximalRating { get; set; } = 5;
    public double MinimalPrice { get; set; } = 0;
    public double MaximalPrice { get; set; } = double.MaxValue;
}