using Cross.Dtos;

namespace Cross.Helpers
{
    public static class GeoUtils
    {
        public static double CalculateDistance(GpsCoordinateDto from, GpsCoordinateDto to)
        {
            // Implementación básica usando la fórmula de Haversine
            double R = 6371000; // Radio de la Tierra en metros
            double lat1 = from.Latitude * Math.PI / 180;
            double lat2 = to.Latitude * Math.PI / 180;
            double dLat = (to.Latitude - from.Latitude) * Math.PI / 180;
            double dLon = (to.Longitude - from.Longitude) * Math.PI / 180;

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(lat1) * Math.Cos(lat2) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c;
        }
    }
}
