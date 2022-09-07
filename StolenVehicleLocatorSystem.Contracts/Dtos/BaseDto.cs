namespace StolenVehicleLocatorSystem.Contracts.Dtos
{
    public class BaseDto
    {
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime LastUpdatedAt { get; set; }

        public bool IsDeleted { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid UpdatedBy { get; set; }

        public Guid DeletedBy { get; set; }
    }
}
