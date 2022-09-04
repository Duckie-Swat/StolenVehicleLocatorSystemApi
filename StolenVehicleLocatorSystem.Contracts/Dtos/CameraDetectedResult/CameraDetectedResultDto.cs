namespace StolenVehicleLocatorSystem.Contracts.Dtos.CameraDetectedResult
{
    public class CameraDetectedResultDto : BaseDto
    {
        public Guid CameraId { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string Location { get; set; }

        public string Photo { get; set; }
    }
}
