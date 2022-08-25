namespace StolenVehicleLocatorSystem.Api.Hubs.Payloads
{
    public class MessagePayload
    {
        public string? To { get; set; }

        public string? Message { get; set; }

        public DateTime Date { get; set; }
    }
}
