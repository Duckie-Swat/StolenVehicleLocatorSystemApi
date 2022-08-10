namespace StolenVehicleLocatorSystem.Contracts.Filters
{
    public class BaseFilter
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 20;

    }
}
