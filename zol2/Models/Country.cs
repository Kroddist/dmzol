namespace zol2.Models
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Capital { get; set; } = string.Empty;
        public long Population { get; set; }
        public double Area { get; set; }
        public string Region { get; set; } = string.Empty;
        
        // Additional field to support Task 4/5 regarding capital statistics
        // even within a single-table constraint.
        public long CapitalPopulation { get; set; }

        public override string ToString()
        {
            return $"{Name} ({Region}) - Capital: {Capital} [{CapitalPopulation}], Pop: {Population}, Area: {Area}";
        }
    }
}

