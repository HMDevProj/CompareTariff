namespace Shared.Models
{
    public class Tariff
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Type { get; set; }
        public decimal BaseCost { get; set; }
        public int? IncludedKwh { get; set; }
        public decimal? AdditionalKwhCost { get; set; }
    }
}

