namespace DTech.Models.EF
{
    public partial class MonthlyRecap
    {
        public int Id { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalCost { get; set; }
        public decimal Profit { get; set; }
        public int TotalLaptops { get; set; }
        public int TotalSmartPhone { get; set; }
        public int TotalTablet { get; set; }
        public int TotalAccessory { get; set; }
    }
}
