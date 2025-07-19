using Cross.Dtos;

namespace Cross.Helpers
{
    public static class GeoUtils
    {
        public static double CalculateDistance(GpsCoordinateDto c1, GpsCoordinateDto c2)
        {
            const double EarthRadius = 6371000; // metros

            double lat1Rad = DegreesToRadians(c1.Latitude);
            double lat2Rad = DegreesToRadians(c2.Latitude);
            double deltaLat = DegreesToRadians(c2.Latitude - c1.Latitude);
            double deltaLon = DegreesToRadians(c2.Longitude - c1.Longitude);

            double a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                       Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                       Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return EarthRadius * c;
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}
