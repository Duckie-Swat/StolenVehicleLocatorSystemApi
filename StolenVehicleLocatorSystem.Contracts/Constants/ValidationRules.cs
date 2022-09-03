namespace StolenVehicleLocatorSystem.Contracts.Constants
{
    public static class ValidationRules
    {
        public static class CommonRules
        {
            public const int MinLenghCharactersForText = 1;
            public const int MaxLenghCharactersForText = 255;
        }

        public static class PlateNumberRule
        {
            public const int MinLenghCharactersForText = 5;
            public const int MaxLenghCharactersForText = 25;
        }
    }
}
