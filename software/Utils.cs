namespace cog1app
{
    public static class Utils
    {
        public static double CelsiusToFahrenheit(double celsius)
        {
            return (celsius * 9 / 5) + 32;
        }

        public static double? CelsiusToFahrenheit(double? celsius)
        {
            if (celsius == null)
                return null;
            return (celsius.Value * 9 / 5) + 32;
        }

    }
}
