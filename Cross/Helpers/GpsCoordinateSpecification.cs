using Cross.Dtos;

namespace Cross.Helpers
{
    public class GpsCoordinateSpecification : ISpecification<GpsCoordinateDto>
    {
        public bool IsSatisfiedBy(GpsCoordinateDto coordinate)
        {
            return coordinate.Latitude >= -90 && coordinate.Latitude <= 90 &&
                   coordinate.Longitude >= -180 && coordinate.Longitude <= 180;
        }
    }
}