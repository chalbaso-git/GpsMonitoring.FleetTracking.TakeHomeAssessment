namespace Cross.Helpers
{
    public class CoordinateSpecification
    {
        public bool IsSatisfiedBy(double latitude, double longitude)
        {
            return latitude >= -90 && latitude <= 90 && longitude >= -180 && longitude <= 180;
        }
    }
}
